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
	
		//refreshtext ();

	}
	void refreshtext(){
		showtext="SpecSize = "+BeatAnalysisManager .SpecSize+"\n";
		showtext+="bufferSize = "+BeatAnalysisManager .bufferSize +"\n";
		showtext+="numBands = "+BeatAnalysisManager .numBands  +"\n";
		showtext+="enegryaddup = "+BeatAnalysisManager .enegryaddup   +"\n";
		T.text = showtext;
		//BeatAnalysisManager.initDictOfBand ();
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
	public void setUseInc(Toggle  t){
		BeatAnalysisManager.CheckWithInc = t.isOn;
	
		Debug.Log (BeatAnalysisManager.CheckWithInc );
	}

	public void showframerate()
	{
		Debug.Log ("length= " + BeatAnalysisManager.MusicArrayList.Count );
		//JsonUtility 
		//_texture = Resources.Load (EleDataToMeshTerrain.savefiledate+"/"+filenameMat) as Texture2D;
//		string st=Resources.Load ("save/m01") as string;
//		Debug.Log ("st=" + st);
	}
}
