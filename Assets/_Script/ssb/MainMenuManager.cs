using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MainMenuManager : MonoBehaviour {

	public int startLives=3; // how many lives to start the game with on New Game

	// references to Submenus
	public GameObject _MainMenu;
	public GameObject _LevelsMenu;
	public GameObject _AboutMenu;
	public GameObject _SettingMenu;


	// references to Button GameObjects
	public GameObject MenuDefaultButton;
	public GameObject AboutDefaultButton;
	public GameObject LevelSelectDefaultButton;
	public GameObject QuitButton;
	public GameObject ContinueButton;
	// list the level names
	public string[] LevelNames;

	// reference to the LevelsPanel gameObject where the buttons should be childed
	public GameObject LevelsPanel;

	// reference to the default Level Button template
	public GameObject LevelButtonPrefab;
	
	// reference the titleText so we can change it dynamically
	public Text titleText;

	// store the initial title so we can set it back
	private string _mainTitle;

	// init the menu
	void Awake()
	{
		// store the initial title so we can set it back
		_mainTitle = titleText.text;

		// disable/enable Level buttons based on player progress
		//setLevelSelect();
		setDataSource (false);
		// determine if Quit button should be shown
		displayQuitWhenAppropriate();
		displayContinue();

		// Show the proper menu
		ShowMenu("MainMenu");
	}
	void displayContinue(){
		if (PlayerPrefManager.GetRecord ().Length > 0) {
			ContinueButton.SetActive (true);
		} else {
			ContinueButton.SetActive (false);
		}
	}
	// loop through all the LevelButtons and set them to interactable 
	// based on if PlayerPref key is set for the level.
//	void setLevelSelect() {
//		_LevelsMenu.SetActive(true);
//		string recordstr = PlayerPrefManager.GetRecord ();
//		//recordstr="12,34|12.34,34,56|1,2|4,5|aaa,ddd|ccc,ddd|6,6|7,77|8,88|9,999|0,00|q,w|e,r|";
//		///>>12,34/12.34,34,56/1,2/4,5/aaa,ddd/ccc,ddd/6,6/7,77/8,88/9,999/0,00/q,w/e,r//
//		if (recordstr.Length > 0) {
//			string sss = "|";
//			string[] records = recordstr.Split (  sss.ToCharArray()[0]);
//			string t = ">>";
//			for (int i=0;i < records.Length &&i<9; i++) {
//				t+=records[i]+"/";
//
//				GameObject levelButton = Instantiate (LevelButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
//				levelButton.name ="lastpoint"+i;// levelname + " Button";
//
//				levelButton.transform.SetParent (LevelsPanel.transform, false);
//				Button levelButtonScript = levelButton.GetComponent<Button> ();
//
//				levelButtonScript.onClick.RemoveAllListeners ();
//				levelButtonScript.onClick.AddListener (() => loadLevelselect (records[i]));
//				Text levelButtonLabel = levelButton.GetComponentInChildren<Text> ();
//				levelButtonLabel.text = records[i];//"start at the last point";//levelname;					
//
//			}
//			Debug.Log (t);
//		}
//
//	}


	// determine if the QUIT button should be present based on what platform the game is running on
	void displayQuitWhenAppropriate() 
	{
		switch (Application.platform) {
			// platforms that should have quit button
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.LinuxPlayer:
				QuitButton.SetActive(true);
				break;

			// platforms that should not have quit button
			// note: included just for demonstration purposed since
			// default will cover all of these. 
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.WebGLPlayer: 
				QuitButton.SetActive(false);
				break;

			// all other platforms default to no quit button
			default:
				QuitButton.SetActive(false);
				break;
		}
	}

	// Public functions below that are available via the UI Event Triggers, such as on Buttons.

	// Show the proper menu
	public void ShowMenu(string name)
	{
		// turn all menus off
		_MainMenu.SetActive (false);
		_AboutMenu.SetActive(false);
		_LevelsMenu.SetActive(false);
		_SettingMenu.SetActive(false );
		// turn on desired menu and set default selected button for controller input
		switch(name) {
		case "MainMenu":
			_MainMenu.SetActive (true);
			EventSystem.current.SetSelectedGameObject (MenuDefaultButton);
			titleText.text = _mainTitle;
			break;
		case "LevelSelect":
			_LevelsMenu.SetActive (true);
			setData (PlayerPrefManager.GetDataSource ());
			EventSystem.current.SetSelectedGameObject (LevelSelectDefaultButton);
			titleText.text = "Level Select";
			break;
		case "About":
			_AboutMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject (AboutDefaultButton);
			titleText.text = "About";
			break;
		case "Setting":
			_SettingMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject (AboutDefaultButton);
			titleText.text = "Setting";
			break;
		}
	}
	// load the specified Unity level
	[SerializeField]
	InputField keyGoogle;
	[SerializeField]
	InputField keyBing;

	public  void setDataSource(bool settoDefault) {
		if (!PlayerPrefs.HasKey ("KeyGoogle") || settoDefault) {
			PlayerPrefManager.SetDataSource (2);
			PlayerPrefManager.SetApiKey ("G", "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE");
			PlayerPrefManager.SetApiKey ("B", "Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_");
			keyGoogle.text = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";
			keyBing.text = "Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_";
			//datasTxt.text = "Default Setting";
			//datasTxt.text += " \n  data source =  Random Map";
			//datasTxt.text += "\nBuild a random terrain. Will not call for network resources.";
			//datasTxt.text += "\nGet a private Apikey for a better play experience.";
		} else {
			keyGoogle.text = PlayerPrefManager.GetApiKey("G");
			keyBing.text = PlayerPrefManager.GetApiKey("B");

		}
		refreshDataSourceTxt ();
	}

	void refreshDataSourceTxt(){
		if (PlayerPrefManager.GetDataSource () == 0) {
			TerrainManagerStatics.DataSource = datasource.bing;
			datasTxt.text = "   data source =  Bing Map";
			datasTxt.text += "\nBuild the terrain with Bing Map elevation data.";
		}else if (PlayerPrefManager.GetDataSource () == 1) {
			TerrainManagerStatics.DataSource = datasource.google ;
			datasTxt.text = "   data source =  Google Map";
			datasTxt.text += "\nBuild the terrain with Google Map elevation data.";
		}else if (PlayerPrefManager.GetDataSource () == 2) {
			TerrainManagerStatics.DataSource = datasource.random ;
			datasTxt.text = "   data source =  Random Map";
			datasTxt.text += "\nBuild a random terrain. Will not call for network resources.";
		}
	}
	public  Text datasTxt;
	public void setData(int deind)
	{
//		string txt="   data source =  ";
//		if (deind == 0) {
//			TerrainManager.DataSource = datasource.bing;
//			txt+="bing";
//		}else 	if (deind == 1) {
//			TerrainManager.DataSource = datasource.google ;
//			txt+="google";
//		}else	if (deind == 2) {
//			TerrainManager.DataSource = datasource.random ;
//			txt+="random map";
//		}
//		datasTxt.text = txt;
		// start new game so initialize player state
		PlayerPrefManager.SetDataSource(deind);
		PlayerPrefManager.SetApiKey ("G", keyGoogle.text );
		PlayerPrefManager.SetApiKey ("B", keyBing .text);
		refreshDataSourceTxt ();
	
	}
	public void setData()
	{
		PlayerPrefManager.SetApiKey ("G", keyGoogle.text );
		PlayerPrefManager.SetApiKey ("B", keyBing .text);
	}
	// load the specified Unity level
	public void LoadScene(string scenename)
	{
		//switch(name) {
		//case "MainMenu":
		SceneManager.LoadScene(scenename);
	}
	public void resetRecord(){
		PlayerPrefManager.clearRecord (); 
	}
	public void testPf()
	{
		Debug.Log ("TEST PF---------");
		Debug.Log ("Lat,lng="+PlayerPrefManager.GetLat()+","+PlayerPrefManager.GetLng());
		Debug.Log ("key B="+PlayerPrefManager.GetApiKey("B"));
		Debug.Log ("key G"+PlayerPrefManager.GetApiKey("G"));
		Debug.Log ("DS="+PlayerPrefManager.GetDataSource());
		Debug.Log ("rec="+PlayerPrefManager.GetRecord ());
	}

	// quit the game
	public void QuitGame()
	{
		Application.Quit ();
	}
}
