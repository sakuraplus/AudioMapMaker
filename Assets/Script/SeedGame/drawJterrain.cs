using UnityEngine;  
//using UnityEditor;  
using System.Collections.Generic ;  
using System.Collections;  
using System;
using System.Text;
using System.IO;

/// <summary>
/// Json map data of google.
/// </summary>
[Serializable]
public class JsonMapDataGoogle{
	public MapRes[] results;
	public string status;
}

[Serializable]
public class MapRes{
	public double elevation;
	public MapLocation location;
	public double resolution;
}

[Serializable]
public class MapLocation{
	public double lat;
	public double lng;
}
/// <summary>
/// /son map data  of bing
/// </summary>
[Serializable]
public class JsonMapDataBing{
	public string authenticationResultCode;
	public string brandLogoUri;
	public string copyright;
	//public BingresourceSets[] resourceSets;
	public BingresourceSets[] resourceSets;
	public string statusDescription;
}

[Serializable]
public class BingresourceSets{
	public string estimatedTotal="x";
	public Bingresources[] resources ;

}

[Serializable]
public class Bingresources{
	public string [] elevations;
	public int zoomLevel = 2;
}


public class drawJterrain : MonoBehaviour {

	//	GameObject Player ;

	string  ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?locations="; 
	string ELEKey = main.ELEAPIkey;//google高度api key = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";//需要自己注册！！
	//string STMKey = main.STMAPIkey ;// google static map api key= "//需要自己注册！！
	//;//"AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk"
	string StrWwwData;

	float steplat ;//每次获取高度数据的间隔
	float steplng ;//每次获取高度数据的间隔
	public bool complete=false;
	public string Trrname;
	public  Vector2 Vpos;
	public  float centerlat;// +-90西北
	public  float centerlng;//+-180西北
	public  float northwestlat;// +-90西北
	public  float northwestlng;//+-180西北
	public  float southeastlat;// +-90东南
	public  float southeastlng;//+-180东南

	Vector2 segment=new Vector2(3,3);//每块分段数量

	int indVertives=0;

	public Material diffuseMap;

	public  Vector3[] vertives;
	// private Vector3[] vtest;///////////////////////////

	[SerializeField ]
	float sizelat=100;
	[SerializeField]
	float  sizelng=100;
	float additionheight=1;


	private Vector2[] uvs;
	private int[] triangles;


	string tempstr="";//打印测试数据用

	//private GameObject terrain;
	[SerializeField]
	Texture2D mapTexture;

	//    public string  test()
	//    {
	//       return terrain.name;
	//    }

	public void initTrr( string _Trrname,Vector2 _segment, Material _matTrr = null)
	{
		diffuseMap = _matTrr;
		Trrname = _Trrname;
		segment=_segment;
		int leng = ((int)segment.x + 1) * ((int)segment.y + 1);
		vertives = new Vector3[leng];//用于存每个点的坐标
		testVertives=new Vector2 [leng];
		GetUV();
		GetTriangles();

		DrawTexture ();
	}
	float trimLng(float lng){
		
		if (lng > 180) {
			lng -=360 ;//将超过180度的经度转换为正常值
		}else if(lng<-180){
			lng += 360;
		}
		return lng;
	}
	//(float _northwestlat,float _northwestlng, float _southeastlat, float _southeastlng,Vector3 _size,Vector2 _vpos)
	public  void loadNewLoc(float _centerlat,float _centerlng, float _steplat, float _steplng,Vector3 _size,Vector2 _vpos)
	{

		complete = false;
		Vpos = _vpos;
		sizelat = _size.z;
		sizelng = _size.x;
		additionheight = _size.y;

		centerlat = _centerlat;
		centerlng =trimLng( _centerlng);
		northwestlat = _centerlat + _steplat;//_northwestlat;// +-90 西北角纬度
		northwestlng = _centerlng - _steplng;// _northwestlng;//+-180西北角经度
		southeastlat = _centerlat - _steplat;// _southeastlat;// +-90 东南角纬度
		southeastlng = _centerlng + _steplng;// _southeastlng;//+-180 东南角经度
		steplat = _steplat*2 / segment.y;//每段跨越的纬度
		steplng = _steplng*2 / segment.x;//每段跨越的纬度
		northwestlng = trimLng (northwestlng);
		southeastlng = trimLng (southeastlng);

		Debug.Log ("*new"+Trrname+" pos="+northwestlat+","+northwestlng+"/"+southeastlat+","+southeastlng+" size="+_size );
//		centerlat = (_northwestlat + _southeastlat)/2;
//		centerlng = (_southeastlng - _northwestlng;
//		northwestlat = _northwestlat;// +-90 西北角纬度
//		northwestlng = _northwestlng;//+-180西北角经度
//		southeastlat = _southeastlat;// +-90 东南角纬度
//		southeastlng = _southeastlng;//+-180 东南角经度
//		steplat = ( northwestlat-southeastlat ) / segment.y;//每段跨越的纬度
//		steplng = ( southeastlng-northwestlng  ) / segment.x;//每段跨越的纬度
		//z正方向为北
		//print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);
		//*************************************

		//************************************
////
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
			fakeloadjson ();
			sampleLerp ();
			DrawMesh ();
			break;
		}


	}
	public  void loadNewLoc(float _centerlat,float _centerlng, Vector2 _vpos)
	{
		
		complete = false;
		Vpos = _vpos;

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
			fakeloadjson ();
			sampleLerp ();
			DrawMesh ();
			break;
		}


	}
	List<int> errorSamples = new List<int> ();
	Vector2[] testVertives;
	void sampleLerp(){
		for(int i=0;i<errorSamples.Count ;i++) {
			int num = 0;
			float sumSamples = 0;
			//			if(
		}
		errorSamples.Clear();
		string stvG="stvg= \n";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x; j++) {
				int ind = i * ((int)segment.y + 1) + j;
				stvG += " / "+ind+"("+vertives [ind].x+","+vertives [ind].y;
				stvG += "," + vertives [ind].z + ")<";
				stvG += testVertives [ind].x+","+testVertives [ind].y+">";
			}
			stvG+="\n";
		}
		Debug.Log (stvG );
	}
	void saveStatic(){
		for (int i = 0; i <= segment.x; i++) {
			for (int j = 0; i <= segment.y; i++) {
				//main.Vertives[Vpos .x*se+j]=vertives [];
				//			if(
			}
		}
	}
	public void test(){
		initTrr ("xxx", new Vector2 (5, 5), diffuseMap );
		fakeloadjson ();
		sampleLerp ();
		DrawMesh ();
	}
	void fakeloadjson(){
		string stt = "";
		for (int i = 0; i <= segment.y; i++) {
			for (int j = 0; j <= segment.x ; j++) {
				int ind = i * (int)(segment.y + 1) + j;
				float a=UnityEngine. Random.Range(0f,5f);
				vertives [ind].y =a;// 0.1f* Vpos.y +0.001f*ind+Vpos.x  ;
				vertives [ind].x =j * sizelng / segment.x;
				vertives [ind].z = i * sizelat / segment.y;
//				vertives [ind].y = ind;// 0.1f* Vpos.y +0.001f*ind+Vpos.x  ;
//				vertives [ind].x =Vpos.x;// j * sizelng / segment.x;
//				vertives [ind].z =Vpos.y;// i * sizelat / segment.y;
				stt +=","+i+","+j+","+ind+ vertives [ind];
			}
			stt+="\n";
		}
	//	Debug.Log (Trrname +" fake json"+stt);

	}
	void clearMainVertives()
	{
		string cleartest = "cleartest= ";
		for (int x = 0; x <= segment.x; x++) {
			for (int y = 0; y <= segment.y; y++) {
				int ind = x * (int)(segment.y + 1) + y;
				if (Vpos.x == main.Pieces.y) {
					



//				if (Vpos.x == main.Pieces.y) {
//					//edge down
//					if(y==segment.y){
//						vertives[ind]=null ;
//					}
//				}
//				if (Vpos.x == 0) {
//					//edge up
//					if(y==0){
//						vertives[ind]=null ;
//					}
//				}
//				if (Vpos.y == main.Pieces.x) {
//					//edge right
//					if(x==segment.x){
//						vertives[ind]=null ;
//					}
//				}
//				if (Vpos.x == 0) {
//					//edge left
//					if(x==0){
//						vertives[ind]=null ;
//					}
				}
			}
		}
		Debug.Log (cleartest);
	}
	void syncMainVertives()
	{
//		string synctest = "synctest= ";
//		for (int i = 0; i <= segment.x; i++) {
//			for (int j = 0; j <= segment.y; j++) {
//				int ind = i * (int)(segment.y + 1) + j;
//
//
//
//
//				if (main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j].y == 0) {
//					main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j] = vertives [ind];
//				} else if (i == 0 || j == 0 || i == segment.x || j == segment.y) {
//					synctest += ind + "(" + Vpos + ")<" + i + "," + j + ">  / ";
//					vertives [ind] = main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j];
//				} else {
//					Debug.LogWarning ("syncError "+i+","+j);
//				}
//			}
//		}
//		Debug.Log (synctest);
	}
//	void syncMainVertives()
//	{
//		string synctest = "synctest= ";
//		for (int i = 0; i <= segment.x; i++) {
//			for (int j = 0; j <= segment.y; j++) {
//				int ind = i * (int)(segment.y + 1) + j;
//
//				if (main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j].y == 0) {
//					main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j] = vertives [ind];
//				} else if (i == 0 || j == 0 || i == segment.x || j == segment.y) {
//					synctest += ind + "(" + Vpos + ")<" + i + "," + j + ">  / ";
//					vertives [ind] = main.Vertives [(int)(Vpos.x * segment.x) + i, (int)(Vpos.y * segment.y) + j];
//				} else {
//					Debug.LogWarning ("syncError "+i+","+j);
//				}
//			}
//		}
//		Debug.Log (synctest);
//	}
	//加载高度数据，按照纬度方向分段多次加载
	public IEnumerator LoadJsonGoogleLng(float lng)
	{  
		if (indVertives*(segment.y+1) >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			syncMainVertives ();
			sampleLerp ();
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
			Debug.LogWarning ("error :"+Trrname +"/"+indVertives +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			main.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertives + i);


			}

			indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
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
					vertives[i*((int)segment.x+1)+indVertives ]= new Vector3(indVertives*sizelng /segment.x, 
						float.Parse(GoogleJsonData.results[i].elevation.ToString())	*main.MeshSize.y , 
						i* sizelat/segment.y);
					testVertives [i*((int)segment.x+1)+indVertives]=new Vector2 ((float )GoogleJsonData.results[i].location .lng,(float )GoogleJsonData.results[i].location .lat);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+=","+(indVertives + i)+vertives[indVertives + i].y;
				}

				indVertives ++;//=indVertives+(int)segment.y+1;//+= GoogleJsonData["results"].Count;/////////
				lng += steplng;           
				StartCoroutine(LoadJsonGoogleLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  	

			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString()+indVertives);  
			}  

			finally  	{}  

		}//end else		
	}//end LoadFile

	//加载高度数据，按照纬度方向分段多次加载
	public IEnumerator LoadJsonGoogleLat(float lat)
	{  
		if (indVertives >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			syncMainVertives ();
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
			Debug.LogWarning ("error :"+Trrname +"/"+indVertives +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			main.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertives + i);


			}

			indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
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
					vertives[indVertives + i]= new Vector3(i*sizelng /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
						*main.MeshSize.y  , (indVertives / GoogleJsonData.results.Length) * sizelat/segment.y);
					testVertives [indVertives + i]=new Vector2 ((float )GoogleJsonData.results[i].location .lng,(float )GoogleJsonData.results[i].location .lat);

					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+=","+(indVertives + i)+vertives[indVertives + i].y;
				}

				indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
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
		if (indVertives >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			syncMainVertives ();
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
			Debug.LogWarning ("error :"+Trrname +"/"+indVertives +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			main.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertives + i);


			}

			indVertives =indVertives+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
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
					vertives[indVertives + i]= new Vector3(i*sizelng /segment.x, float.Parse(bingresults[i]) 
						*main.MeshSize.y  , (indVertives /bingresults.Length ) * sizelat/segment.y);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=bingJsonData.results[i].location.lat.ToString()+","+bingJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+="/"+(indVertives + i)+","+vertives[indVertives + i].y;
				}

				indVertives =indVertives+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
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
	//加载高度数据，按照纬度方向分段多次加载bing
	public IEnumerator LoadJsonBingLng(float lng)
	{  
		if (indVertives*(segment.y+1) >= vertives.Length)		  
		{
			/////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			syncMainVertives ();
			sampleLerp ();
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
			Debug.LogWarning ("error :"+Trrname +"/"+indVertives +"-" + www_data.error+"--"+www_data.isDone  );

			StrWwwData =  "error :" + www_data.error;  
			main.NumError++;

			//
			for (int i=0; i <= segment.x ; i++)		
			{
				errorSamples.Add (indVertives + i);


			}

			indVertives =indVertives+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
			lng += steplng;           
			StartCoroutine(LoadJsonBingLng(trimLng( lng)));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text; 
				Debug.Log (Trrname+","+ southeastlat +","+lng +","+ northwestlat  +","+lng+"\n"+StrWwwData );
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
										
					vertives[i*((int)segment.x+1)+indVertives ]= new Vector3(indVertives*sizelng /segment.x, float.Parse(bingresults[i]) 
						*main.MeshSize.y  , i* sizelat/segment.y);
					//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					//tempstr +=bingJsonData.results[i].location.lat.ToString()+","+bingJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据
					tempstr+="/"+(indVertives + i)+","+vertives[indVertives + i].y;
				}

				indVertives ++;//=indVertives+(int)segment.x+1;//+= bingJsonData["results"].Count;/////////
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
	/// 
	/// 
	/// 

}