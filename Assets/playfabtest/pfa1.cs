using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI ;
using System.Collections.Generic;
using Photon ;
public class pfa1 : PunBehaviour 
{
	[SerializeField ]
	private string _playFabPlayerIdCache;
	[SerializeField ]
	string customId="12354";
	[SerializeField ]
	string roomname;
	[SerializeField]
	Text  t;

	//Run the entire thing on awake
	public void Awake()
	{
		AuthenticateWithPlayFab();
		DontDestroyOnLoad(gameObject);
		//t.text=customId;
	}
	public void testloginbtn()
	{
		   AuthenticateWithPlayFab();
        DontDestroyOnLoad(gameObject);
		
	}

	/*
     * Step 1
     * We authenticate current PlayFab user normally. 
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if 
     * authentication was successful.
     */
	private  void AuthenticateWithPlayFab()
	{
		LogMessage ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
		Debug.Log ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
		//		LoginWithCustomIDRequest LWCrequest = new LoginWithCustomIDRequest
		//		{ CreateAccount=true,CustomId= PlayFabSettings.DeviceUniqueIdentifier};
		//		PlayFabClientAPI.LoginWithCustomID(LWCrequest , RequestPhotonToken, OnPlayFabError);
		//		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
		//			{
		//				CreateAccount = true,
		//				CustomId =customId 
		//			}, RequestPhotonToken, OnPlayFabError);
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = PlayFabSettings.DeviceUniqueIdentifier+"EDITOR"
			}, RequestPhotonToken, OnPlayFabError);
	}
	//CustomId =customId ;// PlayFabSettings.DeviceUniqueIdentifier
	/*
    * Step 2
   
    */
	private void RequestPhotonToken(LoginResult obj)
	{
		LogMessage("PlayFab authenticated. Requesting photon token...");
		Debug.Log("PlayFab authenticated. Requesting photon token...");

		//We can player PlayFabId. This will come in handy during next step
		_playFabPlayerIdCache = obj.PlayFabId;
		//		GetPhotonAuthenticationTokenRequest GPArequest = new GetPhotonAuthenticationTokenRequest {
		//			PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
		//		};
		//		PlayFabClientAPI.GetPhotonAuthenticationToken(GPArequest , AuthenticateWithPhoton, OnPlayFabError);
		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
			}, AuthenticateWithPhoton, OnPlayFabError);
	}


	/*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
	{
		LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");
		Debug.Log("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

		//We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
		var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

		//We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
		customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

		//We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
		customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

		//We finally tell Photon to use this authentication parameters throughout the entire application.
		PhotonNetwork.AuthValues = customAuth;
		PhotonNetwork.autoJoinLobby = true;
	}


	// Add small button to launch our example code 
	public void OnGUI()
	{
		if (GUILayout.Button("Execute Example ")) ExecuteExample(); 
	}


	// Example code which raises custom room event, then sets custom room property
	private void ExecuteExample()
	{

		// Raise custom room event
		var data = new Dictionary<string, object>() { {"Hello","World"} };
		var result = PhotonNetwork.RaiseEvent(15, data, true, new RaiseEventOptions()
			{
				ForwardToWebhook = true,
			});
		LogMessage("New Room Event Post: "+result);

		// Set custom room property
		var properties = new ExitGames.Client.Photon.Hashtable() { { "CustomProperty", "It's Value" } };
		var expectedProperties = new ExitGames.Client.Photon.Hashtable();
		PhotonNetwork.room.SetCustomProperties(properties, expectedProperties, true);
		LogMessage("New Room Properties Set");
	}
	////////////////////////////////////////////////////////
	public void testclr()
	{	
		t.text = "";
	}
	public void test()
	{

		Debug.LogWarning ("//test// AuthValues=" + PhotonNetwork.AuthValues);
		Debug.LogWarning ("//test// countOfPlayers=" + PhotonNetwork.countOfPlayers);
		Debug.LogWarning ("//test// countOfRooms=" + PhotonNetwork.countOfRooms);
		Debug.LogWarning ("//test// inRoom=" + PhotonNetwork.inRoom);
		t.text = ("//test//AuthValues= " + PhotonNetwork.AuthValues)+"\n";
		t.text+= ("//test// countOfPlayers=" + PhotonNetwork.countOfPlayers)+"\n";
		t.text+=  ("//test// countOfRooms=" + PhotonNetwork.countOfRooms)+"\n";
		if (PhotonNetwork.inRoom) {
			t.text+=  ("//test// " + PhotonNetwork.inRoom+"//"+PhotonNetwork.room.Name );
		}

	}
	public void testrooms()
	{

		Debug.LogWarning ("//test// GetRoomList length=" + PhotonNetwork.GetRoomList().Length );
		string ttrr = "";
		RoomInfo[] ri = PhotonNetwork.GetRoomList();
		for (int i = 0; i < ri.Length ; i++) {
			ttrr+=ri[i].Name+" , ";
		}
		t.text = ("//test// room.Name=" + ttrr)+"\n";
		Debug.LogWarning ("//test// room.Name=" + ttrr);

		Debug.LogWarning ("//test// AllocateViewID=" + PhotonNetwork.AllocateViewID());
		Debug.LogWarning ("//test// countOfPlayersInRooms=" + PhotonNetwork.countOfPlayersInRooms);
		Debug.LogWarning ("//test// countOfPlayersOnMaster=" + PhotonNetwork.countOfPlayersOnMaster);
		Debug.LogWarning ("//test// insideLobby=" + PhotonNetwork.insideLobby);
		Debug.LogWarning ("//test// player.UserId=" + PhotonNetwork.player.UserId);


		Debug.LogWarning ("//test// playerList Length=" + PhotonNetwork.playerList.Length );
		string ttpp = "";
		PhotonPlayer [] pl = PhotonNetwork.playerList;
		for (int j = 0; j < pl .Length ; j++) {
			ttpp+=pl[j].IsLocal+" , ";
		}
		t.text += ("//test// playerList=" + ttpp)+"\n";
		Debug.LogWarning ("//test// playerList=" + ttpp);


	}

	public void testConnect()
	{
		if ( PhotonNetwork.connectionState == ConnectionState.Disconnected ) {
			PhotonNetwork.ConnectUsingSettings ("1");//.ConnectToMaster
			Debug.Log ("connect!"+PhotonNetwork.connectionState );
			t.text += "connect!";
		}

		t.text += "ConnectionState="+PhotonNetwork.connectionState;
		Debug.Log ("ConnectionState!"+PhotonNetwork.connectionState );
	}
	public void testDisConnect()
	{
		if ( PhotonNetwork.connectionState == ConnectionState.Connected  ) {
			PhotonNetwork.Disconnect ();// ("1");//.ConnectToMaster
			Debug.Log ("Disconnect!"+PhotonNetwork.connectionState );
			t.text += "Disconnect!";
		}

		t.text += "ConnectionState="+PhotonNetwork.connectionState;
		Debug.Log ("ConnectionState!"+PhotonNetwork.connectionState );
	}
	public void testJoinRandom()
	{	
		if (PhotonNetwork.connected) {
			Debug.Log ("is connected num="+PhotonNetwork.countOfRooms );
			t.text += "con!";
			if (PhotonNetwork.countOfRooms > 0) {
				//bool ttt =
				Debug.Log ("JoinRandomRoom");
				t.text += "   JoinRandomRoom!";
				PhotonNetwork.JoinRandomRoom ();

			} else {
				t.text += "   CreateRoom!";
				Debug.Log ("CreateRoom");
				PhotonNetwork.CreateRoom (roomname);
			}

		}else{
			Debug.Log ("unconnect! can not join"+PhotonNetwork.connecting );
			t.text += "unconnect! can not join!\n";
		}

	}


	public override void OnJoinedRoom()
	{
		LogMessage ("OnJoinedRoom  "+PhotonNetwork.room.Name );
		Debug.LogError  ("OnJoinedRoom  "+PhotonNetwork.room.Name );
		//		GameObject monster = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
		//		monster.GetComponent<myThirdPersonController>().isControllable = true;
		//		myPhotonView = monster.GetComponent<PhotonView>();
	}
	public override void OnReceivedRoomListUpdate()
	{
		LogMessage ("OnReceivedRoomListUpdate  countOfRooms="+PhotonNetwork.countOfRooms  );
		Debug.LogError ("OnReceivedRoomListUpdate  countOfRooms="+PhotonNetwork.countOfRooms );
	}
	public override void OnCreatedRoom()
	{
		LogMessage ("OnCreatedRoom  countOfRooms="+PhotonNetwork.countOfRooms  );
		Debug.LogError ("OnCreatedRoom  countOfRooms="+PhotonNetwork.countOfRooms );
	}
	public override void OnJoinedLobby()
	{
		LogMessage ("OnJoinedLobby  inlobby="+PhotonNetwork.insideLobby+" countinmaster=" +PhotonNetwork.countOfPlayersOnMaster );
		Debug.LogError ("OnJoinedLobby  inlobby="+PhotonNetwork.insideLobby+" countinmaster=" +PhotonNetwork.countOfPlayersOnMaster );
	}
	public override void OnConnectedToPhoton()
	{
		LogMessage ("OnConnectedToPhoton  countOfRooms="+PhotonNetwork.countOfRooms  );
		Debug.LogError ("OnConnectedToPhoton  countOfRooms="+PhotonNetwork.countOfRooms );
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		LogMessage ("OnPhotonPlayerConnected  newPlayer="+newPlayer.ID );
		Debug.LogError ("OnPhotonPlayerConnected newPlayer="+newPlayer.ID  );

	}


	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{	
		LogMessage ("OnPhotonPlayerDisconnected  id="+otherPlayer.ID );
		Debug.LogError ("OnPhotonPlayerDisconnected  id="+otherPlayer.ID  );
	}
	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		LogMessage ("OnPhotonRandomJoinFailed  ="+codeAndMsg );
		Debug.LogError ("OnPhotonRandomJoinFailed  ="+codeAndMsg  );
	}

	public override void OnConnectedToMaster()
	{	
		LogMessage ("OnConnectedToMaster  countOfPlayersOnMaster="+PhotonNetwork. countOfPlayersOnMaster);
		Debug.LogError ("OnConnectedToMaster  countOfPlayersOnMaster="+PhotonNetwork. countOfPlayersOnMaster  );
	}





	/// <summary>
	/// //////////////////////////////////////////////////////
	/// </summary>
	/// <param name="obj">Object.</param>
	private void OnPlayFabError(PlayFabError obj)
	{
		LogMessage(obj.ErrorMessage);
		Debug.Log(obj.ErrorMessage);
	}

	public void LogMessage(string message)
	{
		//Debug.Log
		t.text+=("PlayFab + Photon Example: " + message);
	}

}