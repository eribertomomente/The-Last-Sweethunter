using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class Action : NetworkBehaviour
{
    [SerializeField]
    private float speed;
    private Rigidbody2D rb;
    private float t = 0.0f;
    [SerializeField]
    private float airTime = 1.0f;
    private bool moving = false;
    private bool isOnGround = true;
    [SerializeField]
    private float jumpIntensity = 10f;
    [SerializeField]
    private GameObject bulletPrefab;
    bool isFlipped;

    public LobbyPlayer myPlayerInfo;

    public GameObject floatingText;


    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer) return;

        int numPlayers = 0;
        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        {
            if (lp != null) numPlayers++;
        }
     
        StartCoroutine("startAction");


        // so che il netId del lobbyplayer che mi interessa e pari alla differenza tra il netId di this e il numero di giocatori nella scena
        //int lpID = (int) gameObject.GetComponent<NetworkIdentity>().netId.Value - numPlayers;

        //foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        //{
        //    if (lp != null && lp.GetComponent<NetworkIdentity>().netId.Value == lpId)
        //    {
        //       myPlayerInfo = lp;
        //       Debug.Log("azvegnaaaaaaa il netId e': " + lp.GetComponent<NetworkIdentity>().netId.Value);
        //    }
        //}

        //int x_rnd = new System.Random().Next(-8, 8);

        //Set positions players
        for(int i = 0; i < LobbyManager.s_Singleton.startPositions.Count; i++)
        {
            Transform t = LobbyManager.s_Singleton.startPositions[i];

            if(t != null)
            {
                t.position = new Vector3(i + 1, 0, 0);
            }
        }
     
        
        //this.transform.position = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody2D>();
        if (isLocalPlayer)
        {
            Camera.main.GetComponent<VerticalCameraFollow>().SetPlayer(this.transform);
        }

    }

    IEnumerator startAction()
    {
        while (!ClientScene.ready)
        {
            yield return null;
        }

        SetupLobbyPlayer();

    }

    private void SetupLobbyPlayer() { 

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
                        characterPrefab.GetComponent<Action>().myPlayerInfo = lp;

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
            isFlipped = GetComponent<SpriteRenderer>().flipX;
            foreach (GameObject branch in GameObject.FindGameObjectsWithTag("branch"))
            {
                if (this.GetComponent<BoxCollider2D>().IsTouching(branch.GetComponent<BoxCollider2D>()))
                {
                    isOnGround = true;
                }

            }
            foreach (GameObject ground in GameObject.FindGameObjectsWithTag("CT_Ground"))
            {
                if (this.GetComponent<BoxCollider2D>().IsTouching(ground.GetComponent<BoxCollider2D>()))
                {
                    isOnGround = true;
                }

            }
            if (isOnGround)
            {
                if (gameObject.GetComponent<ChubbyTummySwipeDetector>().GetSwipeUp())
                {
                    isOnGround = false;
                    Debug.Log("pressed up");
                    rb.velocity = new Vector3(0, jumpIntensity, 0);
                    moving = true;
                }
                if (moving)
                {
                    // when the cube has moved over 1 second report it's position
                    t = t + Time.deltaTime;
                    if (t > airTime)
                    {
                        t = 0.0f;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("pressed down");

                this.transform.Translate(0, 0, 0);
            }
            
            if (gameObject.GetComponent<ChubbyTummySwipeDetector>().isSwipingRight())
            {
                // Debug.Log("pressed right");
                this.transform.Translate(0.01f * speed, 0, 0);
                UpdateFlipRenderer(true);
            }
            
            if (gameObject.GetComponent<ChubbyTummySwipeDetector>().isSwipingLeft())
            {
                // Debug.Log("pressed left");
                this.transform.Translate(-0.01f * speed, 0, 0);
                UpdateFlipRenderer(false);

            }

            if (gameObject.GetComponent<ChubbyTummySwipeDetector>().GetFire())
            {
                Debug.Log("pressed fire");

                DoShoot();
                gameObject.GetComponent<ChubbyTummySwipeDetector>().SetFire(false);

            }
        }
    }


    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        Debug.DrawRay(position, direction * .1f, Color.yellow, 10f);
        //RaycastHit2D hit = Physics2D.Raycast(position, direction, DistanceToTheGround + 0.1f, groundLayer);
        //if (hit.collider != null)
        //{
        //    return true;
        //}
        return false;
    }

    public void setIsOnGround(bool flag)
    {
        isOnGround = flag;
    }

    [Client]
    void DoShoot()
    {
        //Only go on for the LocalPlayer
        if (!isLocalPlayer) return;


        // make the change local on this client
        float bulletDistance = 1f;
        if (!isFlipped)
        {
            bulletDistance = -bulletDistance;
        }

        GameObject bullet = Instantiate(bulletPrefab, transform.position + (bulletDistance * Vector3.right), transform.rotation);
        bullet.GetComponent<MovementBullet>().setIsFlipped(isFlipped);

        // invoke the change on the Server as you already named the function
        CmdProvideDoShoot(bulletDistance, isFlipped);
    }


    // invoked by clients but executed on the server only
    [Command]
    void CmdProvideDoShoot(float bulletDistance, bool flag)
    {
        // make the change local on the server
        GameObject bullet = Instantiate(bulletPrefab, transform.position + (bulletDistance * Vector3.right), transform.rotation);
        bullet.GetComponent<MovementBullet>().setIsFlipped(flag);

        // forward the change also to all clients
        RpcSendDoShoot(bulletDistance, flag);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendDoShoot(float bulletDistance, bool flag)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;
        if (isServer) return;

        //make the change local on all clients
        GameObject bullet = Instantiate(bulletPrefab, transform.position + (bulletDistance * Vector3.right), transform.rotation);
        bullet.GetComponent<MovementBullet>().setIsFlipped(flag);
    }



    // Client makes sure this function is only executed on clients
    // If called on the server it will throw an warning
    [Client]
    private void UpdateFlipRenderer(bool value)
    {
        //Only go on for the LocalPlayer
        if (!isLocalPlayer) return;

        // make the change local on this client
        GetComponent<SpriteRenderer>().flipX = value;

        // invoke the change on the Server as you already named the function
        CmdProvideFlipStateToServer(value);
    }

    // invoked by clients but executed on the server only
    [Command]
    void CmdProvideFlipStateToServer(bool value)
    {
        // make the change local on the server
        GetComponent<SpriteRenderer>().flipX = value;

        // forward the change also to all clients
        RpcSendFlipState(value);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendFlipState(bool value)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        //make the change local on all clients
        GetComponent<SpriteRenderer>().flipX = value;
    }

    //public void DecreaseLife(float dec)
    //{
    //    myPlayerInfo.DecrementLifeValue(dec);
    //    if (myPlayerInfo.normalizedLifeValue < 0){
    //        KillPlayer();
    //    }
    //}


    // Client makes sure this function is only executed on clients
    // If called on the server it will throw an warning
    [Client]
    private void KillPlayer(uint netId)
    {
        //Only go on for the LocalPlayer
        if (!isLocalPlayer) return;

        // make the change local on this client
        //Destroy(this.gameObject);
        this.transform.position = new Vector3(5, 0, 0);
        myPlayerInfo.UpdateLifeValue(1f);

        // invoke the change on the Server as you already named the function
        CmdProvideKillPlayer(netId);
    }

    // invoked by clients but executed on the server only
    [Command]
    void CmdProvideKillPlayer(uint netId)
    {
        // make the change local on the server
        //Destroy(this.gameObject);
        //this.transform.position = new Vector3(5, 0, 0);
        // forward the change also to all clients
        RpcSendKillPlayer(netId);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendKillPlayer(uint netId)
    {
        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        {
            if (lp != null && netId == lp.netId.Value)
            {
                lp.UpdateLifeValue(1f);
                return;
            }
        }
        /*// skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        //make the change local on all clients
        //Destroy(this.gameObject);

        this.transform.position = new Vector3(5, 0, 0);
        myPlayerInfo.UpdateLifeValue(1f);*/
    }


    [Client]
    public void DecrementLifeValue(float dec, uint netId)
    {
        if (isServer) RpcDecrementLifeValue(dec, netId);
        else
        {
            myPlayerInfo.normalizedLifeValue -= dec;
            CmdDecrementLifeValue(dec, netId);
        }
       
    }

    [Command]
    void CmdDecrementLifeValue(float dec, uint netId)
    {
        RpcDecrementLifeValue(dec, netId);
    }

    [ClientRpc]
    void RpcDecrementLifeValue(float dec, uint netId)
    {
        
        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        {
            if (lp != null && netId == lp.netId.Value)
            {
                lp.normalizedLifeValue -= dec;
            }
        }
    }


    //Incremento punteggio sincronizzato
    [Client]
    public void IncrementScore(int local, int global, uint netId)
    {

        if (isServer) RpcUpdateScores(local, global, netId);
        else
        {
            myPlayerInfo.localCharacterScore = local;
            myPlayerInfo.globalCharacterScore = global;
            CmdUpdateScores(local, global, netId);
        }
        // invoke the change on the Server as you already named the function
        //CmdUpdateScores(local, global, netId);
    }

    // invoked by clients but executed on the server only
    [Command]
    void CmdUpdateScores(int local, int global, uint id)
    {

        // make the change local on the server
        //foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        //{
        //    if (lp != null && lp.netId == id)
        //    {
        //        lp.localCharacterScore = local;
        //        lp.globalCharacterScore = global;
        //    }
        //}

        // forward the change also to all clients
        RpcUpdateScores(local, global, id);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcUpdateScores(int local, int global, uint id)
    {
        //make the change local on all clients
        // make the change local on the server
        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
        {
            if (lp != null && id == lp.netId.Value)
            {
                lp.localCharacterScore = local;
                lp.globalCharacterScore = global;
            }
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isLocalPlayer) return;
        
        Debug.Log("Collisione tra " + this.gameObject.name + " e " + col.gameObject.name);

        if (col.gameObject.name.Equals("CB_Fireball(Clone)"))
        {
            Debug.Log("Collisione con una fireball");
            Debug.Log("netID" + netId);
            if (myPlayerInfo.normalizedLifeValue > 0f)
            {

                if (myPlayerInfo.normalizedLifeValue <= .4f)
                {
                    DecrementLifeValue(.3f, myPlayerInfo.netId.Value);
                    KillPlayer(myPlayerInfo.netId.Value);
                }

                else DecrementLifeValue(.3f, myPlayerInfo.netId.Value);

                //myPlayerInfo.normalizedLifeValue -= .3f;
                if (isServer) NetworkServer.Destroy(col.gameObject);
                else CmdDestroy(col.gameObject);
            }
        }
        else if (col.gameObject.tag.Equals("Candy"))
        {
            Debug.Log("Collisione con una caramella");

            int points = col.gameObject.GetComponent<Candy>().GetCandyPoint();
            myPlayerInfo.localCharacterScore += points;
            myPlayerInfo.globalCharacterScore += points;

            int local = myPlayerInfo.localCharacterScore;
            int global = myPlayerInfo.globalCharacterScore;

            IncrementScore(local, global, myPlayerInfo.netId.Value);

            Destroy(col.gameObject);
            if (floatingText) ShowFloatingText(points);
        }
    }
    

    void ShowFloatingText(int value)
    {

        var go = Instantiate(floatingText, new Vector3(this.transform.position.x, this.transform.position.y + 1, 0), Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = "+ " + value;
        go.GetComponent<TextMeshPro>().fontSize = 16;

    }

    [Command]
    private void CmdDestroy(GameObject toDestroy)
    {
        Debug.Log("dentro la command e ho una fireball null?" + (toDestroy == null).ToString());
        NetworkServer.Destroy(toDestroy);
    }

}
