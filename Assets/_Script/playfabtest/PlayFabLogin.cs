using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections ;
using System.Collections.Generic;
public class PlayFabLogin : MonoBehaviour
{
	string pfid="aaa";
    public void Start()
    {
		//logintest ();
		AuthenticateWithPlayFab ();
    }

	///////////////////////////////////////////
	/// 
	private  void AuthenticateWithPlayFab()
	{
		//LogMessage ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);
		Debug.Log ("PlayFab authenticating using Custom ID..."+PlayFabSettings.DeviceUniqueIdentifier);

		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = PlayFabSettings.DeviceUniqueIdentifier+"EDITOR"
			}, RequestPhotonToken, OnFailure);
	}
	//CustomId =customId ;// PlayFabSettings.DeviceUniqueIdentifier
	/*
    * Step 2
   
    */
	private void RequestPhotonToken(LoginResult obj)
	{
		//LogMessage("PlayFab authenticated. Requesting photon token...");
		Debug.Log("PlayFab authenticated. Requesting photon token...");

		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
			}, AuthenticateWithPhoton, OnFailure);
	}


	/*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
	{
		
		Debug.Log("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

		//We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
		var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

		//We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
		customAuth.AddAuthParameter("username",pfid );    // expected by PlayFab custom auth service

		//We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
		customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

		//We finally tell Photon to use this authentication parameters throughout the entire application.
		PhotonNetwork.AuthValues = customAuth;
		PhotonNetwork.autoJoinLobby = true;
	}

	// Example for getting the user statistics for a player.
	IEnumerator GetUserStats(float sec = 0)
	{
		yield return new WaitForSeconds(sec);
		GetPlayerStatisticsRequest  request = new GetPlayerStatisticsRequest();
		PlayFabClientAPI.GetPlayerStatistics (request, OnGetUserStatsSuccess, OnFailure);
	}

	// callback on successful GetUserStats request

	List <StatisticValue> userStats;//=new List <StatisticValue >();// <statisticvalue>
	void OnGetUserStatsSuccess(GetPlayerStatisticsResult  result)
	{
		// some logic for determineing if the player's team changed.
//		if(result.Statistics .ContainsKey("BluTeamJoinedCount") && this.userStats.ContainsKey("BluTeamJoinedCount"))
//		{
//			if(result.UserStatistics["BluTeamJoinedCount"] > this.userStats["BluTeamJoinedCount"] )
//			{
//				this.team = "Blu";
//				Debug.Log(result.UserStatistics["BluTeamJoinedCount"] + " : " + this.userStats["BluTeamJoinedCount"]);
//			} 
//		}
//		else if(result.UserStatistics.ContainsKey("BluTeamJoinedCount") && !this.userStats.ContainsKey("BluTeamJoinedCount"))
//		{
//			if(this.userStats.Count != 0)
//			{
//				this.team = "Blu";
//				Debug.Log(result.UserStatistics["BluTeamJoinedCount"] + " : blue" );
//			}
//		}

//		if(result.UserStatistics.ContainsKey("RedTeamJoinedCount") && this.userStats.ContainsKey("RedTeamJoinedCount"))
//		{
//			if(result.UserStatistics["RedTeamJoinedCount"] > this.userStats["RedTeamJoinedCount"] )
//			{
//				this.team = "Red";
//				Debug.Log(result.UserStatistics["RedTeamJoinedCount"] + " : " + this.userStats["RedTeamJoinedCount"]);
//			} 
//		}
//		else if(result.UserStatistics.ContainsKey("RedTeamJoinedCount") && !this.userStats.ContainsKey("RedTeamJoinedCount"))
//		{
//			if(this.userStats.Count != 0)
//			{
//				this.team = "Red";
//				Debug.Log(result.UserStatistics["RedTeamJoinedCount"] + " : red");
//			}
//		}
		// save user stats for game useage
		this.userStats = result.Statistics;
	}

	public void ConnectToMasterServer(string id, string ticket)
	{

		AuthenticationValues customAuth = new AuthenticationValues();
		customAuth.AuthType = CustomAuthenticationType.Custom;
		customAuth.AddAuthParameter("username", id);    // expected by PlayFab custom auth service
		customAuth.AddAuthParameter("token", ticket);   // expected by PlayFab custom auth service
//
//		this.GameInstance.AuthValues = customAuth;
//
//		//this.GameInstance.AutoJoinLobby = false;                      // use this, if you don't list current rooms
//		this.GameInstance.loadBalancingPeer.QuickResendAttempts = 2;    // option to re-send reliable stuff more quickly
//		this.GameInstance.loadBalancingPeer.SentCountAllowance = 8;     // default + some quick resends
//
//
//		this.GameInstance.ConnectToRegionMaster("US");  // connect to the US region of Photon Cloud
	}








	/// <summary>
	/// //////////////////////////////////////////
	/// </summary>

//	public void logintest()
//	{
//		PlayFabSettings.TitleId = "E7D7"; // Please change this value to your own titleId from PlayFab Game Manager
//
//		var request = new LoginWithCustomIDRequest { CustomId = "aaa", CreateAccount = true};
//		PlayFabClientAPI.LoginWithCustomID( request, OnLoginSuccess, OnFailure );
//		Debug.Log ("!!LoginWithCustomID");
//
//	}
	public void getnewstest(){
		var request = new GetTitleNewsRequest  {Count = 25};
	
		PlayFabClientAPI.GetTitleNews(request, OnNewsSuccess, OnFailure,null,null);
		Debug.Log ("!!GetTitleNews");
	}
	public void getstatictest(){
		System.Collections.Generic.List<string> st=new System.Collections.Generic.List<string>();
		st.Add ("startLat");
		st.Add ("startLng");//={"startLat","startLng"};
		var request = new GetPlayerStatisticsRequest{StatisticNames =st};

		PlayFabClientAPI.GetPlayerStatistics (request, OnstaticSuccess, OnFailure);
		Debug.Log ("!!GetTitleNews");
	}
	public void getcatalogtest(){
		var request = new GetCatalogItemsRequest   {CatalogVersion  = "main"};

		PlayFabClientAPI.GetCatalogItems (request, OnCatalogSuccess, OnFailure,null,null);
		Debug.Log ("!!GetCatalogItems");
	}
	public void getpurchtest(){
		var request = new PurchaseItemRequest {ItemId  = "seed",VirtualCurrency ="EP",Price =1};

		PlayFabClientAPI.PurchaseItem ( request, OnPurchSuccess, OnFailure,null,null);
		Debug.Log ("!!PurchaseItemRequest");
	}

	public void getCStest(){
		var request = new ExecuteCloudScriptRequest  {FunctionName   = "bushelOnYourFirstDay" };

		PlayFabClientAPI.ExecuteCloudScript  ( request, OnCSSuccess, OnFailure,null,null);
		Debug.Log ("!!getCStest");
	}
	public void getCSLoctest(){
		//Vector2 vv = new Vector2 (156,489);
		var request = new ExecuteCloudScriptRequest ();
		request.FunctionName   = "UpdateLocAtTheEnd";
		request.FunctionParameter = new{LocLat = 123,LocLng = 147};
		object ob = new{locs=new float[5]};
		PlayFabClientAPI.ExecuteCloudScript  ( request, OnCSSuccess, OnFailure,ob,null);
		Debug.Log ("!!getCSLoctest");
	}
	private void OnCSSuccess(ExecuteCloudScriptResult result)
	{
		Debug.Log("Congratulations, GetTitleNews");
		Debug.Log (result.HttpRequestsIssued    );
		foreach (LogStatement    ne in result.Logs ) {
			Debug.Log (ne.Data    +"/"+ne.Level  +"/"+ne.Message     );
		}

		var request = new GetUserInventoryRequest  {};

		PlayFabClientAPI.GetUserInventory  (request, OnInventorySuccess, OnFailure,null,null);
		Debug.Log ("!!PurchaseItemRequest");
	}

	private void OnPurchSuccess(PurchaseItemResult result)
	{
		Debug.Log("Congratulations, GetTitleNews");
		Debug.Log (result.Items    );
		foreach (ItemInstance   ne in result.Items ) {
			Debug.Log (ne.ItemId   +"/"+ne.PurchaseDate  +"/"+ne.UnitPrice    );
		}

		var request = new GetUserInventoryRequest  {};

		PlayFabClientAPI.GetUserInventory  (request, OnInventorySuccess, OnFailure,null,null);
		Debug.Log ("!!PurchaseItemRequest");
	}
	private void OnInventorySuccess(GetUserInventoryResult     result)
	{
		Debug.Log("Congratulations, OnInventorySuccess");
		Debug.Log (result.VirtualCurrency    );
		foreach (ItemInstance   ne in result.Inventory ) {
			Debug.Log (ne.DisplayName  +"/ "+ne.UnitPrice  +"/ "+ne.RemainingUses    );
		}


	}

	private void OnCatalogSuccess(GetCatalogItemsResult    result)
	{
		Debug.Log("Congratulations, OnCatalogSuccess");
		Debug.Log (result.Catalog   );
		foreach (CatalogItem  ne in result.Catalog) {
			Debug.Log (ne.DisplayName  +"/"+ne.VirtualCurrencyPrices +"/"+ne.Description   );
		}


	}


	private void OnstaticSuccess(GetPlayerStatisticsResult   result)
	{
		Debug.Log("Congratulations, OnstaticSuccess");
		Debug.Log (result.Statistics  );
		foreach (StatisticValue ne in result.Statistics) {
			Debug.Log (ne.StatisticName +"/"+ne.Value +"/"+ne.Version  );
		}
	}


	private void OnNewsSuccess(GetTitleNewsResult  result)
	{
		Debug.Log("Congratulations, GetTitleNews");
		Debug.Log (result.CustomData );
		foreach (TitleNewsItem ne in result.News) {
			Debug.Log (ne.NewsId +"/"+ne.Title +"/"+ne.Body );
		}
	}




    private void OnFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }


//		PlayFab.ClientModels.GetTitleNewsRequest gtn=new PlayFab.ClientModels.GetTitleNewsRequest() ; 
// 		gtn="POST https://E7D7.playfabapi.com/Client/GetTitleNews Content-Type: application/json X-Authentication: <user_session_ticket_value>
//			{
//  "Count": 25
//}
//
//
//		

}