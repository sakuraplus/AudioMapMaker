using UnityEngine;  
//using UnityEditor;  
using System.Collections;  

using System;
using System.Text;
using System.IO;


public class main1 : MonoBehaviour {

	GameObject terrmanager;//= new GameObject();
	//	public  GameObject[] arrTrr;//= new GameObject[9];
	public  GameObject[,] MapObjs;//= new GameObject[9];
	[Header("latitude and longitude of the northwest")]
	[Range (-90,90)]
	public   float lat = 30;			//起点纬度，北极90，南极-90
	[Range (-180,180)]
	public   float lng = 70;			//起点经度，英国东方为正，应该西方为负

	[Header ("latitude and longitude of the southeast")]
	[Range (-90,90)]
	public   float endlat = 20;			//终点纬度
	[Range (-180,180)]
	public   float endlng = 90;			//终点经度



	[SerializeField,HeaderAttribute ("Default material of each block")]
	//[HideInInspector]
	public Material matTrr;	
	//地形预设材质

	[SerializeField,Header ("API KEY of google elevation service")]
	[Tooltip("Get a ELE KEY at developers.google.com/maps/documentation/elevation")]
	string  googleELEAPIKey="";

	[Space(20)]
	public static string ELEAPIkey;
	//AIzaSyAYKDcM7_1gBCXxXGvPk5VgFjWs4w4BTfs
	[SerializeField,HeaderAttribute ("separate the full area to pieces")]
	public Vector2 Pieces=new Vector2(3,3);//地图分块数

	[SerializeField,HeaderAttribute ("segment one mesh block in  lng,lat")]
	public Vector2 SegmentInPiece=new Vector2(5,5);//每块地图分段数

	[Header( "size of the esch piece of mesh in lat")]
	public float SizeOfPiece=100;

	//	[SerializeField,  HeaderAttribute ("size of the mesh")]
	[HideInInspector]
	public  Vector3 size = new Vector3 (100, 100,1);
	[SerializeField,Header("the addition of real height data")]
	[Tooltip("1 means the real scale")]
	[Range (0.01f,1000f)]
	float heightScale=1f;

	[HideInInspector ]
	public GameObject _newmeshobj;//=new GameObject ();

	const float earthR = 6371000;//地球半径
	[SerializeField]
	bool _havelicense=false;

	public static  string savefiledate;
	public static  int NumComplete;
	public static  int NumError;
	//public static float [,] Vertives;


	void Start () {
		NumComplete = 0;
		NumError = 0;
		main . Vertives=new Vector3[(int)(Pieces.x*SegmentInPiece.x+1),(int)(Pieces.y*SegmentInPiece.y+1)] ;
		//StartCoroutine (findLicense ());
		makeTrr ();
	}
	public void testV(){
		string st = "testV\n";
		for (int i = 0; i < (int)(Pieces.x * SegmentInPiece.x + 1); i++) {
			for (int j=0 ; j < (int)(Pieces.y * SegmentInPiece.y + 1); j++) {
				st += ",\t" + main.Vertives [i, j];
			}
			st+="\n";
		}
		Debug.Log (st);
	}

	//
	void makeTrr()
	{
		//		TSM = new TerrainSaveManager ();
		//		savefiledate=DateTime.Now.ToString("yyyy-MM-dd HH-mm");
		//		MTnumError = 0;
		//		MTnumComplete = 0;

		//	Debug.Log ("savefiledate="+savefiledate);
		ELEAPIkey = googleELEAPIKey;

		if (ELEAPIkey.Length < 1) {

			Debug.LogWarning ("you need ele key"+ELEAPIkey+googleELEAPIKey);

			return;
		}

		if ((lat == endlat) || (lng == endlng)) {
			Debug.LogWarning ("incorrect geographical coordinate");
			return;
		}
		if (lat > 85 || lat < -85 || endlat > 85 || endlat < -85) {
			Debug.LogWarning ("you may not get the right map texture above the +-85 latitude");
		}



		//if (GameObject.Find ("TRRMAG")) {
		if(terrmanager!=null){
			DestroyImmediate (terrmanager);
			//terrmanager.name = "TRRMAG"+DateTime.Now.ToString ("MMddHHmm");
		}
		terrmanager = new GameObject();
		terrmanager.name = "TRRMAG";
		//arrTrr = new GameObject[(int)Math.Floor( Pieces.x*Pieces.y)];
		MapObjs = new GameObject[(int)Pieces.x, (int)Pieces.y]; 


		Trimlatlng ();//处理输入的经纬度信息，保证为西北，东南两点

		//每个分块纬度差
		float	steplat=(endlat-lat)/Pieces.y; //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplng;
		if (Math.Abs (endlng - lng) >= 180) {
			//如果=180则认为lat，lng为西北点
			steplng = (360 + endlng - lng) / Pieces.x;
		} else {
			steplng = (endlng - lng) / Pieces.x;
		}

		size = calcMeshSize (SizeOfPiece);//以纬度方向size y计算经度方向距离x
		//zoomrange (steplat,steplng);

		////////////////////////////////
		//起点为左上块


		int offsetx =(int) Mathf.Floor (Pieces .x / 2);
		int offsety = (int)Mathf.Floor (Pieces.y / 2);

		for (int i = 0; i < Pieces.x; i++) {
			for (int j = 0; j < Pieces.y; j++) {
				string nameT = "Trr" + i + j;
				GameObject g = new GameObject ();
				g.name = nameT;
				g.AddComponent<drawJterrain>().initTrr( nameT,SegmentInPiece,matTrr);
				g.GetComponent <drawJterrain>().loadNewLoc(lat+j*steplat,lng+i*steplng ,
					lat+(j+1)*steplat,lng+(i+1)*steplng,size,new Vector2 (i,j));
				//DrawTMesh DTM=new DrawTMesh();
				int arrind = (int)Math.Floor(i * Pieces.y + j);

				g.transform.parent=terrmanager.transform;

				g.transform .Translate(new Vector3((i-offsetx)*size.x, 0, (offsety -j)*size.z));

				//arrTrr [arrind] = g ;
				MapObjs[i,j]=g;
			}
		}
		//_newmeshobj=new GameObject ();

	}












	public void Trimlatlng()
	{
		Vector2 vecnorthwest;
		Vector2 vecsoutheast;
		vecnorthwest.y = Mathf.Max (lat, endlat);
		vecsoutheast.y = Mathf.Min (lat, endlat);
		//if (Mathf.Sign (lng) == Mathf.Sign (endlng)) 
		if(Math.Abs (endlng - lng) < 180){
			// 内角不跨+-180度则经度小的为西侧
			vecnorthwest.x = Mathf.Min (lng, endlng);
			vecsoutheast.x = Mathf.Max (lng, endlng);
		} else if(Math.Abs (endlng - lng) ==180){
			if (lat > endlat) {
				vecnorthwest.x = lng;
				vecsoutheast.x = endlng;
			} else {
				vecnorthwest.x = endlng;
				vecsoutheast.x = lng;
			}
		}else {
			//内角跨+-180度，经度为负的为东侧
			vecnorthwest.x = Mathf.Max  (lng, endlng);
			vecsoutheast.x = Mathf.Min  (lng, endlng);
		}
		lat = vecnorthwest.y;
		lng = vecnorthwest.x;
		endlat = vecsoutheast.y;
		endlng = vecsoutheast.x;
		//	Debug.Log ("trim" + lat + "," + lng + "|" + endlat + "," + endlng);
	}


	public Vector3 calcMeshSize(float sizelat)
	{
		Vector3 size;
		size.z = sizelat;
		float	steplatall=Mathf.Abs (endlat-lat); 
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplngall;
		if (Math.Abs (endlng - lng) >= 180) {
			steplngall = (360 + endlng - lng) ;
		} else {
			steplngall = (endlng - lng) ;
		}

		float ttt=(endlat+lat)/2;//区域的平均纬度
		ttt =Mathf.Deg2Rad*ttt;// 角度转弧度=Mathf.PI * ttt / 180;//
		ttt=  Mathf.Abs (Mathf.Cos(ttt ));//当前纬度下，1纬度与1经度之间的距离比
		size.x =size.z * ttt * Mathf.Abs (steplngall / steplatall)*(Pieces.y/Pieces.x);//根据当前纬度下跨越的纬度与跨越的经度距离的比例关系，求lng方向的mesh尺寸

		float distancelat = 2 * Mathf.PI * earthR * steplatall / 360;//计算纬度方向实际距离
		float _scale =Pieces.y* size.z / distancelat;//单位实际距离对应的mesh大小

		size.y=_scale*heightScale ;
		return size;
		//Debug.Log("steplat=" + steplat + "  steplng=" + steplng+" size="+size );//steplat0.5729578steplng3.71444
	}





}
