using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(camMap   ))]
public class mainHelperEditor : Editor { // extend the Editor class

	float[,][] fff=new float[2,3][] ;


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


		if (GUILayout.Button ("test2")) {
			float[] a = new float[3]{ 1f, 2f, 3f };
			float[] b = new float[3]{ 4,5,6 };
			float[] c = new float[3]{ 7,8,9 };
			float[] q = new float[3]{ 11, 12, 13 };
			float[] w = new float[3]{ 21,22, 23 };
			float[] e = new float[3]{ 31, 32, 33 };
			fff [0, 1] = a;
			fff [0, 1] = b;
			fff [0, 2] = c;
			fff [1, 0] = q;
			fff [1, 1] = w;
			fff [1, 2] = e;
			Debug.Log (fff.Length + ">>1,1,2=" + fff [1, 1] [2]+ ">>1,2,1=" + fff [1, 2] [1]);
			fff [1, 1] = null;
			Debug.Log (fff.Length + ">>1,1,2=" + fff [1, 1]+ ">>1,2,1=" + fff [1, 2]);
		}

	}
}
