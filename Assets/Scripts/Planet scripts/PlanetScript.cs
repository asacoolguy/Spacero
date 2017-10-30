﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetScript : MonoBehaviour { 
	public int type; // 1 for asteroid, 2 for big planet
	public float mass; // mass of the planet, affects its gravitation pull
	public float powerChargePerSecond; // speed at which a player's power will charge
	public float rotationSpeed; // number of degrees the planet rotates every second
	public float explosionSpeed; // speed the planet adds to the players on explosion
	public bool respawn = true;
	public float planetRespawnTime; // how long it takes for this planet to come back
	private bool isDestroyed;
	// variables about planet cracking
	public int maxCracks; // 2 cracks for big planets, 0 cracks for asteroids
	private int crackedState; // 0, 1 or 2 cracks. 3 means destroyed.

	private PlanetGravityScript gravityField;
	public Sprite regular, cracked1, cracked2, explosion;
	public AudioClip explodeSound, respawnSound, crackSound1, crackSound2;
	private CameraShakeScript cameraShaker;

	void Start () {
		isDestroyed = false;
		crackedState = 0;
		gravityField = transform.GetChild(0).GetComponent<PlanetGravityScript>();
		cameraShaker = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<CameraShakeScript>();
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

	public void ChangeSprite(){
		switch(crackedState){
			case 0:
				GetComponent<SpriteRenderer>().sprite = regular;
				break;
			case 1: 
				GetComponent<SpriteRenderer>().sprite = cracked1;
				break;
			case 2: 
				GetComponent<SpriteRenderer>().sprite = cracked2;
				break;
			case -1:
				GetComponent<SpriteRenderer>().sprite = explosion;
				break;
		}
	}

	// called when another player blows this planet up
	public void SelfDestruct(){
		// increments the cracked state and blow it up if it exceeds the max number of allowed cracks. 
		// also do camera shake here? 
		crackedState += 1;
		cameraShaker.ShakeCamera(0.75f * crackedState, 0.2f * crackedState);
		if (crackedState > maxCracks){
			crackedState = -1;
			StartCoroutine(SelfDestructHelper());
		}
		else{
			ChangeSprite();
			if(crackedState == 1){
				GetComponent<AudioSource>().PlayOneShot(crackSound1);
			}
			else{
				GetComponent<AudioSource>().PlayOneShot(crackSound2);
			}

		}

	}

	IEnumerator SelfDestructHelper(){
		// TODO: change this to a better animation method
		isDestroyed = true;
		gravityField.enabled = false;
		GetComponent<CircleCollider2D>().enabled = false;
		ChangeSprite ();
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
		// wait to respawn.if respawn time is lower than 0, then don't respawn
		if (!respawn){
			yield break;
		}
		yield return new WaitForSeconds(planetRespawnTime);
		// check to make sure no one is too close, else wait a little longer
		while (!CanSpawn()) {
			yield return new WaitForSeconds(0.25f);
		}
		// start rendering the planet
		isDestroyed = false;
		crackedState = 0;
		ChangeSprite ();
		gravityField.enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<CircleCollider2D>().enabled = true;
		GetComponent<AudioSource> ().PlayOneShot (respawnSound, 0.6f);
	}

	public bool WillExplodeNext(){
		return crackedState == maxCracks;
	}

	public bool GetIsDestroyed(){
		return isDestroyed;
	}
}
