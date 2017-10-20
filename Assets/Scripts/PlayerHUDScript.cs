using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDScript : MonoBehaviour {
	public GameObject playerObj;
	private PlayerScript player;
	private Slider slider;
	private Text text;
	private Button jumpButton, actionButton;

	// Use this for initialization
	void Start () {
		text = transform.Find("score").gameObject.GetComponent<Text>();
		slider = transform.Find("powerSliders").gameObject.GetComponent<Slider>();
		player = playerObj.GetComponent<PlayerScript>();
		jumpButton = transform.Find("Jump Button").gameObject.GetComponent<Button>();
		actionButton = transform.Find("Action Button").gameObject.GetComponent<Button>();

		jumpButton.onClick.AddListener(() => player.leavePlanet());
		actionButton.onClick.AddListener(() => player.activatePlayerAction());

		disableHUDInteraction();
	}

	// Update is called once per frame
	void Update () {
		text.text = "P" + player.ID + " score: " + player.score;
		slider.value = Mathf.MoveTowards(slider.value, player.powerLevel, 5f);

		if (player.isDead == false && player.landed && player.canJump){
			enableHUDInteraction();
		}
		else{
			disableHUDInteraction();
		}
	}

	public void disableHUDInteraction(){
		jumpButton.interactable = false;
		actionButton.interactable = false;
	}

		public void enableHUDInteraction(){
		jumpButton.interactable = true;
		actionButton.interactable = true;
	}

	public void disableHUDOnGameOver(){
		jumpButton.enabled = false;
		actionButton.enabled = false;
	}
}
