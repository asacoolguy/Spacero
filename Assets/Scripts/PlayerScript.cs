using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {
	public enum actionButton{
		space, backspace
	};

	public actionButton button;
	KeyCode actionKey;

	// basic info
	public int ID;
	public PlanetScript currentPlanet;
	public Vector2 speed = new Vector2(0f, 0f);
	// basic states
	public bool landed = false;
	public bool canJump = false;
	// respawn variables
	public GameObject respawnMarker;
	public bool isDead = false;
	public float respawnTime = 4f;
	public float respawnTimeCount = 0;
	public int coinsLostOnDeath = 3;
	// scores and bars
	public int score = 0;
	public float powerLevel = 25f;
	public float powerLevelDecayPerSecond = 10f;
	public float powerLevelUsedOnJump = 20f;
	public ManagerScript mscript;
	// bossting stuff
	public float boostDuration = 20f;
	private float currentBoostDuration;
	public bool canBoost = true;
	public bool boosting = false;
	public float boostStrength = 1f;

	public ArrayList nearbyPlanets = new ArrayList();

	public AudioClip coinSound, landingSound, leavingSound, deathSound, flipSound, boostSound, chargedSound, respawnSound, explosionSound;

	// Use this for initialization
	void Start () {
		if (button == actionButton.backspace)
			actionKey = KeyCode.Backspace;
		else
			actionKey = KeyCode.Space;

		mscript = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<ManagerScript> ();
		currentBoostDuration = boostDuration;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDead){ 
			if (!landed){ // if player is drifting, it moves based on planets' gravities
				GetComponent<Rigidbody2D>().velocity += calculateGravityPull();
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
			}
			else{ // if player has landed, then it has no velocity and its rotation is that of the planet
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				landingRotate();

				if (button == actionButton.backspace && Input.GetKeyDown (KeyCode.Return)) {
					currentPlanet.rotationSpeed *= -1;
					GetComponent<AudioSource>().PlayOneShot (flipSound);
				}

				// increase power level when landed
				powerLevel += currentPlanet.size * 8 * Time.deltaTime;
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
		else{ // if player is dead, decrease the countdown timer, lock its location
			respawnTimeCount -= Time.deltaTime;
			this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.position = respawnMarker.transform.position;
			transform.rotation = respawnMarker.transform.rotation;

			if (respawnTimeCount < 0){
				respawn();
			}
		}
	}

	// if collide with another player, compares scores and destroy the player with less points
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			PlayerScript otherPlayer = other.gameObject.GetComponent<PlayerScript>();
			if (other.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude >
				this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude){
				suicide();
			}
			else{
				otherPlayer.suicide();
			}
		}
	}
	
	// helper function that lands the player onto a planet given the planet's script
	public void landOnPlanet(PlanetScript pscript){
		if (landed == false){
			currentPlanet = pscript;
			//pscript.addLandedPlayer(this.gameObject);
			landed = true;
			canBoost = false;
			GetComponent<AudioSource>().PlayOneShot (landingSound);
			transform.parent = pscript.gameObject.transform;
			nearbyPlanets.Remove (currentPlanet.gameObject);
			// rotate the player
			landingRotate ();
		}
	}

	// sends player off a planet and blows it up
	public void leavePlanet(){
		if (landed && canJump){
			landed = false;
			canBoost = true;
			transform.parent = null;
			GetComponent<AudioSource>().PlayOneShot (leavingSound);
			GetComponent<AudioSource>().PlayOneShot (explosionSound);

			Vector2 temp = new Vector2 (transform.position.x - currentPlanet.transform.position.x,
			                            transform.position.y - currentPlanet.transform.position.y).normalized;
			GetComponent<Rigidbody2D>().velocity = temp * currentPlanet.explosionSpeed;

			// take away player power
			powerLevel -= powerLevelUsedOnJump;

			//print (rigidbody2D.velocity.x + ", " + rigidbody2D.velocity.y);
			PlayerScript otherPlayer;
			if (ID == 1) {
				otherPlayer = mscript.player2.GetComponent<PlayerScript>();
			}
			else{
				otherPlayer = mscript.player1.GetComponent<PlayerScript>();
			}

			// kills the other player if they're on this planet too
			if (otherPlayer.isDead == false && otherPlayer.currentPlanet &&
			    otherPlayer.currentPlanet.gameObject == this.currentPlanet.gameObject){
				otherPlayer.suicide();
			}

			nearbyPlanets.Remove (currentPlanet.gameObject);
			mscript.deactivate (currentPlanet.gameObject);
			currentPlanet = null;		
		}
	}

	// activate the player's action
	// only boost implemented for now
	public void activatePlayerAction(){
		if (canBoost){
			boosting = true;
			GetComponent<AudioSource>().PlayOneShot (boostSound);
			canBoost = false;
			StartCoroutine(actionBoost());
		}
	}

	IEnumerator actionBoost(){
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
	}

	// rotate the player to face the planet
	private void landingRotate(){
		float angle;
		Vector2 temp = new Vector2 (currentPlanet.transform.position.x - transform.position.x,
		                           currentPlanet.transform.position.y - transform.position.y);
		if (temp.x == 0) {
			if (temp.y > 0)
				angle = 0;
			else{
				angle = 180;
			}
		}
		else{
			angle =  Mathf.Atan2 (temp.y, temp.x) * Mathf.Rad2Deg + 90;
			//print (temp);
			//print (angle);
		}

		transform.eulerAngles = new Vector3 (0, 0, angle);
	}

	public bool isLanded(){
		return landed;
	}

	// disable the player's components and starts the respawn timer
	public void suicide(){
		isDead = true;
		currentPlanet = null;
		powerLevel = 0f;
		nearbyPlanets.Clear();
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
		if (ID == 1){
			mscript.player2.GetComponent<PlayerScript>().score += coinsTaken;
		}
		else{
			mscript.player1.GetComponent<PlayerScript>().score += coinsTaken;
		}

	}

	// respawn the player by reactivating its components
	public void respawn(){
		isDead = false;
		landed = false;
		canJump = false;
		powerLevel = 25f;
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<AudioSource>().PlayOneShot(respawnSound);
	}

	private Vector2 calculateGravityPull(){
		Vector2 final = Vector2.zero;
		//float G = 6.67300f * 1f;
		//float M; // planet mass
		// GM/R2
		float size;
		float mposx = this.transform.position.x;
		float mposy = this.transform.position.y;
		float xpos, ypos, dx, dy, distance, dx2, dy2, dxwrap, dywrap;	
		foreach(GameObject planet in nearbyPlanets) {
			if (planet && planet.activeSelf){
				size = planet.GetComponent<PlanetScript>().size * 10f;
				xpos = planet.transform.position.x;
				ypos = planet.transform.position.y;
				dx = xpos - mposx;
				dy = ypos - mposy;
				dx2 = dx * dx;
				dy2 = dy * dy;
				dxwrap = xpos+mposx + 80;
				dywrap = ypos+mposy + 60;
				if(dxwrap*dxwrap <dx2){
					dx2=dxwrap*dxwrap;
				}
				if(dywrap*dywrap <dy2){
					dy2=dywrap*dywrap;
				}
				distance = dy2 + dx2;
				Vector2 update = new Vector2(dx,dy);
				final+=(size/distance)*update.normalized;
			}
		}
		
		return final;
	}

	public void playCoinSound(){
		GetComponent<AudioSource>().PlayOneShot (coinSound);
	}

}
