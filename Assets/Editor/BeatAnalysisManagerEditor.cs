using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(BeatAnalysisManager ))]
public class BeatAnalysisManagerEditor : Editor { // extend the Editor class
	string bandlengthtxt;


	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// get a reference to the GameManager script on this target gameObject
		BeatAnalysisManager  BAM = (BeatAnalysisManager )target;

		// show the default inspector stuff for this component
		DrawDefaultInspector();

		//apply all setting
		GUILayout.Space (30);
		GUILayout.Label ("Apply all setting if you \n want to run in editor mode",GUILayout.Width(200) );
		if(GUILayout.Button("apply setting and enable editor tool"))
		{
			BAM.Awake ();//.initPara ();
		}
		GUILayout.Space (30);
		// if the manager is init
		if (BeatAnalysisManager._audio != null) {

			//play/stop
			GUILayout.Label ("*buffersize setting helpper*\nplay the default music and tap bpm" );
			string playingstate = "play";
			if (BeatAnalysisManager._audio.isPlaying) {
				playingstate = "stop";
			}

			if (GUILayout.Button (playingstate)) {
				if (!BeatAnalysisManager._audio.isPlaying) {
					BeatAnalysisManager._audio.Play ();
				} else {
					BeatAnalysisManager._audio.Stop ();
				}
			}
			//end play/stop


			// bpm helpper
			GUILayout.Label ("click when you find a beat");
			if (GUILayout.Button ("tap to set bpm")) {
				setBPM ();
			}
			GUILayout.Label (bpmframe + " frames between beats ,\nrecommend buffersize \n" + recbuffersize);
			//end bpm helpper


			GUILayout.Space (30);
			// save beatmap
			if (BeatAnalysisManager.BAL.Count >1) {
				if (GUILayout.Button ("save beatmap")) {
					//if (BeatAnalysisManager.BAL.Count < 1) {
//					if (EditorUtility.DisplayDialog ("error", "The beatmap result is empty", "ok")) {
//					}
//					return;
//				}
					string filepath = EditorUtility.SaveFilePanel ("save json beatmap", "", BeatAnalysisManager._audio.clip.name + ".json", "json");
					if (filepath.Length > 0) {
						Debug.Log (filepath);
						EditorSave (filepath);
					}
					// if button pressed, then call function in script
					//Debug.Log ("oooo");
					//if (EditorUtility.DisplayDialog("????",   "aaaaa?",   "Yes00", "No")) {
					//}
				}
			}//end save beatmap

		}//end if is init

	}//end on ins gui





	//end分频段检测是否为节拍
		int lastSetbpmframe=-1;
		int Setbpmframe=-1;
		int bpmframe=0;
		int recbuffersize=0;
//		int recsampletime=0;
//		[SerializeField]
//		//Text  bpmtxt;
//		bool bpmsetting=false;
	//ArrayList bpmlist;
	public void setBPM()
	{

		if (BeatAnalysisManager._audio==null) {
			//BeatAnalysisManager._audio.Play ();
			//	bpmsetting = true;
			return;
		}
		if (!BeatAnalysisManager._audio.isPlaying) {
			//BeatAnalysisManager._audio.Play ();
			//	bpmsetting = true;
				return;
		}
		Debug.Log ("setBPM  last="+lastSetbpmframe+" this="+Setbpmframe+" --=" +(Setbpmframe - lastSetbpmframe) +"//bpmframe*2="+ (bpmframe * 2));
		if (lastSetbpmframe < 0 &&Setbpmframe < 0) {
			lastSetbpmframe = BeatAnalysisManager._audio.timeSamples  ;
			Debug.Log ("set 1");
			return;
		} 
		if (lastSetbpmframe >= 0&&Setbpmframe < 0) {
			Setbpmframe =  BeatAnalysisManager._audio .timeSamples  ;
			bpmframe = Setbpmframe - lastSetbpmframe;
			Debug.Log ("set 2");
			return;
		} 
		lastSetbpmframe = Setbpmframe;
		Setbpmframe =  BeatAnalysisManager._audio .timeSamples;

		if (lastSetbpmframe >= 0 && Setbpmframe > 0) {
			if (Mathf.Abs (Setbpmframe - lastSetbpmframe ) > (bpmframe * 2) ||Mathf.Abs (Setbpmframe - lastSetbpmframe ) < (bpmframe /2)) {
				lastSetbpmframe = -1;
				Setbpmframe = -1;
				bpmframe = 0;
				Debug.Log ("re set bpm " + (Setbpmframe - lastSetbpmframe) + "//" + (bpmframe * 2));
			} else {
				bpmframe = (bpmframe + Setbpmframe - lastSetbpmframe) / 2;
				//bandlength
				// (int)Mathf.Floor (bpmframe*Application.targetFrameRate);
				recbuffersize=Mathf.RoundToInt (BeatAnalysisManager.samplePerSecond *bpmframe/BeatAnalysisManager._audio.clip.frequency);//( 60*bpmframe/BeatAnalysisManager._audio.clip.frequency) ;
				//Debug.Log ("samplePerSecond =" + BeatAnalysisManager.samplePerSecond+" freq="+BeatAnalysisManager._audio.clip.frequency+"rec=="+recbuffersize);
			}
			//bandlengthtxt = bpmframe.ToString();
		}
			//bpmtxt.text = "bpm="+bpmframe;
	
	}
	public  void EditorSave(string filepath) {  
		savedBeatMap  sbm=new savedBeatMap();
		sbm.MD=new MusicData[BeatAnalysisManager.BAL.Count ] ;


		for (int i = 0; i < BeatAnalysisManager.BAL.Count; i++) {
			//MusicData md =BeatAnalysisManager.BAL [i];
			sbm.MD [i] = BeatAnalysisManager.BAL [i];
		}

		string jsonstr = JsonUtility.ToJson (sbm );
		Debug.Log("ttt="+jsonstr);
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonstr);  
		File.WriteAllBytes (filepath, bts);
	 
	}  
	//
}
