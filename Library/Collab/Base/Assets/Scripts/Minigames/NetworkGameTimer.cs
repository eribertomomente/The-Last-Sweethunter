using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkGameTimer : NetworkBehaviour
{
    [SyncVar] public float gameTime; //The length of a game, in seconds.
    [SyncVar] private float timer = -1; //How long the game has been running. -1=waiting for players, -2=game is done
    [SyncVar] public int minPlayers; //Number of players required for the game to start
    [SyncVar] private bool masterTimer = false; //Is this the master timer?

    [SerializeField] private Text timeLabel;

    NetworkGameTimer serverTimer;

    void Start()
    {

        /*****************************
         ***************************** 
         ***************************** 
         ***************************** 
         ***************************** 
         ***************************** 
         ****************************/
        Debug.Log("inizio");
        GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");
        Debug.Log(lobbyPlayers.Length);
        foreach (GameObject player in lobbyPlayers)
        {
            Debug.Log("#########" + player.ToString());
            //if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
            //    RpcUpdateName(player.GetComponent<LobbyPlayer>().txtPlayerName.text);
        }
        /*****************************
         ***************************** 
         *****************************  
         ***************************** 
         ***************************** 
         ***************************** 
         ****************************/

        if (isServer)
        { // For the host to do: use the timer and control the time.
           
                serverTimer = this;
                masterTimer = true;
          
        }
        else if (isLocalPlayer)
        { //For all the boring old clients to do: get the host's timer.
            NetworkGameTimer[] timers = FindObjectsOfType<NetworkGameTimer>();
            
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].masterTimer)
                {
                    serverTimer = timers[i];
                }
            }
        }
    }
    void Update()
    {
        if (masterTimer)
        { //Only the MASTER timer controls the time
            if (timer >= gameTime)
            {
                timer = -2;
            }
            // mancano ancora giocatori da connettere
            else if (timer == -1)
            {
                if (NetworkServer.connections.Count >= minPlayers)
                {
                    timer = 0;
                }
            }
            // il gioco e' finito
            else if (timer == -2)
            {


                /**********************************************
                 *  SETTARE isActive(true) al canvas di fine partita
                 **********************************************/


                Debug.Log("Gioco finito");
                Debug.Log("Hai raccolto: \n" +
                        "\t" + "Caramelle Basic   " + Score.numBasic + "\n" +
                        "\t" + "Lollipop   " + Score.numLollipop + "\n" +
                        "\t" + "Praline   " + Score.numPraline + "\n" +
                        "\t" + "Licorice   " + Score.numLicorice + "\n" +
                        "\t" + "Gummy Bears   " + Score.numGummyBear + "\n" +
                        "\t" + "Sugar Cane   " + Score.numSugarCane + "\n" +
                        "\t" + "Marshmallow   " + Score.numMarshmallow + "\n" +
                        "\t" + "Macaron   " + Score.numMacaron + "\n" +
                        "\t" + "Donut   " + Score.numDonut + "\n" +
                        "\t" + "Cupacke   " + Score.numCupcake + "\n"
                        );
            }
            else
            {
                timer += Time.deltaTime;
                timeLabel.text = System.Math.Round(gameTime - timer).ToString();

            }
        }

        if (isLocalPlayer)
        { //EVERYBODY updates their own time accordingly.
            if (serverTimer)
            {
                gameTime = serverTimer.gameTime;
                timer = serverTimer.timer;
                minPlayers = serverTimer.minPlayers;
            }
            else
            { //Maybe we don't have it yet?
                NetworkGameTimer[] timers = FindObjectsOfType<NetworkGameTimer>();
                for (int i = 0; i < timers.Length; i++)
                {
                    if (timers[i].masterTimer)
                    {
                        serverTimer = timers[i];
                    }
                }
            }
        }
    }
}