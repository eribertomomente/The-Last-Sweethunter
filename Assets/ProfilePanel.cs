using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    private Text username;

    private GameObject playerInfo;

    void Start()
    {

        username = GetComponentsInChildren<Text>()[0];

        InputField[] fieldList = Resources.FindObjectsOfTypeAll<InputField>();

        foreach (InputField inf in fieldList)
        {
            if (inf.tag == "LoginPlayerName") username.text = inf.text;
        }
    }

}
