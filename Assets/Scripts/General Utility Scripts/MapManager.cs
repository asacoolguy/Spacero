using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles things that happens pertaining to a specific map.
/// Handles player wrapping and detecting win/lose states
/// </summary>
public class MapManager : MonoBehaviour {
	public enum MapType{
		coinCollect, planetCrack, alienHunt, sateliteSoccer, survive
	}

	private MapType currentMapType ;
	private Vector2 boundaries;
	public GameObject player1, player2, gameCanvas;
	public float timeLimit;


	// Use this for initialization
	void Start () {
		boundaries = new Vector2(this.GetComponent<BoxCollider2D> ().size.x / 2,
								 this.GetComponent<BoxCollider2D> ().size.y / 2);
	}

	void Update () {
	/*
		if (gameOver == false){
			// increment the timer
			timePast += Time.deltaTime;
			// if time is up, stop the game and inform the GameManager who won
			if (timePast > timeLimit){
				gameOver == true;
				// inform managerscript that game is over and who won
			}
		}
		*/
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

	// used by GameManager to set up the map
	/*public void SetUpMap(MapType type, GameObject playerObj1, GameObject playerObj2, float time){
		currentMapType = type;
		player1 = playerObj1.GetComponent<PlayerScript>();
		player2 = playerObj2.GetComponent<PlayerScript>();
		timeLimit = time;
	}*/

	// function that detects which player won the game. differs based on the type of map this is.
	// returns the id of the player that won, or -1 if it's a tie
	// TODO: do we need this? can't we just compare scores
	public int DetectWhichPlayerWon(){
		/*if (currentMapType == MapType.coinCollect){
			GameObject[] existingCoins = GameObject.FindGameObjectsWithTag ("Coin");
			if (existingCoins.Length < 1) {
				if (player1.GetScore() > player2.GetScore()){
					return player1.GetPlayerID();
				}
				else if (player1.GetScore() < player2.GetScore()){
					return player2.GetPlayerID();
				}
			}
			else{
				return -1;
			}
		}
		else if(currentMapType == MapType.planetCrack){
			return -1;
		}
		else{
			return -1;
		}*/ 
		return 0;
	}
}
