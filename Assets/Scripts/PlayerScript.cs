using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {
	public enum actionButton{
		space, backspace
	};

	public actionButton button;
	KeyCode actionKey;

	public int ID;
	public PlanetScript currentPlanet;
	public bool landed = false;
	public Vector2 speed = new Vector2(0f, 0f);

	public int score = 0;
	public float powerLevel = 0f;
	public float powerLevelDecayPerSecond = 20f;
	public ManagerScript mscript;

	public float boostDuration = 20f;
	public float currentBoostDuration;
	public bool canBoost = true;
	public bool boosting = false;
	public float boostStrength = 1f;

	public ArrayList nearbyPlanets = new ArrayList();

	public AudioClip coinSound, landingSound, leavingSound, collisionSound, flipSound, boostSound;

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
		if (!landed){
			// calculate planet's affect on this player
			GetComponent<Rigidbody2D>().velocity += calculateGravityPull();
			// decrease power level
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
		else{
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			if (button == actionButton.backspace && Input.GetKeyDown (KeyCode.Return)) {
				currentPlanet.rotationSpeed *= -1;
				GetComponent<AudioSource>().PlayOneShot (flipSound);
			}
		}
	}

	// if collide with another player, compares scores and destroy the player with less points
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			GetComponent<AudioSource>().PlayOneShot (collisionSound);
			if (other.gameObject.GetComponent<PlayerScript>().powerLevel > this.powerLevel){
				mscript.playerWin(other.gameObject);
				Destroy(this.gameObject);
			}
			else if (other.gameObject.GetComponent<PlayerScript>().powerLevel < this.powerLevel){
				mscript.playerWin(this.gameObject);
				Destroy(other.gameObject);
			}
		}
	}
	
	// helper function that lands the player onto a planet given the planet's script
	public void landOnPlanet(PlanetScript pscript){
		if (landed == false){
			currentPlanet = pscript;
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
		if (landed){
			landed = false;
			canBoost = true;
			transform.parent = null;
			GetComponent<AudioSource>().PlayOneShot (leavingSound);

			Vector2 temp = new Vector2 (transform.position.x - currentPlanet.transform.position.x,
			                            transform.position.y - currentPlanet.transform.position.y).normalized;
			GetComponent<Rigidbody2D>().velocity = temp * currentPlanet.explosionSpeed;

			// give player power
			powerLevel += currentPlanet.size * 10;
			print(powerLevel);

			//print (rigidbody2D.velocity.x + ", " + rigidbody2D.velocity.y);
			if (ID == 1) {
				if (mscript.player2 &&
				    mscript.player2.GetComponent<PlayerScript>().currentPlanet &&
				    mscript.player2.GetComponent<PlayerScript>().currentPlanet.gameObject == this.currentPlanet.gameObject){
					mscript.playerWin(this.gameObject);
				}
			}
			else{
				if (mscript.player1 &&
				    mscript.player1.GetComponent<PlayerScript>().currentPlanet &&
				    mscript.player1.GetComponent<PlayerScript>().currentPlanet.gameObject == this.currentPlanet.gameObject){
					mscript.playerWin(this.gameObject);
				}
			}

			nearbyPlanets.Remove (currentPlanet.gameObject);
			//Destroy (currentPlanet.gameObject);
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
