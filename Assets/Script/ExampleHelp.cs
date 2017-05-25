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
	[SerializeField]
	Text T;
	string showtext="";
	//	string AudioName="";
	//int ssize=0;


	void Start () {
		
		_audio=BeatAnalysisManager ._audio ;
	
		refreshtext ();

	}
	void refreshtext(){
		showtext="SpecSize = "+BeatAnalysisManager .SpecSize+"\n";
		showtext+="bufferSize = "+BeatAnalysisManager .bufferSize +"\n";
		showtext+="numBands = "+BeatAnalysisManager .numBands  +"\n";
		showtext+="enegryaddup = "+BeatAnalysisManager .enegryaddup   +"\n";
		T.text = showtext;
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
	public void changebuffersize(Slider s){
		//ssize = Mathf.Pow (2, s.value);
		BeatAnalysisManager .bufferSize  = (int)Mathf.Pow (2, s.value);//ssize;
		refreshtext();
	}
	public void changenumband(Slider s){
		//ssize = Mathf.Pow (2, s.value);
		BeatAnalysisManager .numBands  =(int) s.value;//ssize;
		refreshtext();
	}
	public void changeaddup(Slider s){
		//ssize = Mathf.Pow (2, s.value);
		BeatAnalysisManager .enegryaddup  = s.value;//ssize;
		refreshtext();

	}

	public void test(){
		//ssize = Mathf.Pow (2, s.value);
		//BeatAnalysisManager .enegryaddup  = s.value;//ssize;
		refreshtext();
	}

}
