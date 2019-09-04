using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using System.Collections.Generic;

public class EndGameManager : NetworkBehaviour
{

    //public GameObject endMatchCanvas;
    private bool alreadySent;
    private bool[] readyList;

    public Transform spawnPoint = null;
    public GameObject scrollView;
    public GameObject playerResult;

    [SerializeField]
    private RectTransform content = null;

    private Text number;
    private Text username;
    private GameObject miniature;
    private Text points;

    void Start()
    {

        //setContent Holder Height;
        content.sizeDelta = new Vector2(0, LobbyManager.s_Singleton.numPlayers * 120);

        for (int i = 0; i < LobbyManager.s_Singleton.lobbySlots.Length; ++i)
        {
            LobbyPlayer p = LobbyManager.s_Singleton.lobbySlots[i] as LobbyPlayer;

            if (p != null)
            {
                //Altezza dello slot del playerResult
                float spawnY = i * 120;

                //Nuova posizione dello slot
                Vector3 pos = new Vector3(0, -spawnY, spawnPoint.position.z);

                //Spawnato uno slot player
                GameObject playerSlot = Instantiate(playerResult, pos, spawnPoint.rotation) as GameObject;

                playerSlot.transform.SetParent(spawnPoint, false);

                playerSlot.GetComponentsInChildren<Text>()[0].text = i+1+".";
                playerSlot.transform.GetComponentsInChildren<Image>()[1].sprite = p.imgMiniature.sprite;
                playerSlot.GetComponentsInChildren<Text>()[1].text = p.txtPlayerName.text;
                playerSlot.GetComponentsInChildren<Text>()[2].text = p.globalCharacterScore + "";


            }

        }
        //foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        //{
        //    if (lp != null)
        //    {
        //        lp.isReady = false;
        //        Debug.Log("messo falso");
        //    }
        //}

        //GameObject lm = GameObject.FindGameObjectWithTag("LobbyManager");
        //Debug.Log("lobbyslots trovati: " );
        //foreach (GameObject player in lobbyPlayers)
        //{
        //    if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
        //        Debug.Log("ciao vez");
        //}
        // For the host to do: use the timer and control the time.

        foreach ( GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ( go.GetComponent<SwipeDetector>() != null)
            {
                go.GetComponent<SwipeDetector>().enabled = false;
            }
            if (go.GetComponent<ChubbyTummySwipeDetector>() != null)
            {
                go.GetComponent<ChubbyTummySwipeDetector>().enabled = false;
            }
        }

        GameObject toDeleteButton = GameObject.Find("FireButton");
        if (toDeleteButton != null)
        {
            toDeleteButton.SetActive(false);
        }
        toDeleteButton = GameObject.Find("JumpButton");
        if (toDeleteButton != null)
        {
            toDeleteButton.SetActive(false);
        }


    }


    public void ReadyForNextMatch()
    {
        LobbyManager.s_Singleton.playScene = "MainMenu";
        //GameObject go = GameObject.Find("MinigamesSelection");
        //if (go != null)
        //{
        //    go.SetActive(true);
        //}
        //go = GameObject.Find("LobbyPanelContainer");
        //if (go != null)
        //{
        //    go.SetActive(true);
        //}

        LobbyManager.s_Singleton.PlayScene();
       
    }

    //[Command]
    //public void CmdReadyForNextMatch( int id)
    //{
    //    foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
    //    {
    //        if (lp != null && lp.netId.Value == id)
    //        {
    //            lp.isReady = true;
    //        }
    //    }
    //}

}