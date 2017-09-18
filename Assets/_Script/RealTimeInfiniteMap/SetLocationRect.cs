using UnityEngine;  


public class SetLocationRect : MonoBehaviour {
	

	//"Rect Mode:lat and lng of northwest and southeast"

	[Space (10),HeaderAttribute("lat and lng of northwest and southeast")]
	[Header ("Rect Mode:")]

	[SerializeField, Range (-90,90)]
	float Lat;
	[SerializeField,Range (-180,180)]
	float Lng;


	[SerializeField,Range (-90,90)]
	float EndLat = 20;			//终点纬度
	[SerializeField,Range (-180,180)]
	float EndLng = 90;			//终点经度






	//
	//	// Use this for initialization
	void Start () {}
	void Awake(){
		Trimlatlng( );
		TerrainManagerStatics.Lat = Lat;
		TerrainManagerStatics.Lng = Lng;
		TerrainManagerStatics.EndLat = EndLat;
		TerrainManagerStatics.EndLng = EndLng;

		}


	 void Trimlatlng()
	{
		Vector2 vecnorthwest;
		Vector2 vecsoutheast;
		vecnorthwest.y = Mathf.Max (Lat , EndLat );
		vecsoutheast.y = Mathf.Min (Lat, EndLat);

		if(Mathf.Abs (EndLng  - Lng ) < 180){
			// 内角不跨+-180度则经度小的为西侧
			vecnorthwest.x = Mathf.Min (Lng, EndLng);
			vecsoutheast.x = Mathf.Max (Lng, EndLng);
		} else if(Mathf.Abs (EndLng - Lng) ==180){
			if (Lat > EndLat) {
				vecnorthwest.x = Lng;
				vecsoutheast.x = EndLng;
			} else {
				vecnorthwest.x = EndLng;
				vecsoutheast.x = Lng;
			}
		}else {
			//内角跨+-180度，经度为负的为东侧
			vecnorthwest.x = Mathf.Max  (Lng, EndLng);
			vecsoutheast.x = Mathf.Min  (Lng, EndLng);
		}
		Lat = vecnorthwest.y;
		Lng = vecnorthwest.x;
		EndLat = vecsoutheast.y;
		EndLng = vecsoutheast.x;
		Debug.Log ("trim" + Lat + "," + Lng + "|" + EndLat + "," + EndLng);
	}

}


