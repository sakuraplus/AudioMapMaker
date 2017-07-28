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
			Debug.Log (">>"+(0.25f%1)+","+(0.75f%1)+","+(1f%1)+","+(1.25f%1)+","+(1.75f%1));
			Debug.Log (">>"+Mathf.Repeat (0.25f,1 )+","+Mathf.Repeat (0.75f,1 )+","+Mathf.Repeat (1.25f,1 )+","+Mathf.Repeat (1.75f,1 ));

		}


		if (GUILayout.Button ("test2")) {
			Vector2Int vv = new Vector2Int (2,3);
			Debug.Log (vv + "..." + vv.x + "--" + vv.y);
			string uvtest = "uv= ";

			int sx = 6;
			int sy = 6;
			int sum = (sx+1)*(sy+1);
			Vector2[]  uvs = new Vector2[sum];
			float u = 0.25f;//2*1.0F / sx;
			float v = 0.25f;//2*1.0F / sy;
			uint index = 0;
			for (int i = 0; i < sy + 1; i++)
			{
				for (int j = 0; j < sx + 1; j++)
				{
					//uvs[index] = new Vector2(j * u, i * v);
					float modU=j*u;
					modU = (Mathf.FloorToInt (modU) % 2 == 0) ? modU % 1 : (1 + Mathf.FloorToInt (modU) - modU) % 1;
					float modV=i*v;
					modV = (Mathf.FloorToInt (modV) % 2 == 0) ? modV % 1 : (1 + Mathf.FloorToInt (modV) - modV) % 1;

					uvs[index] = new Vector2(modU, modV);
					uvtest+=index+","+uvs[index]+" ";
					index++;
				}
			}
			Debug.Log (uvtest);


		}

	}
}
