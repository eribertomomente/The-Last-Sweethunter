using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class LobbyPlayer : NetworkLobbyPlayer {

	public string loginName; //TODO: set txtPlayerName.text and sync
	public Text txtPlayerName;
	public Image imgMiniature;
	public Sprite[] miniatures;
	public Button btnReady;
	public Button btnWaitPlayers;
	public Button btnNotReady;
	public Sprite unknownMiniature;

	//Ready button colors
	static Color joinColor = new Color(0.0f/255.0f, 139.0f/255f, 122.0f/255.0f,1.0f);
	static Color cantJoinColor = new Color(0.0f/255.0f, 110.0f/255f, 122.0f/255.0f,0.5f);
	static Color cantJoinTextColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 0.2f);
	static Color notReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
	static Color readyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
	static Color transparentColor = new Color(0, 0, 0, 0);

    //additional fields for TLSH
    [SyncVar]
    public int globalCharacterScore = 732;
    [SyncVar]
    public int localCharacterScore = 732;

    /*
	FIXED IDs
	-1. Unknown
	0.  Mornon
	1.  Indil
	2.  Elanor
	*/
    [SyncVar(hook = "OnCharacterChange")]
	public int characterIndex = -1;

	public bool isReady = false;
    [SyncVar]
    public bool isElected = false;
    [SyncVar]
    public bool pastElected = false;

	private GameObject selectionPanel;
	private Button[] characterButtons;

	void Start() {
		selectionPanel = GameObject.FindGameObjectWithTag ("LobbySelectionPanel");
		//characterButtons = selectionPanel.GetComponentsInChildren<Button> ();
		if (isLocalPlayer) {
			GameObject loginNameField = GameObject.FindGameObjectWithTag ("LoginPlayerName");
			if (loginNameField != null){
				string loginName = loginNameField.GetComponent<Text> ().text;
				if (loginName.Equals (""))
					txtPlayerName.text = "UnknownPlayer" + netId;
				else {
					txtPlayerName.text = loginName;
					CmdUpdateName (loginName);
				}
			} else {
				Debug.Log ("SOMETHING WENT WRONG SETTING LOGIN NAME!");
			}

			loginNameField.GetComponentInParent<InputField> ().interactable = false;
		}

        globalCharacterScore = 732;
        localCharacterScore = 732;
    }

	void Update() {
			btnReady.interactable = characterIndex >= 0 && isLocalPlayer;
	}


	public override void OnClientEnterLobby() {
		base.OnClientEnterLobby();
        
        if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

		LobbyPlayerList._instance.AddPlayer(this);
		LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

		if (isLocalPlayer)
			SetupLocalPlayer();
		else
			SetupRemotePlayer();
	}



	void SetupLocalPlayer() {
		
		ChangeReadyButtonColor(joinColor);

		btnReady.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
		btnReady.interactable = true;
		SetupListener (btnReady, OnPlayerReady);
		loginName = GameObject.FindGameObjectWithTag ("LoginPlayerName").GetComponent<Text> ().text;

		//when OnClientEnterLobby is called, the local PlayerController is not yet created, so we need to redo that here to disable
		//the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
		if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
	}

	void SetupRemotePlayer() {
		ChangeReadyButtonColor(notReadyColor);
		OnClientReady(false);
	}

	public override void OnClientReady(bool readyState){
		isReady = readyState;

		if (readyState) {
			ChangeReadyButtonColor(transparentColor);

			Text textComponent = btnReady.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = "READY";
			textComponent.color = readyColor;
			btnReady.interactable = false;
			CmdUpdateMiniature ();
		} else {
			ChangeReadyButtonColor(isLocalPlayer ? joinColor : notReadyColor);

			Text textComponent = btnReady.transform.GetChild(0).GetComponent<Text>();
			textComponent.text = isLocalPlayer ? "JOIN" : "...";

			textComponent.color = Color.white;
			btnReady.interactable = isLocalPlayer;
		}

	//CmdLockSelection (characterIndex, readyState);
	}

	public override void OnStartAuthority() {
		base.OnStartAuthority();
		btnReady.transform.GetChild(0).GetComponent<Text>().color = Color.white;
		SetupLocalPlayer();
	}

	void ChangeReadyButtonColor(Color c) {
		ColorBlock colorBlock = btnReady.colors;
		colorBlock.normalColor = c;
		colorBlock.pressedColor = c;
		colorBlock.highlightedColor = c;
		if (c != joinColor)
			colorBlock.disabledColor = c;
		else
			colorBlock.disabledColor = cantJoinColor;
		btnReady.colors = colorBlock;
	}

	//Button functions
	private void SetupListener (Button button, UnityEngine.Events.UnityAction delegateFunction) {
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener (delegateFunction);
	}

	void OnPlayerReady() {
		SendReadyToBeginMessage();
	}

	public void ToggleJoinButton(bool enabled){
		btnReady.gameObject.SetActive(enabled);
		btnWaitPlayers.gameObject.SetActive(!enabled);
	}

	public void OnDestroy() {
		LobbyPlayerList._instance.RemovePlayer(this);
		if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);
		//Make character available!
	}

	public void SelectCharacter(int index){
		CmdSelectCharacter (index);
	}

	[Command]
	public void CmdSelectCharacter(int index) {
		characterIndex = index;
		RpcSelectCharacter (index);
		//Debug.Log ("Selected from playerInfo: " + index);
	}

	[ClientRpc]
	public void RpcSelectCharacter(int index){
		characterIndex = index;
	}



    /*********************************************
     *  INIZIO - FACCIAMO UNA PREGHIERA CHE FUNZIONI
     * *******************************************/
    // Client makes sure this function is only executed on clients
    // If called on the server it will throw an warning
    [Server]
    public void IncrementCharacterIndex(int inc)
    {
        GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");

        foreach (GameObject player in lobbyPlayers)
        {
            player.GetComponent<LobbyPlayer>().characterIndex += inc;
        }
        // invoke the change on the Server as you already named the function
        RpcIncrementCharacterIndex(inc);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcIncrementCharacterIndex(int inc)
    {
        if (isServer) return;
        GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag("PlayerInfo");

        foreach (GameObject player in lobbyPlayers)
        {
            player.GetComponent<LobbyPlayer>().characterIndex += inc;
        }
        // invoke the change on the Server as you already named the function
        RpcIncrementCharacterIndex(inc);
    }
    /*********************************************
    *  FINE - FACCIAMO UNA PREGHIERA CHE FUNZIONI
    * *******************************************/


    // Client makes sure this function is only executed on clients
    // If called on the server it will throw an warning
    [Client]
    public void IncrementScore(int points)
    {
        Debug.Log("##### incremento lo score dal lobby Player di punti: " + points);
        //Only go on for the LocalPlayer
        if (!hasAuthority) return;

        // make the change local on this client
        //localCharacterScore += points;
        //globalCharacterScore += points;

        Debug.Log("fatto");
        // invoke the change on the Server as you already named the function
        CmdIncrementScore(points);
    }

    // invoked by clients but executed on the server only
    [Command]
    void CmdIncrementScore(int points)
    {
        // make the change local on the server
        localCharacterScore += points;
        globalCharacterScore += points;

        // forward the change also to all clients
        RpcIncrementScore(points);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcIncrementScore(int points)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (hasAuthority) return;
        //make the change local on all clients
        localCharacterScore += points;
        globalCharacterScore += points;
    }



    public void OnCharacterChange(int oldIndex){
		//CmdUpdateMiniature ();
	}

	[Command]
	public void CmdUpdateMiniature() {
		RpcUpdateMiniature ();
	}

	[ClientRpc]
	public void RpcUpdateMiniature(){
		if (characterIndex >= 0 && characterIndex < miniatures.Length)
			imgMiniature.sprite = miniatures [characterIndex];
		else
			imgMiniature.sprite = unknownMiniature;
	}

	[Command]
	public void CmdUpdateName(string newName) {
		RpcUpdateName (newName);
	}

	[ClientRpc]
	public void RpcUpdateName(string newName){
		if(!isLocalPlayer)
			txtPlayerName.text = newName;
	}

	[Command]
	public void CmdAskNames() {
		
		GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag ("PlayerInfo");

		foreach (GameObject player in lobbyPlayers) {
			if(!player.GetComponent<NetworkIdentity>().isLocalPlayer)
				RpcUpdateName (player.GetComponent<LobbyPlayer>().txtPlayerName.text);
		}
	}

	[Command]
	public void CmdLockSelection(int index, bool isLocked){
		RpcLockSelection (index, isLocked);
	}

	[ClientRpc]
	public void RpcLockSelection(int index, bool isLocked){
		if (index < 0 || !isLocked)
			return;

		if (characterButtons == null) {
			selectionPanel = GameObject.FindGameObjectWithTag ("LobbySelectionPanel");
			characterButtons = selectionPanel.GetComponentsInChildren<Button> ();
		}

		if (isLocalPlayer && isLocked)
			selectionPanel.SetActive (false);

		//characterButtons [index].interactable = false;

		if (!isLocalPlayer && isLocked) {
			GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag ("PlayerInfo");

			foreach (GameObject player in lobbyPlayers) {

				LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer> ();

				if (lobbyPlayer.isLocalPlayer && lobbyPlayer.characterIndex == characterIndex)
					lobbyPlayer.characterIndex = -1;
			}
		}

		characterButtons [index].GetComponent<Image> ().color = new Color (1, 1, 1, 0.3f);
		characterButtons [index].transform.GetChild (1).gameObject.SetActive (true);
	}

	[ClientRpc]
	public void RpcUpdateCountdown(int countdown) {
		LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
		LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
	}

    ////Richiamo della funzione (in base a se è l'host o il client) 
    ////per far partire la scena di un minigioco
    //public void PlayScene()
    //{
    //    if (isServer) LobbyManager.s_Singleton.PlayScene();

    //    else CmdClientPlayScene(LobbyManager.s_Singleton.playScene);

    //}

    //[Command]
    //public void CmdClientPlayScene(string sceneName)
    //{
    //    if(sceneName == "Labyrinth")
    //    {

    //        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
    //        {

    //            if (lp.isServer)
    //            {
    //                lp.IncrementCharacterIndex(3);
    //                break;
    //            }

    //        }

    //    }
    //    else if(sceneName == "Hide_N_Steal")
    //    {
    //        foreach (LobbyPlayer lp in LobbyManager.s_Singleton.lobbySlots)
    //        {
             
    //            if (lp.isServer)
    //            {
    //                lp.IncrementCharacterIndex(6);
    //                break;
    //            }

    //        }
    //    }

    //    LobbyManager.s_Singleton.ClientPlayScene(sceneName);
    //}
}
