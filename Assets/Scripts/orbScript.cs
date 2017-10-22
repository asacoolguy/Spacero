using UnityEngine;
using System.Collections;

public class OrbScript : MonoBehaviour {
	public int pointValue = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// if a player enters the range, set it to land on the planet
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerScript>().AcquiredOrb(pointValue);
			Destroy(this.gameObject);
		}
	}

}
