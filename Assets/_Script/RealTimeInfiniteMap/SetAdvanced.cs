using UnityEngine;  


public class SetAdvanced : MonoBehaviour {
	
	[HideInInspector ][SerializeField,Header ("layer index of blocks")]
	public int intlayer;




	[Space(10),HeaderAttribute ("material and color")]
	//[HeaderAttribute ("***** Other Setting *****")]

	[SerializeField, Tooltip ("set a default material if not load texture from staticmap web service")]
	Material defaultMaterial;	
	/// <summary>
	/// 适用于lowpoly风格
	/// </summary>
	[SerializeField,Tooltip  ("Color of vertex (fit for lowpoly style)")]
	Color  defaultMeshcolor;

	[SerializeField,Tooltip  ("lowpoly style")]
	bool LowpolyStyle;

	/// <summary>
	/// 生成使用法线，适用于lowpoly风格
	/// </summary>
	[SerializeField,Tooltip  ("use normal data (fit for lowpoly style)")]
	bool useNormalData;









	void Start(){
	}
	void Awake(){
		TerrainManagerStatics.colorOfMesh = defaultMeshcolor;
		TerrainManagerStatics.matTrr = defaultMaterial;
		TerrainManagerStatics.useNormal = useNormalData;
		TerrainManagerStatics.lowPolyStyle = LowpolyStyle;
		TerrainManagerStatics.layerOfGround.value  = intlayer ;


	}



}


