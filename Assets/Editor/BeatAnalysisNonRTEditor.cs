using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(BeatAnalysisNonRT ))]
public class BeatAnalysisNonRTEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		BeatAnalysisNonRT  BAnonRT = (BeatAnalysisNonRT )target;

		// add a custom button to the Inspector component
		if(GUILayout.Button("start data process"))
		{
			Debug.Log ("---"+BeatAnalysisManager._audio);
			BAnonRT.Btnseparatedata ();
			// if button pressed, then call function in script
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("start beat analysis"))
		{
			BAnonRT.StartAnaBeat ();
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("init"))
		{
			BAnonRT.ParaInit ();
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
