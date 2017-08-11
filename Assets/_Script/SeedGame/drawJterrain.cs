using UnityEngine;  
//using UnityEditor;  
using System.Collections.Generic ;  
using System.Collections;  
using System;
using System.Text;
using System.IO;



public class drawJterrain : MonoBehaviour {

	public bool edgeUp = false;//测试***
	public bool edgeDown = false;//测试***
	public bool edgeLeft = false;//测试***
	public bool edgeRight = false;//测试***
	string tempstr="";//打印测试数据用***
	//**********************

	string  ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?locations="; 
	string ELEKey = TerrainManager.ELEAPIkey;//google高度api key = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";//需要自己注册！！
	//string STMKey = main.STMAPIkey ;// google static map api key= "//需要自己注册！！
	//;//"AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk"
	string StrWwwData;
	int indVertivesLng=0;
	int indVertivesLat=0;


	float steplat ;//每次获取高度数据的间隔
	float steplng ;//每次获取高度数据的间隔

	public bool complete=false;
	public string Trrname;
	/// <summary>
	/// 索引，x=lat，y=lng
	/// </summary>
	public  Vector2Int Vpos;
	public  float centerlat;// +-90西北
	public  float centerlng;//+-180西北
	public  float northwestlat;// +-90西北
	public  float northwestlng;//+-180西北
	public  float southeastlat;// +-90东南
	public  float southeastlng;//+-180东南

	Vector2Int segment=new Vector2Int(3,3);//每块分段数量


	public Material diffuseMap;
	public Color  meshcolor;
	/// <summary>
	/// 绘制mesh，顶点数据
	/// </summary>
	Vector3[] vertives;
	Vector3[] vertives6;
	/// <summary>
	/// 绘制mesh，uv顶点数据.
	/// </summary>
	private Vector2[] uvs;
	/// <summary>
	/// 转换为6点数据，用于计算法线及颜色The uvs6.
	/// </summary>
	private Vector2[] uvs6;
	/// <summary>
	/// 绘制mesh，三角面数据
	/// </summary>
	private int[] _triangles;
	Vector3[] _normals;
	Color[] _colors;
	/// <summary>
	/// The error samples.
	/// </summary>
	List<int> errorSamples = new List<int> ();




	void xxx(Color c){
		Debug.Log ("xxxxxxxxxxxxxxxxxx-"+c);
	}

	public void initTrr( string _Trrname,Vector2Int _segment, Material _matTrr, Color _colTrr)
	{
		  edgeUp = false;
		  edgeDown = false;
		  edgeLeft = false;
		  edgeRight = false;

		diffuseMap = _matTrr;
		meshcolor = _colTrr;
		Trrname = _Trrname;
		segment=_segment;
		int leng = ((int)segment.x + 1) * ((int)segment.y + 1);
		vertives = new Vector3[leng];//用于存每个点的坐标
		//testVertives=new Vector2 [leng];
		GetUV();
//		GetTriangles();
//		DrawTexture ();
	}


	/// <summary>
	/// Loads the new location.以新中心点坐标加载地理数据
	/// </summary>
	/// <param name="_centerlat">Centerlat.中心纬度</param>
	/// <param name="_centerlng">Centerlng.中心经度</param>
	/// <param name="_vpos">Vpos（x，y），纬度，经度方向的索引</param>
	public  void loadNewLoc(float _centerlat,float _centerlng, Vector2Int _vpos)
	{		
		complete = false;
		Vpos = _vpos;
		indVertivesLng=0;
		centerlat = _centerlat;
		centerlng =trimLng( _centerlng);
		northwestlat = _centerlat + TerrainManager.stepLat ;//_northwestlat;// +-90 西北角纬度
		northwestlng = _centerlng - TerrainManager.stepLng ;// _northwestlng;//+-180西北角经度
		southeastlat = _centerlat - TerrainManager.stepLat ;// _southeastlat;// +-90 东南角纬度
		southeastlng = _centerlng + TerrainManager.stepLng ;// _southeastlng;//+-180 东南角经度
		steplat = TerrainManager.stepLat *2 / segment.y;//每段跨越的纬度
		steplng = TerrainManager.stepLng *2 / segment.x;//每段跨越的纬度
		northwestlng = trimLng (northwestlng);
		southeastlng = trimLng (southeastlng);

//		Debug.Log ("*2new"+Trrname+" pos="+northwestlat+","+northwestlng+"/"+southeastlat+","+southeastlng+" size="+main.MeshSize  );

		//print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);
		//*************************************

		syncMainEdge ();//同步边界
		prevLoad  ();//随机地形
		if (gameObject.GetComponent <MeshFilter> () != null) {
			DrawMesh ();
		} 

		switch (TerrainManager.DataSource){
		case (datasource.google):
			//	StartCoroutine(LoadJsonGoogleLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
			StartCoroutine(LoadJsonGoogleLng(northwestlng));//按精度取值，差值为南北方向
			break;
		case(datasource.bing ):
			//StartCoroutine(LoadJsonBingLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
			StartCoroutine(LoadJsonBingLng(northwestlng));//按精度取值，差值为南北方向
			break;
		case(datasource.random):

			fakeloadjson ();//随机地形
		
			testsampleError ();//随机增加错误数据
			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives ();//更新main数据
			DrawMesh ();
			complete = true;
			break;
		case(datasource.test):
			break;
		}


	}
	void testsampleError()
	{
		//*********************
		int yy=Mathf.FloorToInt ( UnityEngine.Random.Range(0,segment.x+1));//<segx
		string tse=" testsampleError= \n";
		if (yy > 0 && yy<=segment.x) {
			for (int i = 0; i <= segment.y; i++) {
				
				storeErrorSample ((int)(i * (segment.x + 1) + yy));
				//errorSamples.Add ((int)(i * (segment.x + 1) + yy));
				}
		}
		for (int cc = 0; cc <errorSamples.Count ;cc ++) {
			tse += ","+errorSamples[cc];
		}
		if (errorSamples.Count > 0) {
			Debug.Log (Trrname + tse);
		}
	}
	void  storeErrorSample(int index){
		if (edgeUp && index > (int)((segment.x + 1) * segment.y)) {
			Debug.Log ("up  "+index);
			return;
		}
		if (edgeDown && index <=segment.x) {
			Debug.Log ("down  "+index);
			return;
		}
		//if ((edgeLeft && indVertivesLng ==0) ||(edgeRight  && indVertivesLng >segment.x)) {
		if ((edgeLeft &&index% (segment.x+1) ==0) ||(edgeRight  &&(index+1)% (segment.x+1) ==0)) {
			Debug.Log ("leftright  "+index);
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
		string stlerp = Trrname+ "stlerp("+errorSamples.Count+")= ";
		List<float> samples=new List<float> ();
		for(int i=0;i<errorSamples.Count ;i++) {
			
			int sampleIndex =(int) errorSamples [i];
			stlerp+=sampleIndex+",";
			int ix = Mathf.FloorToInt (sampleIndex / ((int)segment.x + 1));
			int iy = sampleIndex - ix * ((int)segment.x + 1);

			if (ix > 0 && !errorSamples.Contains ((ix - 1) * ((int)segment.x + 1) + iy)) {
				samples.Add (vertives [(ix - 1) * ((int)segment.x + 1) + iy].y);
			}
			if (ix < segment.y && !errorSamples.Contains ((ix + 1) * ((int)segment.x + 1) + iy)) {
				samples.Add (vertives [(ix + 1) * ((int)segment.x + 1) + iy].y);
			}
			if (iy > 0 && !errorSamples.Contains (ix * ((int)segment.x + 1) + iy - 1)) {
				samples.Add (vertives [ix * ((int)segment.x + 1) + iy - 1].y);
			}
			if (iy < segment.x && !errorSamples.Contains (ix * ((int)segment.x + 1) + iy + 1)) {
				samples.Add (vertives [ix * ((int)segment.x + 1) + iy + 1].y);
			}	

			if (samples.Count < 0) {
				//vertives [sampleIndex] = UnityEngine.Random.Range (1, 2);
			} else {
				float sum = 0;
				for (int s = 0; s < samples.Count; s++) {
					sum += samples [s];
				}
				vertives [sampleIndex].y = sum / samples.Count;
				samples.Clear ();
			}
			errorSamples [i] = -1;
		}

		errorSamples.Clear ();
	//	Debug.LogWarning (stlerp);

	}



	/// <summary>
	/// 同步所有地图块数据到main，同步边界，避免数据源出错时地块衔接处开裂Syncs the main vertives.
	/// </summary>
	void syncMainEdge()
	{
		string synct="  sync =";//测试***

		if (Vpos.x > 0 && TerrainManager.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] != null) {
			//sync edge up
			edgeUp=true;
			//int iv = (int)((Vpos.x - 1) * main.Pieces.x + Vpos.y);//
			int ib = (int)((segment.x+1) * segment.y );
			synct+=("sync upedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x - 1) +","+(int) Vpos.y);//测试***
			for (int x = 0; x <= segment.x; x++) {
				if (vertives [x+ib].y != TerrainManager.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y) {
					//synct += "," + Vpos + " - " + x;
				}
				vertives [x+ib].y = TerrainManager.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y;
			}
		}
		if (Vpos.x <TerrainManager.Pieces.y -1 && TerrainManager.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] != null) {
			//sync edge down
			edgeDown=true;
			//int iv = (int)((Vpos.x + 1) * main.Pieces.x + Vpos.y);//
			int ib = (int)((segment.x+1) * segment.y);
			synct+=("sync downedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x + 1) +","+(int) Vpos.y);//测试***
			for (int x = 0; x <=segment.x; x++) {
				if (vertives [x].y != TerrainManager.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y) {
					//synct += "," + Vpos + " - " + x;
				}
				vertives [x].y = TerrainManager.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y;
			}
		}


		if (Vpos.y > 0 && TerrainManager.Vertives [(int)Vpos.x  ,(int) Vpos.y - 1] != null) {
			//sync edge left
			edgeLeft=true;
		
			synct+= ("sync leftedge " + Vpos + " ,m.v.ind=" + (int)Vpos.x +","+(int)(Vpos.y-1));//测试***
			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (vertives [ib].y != TerrainManager.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y-1)] [ib+(int)segment.x].y) {
					//synct += "," + Vpos + "," + y+"from:"+main.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y-1)] [ib+(int)segment.x].y+" to:"+vertives [ib].y;
				}
				vertives [ib].y = TerrainManager.Vertives [(int)Vpos.x, (int)(Vpos.y-1)] [ib + (int)segment.x].y;
			}
		}

		if (Vpos.y <TerrainManager.Pieces.x-1 && TerrainManager.Vertives [(int)Vpos.x  ,(int) Vpos.y + 1] != null) {
			//sync edge right
			edgeRight =true;
			synct+=("sync rightedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x ) +","+(int) (Vpos.y+1));//测试***

			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (vertives [(int)segment.x+ib].y != TerrainManager.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y+1)] [ib].y) {
					//synct += "," + Vpos + " - " + y;
				}
				vertives [(int)segment.x+ib].y = TerrainManager.Vertives [(int)(Vpos.x), (int)(Vpos.y+1)] [ib].y;
			}
		}
//		Debug.Log (Trrname+ synct);
	
		//*********************
		string stvG=" syncedge= \n";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x; j++) {
				int ind = i * ((int)segment.x + 1) + j;
				stvG += " / "+ind+","+vertives [ind];//+","+vertives [ind].y;
				//stvG += "," + vertives [ind].z + ")<";
				//stvG += testVertives [ind].x+","+testVertives [ind].y+">";
			}
			stvG+="\n";
		}
		//Debug.Log (Trrname +synct+"\n"+ stvG );
	}
	/// <summary>
	/// 更新main。vertives.
	/// </summary>
	void syncMainVertives()
	{
		//	Debug.Log (Trrname+synct);//测试***
		TerrainManager.Vertives [(int)Vpos.x,(int)Vpos.y] = vertives;


		//*********************
		string stvG=" stvg= \n";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x; j++) {
				int ind = i * ((int)segment.x + 1) + j;
				stvG += " / "+ind+","+vertives [ind];//+","+vertives [ind].y;
				//stvG += "," + vertives [ind].z + ")<";
				//stvG += testVertives [ind].x+","+testVertives [ind].y+">";
			}
			stvG+="\n";
		}
		//Debug.Log (Trrname + stvG );
	}
	 

	#region ~随机地形
	void prevLoad(){
		string stt = "  "+TerrainManager.MeshSize.z+","+TerrainManager.MeshSize.x+">\n";
		int starti = edgeUp ? 1 : 0;
		int endi = edgeDown ? (int)segment.y :(int) segment.y + 1;
		int startj = edgeLeft ? 1 : 0;
		int endj=edgeRight ? (int)segment.x: (int)segment.x + 1;


		for (int i = starti; i < endi ; i++) {
			for (int j = startj; j < endj  ; j++) {
				int ind = i * (int)(segment.x + 1) + j;

				vertives [ind].y =0;//Vpos.x +0.1f*Vpos.y;// // Mathf.Floor (centerlat)*0.01f + Mathf.Floor (centerlng) * 0.00001f;// Vpos.x +0.1f*Vpos.y  ;
				vertives [ind].x =j * TerrainManager.MeshSize.x / segment.x;
				vertives [ind].z = i * +TerrainManager.MeshSize.z / segment.y;
				stt +=","+i+","+j+","+ind+ vertives [ind];
			}
			//indVertivesLng++;
			stt+="\n";
		}
		Debug.Log (Trrname +" prevLoad "+stt);

	}
	/// <summary>
	/// 随机数地形
	/// </summary>
	void fakeloadjson(){
		string stt = "  "+TerrainManager.MeshSize.z+","+TerrainManager.MeshSize.x+">\n";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x ; j++) {
				int ind = i * (int)(segment.x + 1) + j;
				float a=UnityEngine. Random.Range(0f,200*TerrainManager.MeshSize.y);
				//(i % 2 + j % 3)*5;
				vertives [ind].y =a; //(i % 2 + j % 3)*5;//Vpos.x +0.1f*Vpos.y;// // Mathf.Floor (centerlat)*0.01f + Mathf.Floor (centerlng) * 0.00001f;// Vpos.x +0.1f*Vpos.y  ;
				vertives [ind].x =j * TerrainManager.MeshSize.x / segment.x;
				vertives [ind].z = i * +TerrainManager.MeshSize.z / segment.y;
			stt +=","+i+","+j+","+ind+ vertives [ind];
			}
			//indVertivesLng++;
			stt+="\n";
		}
		Debug.Log (Trrname +" fake json"+stt);

	}
	#endregion
	#region ~lng方向插值
	/// <summary>
	///加载高度数据，按照纬度方向分段多次加载
	///<summary>
	public IEnumerator LoadJsonGoogleLng(float lng)
	{  
		if (indVertivesLng>segment.x)		  
		{
			/////////////////(indVertives*(segment.y+1) >= vertives.Length)		
			Debug.Log (Trrname + "Data complete!!!!!!!"+tempstr );

			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives();//更新main数据
			DrawMesh();
			complete = true;
			yield break;
		}

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
		ipaddress +=southeastlat   +","+lng +"|";
		ipaddress += northwestlat   +","+lng ;//获取同一纬度下，东西经度之间的数据
		ipaddress += "&samples=" + (segment.y+1)+"&key=";
		ipaddress +=ELEKey;//需要自己注册！！
		//print(Trrname+"--"+ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  //获得数据后继续

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.LogWarning ("error :"+Trrname +"/"+indVertivesLng +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			TerrainManager.NumError++;

			//
			for (int i=0; i <= segment.y ; i++)		
			{
				//errorSamples.Add (indVertivesLng + i);
				storeErrorSample (i*((int)segment.x+1)+indVertivesLng );

			}

			indVertivesLng =indVertivesLng+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
			lng += steplng;           
			StartCoroutine(LoadJsonGoogleLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text;    
				Debug.Log(ipaddress+"\n"+StrWwwData);
				JsonMapDataGoogle GoogleJsonData = JsonUtility.FromJson<JsonMapDataGoogle>(StrWwwData);
				for (int i=0; i < GoogleJsonData.results.Length ; i++)		
				{
					vertives[i*((int)segment.x+1)+indVertivesLng ]= new Vector3(indVertivesLng*TerrainManager.MeshSize.x /segment.x, 
						float.Parse(GoogleJsonData.results[i].elevation.ToString())	*TerrainManager.MeshSize.y , 
						i* +TerrainManager.MeshSize.z/segment.y);
				//	testVertives [i*((int)segment.x+1)+indVertives]=new Vector2 ((float )GoogleJsonData.results[i].location .lng,(float )GoogleJsonData.results[i].location .lat);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+=","+(indVertivesLng + i)+vertives[indVertivesLng + i].y;
				}

				indVertivesLng ++;//=indVertives+(int)segment.y+1;//+= GoogleJsonData["results"].Count;/////////
				lng += steplng;           
				StartCoroutine(LoadJsonGoogleLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString()+indVertivesLng);  
			}  

			finally  	{}  

		}//end else		
	}//end LoadFile


	//加载高度数据，按照纬度方向分段多次加载bing
	public IEnumerator LoadJsonBingLng(float lng)
	{  
		if(indVertivesLng>segment.x)
		{
			/////////////////(indVertives*(segment.y+1) >= vertives.Length)		
			Debug.Log (Trrname + "Data complete!!!!!!!"+tempstr );

			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives();//更新main数据
			DrawMesh();
			complete = true;
			yield break;
		}
		//https://dev.virtualearth.net/REST/v1/Elevation/Polyline?points=30,60,30,65&heights=ellipsoid&samples=3&key=Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_
		ipaddress = "https://dev.virtualearth.net/REST/v1/Elevation/Polyline?points="; //获取json数据,改为XML获取xml数据
		ipaddress +=southeastlat  +","+lng +",";
		ipaddress += northwestlat  +","+lng ;//获取同一纬度下，东西经度之间的数据
		ipaddress += "&heights=ellipsoid&samples=" + (segment.y+1)+"&key=";
		ipaddress +=ELEKey;//需要自己注册！"Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_";//ELEKey;//需要自己注册！！
		//print(Trrname+"--"+ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  //获得数据后继续

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.LogWarning ("error :"+Trrname +"/"+indVertivesLng +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			TerrainManager.NumError++;

			//
			for (int i=0; i <= segment.y ; i++)		
			{
				//errorSamples.Add (indVertivesLng + i);
				storeErrorSample (i*((int)segment.x+1)+indVertivesLng );
			}

			indVertivesLng =indVertivesLng+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
			lng += steplng;           
			StartCoroutine(LoadJsonBingLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text; 
				Debug.Log (Trrname+","+ southeastlat +","+lng +","+ northwestlat  +","+lng+"\n"+StrWwwData );



				JsonMapDataBing JMDB=JsonUtility.FromJson <JsonMapDataBing>(www_data.text );
				string [] bingresults=JMDB.resourceSets[0].resources[0].elevations ;

				for (int i=0; i < bingresults.Length ; i++)		
				{
										
					vertives[i*((int)segment.x+1)+indVertivesLng ]= new Vector3(indVertivesLng*TerrainManager.MeshSize.x /segment.x, float.Parse(bingresults[i]) 
						*TerrainManager.MeshSize.y  , i*  TerrainManager.MeshSize.z/segment.y);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=bingJsonData.results[i].location.lat.ToString()+","+bingJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+="/"+(indVertivesLng + i)+","+vertives[indVertivesLng + i].y;
				}

				indVertivesLng ++;//=indVertives+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
				lng += steplng;           
				StartCoroutine(LoadJsonBingLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString());  
			}  

			finally  	{}  

		}//end else		
	}//end LoadFile


	#endregion 

	#region ~lat方向插值

	//加载高度数据，按照纬度方向分段多次加载
	public IEnumerator LoadJsonGoogleLat(float lat)
	{  
		if (indVertivesLat >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			syncMainEdge ();
			sampleLerp ();
			DrawMesh();
			yield break;
		}

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
		ipaddress +=lat +","+northwestlng +"|";
		ipaddress += lat  +","+southeastlng ;//获取同一纬度下，东西经度之间的数据
		ipaddress += "&samples=" + (segment.x+1)+"&key=";
		ipaddress +=ELEKey;//需要自己注册！！
		//print(Trrname+"--"+ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  //获得数据后继续

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.LogWarning ("error :"+Trrname +"/"+indVertivesLat +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			TerrainManager.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertivesLat + i);


			}

			indVertivesLat =indVertivesLat+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
			lat += steplat;           
			StartCoroutine(LoadJsonGoogleLat(lat));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text;    
				Debug.Log(StrWwwData);
				JsonMapDataGoogle GoogleJsonData = JsonUtility.FromJson<JsonMapDataGoogle>(StrWwwData);
				for (int i=0; i < GoogleJsonData.results.Length ; i++)		
				{
					vertives[indVertivesLat + i]= new Vector3(i*TerrainManager.MeshSize.x /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
						*TerrainManager.MeshSize.y  , (indVertivesLat / GoogleJsonData.results.Length) * TerrainManager.MeshSize.z/segment.y);
					//testVertives [indVertivesLat + i]=new Vector2 ((float )GoogleJsonData.results[i].location .lng,(float )GoogleJsonData.results[i].location .lat);

					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertivesLat + i].ToString ();//测试数据
					tempstr+=","+(indVertivesLat + i)+vertives[indVertivesLat + i].y;
				}

				indVertivesLat =indVertivesLat+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
				lat += steplat;           
				StartCoroutine(LoadJsonGoogleLat(lat));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString());  
			}  

			finally  	{}  

		}//end else		
	}//end LoadFile


	//加载高度数据，按照纬度方向分段多次加载bing
	public IEnumerator LoadJsonBingLat(float lat)
	{  
		if (indVertivesLat >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			complete = true;
			syncMainEdge ();
			sampleLerp ();
			DrawMesh();
			yield break;
		}
		//https://dev.virtualearth.net/REST/v1/Elevation/Polyline?points=30,60,30,65&heights=ellipsoid&samples=3&key=Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_
		ipaddress = "https://dev.virtualearth.net/REST/v1/Elevation/Polyline?points="; //获取json数据,改为XML获取xml数据
		ipaddress +=lat +","+northwestlng +",";
		ipaddress += lat  +","+southeastlng ;//获取同一纬度下，东西经度之间的数据
		ipaddress += "&heights=ellipsoid&samples=" + (segment.x+1)+"&key=";
		ipaddress +=ELEKey;//需要自己注册！"Alx3lnaKPAchj200vPlB4UXk2UY6JXCm2FNO8LzAzjrftFyzS_2fJGmR_nii9VL_";//！
		//print(Trrname+"--"+ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  //获得数据后继续

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.LogWarning ("error :"+Trrname +"/"+indVertivesLat +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			TerrainManager.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertivesLat + i);


			}

			indVertivesLat =indVertivesLat+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
			lat += steplat;           
			StartCoroutine(LoadJsonBingLat(lat));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text; 
				Debug.Log (Trrname+","+ lat +","+northwestlng +","+ lat  +","+southeastlng+"\n"+StrWwwData );
				//////////////

				//				StrWwwData=(StrWwwData.Substring(StrWwwData.IndexOf("elevations")-1,(StrWwwData.IndexOf("zoomLevel")-StrWwwData.IndexOf("elevations"))));
				//				StrWwwData=StrWwwData.Substring(StrWwwData.IndexOf("[")+1,(StrWwwData.IndexOf("]")-StrWwwData.IndexOf("[")-1));
				//				Debug.Log(StrWwwData);//.Substring(StrWwwData.IndexOf("elevations"),(StrWwwData.IndexOf("zoomLevel")-StrWwwData.IndexOf("elevations"))));
				//				string[] bingresults=StrWwwData.Split (',');
				//				Debug.Log(bingresults.Length);
				//				Debug.Log(bingresults[0]);
				//				Debug.Log(float.Parse(bingresults[0]));
				/////////////


				JsonMapDataBing JMDB=JsonUtility.FromJson <JsonMapDataBing>(www_data.text );
				string [] bingresults=JMDB.resourceSets[0].resources[0].elevations ;

				for (int i=0; i < bingresults.Length ; i++)		
				{
					vertives[indVertivesLat + i]= new Vector3(i*TerrainManager.MeshSize.x /segment.x, float.Parse(bingresults[i]) 
						*TerrainManager.MeshSize.y  , (indVertivesLat /bingresults.Length ) *  TerrainManager.MeshSize.z/segment.y);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=bingJsonData.results[i].location.lat.ToString()+","+bingJsonData.results[i].location.lng.ToString()+vertives[indVertivesLat + i].ToString ();//测试数据
					tempstr+="/"+(indVertivesLat + i)+","+vertives[indVertivesLat + i].y;
				}

				indVertivesLat =indVertivesLat+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
				lat += steplat;           
				StartCoroutine(LoadJsonBingLat(lat));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString());  
			}  

			finally  	{}  

		}//end else		
	}//end LoadFile
	#endregion 
	///////////////////////////
	/// 

	Mesh mesh ;
	//
	private void DrawMesh()
	{
		setMeshPoly();
		DrawTexture ();
		string stt = "V6= ";
		string stg = "TG= ";
		string stn = "TN= ";
		for (int i = 0; i < vertives6.Length; i++) {
			stt += vertives6 [i];
			stg += _triangles [i]+",";
			stn += _normals [i];
		}
		//Debug.Log (stt);
		//Debug.Log (stg);
		//Debug.Log (stn);
		//*****************
		if (gameObject.GetComponent <MeshFilter> () == null) {
			mesh = gameObject.AddComponent<MeshFilter> ().mesh;
		} 
		//给mesh 赋值
		mesh.Clear();
		mesh.vertices = vertives6;//,pos);vertives
		mesh.uv = uvs6;//uvs;
		mesh.normals = _normals;
		mesh.colors = _colors;
		mesh.triangles = _triangles;
		//重置法线
		mesh.RecalculateNormals();
		//重置范围
		mesh.RecalculateBounds();
		TerrainManager.NumComplete++;//加载成功计数。用于计算是否所有块都完成
		//		DrawTexture ();
		////////////////////////
		if (gameObject.AddComponent<MeshCollider> () == null) {
			gameObject.AddComponent<MeshCollider> ();
			gameObject.GetComponent<MeshCollider> ().sharedMesh = mesh;
			gameObject.GetComponent<MeshCollider> ().convex = true;
		}
		//		SaveAsset();
	}

	private void DrawTexture(){

		if (!gameObject.GetComponent <MeshRenderer> ()) {
			gameObject.AddComponent<MeshRenderer> ();
		}

		if (diffuseMap == null)
		{
			diffuseMap = new Material(Shader.Find("Standard"));
		}
		gameObject .GetComponent<Renderer>().material = diffuseMap;
	}




	//设定每个顶点的uv
	private Vector2[] GetUV()
	{
		string uvtest = "uv= ";
		int sum = vertives.Length;
		uvs = new Vector2[sum];
		float u = 1.0F / segment.x;
		float v = 1.0F / segment.y;
		uint index = 0;
		for (int i = 0; i < segment.y + 1; i++)
		{
			for (int j = 0; j < segment.x + 1; j++)
			{
				uvs[index] = new Vector2(j * u, i * v);
				//**********
				//测试uv，循环使用贴图
				//float modU=j*u;
				//modU = (Mathf.FloorToInt (modU) % 2 == 0) ? modU % 1 : (1 + Mathf.FloorToInt (modU) - modU) % 2;
				//float modV=i*v;
				//modV = (Mathf.FloorToInt (modV) % 2 == 0) ? modV % 1 : (1 + Mathf.FloorToInt (modV) - modV) % 2;
				//uvs[index] = new Vector2(modU, modV);
				//********
				uvtest+=uvs[index]+",";
				index++;
			}
		}
		return uvs;
	}

	//*********************

	private int[] GetTrianglesO()
	{
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		_triangles = new int[sum];
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

				int role = Mathf.FloorToInt(segment.x) + 1;
				int self = j +( i*role);                
				int next = j + ((i+1) * role);

				_triangles[index] = self;
				_triangles[index1] = next + 1;
				_triangles[index2] = self + 1;
				_triangles[index3] = self;
				_triangles[index4] = next;
				_triangles[index5] = next + 1;

			
				index += 6;
				//
			}
		}
		return _triangles;
	}
	//********************
	/// <summary>
	/// Sets the mesh poly. 设置uv和vertives6为*6形式，设置三角面，法线，颜色
	/// </summary>
	/// <returns>The mesh poly.</returns>
	private void setMeshPoly()
	{
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		_triangles = new int[sum];
		//******************
		_normals =new Vector3[sum];// new Vector3[vertives.Length ];// new Vector3[sum];
		_colors  = new Color[sum];// new Color[vertives.Length ];//
		vertives6=new Vector3[sum]; 
		uvs6=new Vector2[sum] ;

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
				Vector3 vertex00 = GetFromVertives(j + 0, i + 0);//, segment.x, segment.y, noiseOffset, 1);
				Vector3 vertex01 = GetFromVertives(j + 0, i + 1);//, segment.x, segment.y, noiseOffset, 1);
				Vector3 vertex10 = GetFromVertives(j + 1, i + 0);//, segment.x, segment.y, noiseOffset, 1);
				Vector3 vertex11 = GetFromVertives(j + 1, i + 1);//, segment.x, segment.y, noiseOffset, 1);

				Vector2 uv00 = GetFromUVs(j + 0, i + 0);//, segment.x, segment.y, noiseOffset, 1);
				Vector2 uv01 = GetFromUVs(j + 0, i + 1);//, segment.x, segment.y, noiseOffset, 1);
				Vector2 uv10 = GetFromUVs(j + 1, i + 0);//, segment.x, segment.y, noiseOffset, 1);
				Vector2 uv11 = GetFromUVs(j + 1, i + 1);//, segment.x, segment.y, noiseOffset, 1);

				Vector3 normal000111 = Vector3.Cross(vertex10 - vertex00, vertex11 - vertex00).normalized;
				Vector3 normal001011 = Vector3.Cross(vertex01 - vertex00, vertex11 - vertex00).normalized;


				vertives6 [index] = vertex00;
				vertives6[index1] = vertex01;
				vertives6[index2] = vertex11;
				vertives6[index3] = vertex00;
				vertives6[index4] = vertex11;
				vertives6[index5] = vertex10;

				uvs6 [index] = uv00;
				uvs6[index1] = uv01;
				uvs6[index2] = uv11;
				uvs6[index3] = uv00;
				uvs6[index4] = uv11;
				uvs6[index5] = uv10;

			//	if(meshcolor!=null ){
				_colors [index] =meshcolor ;//new Color (0.5f, 0.5f, 0.2f);//
				_colors[index1] =meshcolor ;//new Color (0.5f, 0.4f, 0.2f); //TerrainManager.gradient.Evaluate(height01);
				_colors[index2] =meshcolor;// new Color (0.4f, 0.5f, 0.2f);//TerrainManager.gradient.Evaluate(height11);
				_colors[index3] =meshcolor;//new Color (0.5f, 0.5f, 0.2f);// TerrainManager.gradient.Evaluate(height00);
				_colors[index4] =meshcolor; //new Color (0.5f, 0.4f, 0.2f);//TerrainManager.gradient.Evaluate(height11);
				_colors[index5] =meshcolor;// new Color (0.4f, 0.5f, 0.2f); //TerrainManager.gradient.Evaluate(height10);
			//	}

				_normals [index] =normal000111;// new Vector3 (0,0.5f,0);// normal000111;
				_normals[index1] =normal000111;// new Vector3 (0,0.5f,0);//normal000111;
				_normals[index2] =normal000111;// new Vector3 (0,0.5f,0);//normal000111;
				_normals[index3] =normal001011;// new Vector3 (0,-0.5f,0);//normal001011;
				_normals[index4] =normal001011;//new Vector3 (0,-0.5f,0);//normal001011;
				_normals[index5] =normal001011;// new Vector3 (0,-0.5f,0);//normal001011;


				//***********************************
				index += 6;
				//
			}
		}
	}
	//********************
	Vector3 GetFromVertives(int x, int z)
	{
		return vertives [x+z*(segment.y+1)];
	}
	Vector2 GetFromUVs(int x, int z)
	{
		return uvs  [x+z*(segment.y+1)];
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




//
//public  void loadNewLoc(float _centerlat,float _centerlng, float _steplat, float _steplng,Vector3 _size,Vector2 _vpos)
//{
//
//	complete = false;
//	Vpos = _vpos;
//	sizelat = _size.z;
//	sizelng = _size.x;
//	additionheight = _size.y;
//
//	centerlat = _centerlat;
//	centerlng =trimLng( _centerlng);
//	northwestlat = _centerlat + _steplat;//_northwestlat;// +-90 西北角纬度
//	northwestlng = _centerlng - _steplng;// _northwestlng;//+-180西北角经度
//	southeastlat = _centerlat - _steplat;// _southeastlat;// +-90 东南角纬度
//	southeastlng = _centerlng + _steplng;// _southeastlng;//+-180 东南角经度
//	steplat = _steplat*2 / segment.y;//每段跨越的纬度
//	steplng = _steplng*2 / segment.x;//每段跨越的纬度
//	northwestlng = trimLng (northwestlng);
//	southeastlng = trimLng (southeastlng);
//
//	Debug.Log ("*new"+Trrname+" pos="+northwestlat+","+northwestlng+"/"+southeastlat+","+southeastlng+" size="+_size );
//	//		centerlat = (_northwestlat + _southeastlat)/2;
//	//		centerlng = (_southeastlng - _northwestlng;
//	//		northwestlat = _northwestlat;// +-90 西北角纬度
//	//		northwestlng = _northwestlng;//+-180西北角经度
//	//		southeastlat = _southeastlat;// +-90 东南角纬度
//	//		southeastlng = _southeastlng;//+-180 东南角经度
//	//		steplat = ( northwestlat-southeastlat ) / segment.y;//每段跨越的纬度
//	//		steplng = ( southeastlng-northwestlng  ) / segment.x;//每段跨越的纬度
//	//z正方向为北
//	//print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);
//	//*************************************
//
//	//************************************
//	////
//	switch (main.DataSource){
//	case (datasource.google):
//		//	StartCoroutine(LoadJsonGoogleLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
//		StartCoroutine(LoadJsonGoogleLng(northwestlng));//按精度取值，差值为南北方向
//		break;
//	case(datasource.bing ):
//		//StartCoroutine(LoadJsonBingLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
//		StartCoroutine(LoadJsonBingLng(northwestlng));//按精度取值，差值为南北方向
//		break;
//	case(datasource.random):
//		fakeloadjson ();
//		sampleLerp ();
//		DrawMesh ();
//		break;
//	}
//
//
//}