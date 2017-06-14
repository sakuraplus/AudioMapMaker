using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(BeatAnalysisManager ))]
public class BeatAnalysisManagerEditor : Editor { // extend the Editor class
	string bandlengthtxt;
	//public  AudioSource _audio;//=BeatAnalysisManager._audio ;

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		BeatAnalysisManager  BAM = (BeatAnalysisManager )target;

		// add a custom button to the Inspector component
		if(GUILayout.Button("Play"))
		{
			// if button pressed, then call function in scrip
			BeatAnalysisManager._audio.Play();
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("stop"))
		{
			// if button pressed, then call function in script
			BeatAnalysisManager._audio.Stop ();
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("tap to set bpm"))
		{
			setBPM ();
		}
		if(GUILayout.Button("start-"))
		{
			BAM.Awake ();//.initPara ();
		}

		GUILayout.Label (bpmframe +" frames between beats ,\n recommend buffer size ="+recbuffersize );
		// //////////////////////////
		if(GUILayout.Button("print"))
		{
			// if button pressed, then call function in script
			Debug.Log ("oooo");
		if (EditorUtility.DisplayDialog("????",   "aaaaa?",   "Yes00", "No")) {
		
		}
		}

	}





	//end分频段检测是否为节拍
		int lastSetbpmframe=-1;
		int Setbpmframe=-1;
		int bpmframe=0;
		int recbuffersize=0;
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
				recbuffersize=Mathf.RoundToInt ( 60*bpmframe/BeatAnalysisManager._audio.clip.frequency) ;
				Debug.Log ("bpmframe *2=" + (bpmframe * 2));
			}
			//bandlengthtxt = bpmframe.ToString();
		}
			//bpmtxt.text = "bpm="+bpmframe;
	
	}

}
