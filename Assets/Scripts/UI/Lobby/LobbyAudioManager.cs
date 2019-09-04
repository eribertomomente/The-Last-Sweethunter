using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAudioManager : MonoBehaviour {

	public string pathToPlayerAudioResources;
	public string pathToCommAudioResources;

	[Header ("Players clip")]
	public AudioSource playerAudioSource;

	public float commClipStep;
	private float commClipTimer = 0f;

	private void Update () {
		if (commClipTimer >= commClipStep) {
			PlayCommentatorClip ();
			commClipTimer = 0f;
		}

		commClipTimer += Time.deltaTime;
	}

	public void PlayClipOnSelect (int i) {
		if (playerAudioSource.isPlaying)
			playerAudioSource.Stop ();
		
		switch (i) {
		case 0:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickPanda", typeof(AudioClip));
			break;
		case 1:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickFox", typeof(AudioClip));
			break;
		case 2:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickWolf", typeof(AudioClip));
			break;
		case 3:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickBunny", typeof(AudioClip));
			break;
		case 4:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickRaven", typeof(AudioClip));
			break;
		case 5:
			playerAudioSource.clip = (AudioClip) Resources.Load (pathToPlayerAudioResources + "pickCyborg", typeof(AudioClip));
			break;
		default:
			return;
			break;
		}

		playerAudioSource.Play ();
	}

	private void PlayCommentatorClip () {
		if (playerAudioSource.isPlaying)
			return;
		
		float rand = Random.value;

		if (rand < 0.33f)
			playerAudioSource.clip = (AudioClip)Resources.Load (pathToCommAudioResources + "commIntro01", typeof(AudioClip));
		else if (rand >= 0.33f && rand < 0.66f)
			playerAudioSource.clip = (AudioClip)Resources.Load (pathToCommAudioResources + "commIntro02", typeof(AudioClip));
		else
			playerAudioSource.clip = (AudioClip)Resources.Load (pathToCommAudioResources + "commIntro04", typeof(AudioClip));

		playerAudioSource.Play ();
	}
}
