using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//TODO implementare network behaviour
public class HealthBar : NetworkBehaviour
{ 
    
    public Color perfetctHealth;
    public Color goodHealth;
    public Color mediumHealth;
    public Color lowHealth;
    public Color zeroHealth;
    
    public Transform bar;
    private Transform barImage;

    public LobbyPlayer myPlayerInfo;

    // Start is called before the first frame update
    private void Start()
    {
        barImage = bar.Find("BarImage");

        GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");
        foreach (GameObject player in lobbyPlayers)
        {
            int characterIndex = player.GetComponent<LobbyPlayer>().characterIndex % 3;
            // TODO attenzione ricordarsi che probabilmente hasAuthority nin funzia bene
            if (player.GetComponent<NetworkIdentity>().hasAuthority)
            {
                myPlayerInfo = player.GetComponent<LobbyPlayer>();
            }
        }
        myPlayerInfo.normalizedLifeValue = 1f;


        bar.localScale = new Vector3(myPlayerInfo.normalizedLifeValue, 1f);

    }

    public void Update()
    {
        //myPlayerInfo.normalizedLifeValue -= dec;
        float lifeVal = myPlayerInfo.normalizedLifeValue;
        
        bar.localScale = new Vector3(lifeVal, 1f);
        

        if (lifeVal > .9f)
        {
            barImage.GetComponent<Image>().color = perfetctHealth;
        }
        else if (lifeVal > .65f)
        {
            barImage.GetComponent<Image>().color = goodHealth;
        }
        else if (lifeVal > .35f)
        {
            barImage.GetComponent<Image>().color = mediumHealth;
        }
        else if (lifeVal > .1f)
        {
            barImage.GetComponent<Image>().color = lowHealth;
        }
        else
        {
            barImage.GetComponent<Image>().color = zeroHealth;
        }
    }
}
