using UnityEngine;  
//using UnityEditor;  
using System.Collections.Generic ;  

using System;
using System.Text;
using System.IO;
#if UNITY_5_3||UNITY_5_4||UNITY_5_5||UNITY_5_6
using UnityEngine.Assertions;
#endif 
public  enum datasource
{
	none,random,google,bing
}
public  enum loadState
{
	none,imgComplete,eleComplete,imgError,EleError,imgLoading,EleLoading
}
[Serializable]
public class Vector2Int
{
	public int x=0;
	public int y = 0;

	public Vector2Int(int _x,int _y){
		x = _x;
		y = _y;
	}
	public Vector2Int(float _x,float _y){
		x = (int)_x;
		y = (int)_y;
	}
	public Vector2Int(Vector2 v){
		x = (int)v.x;
		y = (int)v.y;
	}
}
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
