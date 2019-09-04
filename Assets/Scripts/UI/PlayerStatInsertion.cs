using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatInsertion : MonoBehaviour
{

    public GameObject psMornonPrefab;
    public GameObject psIndilPrefab;
    public GameObject psElanorPrefab;

    private static int sortIndex;

    // Start is called before the first frame update
    void Start()
    {
        sortIndex = 1;

        GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");
        
        foreach (GameObject player in lobbyPlayers)
        {
            int characterIndex = player.GetComponent<LobbyPlayer>().characterIndex % 3;
            if (player.GetComponent<NetworkIdentity>().hasAuthority)
            {
                insertCorrectStat(characterIndex, 0, player);
            }
            else
            {
                insertCorrectStat(characterIndex, sortIndex, player);
                sortIndex++;

            }
        }
    }




    private void insertCorrectStat(int cIndex, int sIndex, GameObject player)
    {
        GameObject go;
        if (cIndex == 0)
        {
            go = Instantiate(psMornonPrefab);
        }
        else if (cIndex == 1)
        {
            go = Instantiate(psIndilPrefab);
        }
        else
        {
            go = Instantiate(psElanorPrefab);

        }
        go.transform.parent = GameObject.Find("PlayersStats").transform;
        go.GetComponent<ScoreBar>().myPlayerInfo = player.GetComponent<LobbyPlayer>();
        go.GetComponent<HealthBar>().myPlayerInfo = player.GetComponent<LobbyPlayer>();
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(0, -sIndex * 100, 0);
        if (sIndex == 0)
        {
            rt.localPosition = new Vector3(10, 0, 0);
            rt.localScale = new Vector3(2.8f, 2.8f, 2.8f);
        }
        else
        {
            rt.localPosition = new Vector3(30, -sIndex*145-145, 0);
            rt.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        }

    }
}
