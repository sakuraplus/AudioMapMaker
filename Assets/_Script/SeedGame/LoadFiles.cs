using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // include so we can load new scenes
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

public class LoadFiles : MonoBehaviour {
	[DllImport("__Internal")]
	//private static extern void ImageUploaderInit();
	private static extern void ImageUploaderCaptureClick();

	public  Text txtFile;	

	public  GameObject  panelAnalysis;
	public  GameObject  PanelExample;
	public  GameObject  PanelStart;
	public  GameObject  PanelOpenfile;
	AudioSource aus;
	WWW www;
	string  filepath;
	string []paths;
	//public AudioClip ac;

	void Start () {

		//tt.text += " > ";
		//ImageUploaderInit ();
		aus=BeatAnalysisManager._audio ;//<AudioSource>();
		if (BeatAnalysisManager.BAL.Count > 0) {
			txtFile.text = "You have an analysised music. Start and play.";
		}
		#if UNITY_WEBGL 
		PanelOpenfile.SetActive(false) ;
		#endif 
	}
	/// <summary>
	///  测试
	/// </summary>


	#if UNITY_WEBGL 
	/// <summary>
	/// sendmessage，选定文件后，需要挂在_script物体上
	/// </summary>
	/// <param name="url">URL.</param>
	public  void FileSelected (string url) {
//		tt.text += " s ";
		StartCoroutine(LoadMusic (url));
	}
	#endif 
	/// <summary>
	/// Loads the url
	/// </summary>
	/// <returns>The texture.</returns>
	/// <param name="url">URL.</param>
	IEnumerator LoadMusic (string url) {
		//tt.text += "\n!" + url;
		www = new WWW (url);
		yield return www;
		//tt.text+="\n OK";
		Tplaymus ();
	}

	/// <summary>
	/// 尝试音频格式，webgl传递的path不含扩展名
	/// </summary>
	/// <returns>The audio type.</returns>
	AudioClip tryAudioType(){
		AudioClip ac=null;

		string s=www.url .Substring(www.url.Length-3,3).ToLower() ;

		if(s=="wav"){
			ac = www.GetAudioClip (false, false,AudioType.WAV  );//,AudioType.WAV  );
			//ac=www.GetAudioClipCompressed(false,AudioType.WAV);

			ac.name = www.url.Substring (www.url.LastIndexOf ("\\")+1, (www.url.Length -1- www.url.LastIndexOf ("\\")));
			return ac;
		}else if(s== "ogg"){
			ac = www.GetAudioClip (false, false,AudioType.OGGVORBIS);
			ac.name = www.url.Substring (www.url.LastIndexOf ("\\")+1, (www.url.Length -1- www.url.LastIndexOf ("\\")));
			return ac;
		}else if(s=="mp3"){
			ac = www.GetAudioClip (false, false,AudioType.MPEG);
			ac.name = www.url.Substring (www.url.LastIndexOf ("\\")+1, (www.url.Length -1- www.url.LastIndexOf ("\\")));
			return ac;
		}


		try {
			ac = www.GetAudioClip (false, false,AudioType.WAV);
			ac.name =s+"_wav";
			//tt.text+="\nwav";
			return ac;
		}catch ( System.Exception  ex  ) {
			Debug.Log( ex);
			//tt.text+="wav-f  ";
		}
		try {
			ac = www.GetAudioClip (false, false,AudioType.MPEG );
			ac.name =s+"_mp3";
			//tt.text+="\nmp3";
			return ac;
		}catch ( System.Exception  ex  ) {
			Debug.Log( ex);
			//tt.text+="mp3-f  ";
		}
		try {
			ac = www.GetAudioClip (false, false,AudioType.OGGVORBIS  );
			ac.name =s+"_ogg";
			//tt.text+="\nogg";
			return ac;
		}catch ( System.Exception  ex  ) {
			Debug.Log( ex);
			//tt.text+="ogg-f  ";
		}
		return ac;
	}



	/// <summary>
	/// 赋值给audiosource，测试播放音乐
	/// </summary>
	public void Tplaymus(){

		AudioClip ac=tryAudioType();
		if (ac == null) {
			//tt.text+="\n acfail>>";
		}else{

		//tt.text +="  length="+ ac.length;
		//Debug.Log("ac="+ac.length  );
		//tt.text += "loadType=" + ac.loadType  +"--samples="+ac.samples+"\n";
		aus.clip =ac;
		//tt.text += "\nname=" + aus.clip.name;
		aus.clip.LoadAudioData ();

		//aus.Play ();
		}
	}
	/// <summary>
	/// Raises the button pointer down event.
	/// </summary>
	public void BtnPointerDown () {
		
		//tt.text+= ">";
		#if UNITY_STANDALONE
		PCopenDialog();
		#else 
		ImageUploaderCaptureClick ();
		#endif 
	}

	#if UNITY_STANDALONE
	/// <summary>
	/// open file dialog
	/// </summary>

	public void PCopenDialog(){
		OpenFileName ofn = new OpenFileName();
		ofn.structSize = Marshal.SizeOf(ofn);
		ofn.filter = "All Files\0*.*\0\0";//"*.wav|*.ogg";//
		ofn.file = new string(new char[256]);
		ofn.maxFile = ofn.file.Length;

		ofn.fileTitle = new string(new char[64]);

		ofn.maxFileTitle = ofn.fileTitle.Length;

		ofn.initialDir =UnityEngine.Application.dataPath;//默认路径

		ofn.title = "Music List";

		ofn.defExt = "wav";//显示文件的类型
		//注意 一下项目不一定要全选 但是0x00000008项不要缺少
		ofn.flags=0x00080000|0x00001000|0x00000800|0x00000200|0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

		if(DllOpenFileDialog.GetOpenFileName( ofn ))
		{
			//StartCoroutine(WaitLoad(ofn.file));//加载图片到panle
			StartCoroutine(LoadMusic("file://"+ofn.file));//加载图片到panle
			Debug.Log( "Selected file with full path: {0}"+ofn.file );
			txtFile.text = ("Selected file with full path:" + ofn.file);
			if (panelAnalysis) {
				panelAnalysis.SetActive (true);
			}
			if (PanelStart) {
				PanelStart.SetActive (true);
			}
			if (PanelExample) {
				PanelExample.SetActive (false);
			}

		}

	}


	#endif 
	public void BtnStartGame(string levelToLoad)
	{
		// start new game so initialize player state
		//PlayerPrefManager.ResetPlayerState(startLives,false);
		if (BeatAnalysisManager.BAL.Count < 1) {
			txtFile .text="There is no Data of beatmap,Try load and analysis a music file, or load an example music.";
			return;
		}
		Debug.LogError ("LOC="+TerrainManagerStatics.Lat+","+TerrainManagerStatics.Lng);
		// load the specified level
		BeatAnalysisManager._audio=aus;
		BeatAnalysisManager.playtime = aus.clip.length;
		BeatAnalysisManager.defaultAudioclip = aus.clip;
		SceneManager.LoadScene(levelToLoad);
	}
	public void BtnMainMenu(string levelToLoad)
	{
		
		SceneManager.LoadScene(levelToLoad);
	}
	public void BtnPlayMusic(){
		//tt.text = www.bytesDownloaded+","+tt.text;
		if (aus.clip != null) {
			aus.Play ();
		}

	}
	public void BtnStopMusic(){
		//tt.text = www.bytesDownloaded+","+tt.text;
		if (aus.clip != null) {
			aus.Stop ();
		}
	}
	public void BtnDefaultMusic(AudioClip acdefault){
		if (panelAnalysis) {
			panelAnalysis.SetActive (false);
		}
		if (PanelExample) {
			PanelExample.SetActive (true);
		}
		aus.clip=acdefault;

	}
	/// <summary>
	/// 测试
	/// </summary>
	public void ttt(){
		//tt.text = www.bytesDownloaded+","+tt.text;
		if (aus.clip != null) {
			float[] d=new float[aus.clip.samples ] ;
			aus.clip.GetData (d, 0);
			//tt.text += "\naus=" + aus.clip.length + "," + d.Length + "," + aus.clip.name;
		}
	}
}
