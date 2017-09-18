using UnityEngine;
using UnityEngine.UI ;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class GameWinManager : MonoBehaviour {
	public Text score;

	void Start(){

		float endlat = PlayerPrefManager.GetLat ();
		float endlng = PlayerPrefManager.GetLng ();
		Debug.Log ("End>>"+TerrainManagerStatics.Lat+","+TerrainManagerStatics.Lng+" to "+endlat+","+endlng);
		Cursor.visible=true;
		score.text = "you settle down at " + PlayerPrefManager.GetLat () + "," + PlayerPrefManager.GetLng ();
		score.text += "\n collected " + PlayerPrefManager.GetHighscore () + " points.";
		float xx =( endlng - TerrainManagerStatics.Lng)*Mathf.PI*TerrainManagerStatics.earthR *Mathf.Cos(( endlng - TerrainManagerStatics.Lng)*Mathf.Deg2Rad )/360;
		float yy = (endlat - TerrainManagerStatics.Lat)*Mathf.PI*TerrainManagerStatics.earthR /180;
		float dist = Mathf.Sqrt (xx*xx+yy*yy);
		score.text +="You traveled "+dist+" m in "+BeatAnalysisManager.playtime +"  seconds";
		TerrainManagerStatics.Lat = endlat;
		TerrainManagerStatics.Lng = endlng;
	}
	public void loadLevel(string levelToLoad)
	{
		SceneManager.LoadScene(levelToLoad);
	}
}
