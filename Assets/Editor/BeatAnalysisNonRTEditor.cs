using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(BeatAnalysisNonRT ))]
public class BeatAnalysisNonRTEditor : Editor { // extend the Editor class
//	public  static	List<MusicData > BALe=new List<MusicData>();
	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		BeatAnalysisNonRT  BAnonRT = (BeatAnalysisNonRT )target;


//		if (GUILayout.Button ("test")) {
//			Debug.Log (BALe.Count +"/b="+BeatAnalysisManager.BAL .Count);
//		}
		if (BeatAnalysisManager._audio == null) {
			GUILayout.Label ("*need enable the beatanalysismanager \n editor tool first*",GUILayout.Width(200)  );
		}else{
			//Debug.Log ("---" + BAnonRT.processbar);
			GUILayout.Space (30);
			// add a custom button to the Inspector component
			GUILayout.Label ("the data process will take\n plenty of time,please wait" ,GUILayout.Width(200) );
			if (GUILayout.Button ("start data process")) {
				Debug.Log ("---" + BeatAnalysisManager._audio);
				BAnonRT.ParaInit ();
				BAnonRT.Btnseparatedata ();
			
			
			}
//			if (BAnonRT.processbar < 1 && BAnonRT.processbar>0.1f) {
//
//				EditorUtility.DisplayProgressBar ("process", "", BAnonRT.processbar);
//			} else {
//				EditorUtility.ClearProgressBar ();
//			}
			// add a custom button to the Inspector component
			if (BeatAnalysisManager.MAL.Count > 1) {
				if (GUILayout.Button ("start beat analysis")) {
					BAnonRT.StartAnaBeat ();
//					BALe.Clear();
//					BALe.AddRange (BeatAnalysisManager.BAL );
				}
			}
//			if (BeatAnalysisManager.BAL .Count <BALe.Count ) {
//				if (GUILayout.Button ("copyto")) {
//					BeatAnalysisManager.BAL.Clear ();//.CopyTo (MALe);
//					BeatAnalysisManager.BAL.AddRange(BALe);
//				}
//			}
		}
	


		// //////////////////////////
//		if(GUILayout.Button("print"))
//		{
//			// if button pressed, then call function in script
//			Debug.Log ("oooo");
//			if (EditorUtility.DisplayDialog("????",   "aaaaa?",   "Yes00", "No")) {
//			
//			}
//		}

	}
}
