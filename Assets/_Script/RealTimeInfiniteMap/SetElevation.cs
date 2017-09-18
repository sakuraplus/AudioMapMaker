using UnityEngine;  

[RequireComponent (typeof( SetTerrains))]
public class SetElevation : MonoBehaviour {
	
	[HeaderAttribute ("Elevation Data")]
	[SerializeField]
	[Tooltip ("select Random to use random value instead of the real world elevation data")]
	datasource DataSource;
	/// <summary>
	/// 高度key
	/// </summary>
	[Tooltip ("input the API Key of appropriate datasource.(bing or google)")]
	[SerializeField]string elevationApiKey;








	void Start(){
	}
	void Awake(){
		#if UNITY_5_0||UNITY_5_1||UNITY_5_2 
		Debug.LogWarning ("The real world elevation terrain will not work correctly below unity 5.3");
		#endif 
		TerrainManagerStatics.DataSource = DataSource;

		TerrainManagerStatics.ELEAPIkey = elevationApiKey;


	}





}


