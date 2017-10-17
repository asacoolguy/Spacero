using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour {
	public Vector2 Boundaries = new Vector2(40, 30);

	public GameObject[] planetPrefabList;
	public GameObject[] coinsPrefabList;
	public GameObject[] existingPlanets;
	public GameObject[] existingCoins;


	public int maxCoinNumber = 20;
	public bool canSpawn = true;
	public float planetRespawnTime = 3f;
	
	//UI stuff
	public Text p1score, p2score, winText, restartText;
	public GameObject player1, player2;

	// game states
	public bool gameOver = false;

	public AudioClip winningSound, planetRespawnSound;

	// Use this for initialization
	void Start () {
		this.GetComponent<BoxCollider2D> ().size = Boundaries * 2;
	}
	
	// Update is called once per frame
	void Update () {
		//existingPlanets = GameObject.FindGameObjectsWithTag ("Planet");
		//if (existingPlanets.Length < maxPlanetNumber && canSpawn) {
			//StartCoroutine(spawnPlanet());
		//}
		// if there are less than 0 coins the player with more points win
		existingCoins = GameObject.FindGameObjectsWithTag ("Coin");
		if (existingCoins.Length < 1) {
			if (player1  && player2 && player1.GetComponent<PlayerScript>().score > player2.GetComponent<PlayerScript>().score)
				playerWin(player1);
			else if (player1  && player2 && player1.GetComponent<PlayerScript>().score < player2.GetComponent<PlayerScript>().score)
				playerWin(player2);
		}

		// updates the score
		if (player1)
			p1score.text = "P1 score: " + player1.GetComponent<PlayerScript> ().score;
		if (player2)
			p2score.text = "P2 score: " + player2.GetComponent<PlayerScript> ().score;

		// handles reset button
		if (gameOver && Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
		else if(gameOver && Input.GetKeyDown(KeyCode.N)){
			if (Application.loadedLevelName == "Diagon")
				Application.LoadLevel("Gambit");
			else 
				Application.LoadLevel ("Diagon");
		}
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
		winText.text = "A WINNER IS PLAYER " + winningPlayer.GetComponent<PlayerScript> ().ID + "!";
		restartText.text = "Press 'R' to restart the game. \n Press 'N' to switch maps.";
		GetComponent<AudioSource> ().PlayOneShot (winningSound);
		gameOver = true;
	}

	public void deactivate(GameObject planet){
		StartCoroutine (diactivateHelper (planet));
	}
	
	IEnumerator diactivateHelper(GameObject planet){
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


		planet.SetActive (false);
		yield return new WaitForSeconds(planetRespawnTime);
		planetRespawnTime++;
		while (!planet.GetComponent<PlanetScript>().canSpawn()) {
			yield return new WaitForSeconds(0.25f);
		}
		planet.GetComponent<PlanetScript> ().changeSprite (1);
		planet.SetActive (true);
		planetRespawnTime++;
		GetComponent<AudioSource> ().PlayOneShot (planetRespawnSound);
	}
	
}
