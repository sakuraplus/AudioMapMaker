using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI ;
using System.Collections.Generic;
using Photon ;
public class pfaExample : PunBehaviour
{
	[SerializeField ]
	private string _playFabPlayerIdCache;
	[SerializeField ]
	string customId;
	[SerializeField ]
	string roomname;
	[SerializeField]
	Text  t;
	[SerializeField ]
	GameObject    Tobj;
	//Run the entire thing on awake
	public void Awake()
	{

		t.text=customId;
	}


	/*
     * Step 1
   
     */
	private  void AuthenticateWithPlayFab()
	{
		LogMessage ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
		Debug.Log ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
				CustomId =customId 
        }, RequestPhotonToken, OnPlayFabError);
		//CustomId =PlayFabSettings.DeviceUniqueIdentifier+"EDITOR"
	}

	/*
    * Step 2
   
    */
	private void RequestPhotonToken(LoginResult obj)
	{
		LogMessage("PlayFab authenticated. Requesting photon token...");
		Debug.Log("PlayFab authenticated. Requesting photon token...");

		//We can player PlayFabId. This will come in handy during next step
		_playFabPlayerIdCache = obj.PlayFabId;

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
		LS = loginstate.loginpfa;////****
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
			ttpp+=pl[j].UserId+" , ";
		}
		t.text += ("//test// playerList=" + ttpp)+"\n";
		Debug.LogWarning ("//test// playerList=" + ttpp);


	}

	#region 同步测试
	public void nextstep()
	{
		switch (LS) {
		case loginstate.start:
			testloginbtn ();
			break;	
		case loginstate.loginpfa:
			Debug.LogWarning ("*****"+LS);
			testConnect();
			break;
		case loginstate.connectPht:
			Debug.LogWarning ("*****"+LS);
			testJoinRandom ();
			break;
		case loginstate.joinroom :
			Debug.LogWarning ("*****"+LS);
			testGetleader  ();
			break;
		} 

	}
//	public void testRoll()
//	{
//		Tobj.transform.RotateAround (Tobj.transform.position, Vector3.forward, 10);
//
//	}
//	public void testLeft()
//	{
//		Tobj.transform.Translate  (Vector3.left );
//
//	}
//	public void testRight()
//	{
//		Tobj.transform.Translate (Vector3.right);
//
//	}
	#endregion
	#region 登录连接

	enum loginstate{
		start,loginpfa,connectPht,joinroom,leaderboard
	}
	loginstate LS=loginstate.start ;
	public void testloginbtn()
	{
		AuthenticateWithPlayFab();
		DontDestroyOnLoad(gameObject);

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
	#endregion
	#region 获取id和loc
	/// <summary>
	/// ///获取排名前几的playfabid
	/// </summary>
	public void testGetleader(){
		var request = new GetLeaderboardRequest {StatisticName="xp",StartPosition=0};

		PlayFabClientAPI.GetLeaderboard   (request, OnLeaderboardSuccess, OnPlayFabError,null,null);
		Debug.Log ("!!testGetleader");
	}
	void OnLeaderboardSuccess(GetLeaderboardResult  result){

		Debug.Log ("!!GetLeaderboard"+result.Leaderboard .Count);
		string ttpp = "";
		List<PlayerLeaderboardEntry > GI =result.Leaderboard;
		for (int i = 0; i < GI .Count  ; i++) {
			//for (int j = 0; j < GI[i].PlayerUserIds.Count   ; j++) {
			ttpp+=GI[i].PlayFabId +"=" +GI[i].StatValue+" , ";
			//}
			ttpp+="\n";
			if (i < 5) {
				IDS [i] = GI [i].PlayFabId;
			}
		}
		t.text += ("//result// Leaderboard=" + ttpp)+"\n";
		Debug.LogWarning ("//result// Leaderboard=" + ttpp);
		testgetUserDataWithID (); /////////////////////////////

	}
	//end 获取排名前几的playfabid

	/// ///////////////////// <summary>
	/// 使用playfabid获取title data
	/// </summary>

	string[] IDS = new string[5];
	[SerializeField ]
	int indexID=0;
	public  void testgetUserDataWithID(){
		System.Collections.Generic.List<string> st=new System.Collections.Generic.List<string>();
		st.Add ("xp");
		st.Add ("LocLat");
		st.Add ("LocLng");//={"startLat","startLng"};
		st.Add ("static");
		st.Add ("preferences");
		var request = new GetUserDataRequest {PlayFabId=IDS[indexID]};
		PlayFabClientAPI.GetUserData  (request, OnUserDataSuccess, OnPlayFabError);
		Debug.Log ("!!GetUserData"+IDS[indexID]);
	}

	private void OnUserDataSuccess(GetUserDataResult    result)
	{
		if (indexID < 4) {
			indexID++;
		} else {
			indexID = 0;
		}
		Debug.Log("OnUserDataSuccess"+result.Data );
		//Debug.Log (result.Data.  );
		Debug.Log ("key="+result.Data.Keys.Count   );
		///Debug.Log ("preferences"+result.Data["preferences"] .Value);
		string ttt = "";
		foreach (string  ne in result.Data.Keys ) {
			ttt += "/ " + ne+"."+result.Data[ne].Value;
		}
		Debug.Log ("keys="+ttt );
	}
	//end使用playfabid获取title data

	////////////////////////update player title data在每关结束后update坐标
 	public  void testupdateuserdata(){
		System.Collections.Generic.Dictionary <string,string > st=new Dictionary<string, string>();
		st.Add ("xpT","10");
		st.Add ("LocLatT","123");
		st.Add ("LocLngT","456");//={"startLat","startLng"};

		var request = new UpdateUserDataRequest  {Data=st ,Permission=UserDataPermission.Public };
		PlayFabClientAPI.UpdateUserData(request, OnupdateUserDataSuccess, OnPlayFabError);

		Debug.Log ("!!testupdateuserdata");
	}

	private void OnupdateUserDataSuccess(UpdateUserDataResult    result)
	{
		Debug.Log("OnupdateUserDataSuccess"+result.DataVersion );
	}
	//end update player title data在每关结束后update坐标

	/// <summary>
	/// /////////////////////////////////
	/// </summary>
	/// 	public void testGetCurrentGames(){
	public void testStartGames(){
		var request = new StartGameRequest {BuildVersion="1",Region=PlayFab.ClientModels.Region.EUWest,GameMode="0"};

		PlayFabClientAPI.StartGame   (request, OnStartGamesSuccess, OnPlayFabError,null,null);
		Debug.Log ("!!testGetCurrentGames");
	}
	void OnStartGamesSuccess(StartGameResult result){

		Debug.Log ("!!OnStartGamesSuccess"+result.LobbyID +"  /"+result.ServerHostname );

	}

	#endregion 

	/// <summary>
	/// ///////////////////////////
	/// </summary>
	public void testGetCurrentGames(){
		var request = new CurrentGamesRequest {};

		PlayFabClientAPI.GetCurrentGames  (request, OnGetCurrentGamesSuccess, OnPlayFabError,null,null);
		Debug.Log ("!!testGetCurrentGames");
	}
	void OnGetCurrentGamesSuccess(CurrentGamesResult  result){

		Debug.Log ("!!GetCurrentGames"+result.Games.Count+"/"+result.GameCount +"/" +result.PlayerCount );
		string ttpp = "";
		List<GameInfo> GI =result.Games;
		for (int i = 0; i < GI .Count  ; i++) {
			for (int j = 0; j < GI[i].PlayerUserIds.Count   ; j++) {
				ttpp+=GI[i].PlayerUserIds[j] +" , ";
			}
			ttpp+="\n";
		}
		t.text += ("//result// PlayerUserIds=" + ttpp)+"\n";
		Debug.LogWarning ("//result// PlayerUserIds=" + ttpp);
	}

	/// <summary>
	/// /////////////////////
	/// </summary>
	/// <remarks>This method is commonly used to instantiate player characters.
	/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's
	/// button-press or a timer.
	/// 
	/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
	/// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find
	/// out if
	/// enough players are in the room to start playing.</remarks>

	public override void OnJoinedRoom()
	{
		LS = loginstate.joinroom;////****
		LogMessage ("OnJoinedRoom  "+PhotonNetwork.room.Name );
		Debug.LogError  ("OnJoinedRoom  "+PhotonNetwork.room.Name );
		//Tobj  = PhotonNetwork.Instantiate("photonPfbTest", Vector3.zero, Quaternion.identity, 0);
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
		LS = loginstate.connectPht ;////****
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