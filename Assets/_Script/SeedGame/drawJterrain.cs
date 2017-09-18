using UnityEngine;  
#if UNITY_EDITOR
using UnityEditor;  
#endif 
using System.Collections.Generic ;  
using System.Collections;  
using System;
using System.Text;
using System.IO;
using System.Xml;



public class drawJterrain : MonoBehaviour {

	/// <summary>
	/// 用于同步各块的边界数据
	/// </summary>
	public bool edgeUp = false;//***
	public bool edgeDown = false;//***
	public bool edgeLeft = false;//***
	public bool edgeRight = false;//***
	string tempstr="";//打印测试数据用***
	//**********************

	/// <summary>
	/// 每次www的url
	/// </summary>
	string  ipaddress = ""; 
	/// <summary>
	/// 每次www返回的数据
	/// </summary>
	string StrWwwData;
	/// <summary>
	/// 高度www
	/// </summary>
	WWW www_data_ELE;
	/// <summary>
	/// 贴图www
	/// </summary>
	WWW www_data_STM;


	/// <summary>
	/// 高度key，需要自己注册
	/// </summary>
	string ELEKey = TerrainManagerStatics.ELEAPIkey;
	/// <summary>
	/// 贴图key，需要自己注册
	/// </summary>
	string STMKey = TerrainManagerStatics.STMAPIkey ;	


	/// <summary>
	/// 记录在同一经度下取各纬度数据的次数
	/// </summary>
	int indVerticesLng=0;
	//int indVerticesLat=0;


	//[SerializeField]
	//float steplat ;//每次获取高度数据的间隔
	[SerializeField ]
	float steplng ;//每次获取高度数据的间隔

	/// <summary>
	/// object名字
	/// </summary>
	public string Trrname;
	/// <summary>
	/// 索引，x=lat，y=lng
	/// </summary>
	public  Vector2Int Vpos;

	public float centerlat;// +-90西北
	public float centerlng;//+-180西北
	[SerializeField ]  float northwestlat;// +-90西北
	[SerializeField ]  float northwestlng;//+-180西北
	[SerializeField ]  float southeastlat;// +-90东南
	[SerializeField ]  float southeastlng;//+-180东南
	float  loadingLng;

	/// <summary>
	/// 每块分段数量
	/// </summary>
	[SerializeField ] Vector2Int segment=new Vector2Int(3,3);

	/// <summary>
	/// 材质
	/// </summary>
	Material diffuseMap;

	/// <summary>
	/// 颜色，当超过+-85时，mesh为基础色加深
	/// </summary>
	Color  meshcolor;
	/// <summary>
	/// 保存基础颜色，在init时赋值，当超过+-85时，mesh为基础色加深，在动态加载地图和lowpoly风格中使用
	/// </summary>
	Color basemeshcolor;
	/// <summary>
	/// 绘制mesh，顶点数据，从高度数据获取，转换换算为6点形式
	/// </summary>
	[HideInInspector]
	public Vector3[] _vertices;
	/// <summary>
	/// 6点形式的顶点数据，用于计算法线
	/// </summary>
	Vector3[] _vertices6;
	/// <summary>
	/// 绘制mesh，uv顶点数据.
	/// </summary>
	private Vector2[] _uvs;
	/// <summary>
	/// 转换为6点数据，用于计算法线及颜色The uvs6.
	/// </summary>
	private Vector2[] _uvs6;
	/// <summary>
	/// 绘制mesh，三角面数据。6点模式
	/// </summary>
	private int[] _triangles;
	/// <summary>
	/// 法线数据，6点模式
	/// </summary>
	Vector3[] _normals6;
	/// <summary>
	/// 颜色数据，6点模式
	/// </summary>
	//[SerializeField ]
	Color[] _colors6;
	/// <summary>
	/// 贴图，剪切掉水印部分
	/// </summary>
	Texture2D mapTexture;
	bool textureCompleted;
	/// <summary>
	/// 保存高度数据加载错误的点，索引，在全部加载完毕时使用插值补齐数据
	/// </summary>
	List<int> errorSamples = new List<int> ();
	/// <summary>
	/// 用于在retry中识别加载状态
	/// </summary>
	public loadState LoadState = loadState.none ;//0未开始，1成功，2失败







	#if UNITY_EDITOR

	//**************************************
	/// <summary>
	/// 控制在editor模式下，停止所有update
	/// </summary>
	bool stopall;
	/// <summary>
	/// 检查 贴图www是否有返回值
	/// </summary>
	[SerializeField ]
	bool checkSTM=true;
	/// <summary>
	/// 检查高度www是否有返回值
	/// </summary>
	[SerializeField ]
	bool checkELE=false;

	/// <summary>
	/// 停止update
	/// </summary>
	public void StopUpdate()
	{
		Debug.LogWarning("stopUpdate  "+Trrname +">"+updateCount);

		stopall = true;
		EditorApplication.update -= UpdateE;
	
	}
	/// <summary>
	/// 重试，根据加载状态选择重试的阶段
	/// </summary>
	public void Retry()
	{
		Debug.LogWarning ("retry "+Trrname +">"+LoadState);
		//0未开始，1成功，2图失败，3ele失败
		ELEKey =TerrainManagerStatics.ELEAPIkey;
		STMKey =TerrainManagerStatics.STMAPIkey;
		if (TerrainManagerStatics.LoadSTM && LoadState==loadState.imgError) {
			www_data_STM = null;
			checkSTM = true;
			Debug.Log("reload img");
			textureCompleted = false;
			loadTexture();

		} else if ( LoadState == loadState.EleError)
		{
			Debug.Log("reload ele, do not reload img");
			www_data_ELE = null;
			checkSTM = false;
			checkELE = true;
			loadingLng = northwestlng;
			indVerticesLng = 0;
			//indVerticesLat = 0;
		//	DrawTexture();
			loadTerrain ();
		}
		if (!Application.isPlaying) {
			updateCount = 0;
			stopall = false;
			EditorApplication.update -= UpdateE;
			EditorApplication.update += UpdateE;
		}
	}


	[SerializeField ]
	float  updateCount;//editor模式，记录update次数*******

	/// <summary>
	/// 在editor模式下，不能使用协程，需要通过update检查www的状态
	/// </summary>
	public   void UpdateE()
	{
		
		updateCount++;
		if (stopall) {
			Debug.LogWarning ("stop!!!!");
			EditorApplication.update -= UpdateE;
			return;
		}

		if (www_data_STM != null && checkSTM)  {
			if (www_data_STM.isDone) {
				
				Debug.Log ("STMdone" + www_data_STM.error);
				loadImgComplete ();
				checkSTM = false;
			} 
			if (!string.IsNullOrEmpty ( www_data_STM.error )) {
				LoadState = loadState.imgError;
				Debug.Log ("STMdone E"+www_data_STM.isDone +"/"+www_data_STM.progress+"/"+www_data_STM.bytesDownloaded+"\n"+ string.IsNullOrEmpty ( www_data_STM.error )+"/"+www_data_STM.error );
				EditorApplication.update -= UpdateE;
			}


		}

		if(indVerticesLng>segment.x   && checkELE )
		{
			checkELE=false;
			Debug.Log (Trrname+ ": ele data download complete.j");
			EditorApplication.update -= UpdateE;
			return;	
		}
		#if (UNITY_5_0||UNITY_5_1||UNITY_5_2 )

		if (www_data_ELE != null && checkELE) {
			if (www_data_ELE.isDone) {
				if (TerrainManagerStatics.DataSource == datasource.google) {					
					XMLGoogleLngComplete();
				} else if (TerrainManagerStatics.DataSource == datasource.bing) {					
					XMLBingLngComplete ();
				}
			} 
			if (!string.IsNullOrEmpty( www_data_ELE.error)) {				
				LoadState =loadState.EleError ;
				Debug.LogWarning  ("www_data_ELE.error-x="+Trrname  +www_data_ELE.error);
			}
		}
		#else

		if (www_data_ELE != null && checkELE) {
			if (www_data_ELE.isDone) {
				if (TerrainManagerStatics.DataSource == datasource.google) {
					JsonGoogleLngComplete ();
				} else if (TerrainManagerStatics.DataSource == datasource.bing) {
					JsonBingLngComplete ();
				}
			} 
			if (!string.IsNullOrEmpty( www_data_ELE.error)) {
				//EditorApplication.update -= UpdateE;
				LoadState =loadState.EleError ;
				Debug.LogWarning  ("www_data_ELE.error-j="+Trrname  +www_data_ELE.error);
			}


		}
		#endif //(UNITY_5_0||UNITY_5_1||UNITY_5_2 )


	}
	#endif //UNITY_EDITOR



	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="_Trrname">object名称.</param>
	/// <param name="_segment">高度数据的精度，分段数量.</param>
	/// <param name="_matTrr">默认材质.</param>
	/// <param name="_colTrr">基础颜色.</param>,Vector2Int _segment, Material _matTrr, Color _colTrr)
	public void initTrr( string _Trrname)
	{	
		if (Trrname == null) {
			Trrname = _Trrname;
		}
		segment = TerrainManagerStatics.SegmentInPiece;//_segment;
		diffuseMap = TerrainManagerStatics.matTrr;//_matTrr;
		meshcolor = TerrainManagerStatics.colorOfMesh ;// _colTrr;
		basemeshcolor =TerrainManagerStatics.colorOfMesh ;// _colTrr;
		ELEKey = TerrainManagerStatics.ELEAPIkey;
		STMKey = TerrainManagerStatics.STMAPIkey ;
	
		int leng = ((int)segment.x + 1) * ((int)segment.y + 1);
		_vertices = new Vector3[leng];//用于存每个点的坐标
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x ; j++) {
				int ind = i * (int)(segment.x + 1) + j;
				_vertices [ind].x =j * TerrainManagerStatics.MeshSize.x / segment.x;
				_vertices [ind].z = i * +TerrainManagerStatics.MeshSize.z / segment.y;
			}
		}

		GetUV();
	}


	/// <summary>
	/// Loads the new location.以新中心点坐标加载地理数据
	/// </summary>
	/// <param name="_centerlat">Centerlat.中心纬度</param>
	/// <param name="_centerlng">Centerlng.中心经度</param>
	/// <param name="_vpos">Vpos（x，y），纬度，经度方向的索引</param>
	public  void loadNewLoc(float _centerlat,float _centerlng, Vector2Int _vpos)
	{		
		#if UNITY_EDITOR 
		if (!Application.isPlaying) {
			EditorApplication.update += UpdateE;
			Debug.Log("start update E");
		}
		#endif 
		checkSTM = TerrainManagerStatics.LoadSTM;
		checkELE=(TerrainManagerStatics.DataSource==datasource.random )?false:true;
		textureCompleted = false;
		//outof85 = false;

		Vpos = _vpos;
		indVerticesLng=0;
		centerlat = _centerlat;
		centerlng =trimLng( _centerlng);
		northwestlat = _centerlat + TerrainManagerStatics.stepLat ;//_northwestlat;// +-90 西北角纬度
		northwestlng = _centerlng - TerrainManagerStatics.stepLng ;// _northwestlng;//+-180西北角经度

		loadingLng = northwestlng;
		southeastlat = _centerlat - TerrainManagerStatics.stepLat ;// _southeastlat;// +-90 东南角纬度
		southeastlng = _centerlng + TerrainManagerStatics.stepLng ;// _southeastlng;//+-180 东南角经度
		//steplat = TerrainManagerStatics.stepLat *2 / segment.y;//每段跨越的纬度
		steplng = TerrainManagerStatics.stepLng *2 / (segment.x);//每段跨越的经度
		northwestlng = trimLng (northwestlng);
		southeastlng = trimLng (southeastlng);
		//在纬度高于+-85时，无法获取贴图，高度信息可能不准确
		if (centerlat > 82 || centerlat < -82) {
			//outof85 = true;
			meshcolor = new Color (basemeshcolor.r / 2, basemeshcolor.g / 2, basemeshcolor.b / 2); 
		} else {
			meshcolor = basemeshcolor;
		}
			

		syncMainEdge ();//同步边界
		prevLoad  ();//随机地形
		if (gameObject.GetComponent <MeshFilter> () != null) {
			Debug.Log ("pre draw mesh!!!");
			DrawMesh ();
		} 
		if (TerrainManagerStatics.LoadSTM) {
			loadTexture ();
		} else {
			loadTerrain ();
		}

	}

	/// <summary>
	/// 根据不同数据源加载贴图
	/// </summary>
	void loadTexture(){
		switch (TerrainManagerStatics.DataSourceSTM) {
		case (datasource.google):
			StartCoroutine (loadimgGoogle ());
			break;
		case (datasource.bing):
			StartCoroutine (loadimgBing ());
			break;
		default:
			Debug.LogWarning ("incorrect datasouse！ confirm your datasource setting and retry");
			break;	
		}
	}

	/// <summary>
	/// 根据不同数据源加载高度数据
	/// </summary>
	void loadTerrain(){
//
//		#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
//		//The real world elevation terrain will not work correctly below unity 5.3
//		fakeloadjson ();//随机地形
//		sampleLerp ();//修正错误数据
//		syncMainEdge ();//同步边界
//		syncMainVertices ();//更新main数据
//		DrawMesh ();
//		DrawTexture ();
//		#else
		switch (TerrainManagerStatics.DataSource){
		case (datasource.google):
			StartCoroutine(LoadJsonGoogleLng(loadingLng ));//按精度取值，差值为南北方向
			break;
		case(datasource.bing ):
			StartCoroutine(LoadJsonBingLng(loadingLng));//按精度取值，差值为南北方向
			break;
		case(datasource.random):
			
			fakeloadjson ();//随机地形
			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertices ();//更新main数据
			DrawMesh ();
			DrawTexture ();

			break;
		
		default :
			Debug.Log ("loadNewLoc Error! " + TerrainManagerStatics.DataSource);
			break;
		}

		//#endif 
	}




	/// <summary>
	/// 保存数据错误的点的索引，当高度www返回错误时调用
	/// </summary>
	/// <param name="index">Index.</param>
	void  storeErrorSample(int index){
		if (edgeUp && index > (int)((segment.x + 1) * segment.y)) {
			return;
		}
		if (edgeDown && index <=segment.x) {
			return;
		}
		if ((edgeLeft &&index% (segment.x+1) ==0) ||(edgeRight  &&(index+1)% (segment.x+1) ==0)) {
			return;
		}
		errorSamples.Add (index);
	}

	/// <summary>
	/// 计算插值补充网络错误造成的异常数据
	/// </summary>
	void sampleLerp(){
		if (errorSamples.Count <= 0) {
			return;
		}
	
		List<float> samples=new List<float> ();
		for(int i=0;i<errorSamples.Count ;i++) {
			
			int sampleIndex =(int) errorSamples [i];
		
			int ix = Mathf.FloorToInt (sampleIndex / ((int)segment.x + 1));
			int iy = sampleIndex - ix * ((int)segment.x + 1);

			if (ix > 0 && !errorSamples.Contains ((ix - 1) * ((int)segment.x + 1) + iy)) {
				samples.Add (_vertices [(ix - 1) * ((int)segment.x + 1) + iy].y);
			}
			if (ix < segment.y && !errorSamples.Contains ((ix + 1) * ((int)segment.x + 1) + iy)) {
				samples.Add (_vertices [(ix + 1) * ((int)segment.x + 1) + iy].y);
			}
			if (iy > 0 && !errorSamples.Contains (ix * ((int)segment.x + 1) + iy - 1)) {
				samples.Add (_vertices [ix * ((int)segment.x + 1) + iy - 1].y);
			}
			if (iy < segment.x && !errorSamples.Contains (ix * ((int)segment.x + 1) + iy + 1)) {
				samples.Add (_vertices [ix * ((int)segment.x + 1) + iy + 1].y);
			}	

			if (samples.Count < 0) {
			} else {
				float sum = 0;
				for (int s = 0; s < samples.Count; s++) {
					sum += samples [s];
				}
				_vertices [sampleIndex].y = sum / samples.Count;
				samples.Clear ();
			}
			errorSamples [i] = -1;
		}

		errorSamples.Clear ();


	}



	/// <summary>
	/// 同步所有地图块数据到main，同步边界，避免数据源出错时地块衔接处开裂Syncs the main vertices.
	/// </summary>
	public void syncMainEdge()
	{
		edgeUp = false;
		edgeDown = false;
		edgeLeft = false;
		edgeRight = false;

		if (Application.isPlaying) {
			updateCount = Time.timeSinceLevelLoad;
		}
		string synct="";//测试***

		if (Vpos.x > 0 && TerrainManagerStatics.VerticesAll [(int)(Vpos.x - 1) ,(int) Vpos.y] != null) {
			//sync edge up
			edgeUp=true;

			int ib = (int)((segment.x+1) * segment.y );
			for (int x = 0; x <= segment.x; x++) {
				if (_vertices [x+ib].y != TerrainManagerStatics.VerticesAll [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y) {
					synct += ",U<" + Vpos + "," + x + ">";
				}
				_vertices [x+ib].y = TerrainManagerStatics.VerticesAll [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y;
			}	
		}
		if (Vpos.x <TerrainManagerStatics.Pieces.y -1 && TerrainManagerStatics.VerticesAll [(int)(Vpos.x + 1) ,(int) Vpos.y] != null) {
			//sync edge down
			edgeDown=true;
			int ib = (int)((segment.x+1) * segment.y);

			for (int x = 0; x <=segment.x; x++) {
				if (_vertices [x].y != TerrainManagerStatics.VerticesAll [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y) {
					synct += ",D<" + Vpos + "," + x + ">";
				}
				_vertices [x].y = TerrainManagerStatics.VerticesAll [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y;
			}
		}


		if (Vpos.y > 0 && TerrainManagerStatics.VerticesAll [(int)Vpos.x  ,(int) Vpos.y - 1] != null) {
			//sync edge left
			edgeLeft=true;
		
			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (_vertices [ib].y != TerrainManagerStatics.VerticesAll [(int)(Vpos.x ) ,(int) (Vpos.y-1)] [ib+(int)segment.x].y) {
					synct += ",L<" + Vpos + "," + y + ">";
				}
				_vertices [ib].y = TerrainManagerStatics.VerticesAll [(int)Vpos.x, (int)(Vpos.y-1)] [ib + (int)segment.x].y;
			}
		}

		if (Vpos.y <TerrainManagerStatics.Pieces.x-1 && TerrainManagerStatics.VerticesAll [(int)Vpos.x  ,(int) Vpos.y + 1] != null) {
			//sync edge right
			edgeRight =true;
	
			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (_vertices [(int)segment.x+ib].y != TerrainManagerStatics.VerticesAll [(int)(Vpos.x ) ,(int) (Vpos.y+1)] [ib].y) {
					synct += ",R<" + Vpos + "," + y + ">";
				}
				_vertices [(int)segment.x+ib].y = TerrainManagerStatics.VerticesAll [(int)(Vpos.x), (int)(Vpos.y+1)] [ib].y;
			}
		}
//		if (synct.Length > 2) {
//			Debug.Log (Trrname + "sync result:\n" + synct);
//		}
	}

	/// <summary>
	/// 更新main。vertices.
	/// </summary>
	public void syncMainVertices()
	{
		TerrainManagerStatics.VerticesAll [(int)Vpos.x,(int)Vpos.y] = _vertices;
	}
	 

	#region ~随机地形
	/// <summary>
	/// 预先加载，从高度列表里获取数据，在加载数据完成前，使新生成的地块与原有的相连
	/// </summary>
	void prevLoad(){
		
		int starti = edgeDown ? 1 : 0;
		int endi =edgeUp  ? (int)segment.y :(int) segment.y + 1;
		int startj = edgeLeft ? 1 : 0;
		int endj=edgeRight ? (int)segment.x: (int)segment.x + 1;


		for (int i = starti; i < endi ; i++) {
			for (int j = startj; j < endj  ; j++) {
				int ind = i * (int)(segment.x + 1) + j;

				_vertices [ind].y =0;
				_vertices [ind].x =j * TerrainManagerStatics.MeshSize.x / segment.x;
				_vertices [ind].z = i * +TerrainManagerStatics.MeshSize.z / segment.y;
			}
		}
	}
	/// <summary>
	/// 随机数地形，模拟加载高度数据
	/// </summary>
	void fakeloadjson(){
		LoadState = loadState.eleComplete;
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x ; j++) {
				int ind = i * (int)(segment.x + 1) + j;
				float a=UnityEngine. Random.Range(0f,20)-5;
				_vertices [ind].y =a; 
			}
		}
	}
	#endregion


	#region ~lng方向插值

	/// <summary>
	///加载高度数据，按照纬度方向分段多次加载
	///<summary>
	IEnumerator LoadJsonGoogleLng(float lng)
	{  
		if (indVerticesLng > segment.x) {
			Debug.Log (Trrname + "Data complete!!!!!!!" + tempstr);
			checkELE = false;
			sampleLerp ();//修正错误数据
			//syncMainEdge ();//同步边界
			syncMainVertices ();//更新main数据
			DrawMesh ();
			DrawTexture ();
			yield break;

		} else {
			LoadState = loadState.EleLoading;
			#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
			ipaddress = "https://maps.googleapis.com/maps/api/elevation/xml?path="; //获取json数据,改为XML获取xml数据
			#else
			ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
			#endif 
			
			ipaddress += southeastlat + "," + lng + "|";
			ipaddress += northwestlat + "," + lng;//获取同一纬度下，东西经度之间的数据
			ipaddress += "&samples=" + (segment.y + 1) + "&key=";
			ipaddress += ELEKey;
			//print (Trrname + "--" + ipaddress + "\n" + Time.deltaTime);
			www_data_ELE = new WWW (ipaddress);  
			yield return www_data_ELE;  //获得数据后继续

			if (Application.isPlaying && www_data_ELE.isDone ) {
				#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
					XMLGoogleLngComplete ();
				#else
					JsonGoogleLngComplete ();
				#endif 
				

			}
		}
			
	}//end LoadFile


	//加载高度数据，按照纬度方向分段多次加载bing
	IEnumerator LoadJsonBingLng(float lng)
	{  
		if (indVerticesLng > segment.x) {

			Debug.Log (Trrname + "Data complete!!!!!!!" + tempstr);
			checkELE = false;
			sampleLerp ();//修正错误数据
			//syncMainEdge ();//同步边界
			syncMainVertices ();//更新main数据
			DrawMesh ();
			DrawTexture ();
			yield break;
		} else {
			LoadState = loadState.EleLoading;
			#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
			ipaddress = "https://dev.virtualearth.net/REST/v1/Elevation/Polyline?output=xml&points="; //获取json数据,改为XML获取xml数据
			#else
			ipaddress = "https://dev.virtualearth.net/REST/v1/Elevation/Polyline?points="; //获取json数据,改为XML获取xml数据
			#endif 
			ipaddress += southeastlat + "," + lng + ",";
			ipaddress += northwestlat + "," + lng;//获取同一纬度下，东西经度之间的数据
			ipaddress += "&heights=ellipsoid&samples=" + (segment.y + 1) + "&key=";
			ipaddress += ELEKey;
			//print (Trrname + "--" + ipaddress);
			www_data_ELE = new WWW (ipaddress);  
			yield return www_data_ELE;  //获得数据后继续
			if (Application.isPlaying && www_data_ELE.isDone ) {
				#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
					XMLBingLngComplete ();
				#else
					JsonBingLngComplete (); 
				#endif 

			}
		}
			
	}


	void XMLGoogleLngComplete()
	{
		StrWwwData = www_data_ELE.text;   

		if (!string.IsNullOrEmpty( www_data_ELE.error))    
		{    
			Debug.LogWarning ("error ELE:"+Trrname +">"+ipaddress+"/*/"+indVerticesLng +"-" + www_data_ELE.error+"--"+www_data_ELE.isDone  );

			TerrainManagerStatics.NumError++;
			LoadState = loadState.EleError;

			for (int i=0; i <= segment.y ; i++)		
			{
				storeErrorSample (i*((int)segment.x+1)+indVerticesLng );
			}

			indVerticesLng =indVerticesLng+(int)segment.x+1;
			loadingLng += steplng;           
			StartCoroutine(LoadJsonGoogleLng(trimLng( loadingLng)));  
			StrWwwData = "";  
		}    
		else    
		{    
			try{  

				StrWwwData = www_data_ELE.text;    
				Debug.Log(Trrname+","+indVerticesLng +" "+ipaddress +"\n"+StrWwwData+"\n"+Time.deltaTime );
				int indss = StrWwwData.IndexOf ("<ElevationResponse>");
				int indse = StrWwwData.IndexOf ("</ElevationResponse>");
				string wwws2 = StrWwwData.Substring (indss, indse - indss+20);
				Debug.Log (">>>>"+indss+"."+indse+"~~"+wwws2);
				XmlDocument xmld=new XmlDocument();
				xmld.LoadXml(StrWwwData);

				int XmlCount = xmld.GetElementsByTagName("ElevationResponse")[0].ChildNodes.Count-1;  

				for (int i = 0; i < XmlCount; i++)  
				{  
					string eleValue = xmld.GetElementsByTagName("ElevationResponse")[0].ChildNodes[i+1].ChildNodes[1].InnerText;  
					_vertices[i*((int)segment.x+1)+indVerticesLng ].y= 	float.Parse(eleValue)	*TerrainManagerStatics.MeshSize.y ;
					tempstr+=","+(indVerticesLng + i)+_vertices[indVerticesLng + i].y;


				}  

				indVerticesLng ++;
				loadingLng += steplng;           
				StartCoroutine(LoadJsonGoogleLng(trimLng( loadingLng)));  
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				LoadState = loadState.EleError;
				checkELE = false;
				Debug.Log(Trrname+","+indVerticesLng+"*"+ex.ToString()+"\n"+www_data_ELE.text);  
			}  

			finally  	{}  

		}//end else	
	}

	void XMLBingLngComplete()
	{
		StrWwwData = www_data_ELE.text;   

		if (!string.IsNullOrEmpty( www_data_ELE.error))    
		{    
			Debug.LogWarning ("error ELE:"+Trrname +">"+indVerticesLng+"/"+indVerticesLng +"-" + www_data_ELE.error+"--"+www_data_ELE.isDone  );

			TerrainManagerStatics.NumError++;
			LoadState = loadState.EleError;

			for (int i=0; i <= segment.y ; i++)		
			{
				storeErrorSample (i*((int)segment.x+1)+indVerticesLng );
			}

			indVerticesLng =indVerticesLng+(int)segment.x+1;
			loadingLng += steplng;           
			StartCoroutine(LoadJsonGoogleLng(trimLng( loadingLng)));  
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data_ELE.text;    
				Debug.Log(Trrname+","+indVerticesLng +" "+ipaddress +"\n"+StrWwwData+"\n"+Time.deltaTime );
			int indss = StrWwwData.IndexOf ("<Elevations>");
			int indse = StrWwwData.IndexOf ("</Elevations>");
			string wwws2 = StrWwwData.Substring (indss, indse - indss+13);
			Debug.Log (indss+"."+indse+"~~"+wwws2);
				XmlDocument xmld=new XmlDocument();
			xmld.LoadXml(wwws2);

			int XmlCount = xmld.GetElementsByTagName("Elevations")[0].ChildNodes.Count;  

				for (int i = 0; i < XmlCount; i++)  
				{  
					string eleValue = xmld.GetElementsByTagName("Elevations")[0].ChildNodes[i].InnerText;  
			
					_vertices[i*((int)segment.x+1)+indVerticesLng ].y= 	float.Parse(eleValue)	*TerrainManagerStatics.MeshSize.y ;
					tempstr+=","+(indVerticesLng + i)+_vertices[indVerticesLng + i].y;
				}  


				indVerticesLng ++;
				loadingLng += steplng;           
				StartCoroutine(LoadJsonBingLng(trimLng( loadingLng)));  
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				LoadState = loadState.EleError;
				checkELE = false;
				Debug.Log(Trrname+","+indVerticesLng+"*"+ex.ToString()+"\n"+www_data_ELE.text);  
			}  

			finally  	{}  

		}//end else	
	}


	#if !(UNITY_5_0||UNITY_5_1||UNITY_5_2 )
	/// <summary>
	/// google高度www加载完成
	/// </summary>
	void JsonGoogleLngComplete()
	{
		StrWwwData = www_data_ELE.text;   

		if (!string.IsNullOrEmpty( www_data_ELE.error))    
		{    
			Debug.LogWarning ("error ELE:"+Trrname +">"+indVerticesLng+"/"+indVerticesLng +"-" + www_data_ELE.error+"--"+www_data_ELE.isDone  );

			TerrainManagerStatics.NumError++;
			LoadState = loadState.EleError;

			for (int i=0; i <= segment.y ; i++)		
			{
				storeErrorSample (i*((int)segment.x+1)+indVerticesLng );
			}

			indVerticesLng =indVerticesLng+(int)segment.x+1;
			loadingLng += steplng;           
			StartCoroutine(LoadJsonGoogleLng(trimLng( loadingLng)));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data_ELE.text;    
				Debug.Log(Trrname+","+indVerticesLng +" "+ipaddress +"\n"+StrWwwData+"\n"+Time.deltaTime );
				JsonMapDataGoogle GoogleJsonData = JsonUtility.FromJson<JsonMapDataGoogle>(StrWwwData);
				for (int i=0; i < GoogleJsonData.results.Length ; i++)		
				{
					_vertices[i*((int)segment.x+1)+indVerticesLng ].y= 	float.Parse(GoogleJsonData.results[i].elevation.ToString())	*TerrainManagerStatics.MeshSize.y ;
					tempstr+=","+(indVerticesLng + i)+_vertices[indVerticesLng + i].y;
				}

				indVerticesLng ++;
				loadingLng += steplng;           
				StartCoroutine(LoadJsonGoogleLng(trimLng( loadingLng)));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				LoadState = loadState.EleError;
				checkELE = false;
				Debug.Log(Trrname+","+indVerticesLng+"*"+ex.ToString()+"\n"+www_data_ELE.text);  
			}  

			finally  	{}  

		}//end else	
	}
	/// <summary>
	/// bing 高度www加载完成
	/// </summary>
	void JsonBingLngComplete()
	{
		if (www_data_ELE.isDone) {
			StrWwwData = www_data_ELE.text; 
		} else {
			Debug.LogWarning (Trrname+" not done! p="+www_data_ELE.progress+"e= "+www_data_ELE.error);
		}
		////////////////////////////
		if (!string.IsNullOrEmpty( www_data_ELE.error))    
		{    
			Debug.LogWarning ("error ELE:"+Trrname +"/"+indVerticesLng +"-" + www_data_ELE.error+"--"+www_data_ELE.isDone  );
			TerrainManagerStatics.NumError++;
			LoadState = loadState.EleError;
			 
			for (int i=0; i <= segment.y ; i++)		
			{
				storeErrorSample (i*((int)segment.x+1)+indVerticesLng );
			}

			indVerticesLng =indVerticesLng+(int)segment.x+1;
			loadingLng += steplng;           
			StartCoroutine(LoadJsonBingLng(trimLng( loadingLng)));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data_ELE.text; 
				Debug.Log (Trrname+","+ southeastlat +","+loadingLng +","+ northwestlat  +","+loadingLng+"\n"+StrWwwData );

				JsonMapDataBing JMDB=JsonUtility.FromJson <JsonMapDataBing>(www_data_ELE.text );
				string [] bingresults=JMDB.resourceSets[0].resources[0].elevations ;

				for (int i=0; i < bingresults.Length ; i++)		
				{

					_vertices[i*((int)segment.x+1)+indVerticesLng ].y=  float.Parse(bingresults[i]) *TerrainManagerStatics.MeshSize.y ;
					tempstr+="/"+(indVerticesLng + i)+","+_vertices[indVerticesLng + i].y;
				}

				indVerticesLng ++;
				loadingLng += steplng;           
				StartCoroutine(LoadJsonBingLng(trimLng( loadingLng)));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				LoadState = loadState.EleError;
				checkELE = false;
				Debug.Log(ex.ToString());  
			}  

			finally  	{}  

		}//end else	
	}
	#endif 

	#endregion 


	///////////////////////////
	#region 获取贴图
	//获取当前范围的贴图并保存
	/// <summary>
	/// 获取贴图的尺寸，在去水印时使用
	/// </summary>
	float sizemapx;
	float sizemapy;
	float newcenterlat ;
	int zoommapfinal;
	/// <summary>
	/// 获取贴图，google和bing都使用墨卡托投影法，不同纬度获取的贴图高度不同
	/// </summary>
	/// <returns>The google.</returns>
	IEnumerator  loadimgGoogle()
	{
		LoadState = loadState.imgLoading;
		mercatorProjection ();

		string 	ipaddress = "https://maps.googleapis.com/maps/api/staticmap?center="; //获取
		ipaddress+=newcenterlat+","+centerlng+"&zoom="+zoommapfinal;
		ipaddress += "&size=" + sizemapx + "x" + (sizemapy + 50) + "&maptype=" + TerrainManagerStatics .MapType;
		ipaddress +="&scale="+TerrainManagerStatics.ImagineScale + "&key=";
		ipaddress += STMKey;


		Debug.Log (Trrname+"  loadimg  "+ipaddress+"\n"+Time.time  );
		www_data_STM = new WWW(ipaddress);  

		yield return www_data_STM;  
	
		if (Application.isPlaying && www_data_STM.isDone) {
			loadImgComplete ();
		}
	}

	//***********************************************
	IEnumerator  loadimgBing()
	{
		LoadState = loadState.imgLoading;

		mercatorProjection ();

		string 	ipaddress = "https://dev.virtualearth.net/REST/v1/Imagery/Map/"; //获取
		ipaddress+=TerrainManagerStatics .MapType+ "/"+newcenterlat+","+centerlng+"/"+zoommapfinal+"?";
		ipaddress += "mapSize=" + sizemapx + "," + (sizemapy + 50);
		ipaddress += "&key="+STMKey;
		Debug.Log (Trrname+"  loadimg  "+ipaddress );
		www_data_STM = null;
		www_data_STM = new WWW(ipaddress);  

		yield return www_data_STM;  
	
		if (Application.isPlaying && www_data_STM.isDone) {
			loadImgComplete ();
		}
	}

	//-*****-----------
	/// <summary>
	/// 墨卡托投影法，根据地理坐标计算在地图中的位置
	/// </summary>
	void mercatorProjection(){
		float lerplng=2*TerrainManagerStatics.stepLng  ;//范围跨越的经度
		sizemapx=Mathf.Abs( lerplng  /360);//完整地图等分360份为跨经度1的宽度，在完整地图中所占的比例

		////////////////
		// north，北端纬度所在完整地图上的位置（比例）
		float sinnorthlat=Mathf.Sin(northwestlat *Mathf.PI /180);
		sinnorthlat = Mathf.Clamp (sinnorthlat, -0.99f, 0.99f);
		float pointnorthlat=(0.5f - Mathf.Log ((1 + sinnorthlat) / (1 - sinnorthlat)) / (4 * Mathf.PI));
		/// 
		// south，南端纬度所在完整地图上的位置（比例）
		float sinsouthlat=Mathf.Sin(southeastlat  *Mathf.PI /180);
		sinsouthlat = Mathf.Min (Mathf.Max (sinsouthlat, -0.99f), 0.99f);
		float pointsouthlat=(0.5f - Mathf.Log ((1 + sinsouthlat) / (1 - sinsouthlat)) / (4 * Mathf.PI));


		sizemapy = Mathf.Abs (pointsouthlat - pointnorthlat);//在完整地图中所占的比例
		Debug.Log (Trrname+" point north lat= " + pointnorthlat + "  south lat= " + pointsouthlat+" sizemapx="+sizemapx +" sizemapy="+sizemapy); 

		/// /////////////////////计算zoom
		int defaultmapsize = TerrainManagerStatics.ImagineSize;//580;//640;//最终获取图片的参考宽度,免费key最大尺寸为640
		int maxmapx;

		if(TerrainManagerStatics.IsAutoZoom )
		{
			if (sizemapx >= sizemapy) {
				maxmapx = defaultmapsize;
			} else {
				//当lat方向较大时，取lat方向=640时，计算lng方向的值
				maxmapx =(int)Mathf.Floor( sizemapx * defaultmapsize / sizemapy);		
			}
			int tempsize =(int)Mathf.Abs( (maxmapx * 360 ) / lerplng);//计算保证获取图片尺寸不超过640时，所需的完整地图尺寸
			int nextpoweroftwo =(int)Mathf.NextPowerOfTwo (tempsize);//计算保证获取图片不超过640时，所需的可用的完整地图尺寸，为2的幂
			if(nextpoweroftwo>tempsize){
				nextpoweroftwo = nextpoweroftwo / 2;
				//如果tempsize即2的倍数则不需要取小于nextpoweroftwo的值
				//否则取完整地图尺寸为小于tempsize的，最大的2的幂数
			}
			zoommapfinal = (int)Mathf.Floor (Mathf.Log(nextpoweroftwo/256 ,2));//根据 完整地图尺寸=256*2的zoom次方，计算zoom的值
		}else{
			zoommapfinal =TerrainManagerStatics.ImagineZoom ;
		}
		Debug.Log (Trrname+" sizemapx="+sizemapx +" sizemapy="+sizemapy);

		//////////////////////////计算当前zoom下取图片的尺寸
		float mapsize = 256 * Mathf.Pow (2, zoommapfinal);//在当前zoom下，完整地图的大小
		sizemapx=Mathf.Max(2, Mathf.RoundToInt ( sizemapx*mapsize));
		sizemapy=Mathf.Max(2,Mathf.RoundToInt  ( sizemapy*mapsize));//在当前完整地图大小下，xy方向的尺寸,取整数

		/// ///////////计算获取所需区域时，需要的center纬度
		///（0.5-新center纬度）*4PI=log（（1+siny）/（1-siny））
		float tempcentery=(pointsouthlat+pointnorthlat )/2;
		float tempc = 4 * Mathf.PI * (0.5f - (tempcentery ));
		float templog = Mathf.Exp (tempc);
		float sincentery = (templog - 1) / (templog + 1);
		newcenterlat =Mathf.Asin(sincentery )*180/Mathf.PI ;
	}

	/// <summary>
	/// Loads the image complete.
	/// </summary>
	void loadImgComplete(){
		if (!string.IsNullOrEmpty(www_data_STM.error) ) {
			Debug.LogWarning ("img load error : "+www_data_STM.error);
			TerrainManagerStatics.NumError ++;
			LoadState = loadState.imgError;
			////出错时计数，用于确定是否所有块都完成工作
		}else{

			LoadState = loadState.imgComplete;
			Texture2D tex2d = www_data_STM.texture;  
			//将图片保存至缓存路径  
			byte[] bytes = tex2d.EncodeToPNG();  
	
			string filepathImg="Assets/Resources/" + TerrainManagerStatics.savefiledate+"/downloadImg";
			if (!Directory.Exists(filepathImg)) 
			{
				Directory.CreateDirectory(filepathImg);
			}     
			string strfilename=filepathImg+"/"+Trrname+"_" + Mathf.FloorToInt( Time.realtimeSinceStartup) +".png";
			File.WriteAllBytes(strfilename, bytes);
			mapTexture = tex2d;

			loadTerrain();
		}
	}
	#endregion //获取贴图

	Mesh mesh ;
	/// <summary>
	/// 绘制mesh，赋三角面，顶点，uv，颜色，法线，生成碰撞体
	/// </summary>
	private void DrawMesh()
	{
		if (gameObject.GetComponent <MeshFilter> () == null) {
			mesh = gameObject.AddComponent<MeshFilter> ().mesh;
		} else {
			mesh= gameObject.GetComponent <MeshFilter> ().mesh;
		}
		//给mesh 赋值
		mesh.Clear();


		if (TerrainManagerStatics.lowPolyStyle) {
			Debug.LogWarning ("lowPolyStyle");
			setMeshPolyTo6Mode ();
			mesh.vertices = _vertices6;
			mesh.uv = _uvs6;
			if (TerrainManagerStatics.useNormal) {
				mesh.normals = _normals6;
			}
			mesh.colors = _colors6;
		} else {
			Debug.LogWarning ("no lowPolyStyle");
			setMeshPoly ();
			mesh.vertices = _vertices ;
			mesh.uv = _uvs;
			if (TerrainManagerStatics.useNormal) {
				mesh.normals = _normals;
			}
			mesh.colors = _colors;
		}

		mesh.triangles = _triangles;
		//重置法线
		mesh.RecalculateNormals();
		//重置范围
		mesh.RecalculateBounds();

		TerrainManagerStatics.NumComplete++;//加载成功计数。用于计算是否所有块都完成
	
		////////////////////////
		if (gameObject.GetComponent<MeshCollider> () ) {
			DestroyImmediate  (gameObject.GetComponent<MeshCollider> ());
		
		} 
		if (gameObject.GetComponent<MeshCollider> () == null) {
			gameObject.AddComponent<MeshCollider> ();
		}
		gameObject.GetComponent<MeshCollider> ().sharedMesh = mesh;
		gameObject.GetComponent<MeshCollider> ().convex = false;
	}


	/// <summary>
	/// 生成贴图，将加载的图片上下去掉各25象素
	/// </summary>
	private void DrawTexture(){

		#if UNITY_EDITOR 
		if (!Application.isPlaying) {
			EditorApplication.update -= UpdateE;
			stopall=true;
		}
		#endif 
		if (LoadState != loadState.imgError || LoadState != loadState.EleError) {
			LoadState = loadState.eleComplete;
		}
		if (!textureCompleted) {
			if (!gameObject.GetComponent <MeshRenderer> ()) {
				gameObject.AddComponent<MeshRenderer> ();
			}

			if (diffuseMap == null) {
				diffuseMap = new Material (Shader.Find ("Standard"));
			}
			if (mapTexture != null) {

				/////////////////////////////////////////
				int sc = 1;
				if (TerrainManagerStatics.DataSourceSTM == datasource.google) {
					sc = TerrainManagerStatics.ImagineScale;
				} 
				Texture2D tx2D = new Texture2D ((int)sizemapx * sc, (int)sizemapy * sc, TextureFormat.ARGB32, false);  
				Color[] tmp = mapTexture.GetPixels (0, 25 * sc, mapTexture.width, mapTexture.height - 50 * sc);
				Debug.Log ("tmp w=" + tmp.Length + "  h=" + (mapTexture.width - 50 * sc));
				tx2D.SetPixels (tmp);  
				tx2D.Apply ();  
				mapTexture = tx2D;

				diffuseMap.SetTexture ("_MainTex", mapTexture);
				textureCompleted = true;
			}
		
			gameObject.GetComponent<Renderer> ().material = diffuseMap;
		}

	}



	/// <summary>
	/// //设定每个顶点的uv
	/// </summary>
	/// <returns>UV.</returns>
	private Vector2[] GetUV()
	{
		string uvtest = "uv= ";
		int sum = _vertices.Length;
		_uvs = new Vector2[sum];
		float u = 1.0F / segment.x;
		float v = 1.0F / segment.y;
		uint index = 0;
		for (int i = 0; i < segment.y + 1; i++)
		{
			for (int j = 0; j < segment.x + 1; j++)
			{
				_uvs[index] = new Vector2(j * u, i * v);

				uvtest+=_uvs[index]+",";
				index++;
			}
		}
		return _uvs;
	}

	//****************
	Color[] _colors;
	Vector3[] _normals;




	/// <summary>
	/// Sets the mesh poly. 生成三角面，设置uv和vertices6为*6形式，设置三角面，法线，颜色
	/// </summary>
	private void setMeshPoly()
	{


		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		_triangles = new int[sum];
		_normals =new Vector3[_vertices.Length  ];
		_colors  = new Color[_vertices.Length ];

		int index = 0;
		for (int i = 0; i < segment.y; i++)
		{
			//y对应z方向
			for (int j = 0; j < segment.x; j++)
			{
				int index1 = index + 1;
				int index2 = index + 2;
				int index3 = index + 3;
				int index4 = index + 4;
				int index5 = index + 5;
				//*****

				int role = segment.x + 1;
				int self = j +( i*role);                
				int next = j + ((i+1) * role);

				_triangles[index] = self;
				_triangles[index1] = next + 1;
				_triangles[index2] = self + 1;
				_triangles[index3] = self;
				_triangles[index4] = next;
				_triangles[index5] = next + 1;


				index += 6;

				_colors [self] = GetVerticesToColor (meshcolor, GetFromVertices (j, i));

				Vector3 vertex00 = GetFromVertices(j + 0, i + 0);
				Vector3 vertex10 = GetFromVertices(j + 1, i + 0);
				Vector3 vertex11 = GetFromVertices(j + 1, i + 1);


				Vector3 normal01 = Vector3.Cross(vertex10 - vertex00, vertex11 - vertex00).normalized;
				_normals [self] =normal01;

			}
		}
			

	}
	/// <summary>
	/// Sets the mesh poly. 生成三角面，设置uv和vertices6为*6形式，设置三角面，法线，颜色
	/// </summary>
	private void setMeshPolyTo6Mode()
	{
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		_triangles = new int[sum];
		//******************
		_normals6 =new Vector3[sum];
		_colors6  = new Color[sum];
		_vertices6=new Vector3[sum]; 
		_uvs6=new Vector2[sum] ;

		int index = 0;
		for (int i = 0; i < segment.y; i++)
		{
			//y对应z方向
			for (int j = 0; j < segment.x; j++)
			{

				//****
				int index1 = index + 1;
				int index2 = index + 2;
				int index3 = index + 3;
				int index4 = index + 4;
				int index5 = index + 5;
				//*****

				_triangles [index] = index;// self;
				_triangles[index1] =index1;// next + 1;
				_triangles[index2] =index2;// self + 1;
				_triangles[index3] = index3;
				_triangles[index4] = index4;
				_triangles[index5] = index5;

				//***********************************
				Vector3 vertex00 = GetFromVertices(j + 0, i + 0);
				Vector3 vertex01 = GetFromVertices(j + 0, i + 1);
				Vector3 vertex10 = GetFromVertices(j + 1, i + 0);
				Vector3 vertex11 = GetFromVertices(j + 1, i + 1);

				Vector2 uv00 = GetFromUVs(j + 0, i + 0);
				Vector2 uv01 = GetFromUVs(j + 0, i + 1);
				Vector2 uv10 = GetFromUVs(j + 1, i + 0);
				Vector2 uv11 = GetFromUVs(j + 1, i + 1);

				Vector3 normal000111 = Vector3.Cross(vertex10 - vertex00, vertex11 - vertex00).normalized;
				Vector3 normal001011 = Vector3.Cross(vertex01 - vertex00, vertex11 - vertex00).normalized;


				_vertices6 [index] = vertex00;
				_vertices6[index1] = vertex01;
				_vertices6[index2] = vertex11;
				_vertices6[index3] = vertex00;
				_vertices6[index4] = vertex11;
				_vertices6[index5] = vertex10;

				_uvs6 [index] = uv00;
				_uvs6[index1] = uv01;
				_uvs6[index2] = uv11;
				_uvs6[index3] = uv00;
				_uvs6[index4] = uv11;
				_uvs6[index5] = uv10;

		
				_colors6 [index] =GetVerticesToColor(meshcolor,vertex00);
				_colors6[index1] =GetVerticesToColor(meshcolor,vertex01);
				_colors6[index2] =GetVerticesToColor(meshcolor,vertex11);
				_colors6[index3] =GetVerticesToColor(meshcolor,vertex00);
				_colors6[index4] =GetVerticesToColor(meshcolor,vertex11);
				_colors6[index5] =GetVerticesToColor(meshcolor,vertex10);

				_normals6 [index] =normal000111;
				_normals6[index1] =normal000111;
				_normals6[index2] =normal000111;
				_normals6[index3] =normal001011;
				_normals6[index4] =normal001011;
				_normals6[index5] =normal001011;

				index += 6;
				//
			}
		}
	}



	/// <summary>
	/// 根据索引从4点式的顶点表中获取数据，给6点式赋值
	/// </summary>
	/// <returns>The from vertices.</returns>
	/// <param name="x">索引x方向.</param>
	/// <param name="z">索引 z方向</param>
	Vector3 GetFromVertices(int x, int z)
	{
		return _vertices [x+z*(segment.x+1)];
	}
	/// <summary>
	/// 根据索引从4点式的uv表中获取数据，给6点式赋值
	/// </summary>
	/// <returns>The from vertices.</returns>
	/// <param name="x">索引x方向.</param>
	/// <param name="z">索引 z方向</param>
	Vector2 GetFromUVs(int x, int z)
	{
		return _uvs  [x+z*(segment.x+1)];
	}
	/// <summary>
	/// 根据顶点高度计算颜色
	/// </summary>
	/// <returns>The vertices to color.</returns>
	/// <param name="cc">Cc.</param>
	/// <param name="vv">Vv.</param>
	Color GetVerticesToColor(Color cc, Vector3 vv)
	{
		float ch = vv.y;//_vertices [x+z*(segment.x+1)].y;
		ch /= TerrainManagerStatics.MeshSize.y;
		ch = Mathf.Clamp (0.5f*ch / 4000, -0.5f, 0.5f) + 0.5f;
		//cc *= ch;
		cc.a = ch;
		return cc;
	}

	//************************
	 
	/// <summary>
	/// 处理经度数值.将超过180度的经度转换为正常值
	/// </summary>
	/// <returns>The lng.</returns>
	/// <param name="lng">Lng.</param>
	float trimLng(float lng){

		if (lng > 180) {
			lng -=360 ;//将超过180度的经度转换为正常值
		}else if(lng<-180){
			lng += 360;
		}
		return lng;
	}

}
