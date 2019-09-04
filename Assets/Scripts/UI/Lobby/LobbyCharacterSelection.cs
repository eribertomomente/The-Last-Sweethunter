using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyCharacterSelection : MonoBehaviour {

	/*public Button btnPanda;
	public Button btnFox;
	public Button btnWolf;
	public Button btnBunny;
	public Button btnRaven;
	public Button btnCyborg;

	public LobbyAudioManager lobbyAudioManager;

	/*
	THIS IS A FIXED ORDERED ARRAY
	0. Panda
	1. Fox
	2. Wolf
	3. Bunny
	4. Raven
	5. Cyborg
	
	//public GameObject[] previewCharacters;

	//private GameObject chosenCharacter;
	private Sprite holdSprite;
	private Button holdButton;
	//private int characterIndex;

	void Start () {
		StartCoroutine (SetupMenuSceneButtons());
	}

	IEnumerator SetupMenuSceneButtons() {
		yield return new WaitForSeconds(0.3f);
		SetupListener (btnPanda, 0);
		SetupListener (btnFox, 1);
		SetupListener (btnWolf, 2);
		SetupListener (btnBunny, 3);
		SetupListener (btnRaven, 4);
		SetupListener (btnCyborg, 5);
	}
		
	void Update () {
		
	}

	private void SetupListener (Button button, int index) {
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener (delegate{SelectCharacter(button, index);});
	}

	void SelectCharacter(Button button, int index) {

		if (holdButton != null) {
			if (!button.Equals (holdButton)) {
				
				holdButton.GetComponent<Image> ().sprite = holdSprite;
				SendSelectionMessage (index);

			} else
				return;
		} else
			SendSelectionMessage (index);

		holdSprite = button.GetComponent<Image> ().sprite;
		holdButton = button;
		//chosenCharacter = previewCharacters [index];
		button.GetComponent<Image> ().sprite = button.spriteState.highlightedSprite;

		//chosenCharacter.gameObject.GetComponent<StayInPlace> ().ResetPosition ();

		if (button.GetComponent<Image> ().color.a == 0.3f) {
			SendSelectionMessage (-1);
		}

		lobbyAudioManager.PlayClipOnSelect(index);
	}

	private void SendSelectionMessage(int index) {
		GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag ("PlayerInfo");

		foreach (GameObject player in lobbyPlayers) {

			LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer> ();

			if (lobbyPlayer.isLocalPlayer)
				lobbyPlayer.SelectCharacter (index);
		}
	}*/

}
