using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class MinigamesSelection : MonoBehaviour
{
    private GameObject[] minigamesList;

    private LobbyManager lobbyManager;

    public int minigameIndex;

    private void Start()
    {

        minigamesList = new GameObject[transform.childCount];

        lobbyManager = LobbyManager.s_Singleton;

        //Si riempie l'array con i modelli
        for (int i = 0; i < minigamesList.Length; i++)
        {
            minigamesList[i] = transform.GetChild(i).gameObject;
        }

        //Si attiva solo il modello selezionato
        if (minigamesList[minigameIndex])
            minigamesList[minigameIndex].transform.GetChild(1).gameObject.SetActive(true);

        

    }
    
    public void SetIndex(int i)
    {
        // questo perche' non dovrebbe essere rigiocabile
        minigamesList[minigameIndex].transform.GetChild(1).gameObject.SetActive(false);
        minigameIndex = i;
        minigamesList[minigameIndex].transform.GetChild(1).gameObject.SetActive(true);

    }


    public void SelectButton()
    {

        switch (minigameIndex)
        {

            case 0:

                lobbyManager.playScene = "ChubbyTummy";
                //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ player info trovati:" + GameObject.FindGameObjectsWithTag("PlayerInfo").Length);
                foreach (LobbyPlayer lp in lobbyManager.lobbySlots)
                {

                    if (lp != null && lp.isServer)
                    {

                        lp.IncrementCharacterIndex(0);
                        break;
                    }

                }
                break;

            case 1:

                lobbyManager.playScene = "Labyrinth";
                
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerInfo"))
                {
                    LobbyPlayer lp = go.GetComponent<LobbyPlayer>();

                    
                    if (lp != null && lp.isServer)
                    {
                        lp.IncrementCharacterIndex(3);
                        break;
                    }
                       
                }
                break;


            case 2:
                lobbyManager.playScene = "Hide_N_Steal";

                foreach (LobbyPlayer lp in lobbyManager.lobbySlots)
                {

                    if (lp != null && lp.isServer)
                    {
                        lp.IncrementCharacterIndex(6);
                        break;
                    }

                }
                break;
        }
        
    }


}
