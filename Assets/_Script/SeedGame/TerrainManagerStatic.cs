/// *****************Sa RTM************
using UnityEngine;  
using System.Collections.Generic ;  

using System;
using System.Text;
using System.IO;

public class TerrainManagerStatics  {


	/// <summary>
	/// 地形块父物体
	/// </summary>
	public static  GameObject terrContainer;
	/// <summary>
	/// 引用,每块
	/// </summary>
	public static  GameObject[,] MapObjs= new GameObject[0,0];
	/// <summary>
	/// 引用.每块上的drawJterrain
	/// </summary>
	public static  drawJterrain[,] ArrDTM= new drawJterrain[0,0];

	//"latitude and longitude of the northwest"
	// Range (-90,90)
	public  static float Lat;
	//Range (-180,180)]
	public  static float Lng;
	public  static  float CenterLat= 999;			//起点纬度，北极90，南极-90
	public  static  float CenterLng= 999;			//起点经度，英国东方为正，西方为负
	//"latitude and longitude of the southeast"
	//Range (-90,90)
	public  static float EndLat = 20;			//终点纬度
	//Range (-180,180)
	public  static float EndLng = 90;			//终点经度

	/// <summary>
	/// 以赤道为基础每块跨越的lng范围/2，每块加载时使用
	/// </summary>
	public static  float stepLng;
	/// <summary>
	/// 以赤道为基础每块跨越的lat范围/2，每块加载时使用
	/// </summary>
	public static  float stepLat;

	/// <summary>
	/// 使用中心点模式/角点模式
	/// </summary>
	public static  bool iscentermode=false;


	/// <summary>
	/// source of ele data
	/// </summary>
	public static datasource DataSource;
	public static datasource DataSourceSTM;
	/// <summary>
	/// 地形预设材质
	/// </summary>
	//"Default material of each block"
	public static   Material matTrr;	
	/// <summary>
	/// 适用于lowpoly风格
	/// </summary>
	public static   Color  colorOfMesh;
	/// <summary>
	/// 生成使用法线，适用于lowpoly风格
	/// </summary>
	public static bool useNormal;
	public static bool lowPolyStyle;
	/// <summary>
	/// staticmap key
	/// </summary>
	public static string STMAPIkey ="";
	/// <summary>
	/// 高度key
	/// </summary>
	public static string ELEAPIkey;
	/// <summary>
	/// 地图类型，卫星，道路。。。
	/// </summary>
	public static string MapType;

	/// <summary>
	/// 获取staticmap的固定zoom倍数
	/// </summary>
	public static  int ImagineZoom;
	/// <summary>
	/// 是否使用计算的最大zoom，不使用则获取固定zoom
	/// </summary>
	public static  bool IsAutoZoom;
	/// <summary>
	/// 贴图放大倍数，google使用，免费用户可以使用2倍，付费用户4倍
	/// </summary>
	public static  int ImagineScale;
	/// <summary>
	/// 贴图最大尺寸，google免费用户支持640*640，bing免费支持2000*1500
	/// </summary>
	public static  int ImagineSize;


	/// <summary>
	/// 地图分块数("separate the full area to pieces")
	/// </summary>
	public static  Vector2Int Pieces=new Vector2Int(3,3);//地图分块数
	/// <summary>
	/// 每块地图分段数("segment one mesh block in  lng,lat")
	/// </summary>
	public static  Vector2Int SegmentInPiece=new Vector2Int(5,5);//每块地图分段数
	/// <summary>
	/// 每块的尺寸，以纬度方向为基础( "size of the each piece of mesh in lat")
	/// </summary>
	public static   float SizeOfPiece=100;
	/// <summary>
	/// 根据经纬度计算的最终每块的尺寸，x=经度，z=纬度，y=高度倍数
	/// </summary>
	public static Vector3 MeshSize = new Vector3 (100, 100,1);


	/// <summary>
	///  高度放大倍数，以实际高度为基础
	/// </summary>
	public static float heightScale=1f;


	/// <summary>
	/// The earth r.6371000;地球半径
	/// </summary>
	public const float earthR = 6371000;

	/// <summary>
	/// The distance earth lat.实际距离,中心点边界的距离，半径,1000000m
	/// </summary>
	public static float distanceEarthLat=300000;


	/// <summary> 
	/// 保存到本地，生成文件夹时使用 
	/// </summary> 
	public static string savefiledate; 
	 /// <summary> 
	 /// 完成加载的块的数量 
	 /// </summary> 
	 public static  int NumComplete; 
	 /// <summary> 
	 /// 加载出错的块的数量，retry 统计时使用 
	 /// </summary> 
	 public static  int NumError;

	/// <summary>
	/// 保存每块加载的高度数据表，用于同步数据，保证边缘能够拼接
	/// </summary>
	public static  Vector3 [,][] VerticesAll;
	/// <summary>
	/// 生成地图块时为其设定layer
	/// </summary>
	public static  LayerMask layerOfGround;

	/// <summary>
	/// 是否加载贴图
	/// </summary>
	public static bool LoadSTM;




	/// <summary>
	/// 初始化，
	/// </summary>
	public static void init(){
	
		NumComplete = 0;
		NumError = 0;

		terrContainer = new GameObject();
		terrContainer.name = "TRRMAG";
		//new array

		MapObjs = new GameObject[(int)Pieces.y, (int)Pieces.x]; 
		ArrDTM = new drawJterrain[(int)Pieces.y, (int)Pieces.x]; 
		VerticesAll=new Vector3[(int)Pieces.y,(int)Pieces.x][] ;

		savefiledate = DateTime.Now.ToString ("yyyy-MM-dd HH-mm");
		//中心/角点模式，计算所需的数据
		if(iscentermode ){
			calcMeshSizeCenter ();//以纬度方向size y计算经度方向距离x
		}else{
			calcMeshSizeRect ();//以纬度方向size y计算经度方向距离x
		}
	}
	public static   void makeTrr()
	{

		init ();
	
		//计算中心方块在piece中的位置，piece为奇数则块为中心块，piece偶数则块为中心点左下块
		int offsetx = Mathf.FloorToInt (Pieces .x / 2);
		int offsety = Mathf.FloorToInt (Pieces.y / 2);

		//生成地图块
		for (int i = 0; i < Pieces.y; i++) {
			for (int j = 0; j < Pieces.x; j++) {
				
				string nameT = "Trr" + i + j;
				GameObject g = new GameObject ();

				g.layer=layerOfGround ;
				g.name = nameT;
				g.AddComponent<drawJterrain> ().initTrr (nameT);//,SegmentInPiece,matTrr,colorOfMesh );

				//计算每块的中心点坐标
				float _clat = CenterLat+((offsety-i) * 2 ) * stepLat;
				float _clng = CenterLng+((j-offsetx )* 2 ) * stepLng;

				VerticesAll[i,j]=null;//初始化高度表

				drawJterrain d = g.GetComponent <drawJterrain> ();
				d.loadNewLoc(_clat ,_clng ,new Vector2Int (i,j));
				
				g.transform.parent=terrContainer.transform;
				g.transform .Translate(new Vector3((j-offsetx)*MeshSize.x, 0, (offsety -i)*MeshSize.z));

				MapObjs[i,j]=g;//保存索引
				ArrDTM [i, j] = d;
			}
		}
	}







	/// <summary>
	/// 中心点模式下，根据角点计算最终每块mesh的尺寸，计算step，需要distanceEarthLat，SizeOfPiece，heightScale
	/// </summary>
	/// <returns>The mesh size.</returns>
	public static void calcMeshSizeCenter()
	{	
		Vector3 size;
		size.z = SizeOfPiece;//以南北方向为基础，单位纬度之间的实际距离相等

		stepLat= distanceEarthLat * 90 / (Mathf.PI * earthR);//从地块中心到边缘的纬度差
		stepLng=stepLat;//中心点模式下，经纬度方向取相同的跨度值
		float ttt=Mathf.Deg2Rad*CenterLat;// 角度转弧度=Mathf.PI * ttt / 180;//
		ttt=  Mathf.Abs (Mathf.Cos(ttt ));//当前纬度下，1纬度与1经度之间的距离比
		ttt=Math.Max  (ttt,0.5f);
		size.x=SizeOfPiece*ttt;
		float _scale = size.z / distanceEarthLat;//单位实际距离对应的mesh大小
		size.y=_scale*heightScale ;
		MeshSize=size;//
		Debug.Log("center mode： steplat/lng= "+stepLat+","+stepLng +"  meshsize="+MeshSize  );			
	}


	/// <summary>
	/// 角点模式下，根据角点计算最终每块mesh的尺寸，计算step和中心点，需要SizeOfPiece，heightScale
	/// </summary>
	public static void calcMeshSizeRect()
	{
		Vector3 size;
		size.z = SizeOfPiece;
		float	steplatall=Mathf.Abs (EndLat-Lat); //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplngall;
		if (Math.Abs (EndLng  - Lng) >= 180) {
			steplngall = (360 + EndLng - Lng) ;
		} else {
			steplngall = (EndLng - Lng) ;
		}
	
		if (EndLng  < 0 && Lng  > 0) {
			//两点精度跨越+-180度线时
			CenterLng  = (360+EndLng + Lng) / 2;
		}else{
			CenterLng= (Lng + EndLng) / 2;
		}
		if (CenterLng > 180) {
			//计算的中心点超过180度时，转换为正常值
			CenterLng = CenterLng - 360;
		}
		CenterLat = (Lat + EndLat)/2;
	
		//每个分块纬度差
		stepLat=(Lat-EndLat)/(2*Pieces.y); //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		if (Math.Abs (EndLng - Lng) >= 180) {
			//如果=180则认为lat，lng为西北点
			stepLng = (360 + EndLng - Lng) /(2* Pieces.x);
		} else {
			stepLng = (EndLng - Lng) /(2* Pieces.x);
		}
	
		///如果分块是偶数，中心偏移step/2
		if (TerrainManagerStatics.Pieces.x % 2 == 0) {
			CenterLng += stepLng;
		}
		if (TerrainManagerStatics.Pieces.y % 2 == 0) {
			CenterLat -= stepLat;
		}
	
		float ttt=CenterLat;//区域的平均纬度
		ttt =Mathf.Deg2Rad*ttt;// 角度转弧度=Mathf.PI * ttt / 180;//
		ttt=  Mathf.Abs (Mathf.Cos(ttt ));//当前纬度下，1纬度与1经度之间的距离比
		size.x =SizeOfPiece * ttt * Mathf.Abs (steplngall / steplatall)*Pieces .y/Pieces.x;//根据当前纬度下跨越的纬度与跨越的经度距离的比例关系，求lng方向的mesh尺寸

		float distancelat = 2 * Mathf.PI * earthR * steplatall / 360;//计算纬度方向实际距离
		float _scale =Pieces.y* size.z / distancelat;//单位实际距离对应的mesh大小

		size.y=_scale*heightScale ;
		MeshSize=size;
		Debug.Log("Rect mode： steplat/lng= "+stepLat+","+stepLng +"  meshsize="+MeshSize  );
	}


	/// <summary>
	/// 转换颜色和EditorPrefs中的string
	/// </summary>
	/// <param name="cc">Cc.颜色，rgb为0到1</param>
	public static string colortostring(Color cc){
		string R, G, B;
		R = Mathf.FloorToInt (cc.r * 256).ToString();
		G = Mathf.FloorToInt (cc.g * 256).ToString();
		B = Mathf.FloorToInt (cc.b * 256).ToString();
		if (R.Length == 1) {
			R = "00" + R;
		} else if (R.Length == 2) {
		
			R = "0" + R;
		}
		if (G.Length == 1) {
			G = "00" + G;
		} else if (G.Length == 2) {
			G = "0" + G;
		}
		if (B.Length == 1) {
			B = "00" + B;
		} else if (B.Length == 2) {
			B = "0" + B;
		}
		return R + G + B;
	}

	/// <summary>
	///  转换颜色和EditorPrefs中的string
	/// </summary>
	/// <param name="ss">Ss.EditorPrefs中获取的</param>
	public static Color stringtocolor(string ss){
		//string R, G, B;
		Color cc=new Color();
		cc.r = float.Parse (ss.Substring (0, 3))/256;
		cc.g = float.Parse (ss.Substring (3, 3))/256;
		cc.b = float.Parse (ss.Substring (6, 3))/256;

		return cc;
	}


	/// <summary>
	/// 使用现有的块，存入array，以便进行合并，无限地图等操作
	/// </summary>
	/// <param name="GameObject">需要使用之前生成的parent gameobject，每个子物体需要带有drawJT，并且有正确的vpos</param>
	public static  void  InitWithExists(GameObject _sourceGO){
		if(_sourceGO==null){
			Debug.Log ("have not define the source gamobject");
			return;
		}
		GameObject[] go = new GameObject[_sourceGO.transform.childCount];
		int vposx = 0;
		int vposy = 0;
		for (int i = 0; i < go.Length; i++) {
			go[i]= _sourceGO.transform.GetChild(i).gameObject ;
			
			if (go [i].GetComponent<drawJterrain> ()) {
				vposx = Mathf.Max (go [i].GetComponent<drawJterrain> ().Vpos.x, vposx);
				vposy = Mathf.Max (go [i].GetComponent<drawJterrain> ().Vpos.y, vposy);
			} else {
				Debug.Log ("can not find necessary component ");
				return ;
			}			
		}
	
	
		MapObjs=new GameObject[vposx+1 ,vposy+1 ] ;
		ArrDTM = new drawJterrain[vposx + 1, vposy + 1]; 
		for(int x=0; x <= vposx ;x++){
			for (int y = 0; y <= vposy; y++) {

				int xx=	go[x*(vposy+1)+y].GetComponent<drawJterrain> ().Vpos.x;
				int yy=	go[x*(vposy+1)+y].GetComponent<drawJterrain> ().Vpos.y;

				if (MapObjs [xx, yy] == null) {
					MapObjs [xx, yy] =	go [x * (vposy+1) + y];
					ArrDTM [xx, yy] =	go [x * (vposy + 1) + y].GetComponent<drawJterrain> ();
					ArrDTM [xx, yy].initTrr ("");
				} else {
					Debug.Log ("the index in array has been used,check the vpos of gameobjects."+xx+","+yy+"~"+MapObjs [xx, yy].name);
					return ;
				}

			}
		}
	
		Pieces.x = vposy+1;
		Pieces.y = vposx+1;
		VerticesAll=new Vector3[(int)Pieces.y,(int)Pieces.x][] ;

		savefiledate = DateTime.Now.ToString ("yyyy-MM-dd HH-mm");


		//中心/角点模式，计算所需的数据
		if(iscentermode ){
			calcMeshSizeCenter ();//以纬度方向size y计算经度方向距离x
		}else{
			calcMeshSizeRect ();//以纬度方向size y计算经度方向距离x
		}

		Renderer r = MapObjs [0, 0].GetComponent<Renderer> ();
		MeshSize.x = r.bounds.extents.x * 2;
		MeshSize.y = r.bounds.extents.y * 2;
		for(int x=0; x <= vposx ;x++){
			for (int y = 0; y <= vposy; y++) {
				
				ArrDTM [x, y].initTrr ("");
			}
		}

	
		Debug.Log ("calc mapobjs ok!"+MapObjs.Length +">"+TerrainManagerStatics .MapObjs[0,0]);

	}
	/// <summary>
	/// Syncs the each piece edges before combine or convert to terrain
	/// </summary>
	public static void syncEachPiece()
	{		
		if (MapObjs.Length < 2) {
			return;
		}
		for (int x = 0; x <TerrainManagerStatics. Pieces.y ; x++) {
			for (int y = 0; y <TerrainManagerStatics. Pieces.x; y++) {
				TerrainManagerStatics.ArrDTM [x, y].syncMainVertices();
				TerrainManagerStatics.ArrDTM [x, y].syncMainEdge();
			}
		}
	}
	public static void Trimlatlng()
	{
		Vector2 vecnorthwest;
		Vector2 vecsoutheast;
		vecnorthwest.y = Mathf.Max (Lat , EndLat );
		vecsoutheast.y = Mathf.Min (Lat, EndLat);
		//if (Mathf.Sign (Lng) == Mathf.Sign (endlng)) 
		if(Math.Abs (EndLng  - Lng ) < 180){
			// 内角不跨+-180度则经度小的为西侧
			vecnorthwest.x = Mathf.Min (Lng, EndLng);
			vecsoutheast.x = Mathf.Max (Lng, EndLng);
		} else if(Math.Abs (EndLng - Lng) ==180){
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




