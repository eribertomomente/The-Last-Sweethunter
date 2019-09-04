using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreBar : MonoBehaviour
{

    public Text scoreValue;
    public LobbyPlayer myPlayerInfo;
    private int currentScore;

    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        //GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");
        //foreach (GameObject player in lobbyPlayers)
        //{
        //    int characterIndex = player.GetComponent<LobbyPlayer>().characterIndex % 3;
        //    // TODO attenzione ricordarsi che probabilmente hasAuthority nin funzia bene
        //    if (player.GetComponent<NetworkIdentity>().hasAuthority)
        //    {
        //        myPlayerInfo = player.GetComponent<LobbyPlayer>();
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(currentScore != myPlayerInfo.localCharacterScore)
        {
            scoreValue.text = myPlayerInfo.localCharacterScore.ToString();
            currentScore = myPlayerInfo.localCharacterScore;
        }
         
    }
}
