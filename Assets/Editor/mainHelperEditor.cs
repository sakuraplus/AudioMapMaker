using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(camMap   ))]
public class mainHelperEditor : Editor { // extend the Editor class
	
	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		if (GUILayout.Button ("test?")) {
			float distanceEarthLat = 10000000;
			float earthR = 6371000;
			float centerLat = 160;

			float stepLat = distanceEarthLat * 90 / (Mathf.PI * earthR);//**
			float startLat = centerLat - stepLat;//**
			startLat=(startLat>180)?(startLat-360):startLat;
			float endLat = centerLat + stepLat;//**
			endLat=(endLat>180)?(endLat-360):endLat;
			Debug.Log (stepLat+"   startLat"+startLat+"/ endLat"+endLat);

		}



	}
}
