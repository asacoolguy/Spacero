using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameNotifier : MonoBehaviour {
	private Image image;
	public GameObject[] texts;
	public GameObject countdown;

	private float maxAlpha;
	public float imageFadeDuration, textFadeDuration, displayDuration, secondsPerCount;
	public AudioClip countdownSound, startSound;

	// Use this for initialization
	void Awake () {
		image = this.GetComponent<Image>();

		maxAlpha = image.color.a;
		ResetImageText();
		ResetCountdown();
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

		// show the info for a set amount of seconds
		yield return new WaitForSeconds(displayDuration);
	}


	// slowly fade out the image and the text at the designated speed, all together
	public IEnumerator HideNotification(){
		float imageFadeSpeed = Mathf.Abs(0f - image.color.a) / imageFadeDuration;
		float textFadeSpeed = 1f / textFadeDuration;
		// dummy variable used to set up colors
		Color newColor;

		while(!Mathf.Approximately(image.color.a, 0f)){
			newColor = image.color;
			newColor.a = Mathf.MoveTowards(image.color.a, 0f, imageFadeSpeed * Time.deltaTime);
			image.color = newColor;
			foreach (GameObject textObj in texts){
				foreach(Text text in textObj.GetComponentsInChildren<Text>()){
					newColor = text.color;
					newColor.a = Mathf.MoveTowards(text.color.a, 0f, textFadeSpeed * Time.deltaTime);
					text.color = newColor;
				}
			}

			yield return null;
		}

		// make sure the values are at the target and disable them
		ResetImageText();
	}


	// shows the countdown animaion
	public IEnumerator DisplayCountdown(){
		countdown.SetActive(true);

		for (int i = 3; i > 0; i--){
			foreach(Text text in countdown.GetComponentsInChildren<Text>()){
				text.text = i.ToString();
			}
			this.GetComponent<AudioSource>().PlayOneShot(countdownSound);
			yield return new WaitForSeconds(secondsPerCount);
		}

		foreach(Text text in countdown.GetComponentsInChildren<Text>()){
			text.text = "GO";
		}
		this.GetComponent<AudioSource>().PlayOneShot(startSound);

		yield return new WaitForSeconds(secondsPerCount);
		ResetCountdown();
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

	// sets the countdown to its default hidden state and disable it
	private void ResetCountdown(){
		countdown.SetActive(false);
	}
}
