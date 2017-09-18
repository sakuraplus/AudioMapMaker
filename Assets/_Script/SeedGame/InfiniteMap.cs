/// *****************Sa RTM************
using UnityEngine;
using System.Collections.Generic ;

public class InfiniteMap : MonoBehaviour {


	/// <summary>
	/// main.MapObjs 
	/// </summary>
	GameObject[,] _MapObjs;

	/// <summary>
	///_Vertices all.
	/// </summary>
	Vector3 [,][] _Vertices;


	Vector2Int  numPieces;
	public  Transform charPos;
	public LayerMask LayerOfGround;
	/// <summary>
	///暂存需要重新加载的块 pool.
	/// </summary>
	List < GameObject> pool;
	bool inited=false;

	//[SerializeField ]
	Vector2Int vpnow;
	//[SerializeField]
	public GameObject onPiece;

	float onPieceLat,onPieceLng;
	void Start() {
		Debug.Log ("IFM init"+TerrainManagerStatics.MapObjs+"/"+TerrainManagerStatics.MapObjs.Length );
		initMaps();	
	}

	void Update() {
		if (!inited) {
			initMaps ();
		} 
		if (TerrainManagerStatics.MapObjs.Length<2 ||TerrainManagerStatics.Pieces.x<2||TerrainManagerStatics.Pieces.y<2) {
			//块数<2*2时无效
			return;
		}	
		onwhichPiece ();
	}



	void initMaps(){
		if (TerrainManagerStatics.MapObjs.Length<2 ) {
			Debug.LogWarning ("can not find necessary gameobjects  ");
			return;
		}

		_MapObjs = TerrainManagerStatics.MapObjs;
		_Vertices= TerrainManagerStatics.VerticesAll ;
		numPieces = TerrainManagerStatics.Pieces;

		int sy =Mathf.FloorToInt( numPieces .x/2);
		int sx = Mathf.FloorToInt( numPieces .x/2);
		vpnow = new Vector2Int (sy,sx);

		pool = new List<GameObject> ();

		if (TerrainManagerStatics.Pieces.x<2||TerrainManagerStatics.Pieces.y<2) {
			Debug.LogWarning ("can not find enough gameobjects for infinite map ,you need at least 2*2 blocks to run Infinite Map");
			return;
		}
		inited = true;
	}


	/// <summary>
	/// 检测character在哪个地块上,如果块改变则根据character位置计算需要重新加载的块
	/// </summary>
	public void onwhichPiece(){
		
		RaycastHit hit;
		if(Physics.Raycast (charPos.position ,Vector3.down,out hit,Mathf.Infinity ,LayerOfGround   ))
		{
			GameObject g=hit.collider.gameObject;
	
					if(onPiece==g){
						return ;
					}
					onPiece =g;
					onPieceLat=g.GetComponent<drawJterrain>().centerlat ;
					onPieceLng=g.GetComponent<drawJterrain>().centerlng ;

			Debug.LogWarning  ("onwhich "+hit.collider.gameObject.name+onPiece.transform.localPosition);
			vpnow = onPiece.GetComponent<drawJterrain> ().Vpos;
			replacePieces (vpnow );
			setpos (onPiece.transform.localPosition );
		}
	}


	/// <summary>
	/// 根据当前位置,将character后方的块移动到前方,并重新加载
	/// </summary>
	/// <param name="vp">当前所在的块的vpos</param>
	void replacePieces(Vector2Int vp){

		//将后方的块存入pool
		int stepX =(int) vp.y -Mathf.FloorToInt( numPieces .x/2);
		int stepY = (int)vp.x - Mathf.FloorToInt( numPieces .y/2);

		if (stepX> 0) {
			//右,j
			for (int i = 0; i < numPieces.y; i++) {
				for (int j = 0; j < numPieces.x; j++) {
					
					if (_MapObjs [i, j] != null) {
						if (j < stepX) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i, j - stepX] = _MapObjs [i, j];
							_MapObjs [i, j - stepX].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i, j - stepX);
							_MapObjs [i, j] = null;
							////////////////////
							_Vertices [i, j - stepX]=_Vertices [i, j];
							_Vertices [i , j] = null;
							////////////////
						}
					}
				}
			}
		}else if(stepX < 0) {
			//左,j
			for (int i = 0; i < numPieces.y; i++) {
				for (int j =(int)numPieces.x-1; j >=0 ; j--) {
					
					if (_MapObjs [i, j] != null) {
						if (j >= (numPieces.x + stepX)) {
							pool.Add (_MapObjs [i, j ]);
						} else {

							_MapObjs [i, j-stepX] = _MapObjs [i, j ];
							_MapObjs [i, j-stepX].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i, j-stepX);
							_MapObjs [i, j ] = null;
							////////////////////
							_Vertices [i, j-stepX]=_Vertices [i,j];
							_Vertices [i , j] = null;
							////////////////
						}
					}
				}
			}
		}
		if (stepY> 0) {
			//down,i
			for (int j = 0; j < numPieces.x; j++) {
				for (int i = 0; i < numPieces.y; i++) {
					
					if (_MapObjs [i, j] != null) {
						if (i < stepY) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i - stepY, j] = _MapObjs [i,j];
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i - stepY, j);
							_MapObjs [i, j] = null;
							////////////////////
							_Vertices [i - stepY, j]=_Vertices [i,j];
							_Vertices [i , j] = null;
							////////////////
						}
					}
				}
			}
		}else if(stepY< 0) {
			//up,i
			for (int j = 0; j < numPieces.x; j++) {
				for (int i =(int)numPieces.y-1; i >=0 ; i--) {
					
					if (_MapObjs [i, j] != null) {
						if (i >= (numPieces.y + stepY)) {
							pool.Add (_MapObjs [i , j]);
						} else {

							_MapObjs [i - stepY, j] = _MapObjs [i, j];
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i - stepY, j);
							_MapObjs [i , j] = null;
							////////////////////
							_Vertices [i - stepY, j]=_Vertices [i,j];
							_Vertices [i , j] = null;
							////////////////
						}
					}
				}
			}
		}

		///并使用新坐标重新加载pool中的块
		Vector2Int  basePiecePos = onPiece.GetComponent<drawJterrain> ().Vpos;
		for (int i = 0; i < numPieces.y; i++) {
			for (int j = 0; j < numPieces.x; j++) {
				if (_MapObjs [i, j] == null && pool.Count>0) {
					_MapObjs [i, j] = pool [0];
					float newClng = (j - basePiecePos.y) * 2 * TerrainManagerStatics.stepLng+onPieceLng ;
					float newClat = (basePiecePos.x - i) * 2 * TerrainManagerStatics.stepLat+onPieceLat ;
					_MapObjs [i, j].GetComponent<drawJterrain> ().loadNewLoc (newClat, newClng, new Vector2Int (i, j));

					pool.RemoveAt (0);				
				}
			}
		}
	}

	/// <summary>
	/// 重新设定所有块的坐标
	/// </summary>
	/// <param name="centerpos">Centerpos.</param>
	void setpos(Vector3 centerpos){
		int cx = Mathf.FloorToInt (numPieces.x / 2);// + 1;
		int cy = Mathf.FloorToInt (numPieces.y / 2) ;//+ 1;

		for (int i = 0; i < numPieces.y; i++) {
			for (int j = 0; j < numPieces.x; j++) {
				Vector3 nv = new Vector3 (centerpos.x + TerrainManagerStatics.MeshSize.x * (j-cx), centerpos.y, centerpos.z +TerrainManagerStatics.MeshSize.z * (cy - i));
				_MapObjs [i, j] .transform.position=nv;
			}
		}
	}






}
