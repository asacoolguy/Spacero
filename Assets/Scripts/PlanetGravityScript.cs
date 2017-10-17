using UnityEngine;
using System.Collections;

public class PlanetGravityScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerScript>().nearbyPlanets.Add(this.transform.parent.gameObject);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerScript>().nearbyPlanets.Remove(this.transform.parent.gameObject);
		}
	}

}
