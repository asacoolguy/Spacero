using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainSpaceScript : PlayerScript {

	// does the player's action. to be implement by each different player class
	public override void ActivatePlayerAction(){
		print ("player action done");
	}

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
}
