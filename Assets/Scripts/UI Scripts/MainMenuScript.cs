using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {
	public GameObject stageSelectionBox;
	public GameObject mainMenuBox;

	// Use this for initialization
	void Start () {
		HideStageSelectionBox();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowStageSelectionBox(){
		stageSelectionBox.SetActive(true);
		mainMenuBox.SetActive(false);
	}

	public void HideStageSelectionBox(){
		stageSelectionBox.SetActive(false);
		mainMenuBox.SetActive(true);
	}
}
