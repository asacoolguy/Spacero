using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Script that manages game states and player wraping
/// </summary>
public class ManagerScript : MonoBehaviour {
	private Vector2 boundaries;
	
	// UI stuff
	public GameObject gameoverScreen, player1HUD, player2HUD;
	public GameObject player1Obj, player2Obj;
	private PlayerScript player1, player2;

	// game states
	public bool gameOver = false;

	// tracking gameobjects
	private GameObject[] existingCoins;
	public AudioClip winningSound;


	void Start () {
		boundaries = new Vector2(this.GetComponent<BoxCollider2D> ().size.x / 2,
								 this.GetComponent<BoxCollider2D> ().size.y / 2);

		player1 = player1Obj.GetComponent<PlayerScript>();
		player2 = player2Obj.GetComponent<PlayerScript>();
		player1.SetPlayerID(0);
		player2.SetPlayerID(1);
	}

	void Update () {
		// if there are less than 0 coins the player with more points win
		if (gameOver == false){
			existingCoins = GameObject.FindGameObjectsWithTag ("Coin");
			if (existingCoins.Length < 1) {
				if (player1.GetScore() > player2.GetScore()){
					PlayerWin(player1);
				}
				else if (player1.GetScore() < player2.GetScore()){
					PlayerWin(player2);
				}
			}
		}
	}

	// handles player wrapping from one side of the screen to the other
	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			if (other.gameObject.transform.position.x > boundaries.x)
				other.gameObject.transform.position = new Vector3(-boundaries.x, other.gameObject.transform.position.y, 0);
			if (other.gameObject.transform.position.x < -boundaries.x)
				other.gameObject.transform.position = new Vector3(boundaries.x, other.gameObject.transform.position.y, 0);
			if (other.gameObject.transform.position.y > boundaries.y)
				other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, -boundaries.y, 0);
			if (other.gameObject.transform.position.y < -boundaries.y)
				other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, boundaries.y, 0);
		}
	}

	public void PlayerWin(PlayerScript winningPlayer){
		gameoverScreen.SetActive(true);
		int id = winningPlayer.GetPlayerID() + 1;
		// TODO: make a menu script that handles this stuff
		gameoverScreen.transform.Find("Win Text").GetComponent<Text>().text = "Player " + id + " wins!";
		gameoverScreen.transform.Find("Reset Button").GetComponent<Button>().onClick.AddListener(() => this.ResetMap());

		player1HUD.GetComponent<PlayerHUDScript>().DisableHUDOnGameOver();
		player2HUD.GetComponent<PlayerHUDScript>().DisableHUDOnGameOver();

		GetComponent<AudioSource> ().PlayOneShot (winningSound);
		gameOver = true;
	}

	// resets this map
	public void ResetMap(){
		Application.LoadLevel(Application.loadedLevel);
	}
	
}
