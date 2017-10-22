using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// PlayerScript is the base class for players. It defines basic properties of players such as jumping and landing.
/// different charaters should extend this class to implement their unique abilities and properties. 
/// </summary>

public abstract class PlayerScript : MonoBehaviour {
	// basic info and states
	private int playerID;
	private bool isLanded;
	private bool canJump;
	private int score;
	private Vector3 initialPosition;
	private Vector3 initialRotation;
	private PlanetScript currentPlanet;
	private PlayerScript otherPlayer;
	// respawn variables
	private bool isDead;
	public float respawnTime;
	private float respawnTimeCount = 0;
	public int coinsLostOnDeath;
	// scores and bars
	public float powerLevelInitial;
	private float powerLevel;
	public float powerLevelDecayPerSecond;
	public float powerLevelUsedOnJump;
	// bossting stuff. saved for later 
	/*public float dashDuration;
	private float currentDashDuration;
	private bool canDash = true;
	private bool isDashing = false;
	public float dashStrength = 1f;*/

	private ArrayList nearbyPlanets = new ArrayList();
	public AudioClip orbCollectSound, landingSound, leavingSound, deathSound, dashSound, chargedSound, respawnSound;

	public void Start () {
		score = 0;
		initialPosition = transform.position;
		initialRotation = transform.rotation.eulerAngles;

		// keep track of the other player object for convenience
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (players[0] == this.gameObject){
			otherPlayer = players[1].GetComponent<PlayerScript>();
		}
		else{
			otherPlayer = players[0].GetComponent<PlayerScript>();
		}

		// puts player in its initial state
		ResetPlayerStates();
	}
	
	// Update is called once per frame
	public void Update () {
		if (!isDead){ 
			if (!isLanded){ 
				// if player is drifting, it should move based on gravity from nearby planets and rotate to face forwards
				GetComponent<Rigidbody2D>().velocity += CalculateGravityPull();
				//RotateToVelocity(); maybe this should be reserved for captain hook

				// decrease power level when drifting
				if (powerLevel > 0f){
					powerLevel -= Time.deltaTime * powerLevelDecayPerSecond;
					if (powerLevel < 0f){
						powerLevel = 0f;
					}
					else if (powerLevel > 100){
						powerLevel = 100f;
					}
				}

				// run an abstract class here that handles taking inputs for special actions
			}
			else{ 
				// if player has landed, then it has no velocity and it should rotate to stand on the planet
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				//RotateToPlanet();

				// increase power level when landed
				powerLevel += currentPlanet.powerChargePerSecond * Time.deltaTime;

				// enable jumping when powerlevel reaches a certain point
				if (powerLevel < powerLevelUsedOnJump){
					canJump = false;
				}
				else{
					if (canJump == false){
						canJump = true;
						GetComponent<AudioSource>().PlayOneShot(chargedSound);
					}
				}
			}
		}
		else{ 
			// if player is dead, decrease the countdown timer
			respawnTimeCount -= Time.deltaTime;
			this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.position = initialPosition;

			if (respawnTimeCount < 0){
				Respawn();
			}
		}
	}

	// on collision with the other player, compares scores and destroy the player with less speed
	// TODO: this could use more explanation/animation to seem more apparent
	// TODO: what if both players have the same speed?
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			if (otherPlayer.GetComponent<PlayerScript>().GetIsLanded()){
				otherPlayer.Suicide();
			}
			else if (GetComponent<Rigidbody2D>().velocity.magnitude >
					other.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude){
				otherPlayer.Suicide();
			}
			else{
				Suicide();
			}
		}
		else if (other.gameObject.tag == "Planet" && isDead == false && isLanded == false) {
			LandOnPlanet(other.gameObject);
		}
	}

	// helper function that resets the player to initial drifting state
	private void ResetPlayerStates(){
		isDead = false;
		isLanded = false;
		canJump = false;
		transform.parent = null;
		currentPlanet = null;
		transform.position = initialPosition;
		transform.eulerAngles = initialRotation;
		powerLevel = powerLevelInitial;
		nearbyPlanets.Clear();

		// give a random starting velocity to prevent being stuck
		float startingSpeed = 5f;
		float angle = Random.Range(0,360) * Mathf.Deg2Rad;
		GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle) * startingSpeed,
															Mathf.Sin(angle) * startingSpeed);
	}

	// helper function that lands the player onto a given planet
	public void LandOnPlanet(GameObject planet){
		// make sure that the player hasn't already landed
		if (isLanded == false && currentPlanet == null){
			isLanded = true;
			currentPlanet = planet.GetComponent<PlanetScript>();
			GetComponent<AudioSource>().PlayOneShot (landingSound);

			// rotates and repositions the player accordingly
			Vector3 difference = transform.position - currentPlanet.transform.position;
			float radius = currentPlanet.gameObject.GetComponent<CircleCollider2D>().radius * currentPlanet.transform.lossyScale.x + 
							GetComponent<BoxCollider2D>().size.y * transform.lossyScale.x / 2;
			Vector3 newDifference = difference.normalized * radius;
			transform.position = transform.position - difference + newDifference;
			RotateToPlanet();

			GetComponent<Rigidbody2D>().freezeRotation = true;
			transform.parent = planet.transform;
			nearbyPlanets.Remove (currentPlanet.gameObject);
		}
		else{
			print("Error: cannot land on " + planet.name);
		}
	}

	// sends player off a planet and blows it up
	public void LeavePlanet(){
		// make sure there is a currentplanet and the player can jump off it
		if (isLanded && currentPlanet != null && canJump){
			isLanded = false;
			transform.parent = null;
			canJump = false;
			GetComponent<AudioSource>().PlayOneShot (leavingSound);

			Vector2 leavingAngle = new Vector2 (transform.position.x - currentPlanet.transform.position.x,
			                            transform.position.y - currentPlanet.transform.position.y).normalized;
            // TODO: check if applyForce is better
			GetComponent<Rigidbody2D>().velocity = leavingAngle * currentPlanet.explosionSpeed;
			GetComponent<Rigidbody2D>().freezeRotation = false;

			// consume powerLevel
			powerLevel -= powerLevelUsedOnJump;

			// kills the other player if they're on this planet too
			if (otherPlayer.isDead == false && otherPlayer.currentPlanet != null &&
			    otherPlayer.currentPlanet.gameObject == this.currentPlanet.gameObject){
				otherPlayer.Suicide();
			}

			nearbyPlanets.Remove (currentPlanet.gameObject);
			currentPlanet.SelfDestruct();
			currentPlanet = null;		
		}
	}

	// helper function that rotates the player to "stand on" a planet
	private void RotateToPlanet(){
		float angle;
		Vector3 difference = currentPlanet.transform.position - transform.position;

		if (difference.x == 0) {
			if (difference.y > 0)
				angle = 0;
			else{
				angle = 180;
			}
		}
		else{
			angle =  Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg + 90;
		}

		transform.eulerAngles = new Vector3 (0, 0, angle);
	}

	// helper function that rotates the player to face its moving direction
	private void RotateToVelocity(){
		float angle;
		Vector2 direction = GetComponent<Rigidbody2D>().velocity;

		if (direction.x == 0) {
			if (direction.y > 0)
				angle = 0;
			else{
				angle = 180;
			}
		}
		else{
			angle =  Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg - 90;
		}

		transform.eulerAngles = new Vector3 (0, 0, angle);
	}

	// disable the player's components and starts the respawn timer
	public void Suicide(){
		isDead = true;
		powerLevel = 0f;
		respawnTimeCount = respawnTime;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<AudioSource>().PlayOneShot(deathSound);
		// give other player coins
		int coinsTaken = coinsLostOnDeath;
		if (score < coinsLostOnDeath){
			coinsTaken = score;
		}
		score -= coinsTaken;
		otherPlayer.score += coinsTaken;
	}

	// respawn the player by reactivating its components
	public void Respawn(){
		ResetPlayerStates();
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<BoxCollider2D>().enabled = true;

		GetComponent<AudioSource>().PlayOneShot(respawnSound);
	}

	// TODO: reimplement this
	private Vector2 CalculateGravityPull(){
		Vector2 final = Vector2.zero;
		float G = 6.67300f * 1f;  // should be 10 to the -11th power, but we're keeping planet mass low to compensate
		float m = 1; // player mass
		foreach(GameObject p in nearbyPlanets) {
			PlanetScript planet = p.GetComponent<PlanetScript>();
			if (planet && planet.GetIsDestroyed() == false){
				Vector2 r = new Vector2(planet.transform.position.x - transform.position.x,
										planet.transform.position.y - transform.position.y);
				Vector2 direction = r.normalized;
				// raised to power of 2.5 instead of 2 to make gravitation attraction slightly weaker than normal
				float r_squared = Mathf.Pow(r.magnitude, 3f); 
				// F = G*M*m/r_squared
				float F = G * planet.mass * m / r_squared;
				final+= F * direction;
			}
		}
		
		return final;
	}

	public void AcquiredOrb(int orbValue){
		score += orbValue;
		GetComponent<AudioSource>().PlayOneShot (orbCollectSound);
	}

	// does the player's action. to be implement by each different player class
	public abstract void ActivatePlayerAction();

	/*IEnumerator actionBoost(){
		while(boosting){
			GetComponent<Rigidbody2D>().velocity += new Vector2(transform.position.normalized.x * boostStrength,
			                                                    transform.position.normalized.y * boostStrength);
			//print ("boosted " + currentBoostDuration);
			currentBoostDuration -= 1;
			if (currentBoostDuration < 0){
				//print ("finished");
				boosting = false;
				currentBoostDuration = boostDuration;
			}
			yield return 0;
		}
	}*/

	// all the getters and setters
	public bool GetIsLanded(){
		return isLanded;
	}

	public bool GetIsDead(){
		return isDead;
	}

	public bool GetCanJump(){
		return canJump;
	}

	public int GetScore(){
		return score;
	}

	public int GetPlayerID(){
		return playerID;
	}

	public float GetPowerLevel(){
		return powerLevel;
	}

	public void SetPlayerID(int id){
		playerID = id;
	}

	public void AddNearByPlanet(GameObject planet){
		nearbyPlanets.Add(planet);
	}

	public void RemoveNearByPlanet(GameObject planet){
		nearbyPlanets.Remove(planet);
	}

}
