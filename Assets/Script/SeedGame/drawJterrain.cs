﻿using UnityEngine;  
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
	string ELEKey = main.ELEAPIkey;//google高度api key = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";//需要自己注册！！
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
	public  Vector2 Vpos;
	public  float centerlat;// +-90西北
	public  float centerlng;//+-180西北
	public  float northwestlat;// +-90西北
	public  float northwestlng;//+-180西北
	public  float southeastlat;// +-90东南
	public  float southeastlng;//+-180东南

	Vector2 segment=new Vector2(3,3);//每块分段数量





	// private Vector3[] vtest;///////////////////////////
	//	[SerializeField ]
	//	float sizelat=100;
	//	[SerializeField]
	//	float  sizelng=100;
	//	float additionheight=1;

	public Material diffuseMap;
	/// <summary>
	/// 绘制mesh，顶点数据
	/// </summary>
	Vector3[] vertives;
	/// <summary>
	/// 绘制mesh，uv顶点数据.
	/// </summary>
	private Vector2[] uvs;
	/// <summary>
	/// 绘制mesh，三角面数据
	/// </summary>
	private int[] triangles;

	/// <summary>
	/// The error samples.
	/// </summary>
	List<int> errorSamples = new List<int> ();


	//private GameObject terrain;
	[SerializeField]
	Texture2D mapTexture;



	public void initTrr( string _Trrname,Vector2 _segment, Material _matTrr = null)
	{
		  edgeUp = false;
		  edgeDown = false;
		  edgeLeft = false;
		  edgeRight = false;

		diffuseMap = _matTrr;
		Trrname = _Trrname;
		segment=_segment;
		int leng = ((int)segment.x + 1) * ((int)segment.y + 1);
		vertives = new Vector3[leng];//用于存每个点的坐标
		//testVertives=new Vector2 [leng];
		GetUV();
		GetTriangles();
		DrawTexture ();
	}


	/// <summary>
	/// Loads the new location.以新中心点坐标加载地理数据
	/// </summary>
	/// <param name="_centerlat">Centerlat.中心纬度</param>
	/// <param name="_centerlng">Centerlng.中心经度</param>
	/// <param name="_vpos">Vpos（x，y），纬度，经度方向的索引</param>
	public  void loadNewLoc(float _centerlat,float _centerlng, Vector2 _vpos)
	{		
		complete = false;
		Vpos = _vpos;
		indVertivesLng=0;
		centerlat = _centerlat;
		centerlng =trimLng( _centerlng);
		northwestlat = _centerlat + main.stepLat ;//_northwestlat;// +-90 西北角纬度
		northwestlng = _centerlng - main.stepLng ;// _northwestlng;//+-180西北角经度
		southeastlat = _centerlat - main.stepLat ;// _southeastlat;// +-90 东南角纬度
		southeastlng = _centerlng + main.stepLng ;// _southeastlng;//+-180 东南角经度
		steplat = main.stepLat *2 / segment.y;//每段跨越的纬度
		steplng = main.stepLng *2 / segment.x;//每段跨越的纬度
		northwestlng = trimLng (northwestlng);
		southeastlng = trimLng (southeastlng);

//		Debug.Log ("*2new"+Trrname+" pos="+northwestlat+","+northwestlng+"/"+southeastlat+","+southeastlng+" size="+main.MeshSize  );

		//print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);
		//*************************************
		fakeloadjson ();//随机地形
		syncMainEdge ();//同步边界
		DrawMesh ();
		switch (main.DataSource){
		case (datasource.google):
			//	StartCoroutine(LoadJsonGoogleLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
			StartCoroutine(LoadJsonGoogleLng(northwestlng));//按精度取值，差值为南北方向
			break;
		case(datasource.bing ):
			//StartCoroutine(LoadJsonBingLat(southeastlat));//按纬度取值，差值为与赤道相交的平面，非东西方向
			StartCoroutine(LoadJsonBingLng(northwestlng));//按精度取值，差值为南北方向
			break;
		case(datasource.random):
//			syncMainEdge ();//同步边界
//			fakeloadjson ();//随机地形
//			DrawMesh ();
//		
			testsampleError ();//随机增加错误数据
			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives();//更新main数据
			DrawMesh ();
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
			return;
		}
		if (edgeDown && index <=segment.x) {
			return;
		}
		if ((edgeLeft && indVertivesLng ==0) ||(edgeRight  && indVertivesLng >segment.x)) {
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
		Debug.LogWarning (stlerp);

	}



	/// <summary>
	/// 同步所有地图块数据到main，同步边界，避免数据源出错时地块衔接处开裂Syncs the main vertives.
	/// </summary>
	void syncMainEdge()
	{
		string synct="  sync =";//测试***

		if (Vpos.x > 0 && main.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] != null) {
			//sync edge up
			edgeUp=true;
			//int iv = (int)((Vpos.x - 1) * main.Pieces.x + Vpos.y);//
			int ib = (int)((segment.x+1) * segment.y );
			synct+=("sync upedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x - 1) +","+(int) Vpos.y);//测试***
			for (int x = 0; x <= segment.x; x++) {
				if (vertives [x+ib].y != main.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y) {
					//synct += "," + Vpos + " - " + x;
				}
				vertives [x+ib].y = main.Vertives [(int)(Vpos.x - 1) ,(int) Vpos.y] [x].y;
			}
		}
		if (Vpos.x <main.Pieces.y -1 && main.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] != null) {
			//sync edge down
			edgeDown=true;
			//int iv = (int)((Vpos.x + 1) * main.Pieces.x + Vpos.y);//
			int ib = (int)((segment.x+1) * segment.y);
			synct+=("sync downedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x + 1) +","+(int) Vpos.y);//测试***
			for (int x = 0; x <=segment.x; x++) {
				if (vertives [x].y != main.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y) {
					//synct += "," + Vpos + " - " + x;
				}
				vertives [x].y = main.Vertives [(int)(Vpos.x + 1) ,(int) Vpos.y] [x+ib].y;
			}
		}


		if (Vpos.y > 0 && main.Vertives [(int)Vpos.x  ,(int) Vpos.y - 1] != null) {
			//sync edge left
			edgeLeft=true;
		
			synct+= ("sync leftedge " + Vpos + " ,m.v.ind=" + (int)Vpos.x +","+(int)(Vpos.y-1));//测试***
			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (vertives [ib].y != main.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y-1)] [ib+(int)segment.x].y) {
					//synct += "," + Vpos + "," + y+"from:"+main.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y-1)] [ib+(int)segment.x].y+" to:"+vertives [ib].y;
				}
				vertives [ib].y = main.Vertives [(int)Vpos.x, (int)(Vpos.y-1)] [ib + (int)segment.x].y;
			}
		}

		if (Vpos.y <main.Pieces.x-1 && main.Vertives [(int)Vpos.x  ,(int) Vpos.y + 1] != null) {
			//sync edge right
			edgeRight =true;
			synct+=("sync rightedge " + Vpos + " ,m.v.ind=" + (int)(Vpos.x ) +","+(int) (Vpos.y+1));//测试***

			for (int y = 0; y <=segment.y; y++) {
				int ib = (int)((segment.x+1) * y);
				if (vertives [(int)segment.x+ib].y != main.Vertives [(int)(Vpos.x ) ,(int) (Vpos.y+1)] [ib].y) {
					//synct += "," + Vpos + " - " + y;
				}
				vertives [(int)segment.x+ib].y = main.Vertives [(int)(Vpos.x), (int)(Vpos.y+1)] [ib].y;
			}
		}
		Debug.Log (Trrname+ synct);
	

	}
	/// <summary>
	/// 更新main。vertives.
	/// </summary>
	void syncMainVertives()
	{
		//	Debug.Log (Trrname+synct);//测试***
		main.Vertives [(int)Vpos.x,(int)Vpos.y] = vertives;


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
		Debug.Log (Trrname + stvG );
	}
	 

	#region ~随机地形
	/// <summary>
	/// 随机数地形
	/// </summary>
	void fakeloadjson(){
		string stt = "  "+main.MeshSize.z+","+main.MeshSize.x+">\n";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x ; j++) {
				int ind = i * (int)(segment.x + 1) + j;
				float a=UnityEngine. Random.Range(0f,10f);

				vertives [ind].y =a;//Vpos.x +0.1f*Vpos.y;// // Mathf.Floor (centerlat)*0.01f + Mathf.Floor (centerlng) * 0.00001f;// Vpos.x +0.1f*Vpos.y  ;
				vertives [ind].x =j * main.MeshSize.x / segment.x;
				vertives [ind].z = i * +main.MeshSize.z / segment.y;
			stt +=","+i+","+j+","+ind+ vertives [ind];
			}
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
		if (indVertivesLng>=segment.x)		  
		{
			/////////////////(indVertives*(segment.y+1) >= vertives.Length)		
			Debug.Log (Trrname + "Data complete!!!!!!!"+tempstr );
			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives();//更新main数据
			DrawMesh();
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
			main.NumError++;

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
					vertives[i*((int)segment.x+1)+indVertivesLng ]= new Vector3(indVertivesLng*main.MeshSize.x /segment.x, 
						float.Parse(GoogleJsonData.results[i].elevation.ToString())	*main.MeshSize.y , 
						i* +main.MeshSize.z/segment.y);
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
		if(indVertivesLng>=segment.x)
		{
			/////////////////(indVertives*(segment.y+1) >= vertives.Length)		
			Debug.Log (Trrname + "Data complete!!!!!!!"+tempstr );
			sampleLerp ();//修正错误数据
			syncMainEdge ();//同步边界
			syncMainVertives();//更新main数据
			DrawMesh();
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
			main.NumError++;

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
										
					vertives[i*((int)segment.x+1)+indVertivesLng ]= new Vector3(indVertivesLng*main.MeshSize.x /segment.x, float.Parse(bingresults[i]) 
						*main.MeshSize.y  , i*  main.MeshSize.z/segment.y);
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
			main.NumError++;

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
					vertives[indVertivesLat + i]= new Vector3(i*main.MeshSize.x /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
						*main.MeshSize.y  , (indVertivesLat / GoogleJsonData.results.Length) * main.MeshSize.z/segment.y);
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
			main.NumError++;

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
					vertives[indVertivesLat + i]= new Vector3(i*main.MeshSize.x /segment.x, float.Parse(bingresults[i]) 
						*main.MeshSize.y  , (indVertivesLat /bingresults.Length ) *  main.MeshSize.z/segment.y);
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
		if (gameObject.GetComponent <MeshFilter> () == null) {
			mesh = gameObject.AddComponent<MeshFilter> ().mesh;
		} 
		//给mesh 赋值
		mesh.Clear();
		mesh.vertices = vertives;//,pos);
		mesh.uv = uvs;
		mesh.triangles = triangles;
		//重置法线
		mesh.RecalculateNormals();
		//重置范围
		mesh.RecalculateBounds();
		main.NumComplete++;//加载成功计数。用于计算是否所有块都完成
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

		gameObject .AddComponent<MeshRenderer>();


		if (diffuseMap == null)
		{
			diffuseMap = new Material(Shader.Find("Standard"));
			if(mapTexture!=null){
				diffuseMap.SetTexture ("_MainTex",mapTexture);
			}

		}
		gameObject .GetComponent<Renderer>().material = diffuseMap;
	}




	//设定每个顶点的uv
	private Vector2[] GetUV()
	{
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
				index++;
			}
		}
		return uvs;
	}


	private int[] GetTriangles()
	{
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		triangles = new int[sum];
		uint index = 0;
		for (int i = 0; i < segment.y; i++)
		{
			//y对应z方向
			for (int j = 0; j < segment.x; j++)
			{
				int role = Mathf.FloorToInt(segment.x) + 1;
				int self = j +( i*role);                
				int next = j + ((i+1) * role);
				triangles[index] = self;
				triangles[index + 1] = next + 1;
				triangles[index + 2] = self + 1;
				triangles[index + 3] = self;
				triangles[index + 4] = next;
				triangles[index + 5] = next + 1;
				index += 6;
				//
			}
		}
		return triangles;
	}


	 
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