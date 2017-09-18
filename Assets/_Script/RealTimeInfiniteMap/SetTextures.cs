using UnityEngine;  


public class SetTextures : MonoBehaviour {


	[SerializeField,Tooltip ("select Random to use random value instead of the real world elevation data")]
	datasource DataSourceOfStaticmap;
	/// <summary>
	/// staticmap key
	/// </summary>HeaderAttribute ("API KEY of staticmap (google/bing)")
	[Tooltip ("input the API Key of appropriate datasource")]
	[SerializeField] string staticmapApiKey ="";
	/// <summary>
	/// 是否使用计算的最大zoom，不使用则获取固定zoom
	/// </summary>
	[SerializeField,Tooltip  ("calculator the zoomlevel automatically")]bool IsAutoZoom;
	/// <summary>
	/// 获取staticmap的固定zoom倍数
	/// </summary>
	[SerializeField,Tooltip ("use a specified zoom level.")]int TextureZoomLevel=1;
	//Some imagery may not be available at all zoom levels for all locations. If imagery is not available at a location, a message is returned in 
	/// <summary>
	/// 贴图放大倍数，google使用，免费用户可以使用2倍，付费用户4倍
	/// </summary>
	[SerializeField,HeaderAttribute ("texture scale(only for google)")]int ImagineScale=1;
	/// <summary>
	/// 贴图最大尺寸，google免费用户支持640*640，bing免费支持2000*1500
	/// </summary>
	[Tooltip  ("The standard size of download texture when calculator the zoomlevel. \nto free API user, between 80 and 1400 (bing),\nbetween 64 and 590 (Google) ")]
	[SerializeField]int ImagineSize=100;
	[SerializeField,Header ("maptype of google or bing staticmap")]
	maptypeG mapTypeGoogle;
	[SerializeField]
	maptypeB mapTypeBing;


	enum maptypeG
	{
		satellite, roadmap, terrain, hybrid 
	}
	enum maptypeB{
		Aerial, Road, CanvasGray, AerialWithLabels
	}






	void Start(){
	}
	void Awake(){


		TerrainManagerStatics.DataSourceSTM = DataSourceOfStaticmap;

		TerrainManagerStatics.STMAPIkey = staticmapApiKey;
	

		if (DataSourceOfStaticmap  == datasource.google) {
			TerrainManagerStatics.MapType = mapTypeGoogle.ToString ();
		} else if (DataSourceOfStaticmap == datasource.bing) {
			TerrainManagerStatics.MapType = mapTypeBing.ToString ();
		} else {
			Debug.LogWarning (DataSourceOfStaticmap+"do not need maptype");
		}


		TerrainManagerStatics.ImagineZoom = Mathf .Clamp( TextureZoomLevel,0,21);
		TerrainManagerStatics.IsAutoZoom = IsAutoZoom;
		TerrainManagerStatics.ImagineScale =Mathf.Clamp( ImagineScale,1,4);
		TerrainManagerStatics.ImagineSize = ImagineSize;


	}



}


