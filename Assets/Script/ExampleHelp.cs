using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;




/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
/// 
public class ExampleHelp : MonoBehaviour {
	
	AudioSource _audio;
	[SerializeField]
	AudioClip[] musics;
//	string AudioName="";



	void Start () {
		
		_audio=BeatAnalysisManager._audio ;
		//AudioName = _audio.name;
	}
	public void btnChangemusic(int i){

		if (i < musics.Length) {
			if (musics [i] == null) {
				Debug.LogError ("wrong music!");
				return;
			}
			_audio.clip = musics [i];
		}
	}



}
