using UnityEngine;
using UnityEngine.UI ;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class GameWinManager : MonoBehaviour {
	public Text score;

	void Start(){

		float endlat = PlayerPrefManager.GetLat ();
		float endlng = PlayerPrefManager.GetLng ();
		Debug.Log ("End>>"+TerrainManager.lat+","+TerrainManager.lng+" to "+endlat+","+endlng);
		Cursor.visible=true;
		score.text = "you settle down at " + PlayerPrefManager.GetLat () + "," + PlayerPrefManager.GetLng ();
		score.text += "\n collected " + PlayerPrefManager.GetHighscore () + " points.";
		float xx =( endlng - TerrainManager.lng)*Mathf.PI*TerrainManager.earthR *Mathf.Cos(( endlng - TerrainManager.lng)*Mathf.Deg2Rad )/360;
		float yy = (endlat - TerrainManager.lat)*Mathf.PI*TerrainManager.earthR /180;
		float dist = Mathf.Sqrt (xx*xx+yy*yy);
		score.text +="You traveled "+dist+" m in "+BeatAnalysisManager.playtime +"  seconds";
		TerrainManager.lat = endlat;
		TerrainManager.lng = endlng;
	}
	public void loadLevel(string levelToLoad)
	{
		SceneManager.LoadScene(levelToLoad);
	}
}
