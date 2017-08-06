using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(TestTest))]
public class mainHelperEditor : Editor { // extend the Editor class

	float[,][] fff=new float[2,3][] ;


	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		if (GUILayout.Button ("test?")) {
//			Debug.Log (Mathf.Log (3));
//			float northwestlat = 45;
//			float sinnorthlat=Mathf.Sin(northwestlat *Mathf.PI /180);
//			sinnorthlat = Mathf.Min (Mathf.Max (sinnorthlat, -0.99f), 0.99f);
//			float pointnorthlat=(0.5f - Mathf.Log ((1 + sinnorthlat) / (1 - sinnorthlat)) / (4 * Mathf.PI));
			/// 
			// south，南端纬度所在完整地图上的位置（比例）
			float southeastlat=30;
			float sinsouthlat=Mathf.Sin(southeastlat  *Mathf.PI /180);
			sinsouthlat = Mathf.Min (Mathf.Max (sinsouthlat, -0.99f), 0.99f);
			float pointsouthlat=(0.5f - Mathf.Log ((1 + sinsouthlat) / (1 - sinsouthlat)) / (4 * Mathf.PI));
			Debug.Log ("pointsouthlat="+pointsouthlat+">>"+pointsouthlat*512);

			Debug.Log (Mathf.Epsilon+System.Math.E );
			Debug.Log(Mathf.Pow(3,2));
			float yy=(0.5f-pointsouthlat)*4*Mathf.PI ;
			float pp = Mathf.Pow ((float)System.Math.E , yy);
			Debug.Log(yy+"pow="+Mathf.Pow(2.71828f,pp));
			Debug.Log((pp-1)/(pp+1)+">>srcs="+Mathf.Asin((pp-1)/(pp+1))+"deg="+Mathf.Asin((pp-1)/(pp+1))*Mathf.Rad2Deg );

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
		if (GUILayout.Button ("test latlng")) {
			Debug.LogWarning ("test latlng\t"+TerrainManager.lat +","+TerrainManager.lng );
		}
		if (GUILayout.Button ("test aus")) {
			Debug.LogWarning ("test aus\t"+BeatAnalysisManager._audio.clip .name );
		}
		if (GUILayout.Button ("test BAL")) {
			Debug.LogWarning ("test bal\t"+BeatAnalysisManager.BAL.Count  );
			Debug.LogWarning ("test mal\t"+BeatAnalysisManager.MAL .Count  );
		}
	}
}
