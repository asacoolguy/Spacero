using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour {
	private Vector2 Boundaries = new Vector2(40, 30);

	public GameObject[] planetPrefabList;
	public GameObject[] coinsPrefabList;
	public GameObject[] existingPlanets;
	public GameObject[] existingCoins;


	public int maxCoinNumber = 20;
	public bool canSpawn = true;
	public float planetRespawnTime = 3f;
	
	//UI stuff
	public GameObject gameoverScreen, player1HUD, player2HUD;
	public GameObject player1, player2;

	// game states
	public bool gameOver = false;

	public AudioClip winningSound, planetRespawnSound;

	// Use this for initialization
	void Start () {
		Boundaries.x = this.GetComponent<BoxCollider2D> ().size.x / 2;
		Boundaries.y = this.GetComponent<BoxCollider2D> ().size.y / 2;
	}
	
	// Update is called once per frame
	void Update () {
		//existingPlanets = GameObject.FindGameObjectsWithTag ("Planet");
		//if (existingPlanets.Length < maxPlanetNumber && canSpawn) {
			//StartCoroutine(spawnPlanet());
		//}
		// if there are less than 0 coins the player with more points win
		if (gameOver == false){
			existingCoins = GameObject.FindGameObjectsWithTag ("Coin");
			if (existingCoins.Length < 1) {
				if (player1  && player2 && player1.GetComponent<PlayerScript>().score > player2.GetComponent<PlayerScript>().score)
					playerWin(player1);
				else if (player1  && player2 && player1.GetComponent<PlayerScript>().score < player2.GetComponent<PlayerScript>().score)
					playerWin(player2);
			}
		}

		/*// handles reset button
		if (gameOver && (Input.GetKeyDown (KeyCode.R) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began))) {
			Application.LoadLevel(Application.loadedLevel);
		}
		else if(gameOver && Input.GetKeyDown(KeyCode.N)){
			if (Application.loadedLevelName == "Diagon")
				Application.LoadLevel("Gambit");
			else 
				Application.LoadLevel ("Diagon");
		}*/
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			if (other.gameObject.transform.position.x > Boundaries.x)
				other.gameObject.transform.position = new Vector3(-Boundaries.x, other.gameObject.transform.position.y, 0);
			if (other.gameObject.transform.position.x < -Boundaries.x)
				other.gameObject.transform.position = new Vector3(Boundaries.x, other.gameObject.transform.position.y, 0);
			if (other.gameObject.transform.position.y > Boundaries.y)
				other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, -Boundaries.y, 0);
			if (other.gameObject.transform.position.y < -Boundaries.y)
				other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, Boundaries.y, 0);
		}
	}

	public void playerWin(GameObject winningPlayer){
		gameoverScreen.SetActive(true);
		int id = winningPlayer.GetComponent<PlayerScript>().ID;
		gameoverScreen.transform.Find("Win Text").GetComponent<Text>().text = "Player " + id + " wins!";
		gameoverScreen.transform.Find("Reset Button").GetComponent<Button>().onClick.AddListener(() => this.resetMap());

		player1HUD.GetComponent<PlayerHUDScript>().disableHUDOnGameOver();
		player2HUD.GetComponent<PlayerHUDScript>().disableHUDOnGameOver();

		GetComponent<AudioSource> ().PlayOneShot (winningSound);
		gameOver = true;
	}

	// resets this map
	public void resetMap(){
		Application.LoadLevel(Application.loadedLevel);
	}


	// starts coroutine that deactivates a planet for a while before respawning it
	public void deactivate(GameObject planet){
		StartCoroutine (deactivateHelper (planet));
	}
	
	IEnumerator deactivateHelper(GameObject planet){
		// TODO: change this to a better animation method
		planet.GetComponent<PlanetScript> ().changeSprite (0);
		yield return 0;
		planet.GetComponent<PlanetScript> ().changeSprite (0);
		yield return 0;
		planet.GetComponent<PlanetScript> ().changeSprite (0);
		yield return 0;
		planet.GetComponent<PlanetScript> ().changeSprite (0);
		yield return 0;
		planet.GetComponent<PlanetScript> ().changeSprite (0);
		yield return 0;

		// moves player out if this planet is a parent of the player
		/*if (player1.gameObject.transform.parent == planet){
			player1.gameObject.transform.parent = null;
		}
		if (player2.gameObject.transform.parent == planet){
			player2.gameObject.transform.parent = null;
		}*/
		if (planet.transform.childCount > 1){
			for(int i = 1; i < planet.transform.childCount; i++){
				planet.transform.GetChild(i).parent = null;
			}
		}
		planet.SetActive (false);
		yield return new WaitForSeconds(planetRespawnTime);
		planetRespawnTime++;
		while (!planet.GetComponent<PlanetScript>().canSpawn()) {
			yield return new WaitForSeconds(0.25f);
		}
		planet.GetComponent<PlanetScript> ().changeSprite (1);
		planet.SetActive (true);
		planetRespawnTime++;
		GetComponent<AudioSource> ().PlayOneShot (planetRespawnSound, 0.6f);
	}
	
}
