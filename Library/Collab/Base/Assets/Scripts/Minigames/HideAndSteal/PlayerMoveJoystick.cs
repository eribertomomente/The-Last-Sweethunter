using Prototype.NetworkLobby;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMoveJoystick : NetworkBehaviour
{

    public Vector3 moveVector;
    public float speed;
    public Joystick joystick;
    public GameObject ps;

    public GameObject floatingText;

    public LobbyPlayer myPlayerInfo;

    public Animator animator;


    private void Start()
    {

        if (isLocalPlayer)
        {
            GameObject j = GameObject.Find("MyJoystick");
            if (j != null)
            {
                joystick = j.GetComponent<Joystick>();
            }

            moveVector = new Vector3(4f, -10.9f, 0);
            gameObject.transform.position = moveVector;

            //print(transform.position);
            Camera.main.GetComponent<CameraFollow>().SetPlayer(this.transform);

            StartCoroutine("startPlayerMoveJoystick");
        }
    }

    IEnumerator startPlayerMoveJoystick()
    {
        while (!ClientScene.ready)
        {
            yield return null;
        }

        SetupLobbyPlayer();

    }


    private void SetupLobbyPlayer()
    {
        int dummy = 0;
        int lpId = 0;

        //Si settano i playerInfo con i prefab presenti in scena
        foreach (GameObject characterPrefab in GameObject.FindGameObjectsWithTag("Player"))
        {

            Debug.Log("Sono dentro al for.\n\tdummy e': " + dummy + "\n\tnetId: " + characterPrefab.GetComponent<NetworkIdentity>().netId);


            if (characterPrefab.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                lpId = dummy + 2;
                Debug.Log("lpId: " + lpId);
                foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
                {
                    if (lp != null && lp.GetComponent<NetworkIdentity>().netId.Value == lpId)
                    {
                        myPlayerInfo = lp;

                    }
                }
            }
            else
            {
                lpId = dummy + 2;
                Debug.Log("lpId: " + lpId);
                foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
                {
                    if (lp != null && lp.GetComponent<NetworkIdentity>().netId.Value == lpId)
                    {
                        characterPrefab.GetComponent<PlayerMoveJoystick>().myPlayerInfo = lp;

                    }
                }
            }
            dummy++;
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (isLocalPlayer)
        {

            moveVector = new Vector3(joystick.Horizontal, joystick.Vertical, 0);
            gameObject.transform.Translate(moveVector * speed * Time.deltaTime);
            animator.SetFloat("Speed", Mathf.Abs(joystick.Horizontal));


        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag.Equals("Candy"))
        {
            Candy candy = (Candy)col.gameObject.GetComponent<Candy>();
            int points = candy.GetCandyPoint();
            
            myPlayerInfo.localCharacterScore += points;
            myPlayerInfo.globalCharacterScore += points;

            //int local = myPlayerInfo.localCharacterScore;
            //int global = myPlayerInfo.globalCharacterScore;
            //myPlayerInfo.IncrementScore(local, global);

            DestroyCandy(col.gameObject);
            if (floatingText) ShowFloatingText(points);
        }


    }

    void ShowFloatingText(int value)
    {
        var go = Instantiate(floatingText, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = "+ " + value;
        go.GetComponent<TextMeshPro>().fontSize = 16;

    }



    private void DestroyCandy(GameObject candy)
    {
        Debug.Log("dentro la prima");
        //Only go on for the LocalPlayer
        if (!isLocalPlayer)
        {
            Debug.Log("non sono un local player");
            return;
        }

        // make the change local on this client
        if (!isServer)
        {
            Debug.Log("non sono un server");
            Destroy(candy);
        }

        // invoke the change on the Server as you already named the function
        CmdDestroyCandy( candy );
    }

    [Command]
    private void CmdDestroyCandy(GameObject candy)
    {
        Debug.Log("dentro la seconda");
        // make the change local on the server
        Destroy(candy);

        // forward the change also to all clients
        RpcDestroyCandy(candy);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcDestroyCandy(GameObject candy)
    {
        Debug.Log("dentro la terza");
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer)
        {
            Debug.Log("sono un local player");
            return;
        }

        //make the change local on all clients
        Destroy(candy);
    }

}