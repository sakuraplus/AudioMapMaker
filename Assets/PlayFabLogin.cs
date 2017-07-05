using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public void Start()
    {
       

    }
	public void logintest()
	{
		PlayFabSettings.TitleId = "E7D7"; // Please change this value to your own titleId from PlayFab Game Manager

		var request = new LoginWithCustomIDRequest { CustomId = "aaa", CreateAccount = true};
		PlayFabClientAPI.LoginWithCustomID( request, OnLoginSuccess, OnFailure );
		Debug.Log ("!!LoginWithCustomID");

	}
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
		var request = new ExecuteCloudScriptRequest  {FunctionName   = "bushelOnYourFirstDay"};

		PlayFabClientAPI.ExecuteCloudScript  ( request, OnCSSuccess, OnFailure,null,null);
		Debug.Log ("!!PurchaseItemRequest");
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



    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
		Debug.Log (result.LastLoginTime);
		Debug.Log (result.PlayFabId);
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