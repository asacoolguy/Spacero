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

	// UI stuff
	private PlayerHUDScript player1HUD, player2HUD;
	private PreGameNotifier preGameNotifier;
	private PostGameNotifier postGameNotifier;
	private PlayerScript player1, player2;

	// game states
	// TODO: do we even need these
	public enum GameState{
		MainMenu, PreGameHint, GameRunning, GameOver, MapTransition, GameResults
	};
	//public GameState gameState;
	private bool gameLooping;


	// timer
	private float timeLimit;
	private float timeLeft;

	// tracking gameobjects
	public AudioClip winningSound;

	// tracking maps
	private MapManager mapManager;

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


	// do nothing in main menu state. start the game loop when map is loaded
	/*
	private IEnumerator MainMenuLoop(){
		 while(this.gameState == GameState.MainMenu){
			// TODO: in the future, saved data about stage and character unlocks should be read here
			yield return null;
		}

		// once the game is out of the MainMenu state, start the game loop
		if (this.gameState == GameState.PreGameHint){
			StartCoroutine(GameLoop());
		}
		else{
			Debug.Log("MainMenuLoop didn't start the GameLoop correctly");
		}
	}
	*/

	// loop that is run once on every map
	private IEnumerator GameLoop(){
		if (gameLooping == false){
			gameLooping = true;

			// show this map's objective before the game starts
			yield return StartCoroutine(ShowGameObjective());
			// TODO: do we even need game states for individual phases of a map? or can we just delegate everything in the game loop?

			// run the game 
			yield return StartCoroutine(RunGame());

			// end the game, show results 
			yield return StartCoroutine(GameOver());

			// load the next map
			this.LoadMap("Gambit");
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
		yield return new WaitForSeconds(0.5f);

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
		PlayerScript winner = player1; // TODO: actually find winner
		if (player1.GetScore() > player2.GetScore()){
			winner = player1;
		}
		else if(player2.GetScore() > player1.GetScore()){
			winner = player2;
		}
		else{
			//TODO: tie condition
			Debug.Log("This is a tie");
		}

		// show results
		yield return StartCoroutine(postGameNotifier.DisplayGameOverText());

		yield return StartCoroutine(postGameNotifier.DisplayNotification());

		yield return new WaitForSeconds(5f); //TODO: this needs to be determined via a variable

		gameLooping = false;
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

			StartCoroutine(GameLoop());
		}
	}

	public void PlayerWin(PlayerScript winningPlayer){
		/*gameoverScreen.SetActive(true);
		int id = winningPlayer.GetPlayerID();
		// TODO: make a menu script that handles this stuff
		gameoverScreen.transform.Find("Win Text").GetComponent<Text>().text = "Player " + id + " wins!";
		gameoverScreen.transform.Find("Reset Button").GetComponent<Button>().onClick.AddListener(() => this.LoadMap("Gambit"));

		GetComponent<AudioSource> ().PlayOneShot (winningSound);
		//gameOver = true;
		*/
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

}
