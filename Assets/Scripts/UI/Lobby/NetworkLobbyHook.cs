using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
	
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		LobbyPlayer playerInfo = lobbyPlayer.GetComponent<LobbyPlayer>();

		//gamePlayer.GetComponent<PlayerStatus> ().playerName = playerInfo.txtPlayerName.text;
		//Destroy (gamePlayer);
	}
}
