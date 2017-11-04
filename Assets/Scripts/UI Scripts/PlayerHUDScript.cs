using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDScript : MonoBehaviour {
	public GameObject playerObj;
	public GameObject floatingText;
	private PlayerScript player;
	private PlayerAudioScript playerAudio;

	private Slider slider;
	private Text scoreText, timerText;
	private Button jumpButton, actionButton;

	public bool isHUDActive;

	// charge variables
	private float jumpButtonHeldTime;
	private bool jumpButtonHeld;
	private bool jumpButtonHeldMaxedOut;

	void Awake () {
		// sets up all the GUI elements
		scoreText = transform.Find("Score").gameObject.GetComponent<Text>();
		timerText = transform.Find("Timer").gameObject.GetComponent<Text>();
		slider = transform.Find("Power Slider").gameObject.GetComponent<Slider>();
		jumpButton = transform.Find("Jump Button").gameObject.GetComponent<Button>();
		actionButton = transform.Find("Action Button").gameObject.GetComponent<Button>();

		player = playerObj.GetComponent<PlayerScript>();
		player.SetPlayerHUD(this);

		jumpButtonHeld = false;
		jumpButtonHeldTime = 0f;
		jumpButtonHeldMaxedOut = false;

		slider.maxValue = player.maxChargeTime;

		//EnableHUDInteraction(false);
		isHUDActive = false;
	}

	void Start(){
		playerAudio = player.GetPlayerAudio();
	}

	// Update is called once per frame
	void Update () {
		UpdateScoreText(player.GetScore());
		UpdateSliderValue();


		if (isHUDActive && player.GetIsDead() == false && player.GetIsLanded() && player.GetCanJump()){
			EnableHUDInteraction(true);
		}
		else{
			EnableHUDInteraction(false);
		}

		if (jumpButtonHeld){
			jumpButtonHeldTime += Time.deltaTime;
		}
		if (jumpButtonHeld && jumpButtonHeldMaxedOut == false && jumpButtonHeldTime > slider.maxValue){
			jumpButtonHeldMaxedOut = true;
			playerAudio.PlayMaxedChargingSound();
		}

	}

	public void UpdateScoreText(int score){
		scoreText.text = "Coins collected: " + score;
	}

	public void UpdateTimerText(float time){
		if (time < 0){
			time = 0;
		}
		timerText.text = "Time Remaining: " + Mathf.FloorToInt(time);
	}

	public void UpdateSliderValue(){
		slider.value = Mathf.MoveTowards(slider.value, jumpButtonHeldTime, 1f);
	}

	public void StartJumpButtonPress(){
		if (isHUDActive && player.GetCanJump()){
			jumpButtonHeld = true;
			playerAudio.PlayChargingSound();
		}
	}

	public void EndJumpButtonPress(){
		if (isHUDActive && player.GetCanJump()){
			player.LeavePlanet(jumpButtonHeldTime);
			ResetChargeButton();
		}
	}

	public void EnableHUDInteraction(bool b){
		jumpButton.interactable = b;
		actionButton.interactable = b;

		// stop charging when the HUD is disabled
		if (b == false){
			ResetChargeButton();
		}
	}

	public void ShowFloatingText(int value, Vector3 position){
		Vector3 rotation = Vector3.zero;
		if (player.GetPlayerID() == 2){
			rotation.z = 180;
		}
		string s = "+" + value + "pt";
		if (value > 1){
			s += "s";
		}

		Color c = Color.yellow;
		if (value < 0){
			c = Color.red;
		}

		GameObject text = Instantiate(floatingText, Vector3.zero, Quaternion.identity) as GameObject;
		text.transform.SetParent(transform.parent, false);
		text.transform.eulerAngles = rotation;
		text.GetComponent<RectTransform>().position = position;
		text.GetComponent<Text>().text = s;
		text.GetComponent<Text>().color = c;

		StartCoroutine(DestroyText(text, 1f));
	}

	private IEnumerator DestroyText(GameObject text, float seconds){
		yield return new WaitForSeconds(seconds);
		Destroy(text);
	}

	private void ResetChargeButton(){
		jumpButtonHeld = false;
		jumpButtonHeldMaxedOut = false;
		playerAudio.StopChargingSound();
		jumpButtonHeldTime = 0f;
	}

	public void SetIsHUDActive(bool b){
		isHUDActive = b;
	}
}
