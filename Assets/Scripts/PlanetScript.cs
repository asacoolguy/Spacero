using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {
	public float size;
	public float rotationSpeed = 10f; // number of degrees the planet rotates every second
	public float explosionSpeed = 5f; // speed the planet adds to the players on explosion
	public Sprite regular, explosion;

	// Use this for initialization
	void Start () {
		size = this.GetComponent<CircleCollider2D> ().radius * this.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		// rotate the planet
		transform.Rotate (0, 0, Time.deltaTime * rotationSpeed);
	}

	// if a player enters the range, set it to land on the planet
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerScript>().landOnPlanet(this);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			//other.gameObject.GetComponent<PlayerScript>().leavePlanet();
		}
	}

	// returns true if no player is currently in it
	public bool canSpawn(){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject obj in players) {
			float xDist = obj.transform.position.x - this.transform.position.x;
			float yDist = obj.transform.position.y - this.transform.position.y;
			float safeDistance = this.transform.GetComponent<CircleCollider2D>().radius + obj.GetComponent<BoxCollider2D>().size.y;
			if (Mathf.Sqrt(xDist * xDist + yDist * yDist) < safeDistance){
				return false;
			}
		}

		return true;
	}

	// if i = 0 its inactive, if i = 1 it's active
	public void changeSprite(int i){
		if (i == 0) {
			GetComponent<SpriteRenderer>().sprite = explosion;
		}
		else{
			GetComponent<SpriteRenderer>().sprite = regular;
		}
	}
}
