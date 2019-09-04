using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class LevelSelectionManager : NetworkBehaviour
{
    public LobbyManager lobbyManager;

    public GameObject minigameSelection;
    public GameObject candyMachine;
    public GameObject background;

    private List<LobbyPlayer> players;

    private List<LobbyPlayer> electedPlayers;
    
     
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
            randomPlayer = (LobbyPlayer)lobbyManager.lobbySlots[1];//Random.Range(0, lobbyManager.numPlayers)];
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

        GameObject[] playerInfos = GameObject.FindGameObjectsWithTag("PlayerInfo");

        foreach (GameObject playerInfo in playerInfos)
        {
            LobbyPlayer lobbyPlayer = playerInfo.GetComponent<LobbyPlayer>();

            if (lobbyPlayer.netId.Value == playerElected)
            {
                lobbyPlayer.isElected = lobbyPlayer.pastElected = true;
            }
        }

    }


    [ClientRpc]
    public void RpcAllowSelectionPanel()
    {
        bool value = false;

        GameObject[] playerInfos = GameObject.FindGameObjectsWithTag("PlayerInfo");

        foreach (GameObject playerInfo in playerInfos)
        {
            LobbyPlayer lobbyPlayer = playerInfo.GetComponent<LobbyPlayer>();
            print(lobbyPlayer.isElected);
            if (lobbyPlayer.isLocalPlayer && lobbyPlayer.isElected)
            {

                value = true;
            }
        }

        background.SetActive(false);
        minigameSelection.SetActive(value);
        candyMachine.SetActive(!value);
    }

    //Funzione per richiamare il metodo del Lobby player per il caricamento della scena del minigioco 
    public void PlayScene()
    {
        for (int i = 0; i < lobbyManager.lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbyManager.lobbySlots[i] as LobbyPlayer;

            if (p != null && p.isLocalPlayer)
            {
                p.PlayScene();
            }
        }
    }

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

}
