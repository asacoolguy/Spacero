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
	}

	// Update is called once per frame
	void Update () {
		text.text = "P" + player.ID + " score: " + player.score;
		slider.value = Mathf.MoveTowards(slider.value, player.powerLevel, 10f);
	}
}
