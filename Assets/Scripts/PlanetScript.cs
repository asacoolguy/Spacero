using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetScript : MonoBehaviour { 
	public float mass; // mass of the planet, affects its gravitation pull
	public float powerChargePerSecond; // speed at which a player's power will charge
	public float rotationSpeed; // number of degrees the planet rotates every second
	public float explosionSpeed; // speed the planet adds to the players on explosion
	public float planetRespawnTime; // how long it takes for this planet to come back
	private bool isDestroyed;
	public Sprite regular, explosion;

	public AudioClip explodeSound, respawnSound;

	void Start () {
		isDestroyed = false;
	}

	void Update () {
		// rotate the planet every frame
		transform.Rotate (0, 0, Time.deltaTime * rotationSpeed);
	}

	// checks all players to see if they overlap with this planet
	public bool CanSpawn(){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject obj in players) {
			if (obj.GetComponent<PlayerScript>().GetIsDead() == false){
				float xDist = obj.transform.position.x - this.transform.position.x;
				float yDist = obj.transform.position.y - this.transform.position.y;
				float safeDistance = this.gameObject.GetComponent<CircleCollider2D>().radius + obj.GetComponent<BoxCollider2D>().size.y;
				if (Mathf.Sqrt(xDist * xDist + yDist * yDist) < safeDistance){
					return false;
				}
			}
		}

		return true;
	}

	// if i = 0 its inactive, if i = 1 it's active
	public void ChangeSprite(int i){
		if (i == 0) {
			GetComponent<SpriteRenderer>().sprite = explosion;
		}
		else{
			GetComponent<SpriteRenderer>().sprite = regular;
		}
	}

	// called when another player blows this planet up
	public void SelfDestruct(){
		StartCoroutine(SelfDestructHelper());
	}

	IEnumerator SelfDestructHelper(){
		// TODO: change this to a better animation method
		isDestroyed = true;
		GetComponent<CircleCollider2D>().enabled = false;
		ChangeSprite (0);
		gameObject.GetComponent<AudioSource> ().PlayOneShot (explodeSound);
		yield return new WaitForSeconds(0.2f);

		// moves player out if this planet is a parent of the player
		for(int i = 0; i < transform.childCount; i++){
			if (transform.GetChild(i).tag == "Player"){
				transform.GetChild(i).parent = null;
			}
		}
		// stop rendering the planet
		GetComponent<SpriteRenderer>().enabled = false;
		// wait to respawn. 
		yield return new WaitForSeconds(planetRespawnTime);
		// check to make sure no one is too close, else wait a little longer
		while (!CanSpawn()) {
			yield return new WaitForSeconds(0.25f);
		}
		// start rendering the planet
		isDestroyed = false;
		ChangeSprite (1);
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<CircleCollider2D>().enabled = true;
		GetComponent<AudioSource> ().PlayOneShot (respawnSound, 0.6f);
	}


	public bool GetIsDestroyed(){
		return isDestroyed;
	}
}
