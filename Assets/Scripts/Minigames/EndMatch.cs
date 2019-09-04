using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class EndMatch : NetworkBehaviour
{
    public GameObject endMatchCanvas;
    bool[] readyList;

    void Start()
    {

        Debug.Log("is server?" + isServer);
        Debug.Log("is client?" + isClient);
        //readyList = new bool[NetworkServer.connections.Count];
        //for (int i = 0; i < readyList.Length; i++)
        //{
        //    readyList[i] = false;
        //}
    }


    void Update()
    {
    }

    //public void ReadyForNextMatch()
    //{
    //    Debug.Log("Ready for next match");
    //    DebugWindow.Instance.Log("Ready for next match");
    //    if (isClient)
    //    {
    //        Debug.Log("Sono un client");
    //        DebugWindow.Instance.Log("Sono un client");
    //        //CmdImReady(true);
    //    } else if (isServer)
    //    {
    //        Debug.Log("Sono un server ed ecco il mio array");
    //        readyList[0] = true;
    //        foreach(bool value in readyList)
    //        {
    //            Debug.Log( value.ToString());
    //        }
    //        Debug.Log("finito array");
    //        DebugWindow.Instance.Log("sono un server");
    //    } else
    //    {
    //        Debug.Log("QUESTO NON DOVREBBE USCIRE");
    //        DebugWindow.Instance.Log("QUESTO NON DOVREBBE USCIRE");

    //        DebugWindow.Instance.Log("\t valore di isServer:" + isServer);
    //        DebugWindow.Instance.Log("\t valore di isClient:" + isClient);
    //        Debug.Log(isServer.GetType().Name);


    //    }
    //}

    public void ReadyForNextMatch()
    {
        Debug.Log("Ready for next match");
        DebugWindow.Instance.Log("Ready for next match");
        CmdImReady(true);
    }

    [Command]
    public void CmdImReady(bool ready)
    {
        Debug.Log("nella command e l'id e': " + connectionToClient.connectionId);
        DebugWindow.Instance.Log("nella command");
        readyList[connectionToClient.connectionId] = true;
        
    }

    

}
