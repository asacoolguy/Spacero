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

	void Start () {
		// sets up all the GUI elements
		text = transform.Find("score").gameObject.GetComponent<Text>();
		slider = transform.Find("powerSliders").gameObject.GetComponent<Slider>();
		player = playerObj.GetComponent<PlayerScript>();
		jumpButton = transform.Find("Jump Button").gameObject.GetComponent<Button>();
		actionButton = transform.Find("Action Button").gameObject.GetComponent<Button>();

		jumpButton.onClick.AddListener(() => player.LeavePlanet());
		actionButton.onClick.AddListener(() => player.ActivatePlayerAction());

		DisableHUDInteraction();
	}

	// Update is called once per frame
	void Update () {
		text.text = "P" + (player.GetPlayerID() + 1) + " score: " + player.GetScore();
		slider.value = Mathf.MoveTowards(slider.value, player.GetPowerLevel(), 5f);

		if (player.GetIsDead() == false && player.GetIsLanded() && player.GetCanJump()){
			EnableHUDInteraction();
		}
		else{
			DisableHUDInteraction();
		}
	}

	public void DisableHUDInteraction(){
		jumpButton.interactable = false;
		actionButton.interactable = false;
	}

	public void EnableHUDInteraction(){
		jumpButton.interactable = true;
		actionButton.interactable = true;
	}

	public void DisableHUDOnGameOver(){
		jumpButton.enabled = false;
		actionButton.enabled = false;
	}
}
