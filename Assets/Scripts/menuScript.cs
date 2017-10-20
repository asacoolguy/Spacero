using UnityEngine;
using System.Collections;

public class menuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// handles reset button
		if (Input.GetKeyDown (KeyCode.X)) {
			Application.LoadLevel("Diagon");
		}
	}
}
