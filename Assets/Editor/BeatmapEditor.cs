using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(ShowBeatMap  ))]
public class BeatmapEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();
		ShowBeatMap SBM = (ShowBeatMap )target;
	
		if (BeatAnalysisManager.BAL  != null) {
			// add a custom button to the Inspector component
			GUILayout.Label ("test the beatmap in waterfall mode" ,GUILayout.Width(200) );
			if (GUILayout.Button ("show beatmap")) {
				SBM.DrawBeatMap ();
				// if button pressed, then call function in script
			}

			// add a custom button to the Inspector component
			//if (SBM.) {
			if (GUILayout.Button ("play beatmap waterfall")) {
				SBM.btnPlaymap ();
			}
			//}
		}
	}
}
