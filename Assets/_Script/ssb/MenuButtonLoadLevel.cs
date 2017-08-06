using UnityEngine;
using UnityEngine.UI ;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MenuButtonLoadLevel : MonoBehaviour {
	public Text score;
	void Start(){
		Cursor.visible=true;
		score.text = "you settle down at " + PlayerPrefManager.GetLat () + "," + PlayerPrefManager.GetLng ();
		score.text+="\n collected "+PlayerPrefManager.GetScore ()+" points in "+BeatAnalysisManager.playtime +"  seconds";
	}
	public void loadLevel(string levelToLoad)
	{
		SceneManager.LoadScene(levelToLoad);
	}
}
