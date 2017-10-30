using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameNotifier : MonoBehaviour {
	private Image image;
	public GameObject gameOverText;
	public GameObject[] texts;

	private float maxAlpha;
	public float imageFadeDuration, textFadeDuration, gameOverTextDisplayDuration;
	public AudioClip countdownSound, finishSound;

	// Use this for initialization
	void Awake () {
		image = this.GetComponent<Image>();
		gameOverText = this.transform.Find("Game Over").gameObject;

		maxAlpha = image.color.a;
		ResetImageText();
		ResetGameOverText();
	}


	// slowly fade in the image and the text at the designated speed, one by one
	public IEnumerator DisplayNotification(){
		// first enable the images and texts back up
		image.enabled = true;
		foreach (GameObject textObj in texts){
			textObj.SetActive(true);
		}
		// dummy variable used to help set the alpha of color
		Color newColor;
		
		float imageFadeSpeed = Mathf.Abs(maxAlpha - image.color.a) / imageFadeDuration;
		float textFadeSpeed = 1f / textFadeDuration;

		// first fade in image color
		while(!Mathf.Approximately(image.color.a, maxAlpha)){
			newColor = image.color;
			newColor.a = Mathf.MoveTowards(image.color.a, maxAlpha, imageFadeSpeed * Time.deltaTime);;
			image.color = newColor;
			yield return null;
		}

		// then fade in the texts one by one
		foreach(GameObject textObj in texts){
			while(!Mathf.Approximately(textObj.GetComponentInChildren<Text>().color.a, 1f)){
				foreach(Text text in textObj.GetComponentsInChildren<Text>()){
					newColor = text.color;
					newColor.a = Mathf.MoveTowards(text.color.a, 1f, textFadeSpeed * Time.deltaTime);
					text.color = newColor;
				}
				yield return null;
			}
		}
	}


	// shows the Game Over Text
	public IEnumerator DisplayGameOverText(){
		gameOverText.SetActive(true);
		yield return new WaitForSeconds(gameOverTextDisplayDuration);
		ResetGameOverText();
	}

	// sets the image and texts to their default hidden state and then disable them
	private void ResetImageText(){
		Color imageColor = image.color;
		imageColor.a = 0f;
		image.color = imageColor;
		image.enabled = false;

		foreach(GameObject textObj in texts){
			foreach(Text text in textObj.GetComponentsInChildren<Text>()){
				Color c = text.color;
				c.a = 0f;
				text.color = c;
			}
			textObj.SetActive(false);
		}
	}

	// sets the gameOverText to its default hidden state and disable it
	private void ResetGameOverText(){
		gameOverText.SetActive(false);
	}
}
