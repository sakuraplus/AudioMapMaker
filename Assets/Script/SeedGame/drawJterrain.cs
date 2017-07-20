﻿using UnityEngine;  
//using UnityEditor;  
using System.Collections.Generic ;  
using System.Collections;  
using System;
using System.Text;
using System.IO;

[Serializable]
public class JsonMapData{
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
	
public class drawJterrain : MonoBehaviour {
	
//	GameObject Player ;

	string  ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?locations="; 
	string ELEKey = main.ELEAPIkey;//google高度api key = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";//需要自己注册！！
	//string STMKey = main.STMAPIkey ;// google static map api key= "//需要自己注册！！
	//;//"AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk"
	string StrWwwData;

    float steplat ;//每次获取高度数据的间隔

	public bool complete=false;
    public string Trrname;
	public  Vector2 Vpos;
    public  float northwestlat;// +-90西北
    public  float northwestlng;//+-180西北
    public  float southeastlat;// +-90东南
    public  float southeastlng;//+-180东南

	Vector2 segment=new Vector2(3,3);//每块分段数量
	
    int indVertives=0;

    public Material diffuseMap;

    public  Vector3[] vertives;
   // private Vector3[] vtest;///////////////////////////


	float sizelat=100;
	float  sizelng=100;
	float additionheight=1;


    private Vector2[] uvs;
	private int[] triangles;
	

	string tempstr="";//打印测试数据用

	//private GameObject terrain;

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

		GetUV();
		GetTriangles();

		DrawTexture ();
    }
	//
	public  void loadNewLoc(float _northwestlat,float _northwestlng, float _southeastlat, float _southeastlng,Vector3 _size)
	{
		complete = false;
		sizelat = _size.z;
		sizelng = _size.x;
		additionheight = _size.y;
		if (_northwestlng > 180) {
			_northwestlng -=360 ;//将超过180度的经度转换为正常值
		}
		if (_southeastlng > 180) {
			_southeastlng -=360;//将超过180度的经度转换为正常值
		}
		northwestlat = _northwestlat;// +-90 西北角纬度
		northwestlng = _northwestlng;//+-180西北角经度
		southeastlat = _southeastlat;// +-90 东南角纬度
		southeastlng = _southeastlng;//+-180 东南角经度
		steplat = ( northwestlat-southeastlat ) / segment.y;//每段跨越的纬度
		//z正方向为北
		//print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);

		StartCoroutine(LoadJson(southeastlat));

	}
  
	List<int> errorSamples = new List<int> ();
	void sampleLerp(){
		for(int i=0;i<errorSamples.Count ;i++) {
			int num = 0;
			float sumSamples = 0;
//			if(
		}
		errorSamples.Clear();
	}
	void saveStatic(){
		for (int i = 0; i <= segment.x; i++) {
			for (int j = 0; i <= segment.y; i++) {
				//main.Vertives[Vpos .x*se+j]=vertives [];
				//			if(
			}
		}
	}
	//加载高度数据，按照纬度方向分段多次加载
    public IEnumerator LoadJson(float lat)
	{  
	   	if (indVertives >= vertives.Length)		  
		{
		   /////////////////
			Debug.LogWarning (Trrname + "Data complete!!!!!!!"+tempstr );
			sampleLerp ();
            DrawMesh();
			yield break;
		}

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
        ipaddress +=lat +","+northwestlng +"|";
        ipaddress += lat  +","+southeastlng ;//获取同一纬度下，东西经度之间的数据
        ipaddress += "&samples=" + (segment.x+1)+"&key=";
        ipaddress +=ELEKey;//需要自己注册！！
		print(Trrname+"--"+ipaddress);
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

//				vertives[indVertives + i]= new Vector3(i*sizelng /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
//					*additionheight , (indVertives / GoogleJsonData.results.Length) * sizelat/segment.y);
				//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
				tempstr +="Ea,En"+vertives[indVertives + i].ToString ();//测试数据

			}

			indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
			lat += steplat;           
			StartCoroutine(LoadJson(lat));  //获取下一纬度，东西经度之间的数据
			StrWwwData = "";  
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text;    
				JsonMapData GoogleJsonData = JsonUtility.FromJson<JsonMapData>(StrWwwData);
				for (int i=0; i < GoogleJsonData.results.Length ; i++)		
                {
					vertives[indVertives + i]= new Vector3(i*sizelng /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
						*additionheight , (indVertives / GoogleJsonData.results.Length) * sizelat/segment.y);
					 //100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据

                }
              
				indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
                lat += steplat;           
                StartCoroutine(LoadJson(lat));  //获取下一纬度，东西经度之间的数据
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


  //
	private void DrawMesh()
	{
		
		Mesh mesh = gameObject .AddComponent<MeshFilter>().mesh;
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
//        terrain.AddComponent<MeshCollider>();
//        terrain.GetComponent<MeshCollider>().sharedMesh = mesh ;
//        terrain.GetComponent<MeshCollider>().convex = true;
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