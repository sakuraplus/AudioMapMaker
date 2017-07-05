using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(ExampleHelp  ))]
public class ExampleHelperEditor : Editor { // extend the Editor class
	int indjson=0;
	//int indmus=0;
	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();
		ExampleHelp EHE = (ExampleHelp )target;
	
		if (BeatAnalysisManager.BAL  != null) {
			// add a custom button to the Inspector component
			GUILayout.Label ("test the beatmap in waterfall mode" ,GUILayout.Width(200) );
			if (GUILayout.Button ("load next beatmap")) {
				
				EHE.btnloadjson(indjson );
				if(indjson<EHE.jsonfileAsset.Length-1 ){
					indjson++;
				}else{
					indjson=0;
				}
				// if button pressed, then call function in script
			}

			// add a custom button to the Inspector component
//			if (SBM.readytoplay ) {
//				if (GUILayout.Button ("play beatmap waterfall")) {
//					SBM.btnPlaymap ();
//				}
//			}
		//	[SerializeField ]
//			GUILayout. TextAsset ta;
//			if (GUILayout.Button ("load beatmap")) {
//				//SBM.LoadJsonMap  (ta);
//			}

		}
	}
}
