using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dedicated to playing various sound effects for the player object
/// </summary>
public class PlayerAudioScript : MonoBehaviour {
	public AudioClip landingSound, leavingSound, deathSound, dashSound, chargedSound, respawnSound, orbSound;
	private AudioSource audio;
	private AudioSource orbAudio;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		orbAudio = transform.Find("Orb Audio").GetComponent<AudioSource>();
	}

	public void PlayLandingSound(){
		audio.PlayOneShot(landingSound);
	}

	public void PlayLeavingSound(){
		audio.PlayOneShot(leavingSound);
	}

	public void PlayDeathSound(){
		audio.PlayOneShot(deathSound);
	}

	public void PlayRespawnSound(){
		audio.PlayOneShot(respawnSound);
	}

	public void PlayDashSound(){
		audio.PlayOneShot(dashSound);
	}

	public void PlayChargedSound(){
		audio.PlayOneShot(chargedSound);
	}

	public void PlayOrbSound(int combo){
		orbAudio.pitch = 1f + (combo - 1f) * 0.1f;
		orbAudio.PlayOneShot(orbSound);
	}
}
