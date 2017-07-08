using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI ;
public class pfa : MonoBehaviour
{
	[SerializeField ]
	private string _playFabPlayerIdCache;
	[SerializeField]
	Text  t;

	//Run the entire thing on awake
	public void Awake()
	{
		AuthenticateWithPlayFab();
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
	private void AuthenticateWithPlayFab()
	{
		LogMessage ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
		Debug.Log ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
//		LoginWithCustomIDRequest LWCrequest = new LoginWithCustomIDRequest
//		{ CreateAccount=true,CustomId= PlayFabSettings.DeviceUniqueIdentifier};
//		PlayFabClientAPI.LoginWithCustomID(LWCrequest , RequestPhotonToken, OnPlayFabError);
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = PlayFabSettings.DeviceUniqueIdentifier
			}, RequestPhotonToken, OnPlayFabError);
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
	}
	////////////////////////////////////////////////////////
	public void test()
	{

		Debug.LogError ("//test// " + PhotonNetwork.AuthValues);
		Debug.LogError ("//test// " + PhotonNetwork.countOfPlayers);
		Debug.LogError ("//test// " + PhotonNetwork.countOfRooms);
		Debug.LogError ("//test// " + PhotonNetwork.inRoom);
		t.text = ("//test// " + PhotonNetwork.AuthValues);
		t.text+= ("//test// " + PhotonNetwork.countOfPlayers);
		t.text+=  ("//test// " + PhotonNetwork.countOfRooms);
		t.text+=  ("//test// " + PhotonNetwork.inRoom+"//"+PhotonNetwork.room.Name );
	}

	public void testJoinRandom()
	{
		
		
	//	ConnectAndJoinRandom ();
		if (PhotonNetwork.connected) {
			Debug.Log ("connected");
			t.text += "con!";
			if (PhotonNetwork.countOfRooms > 0) {
				//bool ttt =
				t.text += "   JoinRandomRoom!";
				PhotonNetwork.JoinRandomRoom ();

			} else {
				t.text += "   CreateRoom!";
				PhotonNetwork.CreateRoom ("testroom");
			}

		} else {
			Debug.Log ("unconnect!");
			t.text += "uncon!";
			PhotonNetwork.ConnectUsingSettings ("1");//.ConnectToMaster
		}

	}


	public override void OnJoinedRoom()
	{
		LogMessage ("OnJoinedRoom  "+PhotonNetwork.room.Name );
		Debug.Log ("OnJoinedRoom  "+PhotonNetwork.room.Name );
//		GameObject monster = PhotonNetwork.Instantiate("monsterprefab", Vector3.zero, Quaternion.identity, 0);
//		monster.GetComponent<myThirdPersonController>().isControllable = true;
//		myPhotonView = monster.GetComponent<PhotonView>();
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
		t.text=("PlayFab + Photon Example: " + message);
	}

}