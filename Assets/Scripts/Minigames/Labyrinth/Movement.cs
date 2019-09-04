using System.Collections;
using Prototype.NetworkLobby;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour
{
    [SerializeField]
    private GameObject player;
    private static Character character;

    private int x = DijkstraSquare.GetDimensions()[0];
    private int y = DijkstraSquare.GetDimensions()[1];
    private Node[,] matrix;
    private Graph g;

    private float gap;

    public GameObject floatingText;

    public LobbyPlayer myPlayerInfo;

    private void Start()
    {

        SetupLobbyPlayer();
        /**
         * ATTENZIONE:
         * Mi devo preoccupare che tutta la scena venga caricata correttamente prima di fare operazioni su di essa.
         * Per fare cio' non posso mettere nello start un ciclo while che aspetti qualcosa o un certo ammontare di tempo
         * perche' Unity si impianta e noi non possiamo passare le giornate a piangere.
         * Quindi facciamo così: facciamo partire una coroutine in cui aspetteremo che la scena si carichi, nel frattempo 
         * Unity sarà contento perchè puo' finire il suo start del ca**o.
         * Vedi proseguo della spiegazione sopra la definizione della coroutine.
         **/
        StartCoroutine("startMovement");

    }

    /**
     * Questa e' la coroutine che ci salva la vita.
     * Aspettiamo che la scena sia pronta in ogni client, o meglio finchè non e' pronta in ogni client perdiamo un frame (return null)
     * Dopodiche' facciamo partire il setup Player e siamo tutti happy :)
     **/
    IEnumerator startMovement()
    {

        while (!ClientScene.ready)
        {
            yield return null;
        }

        if (isServer) RpcSetupPlayer();

    }

    [ClientRpc]
    public void RpcSetupPlayer()
    {
        //while (!DijkstraSquare.readyForMovement)
        //{
        //    Debug.Log("Sto aspettando");
        //}

        player.transform.position = new Vector3(-5, -3, 0);
        character = new Character(0, 0, player);

        if (isLocalPlayer)
        {
            Camera.main.GetComponent<VerticalCameraFollow>().SetPlayer(this.transform);
        }
        StartCoroutine("waitDijkstra");
        
    }

    IEnumerator waitDijkstra()
    {
        
        while (matrix == null)
        {
            matrix = DijkstraSquare.GetMatrix();
            yield return null;
        }
        Debug.Log("Is matrix null? " + (matrix == null).ToString());
        g = DijkstraSquare.GetGraph();
        gap = DijkstraSquare.GetGap();
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
                        characterPrefab.GetComponent<Movement>().myPlayerInfo = lp;

                    }
                }
            }
            dummy++;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (isLocalPlayer && character != null)
        {
            int i = character.x;
            int j = character.y;

            if (SwipeDetector.GetSwipeUp())
            {
                if (j + 1 < y && g.areConnected(matrix[i, j], matrix[i, j + 1]))
                {
                    player.transform.position = player.transform.position + transform.up * gap;
                    character.y = character.y + 1;
                }
                SwipeDetector.GestureCompleted();
            }

            if (SwipeDetector.GetSwipeDown())
            {
                if (j - 1 >= 0 && g.areConnected(matrix[i, j], matrix[i, j - 1]))
                {
                    player.transform.position = player.transform.position + transform.up * -gap;
                    character.y = character.y - 1;
                }
                SwipeDetector.GestureCompleted();
            }
            if (SwipeDetector.GetSwipeRight())
            {
                if (i + 1 < x && g.areConnected(matrix[i, j], matrix[i + 1, j]))
                {
                    player.transform.position = player.transform.position + transform.right * gap;
                    character.x = character.x + 1;
                }
                SwipeDetector.GestureCompleted();
            }
            if (SwipeDetector.GetSwipeLeft())
            {
                if (i - 1 >= 0 && g.areConnected(matrix[i, j], matrix[i - 1, j]))
                {
                    player.transform.position = player.transform.position + transform.right * -gap;
                    character.x = character.x - 1;
                }
                SwipeDetector.GestureCompleted();
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

        if (col.gameObject.tag.Equals("Candy"))
        {
            int points = col.gameObject.GetComponent<Candy>().GetCandyPoint();

            myPlayerInfo.localCharacterScore += points;
            myPlayerInfo.globalCharacterScore += points;

            int local = myPlayerInfo.localCharacterScore;
            int global = myPlayerInfo.globalCharacterScore;

            switch (col.gameObject.GetComponent<Candy>().GetCandyName())
            {
                case "Basic":

                    IncrementScore(local, global, myPlayerInfo.netId.Value);

                    if (floatingText) ShowFloatingText(points);
                    Destroy(col.gameObject);
                    break;


                case "ChocolateBar":

                    IncrementScore(local, global, myPlayerInfo.netId.Value);

                    SetEndTime();
                    Destroy(col.gameObject);

                    break;

            }
        }
    }

    void ShowFloatingText(int value)
    {
        Debug.Log("Sono nella funzione di ShowFloatingText ");

        var go = Instantiate(floatingText, new Vector3(player.transform.position.x, player.transform.position.y + 1, 0), Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = "+ " + value;
    }

    [Client]
    private void SetEndTime()
    {
      
        GameObject.Find("GameManager").GetComponent<NetworkGameTimer>().gameTime = 0;
        if (isServer) RpcSetEndTime();
        else CmdSetEndTime();
    }

    [Command]
    private void CmdSetEndTime()
    {
        print("sono nel cmd di fine minigame");
        RpcSetEndTime();
    }

    [ClientRpc]
    private void RpcSetEndTime()
    {
        if (!isLocalPlayer)
        {
            print("sono nella rpc di fine minigame");

            GameObject.Find("GameManager").GetComponent<NetworkGameTimer>().gameTime = 0;
        }
    }


}