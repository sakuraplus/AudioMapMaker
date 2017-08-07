using UnityEngine;
//using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(SeedBeatMap  ))]
public class SeedmapEditor : Editor
{ // extend the Editor class
	
	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();
		SeedBeatMap SBM = (SeedBeatMap )target;
	

				// add a custom button to the Inspector component
				GUILayout.Label ("test the beatmap in waterfall mode" ,GUILayout.Width(200) );


		if (GUILayout.Button ("show")) {
			MusicData mdt = new MusicData ();
			SBM.nextSeedPosS(mdt,0);
					// if button pressed, then call function in script
		}

				// add a custom button to the Inspector component
				if (SBM.readytoplay ) {
					if (GUILayout.Button ("play beatmap waterfall")) {
						SBM.btnPlaymap ();
					}
				}
	
	}

}
