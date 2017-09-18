using UnityEngine;  


public class SetTerrains : MonoBehaviour {
	

	[HeaderAttribute ("run with exist gameobjects")]
	[SerializeField,Tooltip("run with exist gameobject or generate new ones")]
	bool runWithExists=false;
	[SerializeField]
	[Tooltip ("exist gameobject (parent).\nif the gameobject can not convert to needed data, it will generate new ones ")]
	GameObject ContainerOfPieces;





	/// <summary>
	/// 地图分块数
	/// </summary>
	[Space(10), SerializeField,HeaderAttribute ("number of segments and samples")]
	[Tooltip("separate the full area to pieces.\nin east-west and north-south")]
	Vector2Int Pieces=new Vector2Int(3,3);//地图分块数
	/// <summary>
	/// 每块地图分段数
	/// </summary>Header ("number of samples")
	[SerializeField]
	[Tooltip ("segment one block in east-west and north-south. higher segment,more details.")]
	Vector2Int Segments=new Vector2Int(5,5);//每块地图分段数
	/// <summary>
	/// 每块的尺寸，以纬度方向为基础
	/// </summary>
	[SerializeField,  Header( "size of the each piece of mesh")]
	[Tooltip("size of the each piece of mesh ,in north-south")]
	float SizeOfPiece=100;




	[SerializeField]
	[Tooltip("the addition of real height data, 1 means the real scale")]
	[Range (0.01f,1000f)]
	/// <summary>
	///  高度放大倍数，以实际高度为基础
	/// </summary>
	float heightScale=1f;


	void Start(){

		bool hasSLC = false;
		bool hasSLR = false;
		if (GetComponent<SetLocationRect> ()) {
			if (GetComponent<SetLocationRect> ().isActiveAndEnabled) {
				hasSLR = true;
			} else {
				hasSLR = false;
			}
		}
		if (GetComponent<SetLocationCenter> ()) {
			if (GetComponent<SetLocationCenter> ().isActiveAndEnabled) {
				hasSLC = true;
			} else {
				hasSLC = false;
			}
		}
		if (hasSLR) {
			TerrainManagerStatics.iscentermode = false;
		} else if (hasSLC) {
			TerrainManagerStatics.iscentermode = true;
		} else {
			Debug.LogError ("location not set");
			return;
		}


		if (!GetComponent<SetTextures  > ()) {
			TerrainManagerStatics.LoadSTM = false;
			Debug.LogError ("Textures not set");
		} else if (GetComponent<SetTextures  > ().isActiveAndEnabled) {
			TerrainManagerStatics.LoadSTM = true;
		} else {
			TerrainManagerStatics.LoadSTM = false;
			Debug.LogWarning ("Textures not set,please enable the script if you want to load texture from map service.");
		}
		if (!GetComponent<SetElevation  > ()) {
			TerrainManagerStatics.DataSource = datasource.random;
			Debug.LogError ("SetElevation not set");
		} else if (!GetComponent<SetElevation  > ().isActiveAndEnabled) {
			TerrainManagerStatics.DataSource = datasource.random;
			Debug.LogError ("Elevation not set,will build a random terrain. please enable the script " +
				"if you want to load elevation from map service.(for unity version higher then 5.3)");
		}
		string strsavepref = "**SetTerrains**TM+";
		strsavepref += "\n iscenter=" + TerrainManagerStatics.iscentermode  ;
		strsavepref += "\n key= s=  " + TerrainManagerStatics.STMAPIkey +"  // e="+TerrainManagerStatics.ELEAPIkey   ;
		strsavepref += "\n CenterLat/lng=" + TerrainManagerStatics.CenterLat +","+TerrainManagerStatics.CenterLng +"/"+TerrainManagerStatics.distanceEarthLat ;
		strsavepref += "\n lat/lng,end=" + TerrainManagerStatics.Lat +","+TerrainManagerStatics.Lng+" / "+TerrainManagerStatics.EndLat +","+TerrainManagerStatics.EndLng ;
		strsavepref += "\n step=" + TerrainManagerStatics.stepLat +","+TerrainManagerStatics.stepLng ;
		strsavepref += "\n Pieces=" + TerrainManagerStatics.Pieces .x+","+TerrainManagerStatics.Pieces.y  ;
		strsavepref += "\n SegmentInPiece=" + TerrainManagerStatics.SegmentInPiece.x+","+TerrainManagerStatics.SegmentInPiece.y ;
		strsavepref += "\n SizeOfPiece=" + TerrainManagerStatics.SizeOfPiece +"  heightScale=" + TerrainManagerStatics.heightScale ;
		strsavepref += "\n sizeofmesh=" + TerrainManagerStatics.MeshSize ;
		strsavepref += "\n MapType=" + TerrainManagerStatics.MapType  ;
		strsavepref += "\n loadimg=" + TerrainManagerStatics.LoadSTM +"  layer="+TerrainManagerStatics.layerOfGround.value    ;
		strsavepref += "\n DataSource=" + TerrainManagerStatics.DataSource+"/"+TerrainManagerStatics.DataSourceSTM  ;
		strsavepref += "\n ADV  normal=" + TerrainManagerStatics.useNormal +"/mat="+TerrainManagerStatics.matTrr   ;

		Debug.Log(strsavepref);

		if (runWithExists && ContainerOfPieces!=null ) {
			Debug.Log ("run with exist gameobjects");
			TerrainManagerStatics.InitWithExists (ContainerOfPieces);
		} else {
			Debug.Log ("will generate new gameobjects.");
			TerrainManagerStatics.Pieces = Pieces;

			TerrainManagerStatics.makeTrr ();
		}



	}
	void Awake(){
		TerrainManagerStatics.SizeOfPiece = SizeOfPiece;
		TerrainManagerStatics.heightScale = heightScale;
		TerrainManagerStatics.SegmentInPiece = Segments;


		string strsavepref = "*Setterrain***TM+";
		strsavepref += "\n iscenter=" + TerrainManagerStatics.iscentermode  ;
		strsavepref += "\n key= s=  " + TerrainManagerStatics.STMAPIkey +"  // e="+TerrainManagerStatics.ELEAPIkey   ;
		strsavepref += "\n CenterLat/lng=" + TerrainManagerStatics.CenterLat +","+TerrainManagerStatics.CenterLng +"/"+TerrainManagerStatics.distanceEarthLat ;
		strsavepref += "\n lat/lng,end=" + TerrainManagerStatics.Lat +","+TerrainManagerStatics.Lng+" / "+TerrainManagerStatics.EndLat +","+TerrainManagerStatics.EndLng ;
		strsavepref += "\n step=" + TerrainManagerStatics.stepLat +","+TerrainManagerStatics.stepLng ;
		strsavepref += "\n Pieces=" + TerrainManagerStatics.Pieces .x+","+TerrainManagerStatics.Pieces.y  ;
		strsavepref += "\n SegmentInPiece=" + TerrainManagerStatics.SegmentInPiece.x+","+TerrainManagerStatics.SegmentInPiece.y ;
		strsavepref += "\n SizeOfPiece=" + TerrainManagerStatics.SizeOfPiece +"  heightScale=" + TerrainManagerStatics.heightScale ;
		strsavepref += "\n sizeofmesh=" + TerrainManagerStatics.MeshSize ;
		strsavepref += "\n MapType=" + TerrainManagerStatics.MapType  ;
		strsavepref += "\n loadimg=" + TerrainManagerStatics.LoadSTM +"  layer="+TerrainManagerStatics.layerOfGround.value    ;
		strsavepref += "\n DataSource=" + TerrainManagerStatics.DataSource+"/"+TerrainManagerStatics.DataSourceSTM  ;
		strsavepref += "\n LoadSTM=" + TerrainManagerStatics.LoadSTM;
		strsavepref += "\n style n=" + TerrainManagerStatics.useNormal +",p="+TerrainManagerStatics.lowPolyStyle  ;
		Debug.Log (strsavepref);
	
	}


}


