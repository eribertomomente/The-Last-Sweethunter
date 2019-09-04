using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CTCandySpwner : NetworkBehaviour
{
    public GameObject candyPrefab;

    private GameObject[] branches;

    public float probability;

    private int x_rnd;

    public DebugWindow dw;


    void Start()
    {
        branches = GameObject.FindGameObjectsWithTag("branch");
        SpawnCandies();

    }

    /*IEnumerator startSpawn()
    {
        while(!ClientScene.ready)
        {
            yield return null;
        }

        dw.Log("sono nello startt");


    }*/

    void SpawnCandies()
    {
        System.Random rnd = new System.Random();
        foreach (GameObject branch in branches)
        {
            int current = rnd.Next(100);
            if (current < probability * 100)
            {
                float x = branch.transform.position.x;
                float y = branch.transform.position.y;

                candyPrefab.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

                Instantiate(candyPrefab, new Vector2(x, y), Quaternion.identity);
                //RpcSpawnCandies(x, y);

            }
        }

    }

    //ClientRpc]
    /*void RpcSpawnCandies(float x, float y)
    {

        dw.Log("sono nello spawnn rpc");
        dw.Log("candy: " + (candyPrefab == null).ToString());

        if (!isServer)
        {
            Instantiate(candyPrefab, new Vector2(x, y), Quaternion.identity);
            print("sono nel client spawn");
        }

    }*/
}
