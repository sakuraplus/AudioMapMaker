using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager

public static class PlayerPrefManager {

	public static float GetLat() {
		if (PlayerPrefs.HasKey("Lat")) {
			return PlayerPrefs.GetFloat ("Lat");
		} else {
			return 0;
		}
	}

	public static void SetLat(float  _lat) {
		PlayerPrefs.SetFloat("Lat",_lat);
	}
	//==================================
	public static float GetLng() {
		if (PlayerPrefs.HasKey("Lng")) {
			return PlayerPrefs.GetFloat ("Lng");
		} else {
			return 0;
		}
	}

	public static void SetLng(float  _lng) {
		PlayerPrefs.SetFloat("Lng",_lng);
	}
	//=====================================
	public static int GetDataSource() {
		if (PlayerPrefs.HasKey("DataSource")) {
			return PlayerPrefs.GetInt("DataSource");
		} else {
			return 2;
		}
	}
	public static void SetDataSource(int score) {
		PlayerPrefs.SetInt("DataSource",score);
	}
	//=====================================
	public static string GetApiKey(string st) {
		if (st == "G") {
			if (PlayerPrefs.HasKey("KeyGoogle")) {
				return PlayerPrefs.GetString("KeyGoogle");
			} else {
				return "";
			}
		}else if (st == "B") {
			if (PlayerPrefs.HasKey("KeyBing")) {
				return PlayerPrefs.GetString("KeyBing");
			} else {
				return "";
			}
		}
		return "";
	}
	public static void SetApiKey(string  st,string key) {
		if (st == "G") {
			PlayerPrefs.SetString ("KeyGoogle", key);
		} else if (st == "B") {
			PlayerPrefs.SetString ("KeyBing", key);
		}
	}
	//=====================================
	public static int GetScore() {
		if (PlayerPrefs.HasKey("Score")) {
			return PlayerPrefs.GetInt("Score");
		} else {
			return 0;
		}
	}
	public static void SetScore(int score) {
		PlayerPrefs.SetInt("Score",score);
	}
	//=====================================
	public static float GetHighscore() {
		if (PlayerPrefs.HasKey("Highscore")) {
			return PlayerPrefs.GetFloat("Highscore");
		} else {
			return 0;
		}
	}

	public static void SetHighscore(int highscore) {
		PlayerPrefs.SetFloat("Highscore",highscore);
	}
	//=====================================
	public static string  GetRecord() {
		if (PlayerPrefs.HasKey("GameRecord")) {
			return PlayerPrefs.GetString("GameRecord");
		} else {
			return "";
		}
	}

	public static void SetRecord(string  newrecord) {
		string s=PlayerPrefs.GetString("GameRecord");
		PlayerPrefs.SetString("GameRecord",newrecord+"|"+s);
	}
	public static void clearRecord() {
		
		PlayerPrefs.SetString("GameRecord","");
	}

	//=====================================

	// story the current player state info into PlayerPrefs
	public static void SavePlayerState(int score, float highScorePer, float _lat,float _lng) {
		// save currentscore and lives to PlayerPrefs for moving to next level
		PlayerPrefs.SetInt("Score",score);
		//PlayerPrefs.SetInt("Lives",lives);
		PlayerPrefs.SetFloat("Highscore",highScorePer);
		PlayerPrefs.SetFloat("Lng",_lng);
		PlayerPrefs.SetFloat("Lat",_lat);
		SetRecord (_lat+","+_lng);
	}
	
	// reset stored player state and variables back to defaults
	public static void ResetPlayerState(int startLives, bool resetHighscore) {
		Debug.Log ("Player State reset.");
		PlayerPrefs.SetInt("Lives",startLives);
		PlayerPrefs.SetInt("Score", 0);

		if (resetHighscore)
			PlayerPrefs.SetFloat("Highscore", 0);
	}

	// store a key for the name of the current level to indicate it is unlocked
	public static void UnlockLevel() {
		// get current scene
		Scene scene = SceneManager.GetActiveScene();
	Debug.Log("unlock "+scene.name);
		PlayerPrefs.SetInt(scene.name,1);
	}

	// determine if a levelname is currently unlocked (i.e., it has a key set)
	public static bool LevelIsUnlocked(string levelName) {
		return (PlayerPrefs.HasKey(levelName));
	}

	// output the defined Player Prefs to the console
	public static void ShowPlayerPrefs() {
		// store the PlayerPref keys to output to the console
		string[] values = {"Score","Highscore","Lives"};

		// loop over the values and output to the console
		foreach(string value in values) {
			if (PlayerPrefs.HasKey(value)) {
				Debug.Log (value+" = "+PlayerPrefs.GetInt(value));
			} else {
				Debug.Log (value+" is not set.");
			}
		}
	}
}
