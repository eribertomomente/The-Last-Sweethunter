using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelSelectionManager : NetworkBehaviour
{
    public LobbyManager lobbyManager;
    private LobbyPlayer lobbyPlayer;

    public GameObject minigameSelection;
    public GameObject candyMachine;
    public GameObject lobbyPanel;
    public GameObject background;
    public GameObject title;

    private List<LobbyPlayer> players;

    private List<LobbyPlayer> electedPlayers;

    private void Start()
    {
        if (LobbyManager.s_Singleton.isFirstTime)
        {
            lobbyPlayer = new LobbyPlayer();
        }

    }
    public void AllowSelectionPanel()
    {
        if (!isServer)
            return;

        lobbyManager = LobbyManager.s_Singleton;

        RpcResetElection();

        LobbyPlayer randomPlayer = new LobbyPlayer();
        bool foundElected = false;

        while (!foundElected)
        {
            randomPlayer = (LobbyPlayer)lobbyManager.lobbySlots[0];//Random.Range(0, lobbyManager.numPlayers)];
            foundElected = !randomPlayer.pastElected && !randomPlayer.isElected;
        }


        RpcElectionPlayer(randomPlayer.netId.Value);

        RpcAllowSelectionPanel();
    }

    [ClientRpc]
    private void RpcResetElection()
    {
        for (int i = 0; i < lobbyManager.lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbyManager.lobbySlots[i] as LobbyPlayer;

            if (p != null)
            {
                p.isElected = false;
            }
        }
    }


    [ClientRpc]
    public void RpcElectionPlayer(uint playerElected)
    {

        //GameObject[] playerInfos = GameObject.FindGameObjectsWithTag("PlayerInfo");

        foreach (LobbyPlayer lp in lobbyManager.lobbySlots)
        {
            if (lp != null)
            {
                if (lp.netId.Value == playerElected)
                {
                    lp.isElected = lobbyPlayer.pastElected = true;
                }
            }
        }

    }


    [ClientRpc]
    public void RpcAllowSelectionPanel()
    {
        bool value = false;


        foreach (LobbyPlayer lp in lobbyManager.lobbySlots)
        {
            if (lp != null)
            {
                if (lp.isLocalPlayer && lp.isElected)
                {

                    value = true;
                }
            }
        }

        background.SetActive(false);
        title.SetActive(false);
        minigameSelection.SetActive(value);
        candyMachine.SetActive(!value);
    }

    //Funzione per richiamare il metodo del Lobby player per il caricamento della scena del minigioco 
   /* public void PlayScene()
    {
        for (int i = 0; i < lobbyManager.lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbyManager.lobbySlots[i] as LobbyPlayer;

            if (p != null && p.isLocalPlayer)
            {
                p.PlayScene();
            }
        }
    }*/

    //Funzione per la chisura dei pannelli del minigioco e candy machine
    public void Close()
    {

        if (isServer) RpcClose();
        else CmdClose();

    }

    [Command]
    public void CmdClose()
    {
        minigameSelection.SetActive(false);
        candyMachine.SetActive(false);

        RpcClose();
    }

    [ClientRpc]
    public void RpcClose()
    {
        minigameSelection.SetActive(false);
        candyMachine.SetActive(false);

       
    }

    public void Open()
    {
        if (isServer)
        {
            RpcOpen();
            LobbyManager.s_Singleton.isNewMinigame = true;
        }
    }

    [ClientRpc]
    public void RpcOpen()
    {
        if (isServer)
        {
            lobbyPanel.SetActive(true);
            minigameSelection.SetActive(true);
            LobbyManager.s_Singleton.Support_FindLocalPlayerSelectionIndex();
            lobbyPanel.GetComponent<CanvasGroup>().alpha = 0;
            //LobbyManager.s_Singleton.topPanel.gameObject.SetActive(false);
            //LobbyManager.s_Singleton.lobbyPanel.parent.gameObject.SetActive(false);
            //Debug.Log("ho trovato miniature null? " + (GameObject.Find("Miniature").GetComponent<Image>() == null).ToString());
            //Debug.Log("L'id del playerinfo e': " + GameObject.FindGameObjectWithTag("PlayerInfo").GetComponent<NetworkIdentity>().netId);
            //GameObject.Find("Miniature").GetComponent<Image>().enabled = false;
            //GameObject.Find("ReadyButton").GetComponent<Image>().enabled = false;
            //GameObject.Find("PlayerName").GetComponent<Image>().enabled = false;

        }
        else
        {
            candyMachine.SetActive(true);
            lobbyPanel.SetActive(true);

        }
    }


    public void ManagePanel()
    {
        if (LobbyManager.s_Singleton.isFirstTime || LobbyManager.s_Singleton.isNewMinigame )
        {
            Close();
            LobbyManager.s_Singleton.isFirstTime = false;
            LobbyManager.s_Singleton.isNewMinigame = false;
        }
        else
        {
            Open();
            LobbyManager.s_Singleton.isNewMinigame = true;
        }

        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        {
            if (lp != null) lp.localCharacterScore = 0;
            
        }

        lobbyPlayer.numPlayedMinigames += 1;

        
    }

}