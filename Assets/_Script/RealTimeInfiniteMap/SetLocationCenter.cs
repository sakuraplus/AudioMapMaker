using UnityEngine;  


public class SetLocationCenter : MonoBehaviour {
	


	[Header("Center Mode: lat and lng of center")]
	[SerializeField, Range (-90,90)]
	float CenterLat;
	[SerializeField,Range (-180,180)]//起点纬度，北极90，南极-90
	float CenterLng;			//起点经度，英国东方为正，西方为负
	[SerializeField,HeaderAttribute ("Radius of each piece (meter)")] float distanceEarthLat=300000;



	void Start(){}
	void Awake(){
		TerrainManagerStatics.CenterLat = CenterLat;
		TerrainManagerStatics.CenterLng = CenterLng;

		TerrainManagerStatics.distanceEarthLat = distanceEarthLat;

	}




}


