using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Overall manager of the entire game. Handles scene loading and game states.
/// </summary>
public class GameManager : MonoBehaviour {	
	// singleton behavior
	public static GameManager instance = null;
	private bool gameLooping;

	// UI stuff
	private PlayerHUDScript player1HUD, player2HUD;
	private PreGameNotifier preGameNotifier;
	private PostGameNotifier postGameNotifier;
	private PlayerScript player1, player2;

	// timer
	private float timeLimit;
	private float timeLeft;

	// tournament info
	[SerializeField]private string[] mapNames;
	private int currentMapIndex;
	[SerializeField]private int roundsRequiredToWin;
	private int currentRound;
	[SerializeField]private int player1WinCount;
	[SerializeField]private int player2Wincount;

	// getting this map's info
	private MapManager mapManager;

	// developer tools
	public bool showPreNotification = true;
	public bool showPostNotification = true;

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}

	void Awake () {
		// makes this into a singleton
		if (instance == null){
			instance = this;
		}
		else if(instance != this){
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		// initiate the GameState
		//gameState = GameState.MainMenu;
		gameLooping = false;
	}

	void Update () {


	}

	// loop that is run once on every map
	private IEnumerator GameLoop(){
		if (gameLooping == false){
			gameLooping = true;

			// show this map's objective before the game starts
			if (showPreNotification)
				yield return StartCoroutine(ShowGameObjective());

			// run the game 
			yield return StartCoroutine(RunGame());

			// end the game, show results 
			if (showPostNotification)
				yield return StartCoroutine(GameOver());
		}
		else{
			Debug.Log("error: gameloop cannot start because game is already looping!");
		}
	}

	// coroutine that shows the round's objects before the round starts
	private IEnumerator ShowGameObjective(){
		player1HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(false);
		player2HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(false);

		// TODO: improve animation for showing objectives
		// initial wait for player to orient themselves with the map 
		yield return new WaitForSeconds(0.5f);

		preGameNotifier.SetUpText(currentRound, mapManager.GetPreGameObjectiveText());

		yield return StartCoroutine(preGameNotifier.DisplayNotification());

		yield return StartCoroutine(preGameNotifier.HideNotification());
		yield return StartCoroutine(preGameNotifier.DisplayCountdown());
	}


	private IEnumerator RunGame(){
		player1HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(true);
		player2HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(true);

		while(timeLeft > 0){
			// decrease the timer
			timeLeft -= Time.deltaTime;
			player1HUD.GetComponent<PlayerHUDScript>().UpdateTimerText(timeLeft);
			player2HUD.GetComponent<PlayerHUDScript>().UpdateTimerText(timeLeft);
			yield return null;
		}
	}

	private IEnumerator GameOver(){
		// disable controls
		player1HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(false);
		player2HUD.GetComponent<PlayerHUDScript>().SetIsHUDActive(false);
		// find the winner
		// TODO: for now score is king. when other win conditions come in, move this code elsewhere
		PlayerScript winner = null; 
		if (player1.GetScore() > player2.GetScore()){
			winner = player1;
			player1WinCount += 1;
		}
		else if(player2.GetScore() > player1.GetScore()){
			winner = player2;
			player2Wincount += 1;
		}

		// show the game over text first
		yield return StartCoroutine(postGameNotifier.DisplayGameOverText());

		// give info to postGameNotifier
		postGameNotifier.SetUpText(mapManager.GetPostGameObjectiveText(), player1.GetScore(), player2.GetScore(), winner);

		// display the results of the game
		yield return StartCoroutine(postGameNotifier.DisplayNotification());

		// if a player has won the tournament, display tournament results
		if (Mathf.Max(player1WinCount, player2Wincount) >= roundsRequiredToWin){
			// TODO: make a nice tournament results menu
			if (player1WinCount > player2Wincount){
				Debug.Log("Player 1 won");
			}
			else{
				Debug.Log("Player 2 won");
			}
			SceneManager.LoadScene("Main Menu");
		}
		// otherwise the tournament keeps going
		else{
			// countdown to the next map
			yield return StartCoroutine(postGameNotifier.CountDownToNextMap());

			gameLooping = false;

			// load the next map
			currentMapIndex += 1;
			// if we are on the last map 
			LoadMap(mapNames[currentMapIndex % mapNames.Length]);
		}

	}

	// runs when the scene is loaded, sets up variables for this scene
	private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
		// make sure not to run this function for the main menu scene
		if (scene.name != "Main Menu"){
			//this.gameState = GameState.PreGameHint;

			// find the local mapManager
			mapManager = GameObject.FindGameObjectsWithTag("MapManager")[0].GetComponent<MapManager>();
			// get player elements from mapManager
			player1 = mapManager.player1.GetComponent<PlayerScript>();
			player1.SetPlayerID(1);
			player2 = mapManager.player2.GetComponent<PlayerScript>();
			player2.SetPlayerID(2);
			// get UI elements from mapManager
			player1HUD = mapManager.gameCanvas.transform.Find("Player1 HUD").GetComponent<PlayerHUDScript>();
			player2HUD = mapManager.gameCanvas.transform.Find("Player2 HUD").GetComponent<PlayerHUDScript>();
			preGameNotifier = mapManager.gameCanvas.transform.Find("PreGame Notification").GetComponent<PreGameNotifier>();
			postGameNotifier = mapManager.gameCanvas.transform.Find("PostGame Notification").GetComponent<PostGameNotifier>();
			// get Map info from mapManager
			timeLimit = mapManager.timeLimit;
			timeLeft = timeLimit;	
			// increase round 
			currentRound += 1;

			StartCoroutine(GameLoop());
		}
	}

	// resets this map
	public void ResetMap(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadMap(string sceneName){
		if (gameLooping == false){
			SceneManager.LoadScene(sceneName);
		}
	}

	// called by Main Menu to start a certain tournament, given all the required info
	public void StartGalaxy(int roundsRequired, string[] mapNames){
		if (gameLooping == false){
			roundsRequiredToWin = roundsRequired;
			currentRound = 0;
			player1WinCount = 0;
			player2Wincount = 0;

			this.mapNames = mapNames;
			currentMapIndex = 0;

			LoadMap(this.mapNames[currentMapIndex]);
		}
	}
}
