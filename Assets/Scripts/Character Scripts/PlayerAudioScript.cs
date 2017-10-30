using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dedicated to playing various sound effects for the player object
/// </summary>
public class PlayerAudioScript : MonoBehaviour {
	public AudioClip landingSound, leavingSound, deathSound, dashSound, chargedSound, respawnSound, orbSound, chargingSound, maxedChargingSound, collisionSound;
	private AudioSource audioSource, orbAudioSource, chargeAudioSource;

	private bool playingChargeSound;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		orbAudioSource = transform.Find("Orb Audio").GetComponent<AudioSource>();
		chargeAudioSource = transform.Find("Charge Audio").GetComponent<AudioSource>();
		chargeAudioSource.clip = chargingSound;
		chargeAudioSource.loop = true;
		playingChargeSound = false;
	}

	void Update(){
		if (playingChargeSound && chargeAudioSource.isPlaying == false){
			chargeAudioSource.Play();
		}
		else if(playingChargeSound == false){
			chargeAudioSource.Stop();
		}
	}

	public void PlayLandingSound(){
		audioSource.PlayOneShot(landingSound);
	}

	public void PlayLeavingSound(){
		audioSource.PlayOneShot(leavingSound);
	}

	public void PlayCollisionSound(){
		audioSource.PlayOneShot(collisionSound);
	}

	public void PlayDeathSound(){
		audioSource.PlayOneShot(deathSound);
	}

	public void PlayRespawnSound(){
		audioSource.PlayOneShot(respawnSound);
	}

	public void PlayDashSound(){
		audioSource.PlayOneShot(dashSound);
	}

	public void PlayChargedSound(){
		audioSource.PlayOneShot(chargedSound);
	}

	public void PlayChargingSound(){
		playingChargeSound = true;
	}

	public void PlayMaxedChargingSound(){
		chargeAudioSource.clip = maxedChargingSound;
		chargeAudioSource.pitch = 1.2f;
	}

	public void StopChargingSound(){
		chargeAudioSource.clip = chargingSound;
		chargeAudioSource.pitch = 1f;
		playingChargeSound = false;
	}

	public void PlayOrbSound(int combo){
		orbAudioSource.pitch = 1f + (combo - 1f) * 0.1f;
		orbAudioSource.PlayOneShot(orbSound);
	}
}
