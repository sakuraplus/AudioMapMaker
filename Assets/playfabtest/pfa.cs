using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI ;
using System.Collections.Generic;
public class pfa : MonoBehaviour
{
	[SerializeField ]
	private string _playFabPlayerIdCache;
	[SerializeField ]
	string customId;
	[SerializeField ]
	string roomname;
	[SerializeField]
	Text  t;

	//Run the entire thing on awake
	public void Awake()
	{

		t.text=customId;
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
	public void test()
	{

		Debug.LogError ("//test// AuthValues=" + PhotonNetwork.AuthValues);
		Debug.LogError ("//test// countOfPlayers=" + PhotonNetwork.countOfPlayers);
		Debug.LogError ("//test// countOfRooms=" + PhotonNetwork.countOfRooms);
		Debug.LogError ("//test// inRoom=" + PhotonNetwork.inRoom);
		t.text = ("//test//AuthValues= " + PhotonNetwork.AuthValues)+"\n";
		t.text+= ("//test// countOfPlayers=" + PhotonNetwork.countOfPlayers)+"\n";
		t.text+=  ("//test// countOfRooms=" + PhotonNetwork.countOfRooms)+"\n";
		if (PhotonNetwork.inRoom) {
			t.text+=  ("//test// " + PhotonNetwork.inRoom+"//"+PhotonNetwork.room.Name );
		}

	}
	public void testrooms()
	{

		Debug.LogError ("//test// AuthValues=" + PhotonNetwork.GetRoomList());
		string ttrr = "";
		RoomInfo[] ri = PhotonNetwork.GetRoomList();
		for (int i = 0; i < PhotonNetwork.countOfRooms; i++) {
			ttrr+=ri[i].Name+" , ";
		}
		t.text = ("//test// room.Name=" + ttrr)+"\n";
		Debug.LogError ("//test// room.Name=" + ttrr);

		Debug.LogError ("//test// AllocateViewID=" + PhotonNetwork.AllocateViewID());
		Debug.LogError ("//test// countOfPlayersInRooms=" + PhotonNetwork.countOfPlayersInRooms);
		Debug.LogError ("//test// countOfPlayersOnMaster=" + PhotonNetwork.countOfPlayersOnMaster);
		Debug.LogError ("//test// insideLobby=" + PhotonNetwork.insideLobby);
		Debug.LogError ("//test// player.UserId=" + PhotonNetwork.player.UserId);


		Debug.LogError ("//test// playerList=" + PhotonNetwork.playerList);
		string ttpp = "";
		PhotonPlayer [] pl = PhotonNetwork.playerList;
		for (int j = 0; j < pl .Length ; j++) {
			ttpp+=pl[j].IsLocal+" , ";
		}
		t.text += ("//test// playerList=" + ttpp)+"\n";
		Debug.LogError ("//test// playerList=" + ttpp);


	}
	public void testConnect()
	{
		if (!PhotonNetwork.connected && PhotonNetwork.connectionState != ConnectionState.Connecting) {
			PhotonNetwork.ConnectUsingSettings ("1");//.ConnectToMaster
			Debug.Log ("connect!"+PhotonNetwork.connecting );
			t.text += "connect!";
		}
		if (PhotonNetwork.connectionState==ConnectionState.Connecting) {
			Debug.Log ("is Connecting");
			t.text += "connecting!";
		} else if (PhotonNetwork.connected) {
			Debug.Log ("connected!"+PhotonNetwork.connecting );
			t.text += "connected!";
		}else{
			Debug.Log ("unconnect!"+PhotonNetwork.connecting );
			t.text += "uncon!";
		}

	}
	public void testJoinRandom()
	{	
		if (PhotonNetwork.connected) {
			Debug.Log ("is connected");
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


//	public override void OnJoinedRoom()
//	{
//		LogMessage ("OnJoinedRoom  "+PhotonNetwork.room.Name );
//		Debug.Log ("OnJoinedRoom  "+PhotonNetwork.room.Name );
////		GameObject monster = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
////		monster.GetComponent<myThirdPersonController>().isControllable = true;
////		myPhotonView = monster.GetComponent<PhotonView>();
//	}














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
		t.text=("PlayFab + Photon Example: " + message);
	}

}