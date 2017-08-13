using UnityEngine;
using System.Collections.Generic ;
//using UnityStandardAssets.CrossPlatformInput ;

public class InfiniteMap : MonoBehaviour {

	//测试***
	//public float distanceH = 7f;
	//public float distanceV = 4f;
	//public GameObject [] testObjs=new GameObject[9];
	//*****

	/// <summary>
	/// main.MapObjs The map objects.
	/// </summary>
	GameObject[,] _MapObjs;

	/// <summary>
	/// main.vertives.
	/// </summary>
	 Vector3 [,][] _Vertives;



	//	Dictionary <pos,GameObject > DictMapObj;
	//public Vector2 NumChunk;
	public Vector2Int  NumChunk;
	public  Transform charPos;
	//public static Vector3[,] Vertives;
	//[SerializeField ]
	Vector2Int vpnow;
	//Vector2Int vplast;//测试用
	[SerializeField]
	public  GameObject onchunk;
	float onchunkLat,onchunkLng;
	void Start() {

		_MapObjs = TerrainManager.MapObjs;//new GameObject[(int)NumChunk.x ,(int) NumChunk.y] ;
		_Vertives= TerrainManager.Vertives ;
		NumChunk = TerrainManager.Pieces;
		initMaps();

	}

	void Update() {
//		_MapObjs = main.MapObjs;
		onwhichchunk ();

	}
	List < GameObject> pool;
	void initMaps(){

		int sy =Mathf.FloorToInt( NumChunk .x/2);
		int sx = Mathf.FloorToInt( NumChunk .x/2);
		vpnow = new Vector2Int (sy,sx);
		//vplast = vpnow;//测试用
		_MapObjs = TerrainManager.MapObjs;// new GameObject[(int)NumChunk.x ,(int)NumChunk.y ] ;
		pool = new List<GameObject> ();

	}



	public  void onwhichchunk(){
		
		RaycastHit hit;
		if(Physics.Raycast (charPos.position ,Vector3.down ,out hit ))
		{
			//LayerMask.NameToLayer("Ground")//.transform.parent .gameObject 
			bool objFound = false;
			GameObject g=hit.collider.gameObject;
			do {
				if (g.tag =="Ground" && g.GetComponent<drawJterrain > ()) {
					if(onchunk==g){
						return ;
					}
					onchunk =g;
					onchunkLat=g.GetComponent<drawJterrain>().centerlat ;
					onchunkLng=g.GetComponent<drawJterrain>().centerlng ;
				
					objFound=true;
					break ;
				}
				if(g.transform.parent !=null){
					g=g.transform.parent.gameObject ;
				}else{
					return  ;
				}
			} while(objFound == false);
				
		
			Debug.LogWarning  ("onwhich "+hit.collider.gameObject.name+onchunk.transform.localPosition);
			vpnow = onchunk.GetComponent<drawJterrain> ().Vpos;
			//if (vpnow != vplast) {
			replacechunk (vpnow );
			setpos (onchunk.transform.localPosition );
		}
	}


	/////////////////////////

	void replacechunk(Vector2Int vp){
//		if (vplast == vp) {
//			Debug.Log ("vplast == vp");
//			return;
//		}

		int stepX =(int) vp.y -Mathf.FloorToInt( NumChunk .x/2);
		int stepY = (int)vp.x - Mathf.FloorToInt( NumChunk .y/2);
//		Debug.LogWarning (vp+"/"+vplast +" stepxy="+stepX+","+stepY );
		//vplast = vp;//测试用
		if (stepX> 0) {
			//右,j
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j = 0; j < NumChunk.x; j++) {
					
					if (_MapObjs [i, j] != null) {
						if (j < stepX) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i, j - stepX] = _MapObjs [i, j];
							_MapObjs [i, j - stepX].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i, j - stepX);

							_MapObjs [i, j] = null;
							////////////////////
							_Vertives [i, j - stepX]=_Vertives [i, j];
							_Vertives [i , j] = null;
							////////////////
						}
					}
				}
			}
		}else if(stepX < 0) {
			//左,j
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j =(int)NumChunk.x-1; j >=0 ; j--) {
					
					if (_MapObjs [i, j] != null) {
						if (j >= (NumChunk.x + stepX)) {
							pool.Add (_MapObjs [i, j ]);
						} else {

							_MapObjs [i, j-stepX] = _MapObjs [i, j ];
							_MapObjs [i, j-stepX].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i, j-stepX);
							_MapObjs [i, j ] = null;
							////////////////////
							_Vertives [i, j-stepX]=_Vertives [i,j];
							_Vertives [i , j] = null;
							////////////////
						}
					}
				}
			}
		}
		if (stepY> 0) {
			//down,i
			for (int j = 0; j < NumChunk.x; j++) {
				for (int i = 0; i < NumChunk.y; i++) {
					
					if (_MapObjs [i, j] != null) {
						if (i < stepY) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i - stepY, j] = _MapObjs [i,j];
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i - stepY, j);
							_MapObjs [i, j] = null;
							////////////////////
							_Vertives [i - stepY, j]=_Vertives [i,j];
							_Vertives [i , j] = null;
							////////////////
						}
					}
				}
			}
		}else if(stepY< 0) {
			//up,i
			for (int j = 0; j < NumChunk.x; j++) {
				for (int i =(int)NumChunk.y-1; i >=0 ; i--) {
					
					if (_MapObjs [i, j] != null) {
						if (i >= (NumChunk.y + stepY)) {
							pool.Add (_MapObjs [i , j]);
						} else {

							_MapObjs [i - stepY, j] = _MapObjs [i, j];
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2Int (i - stepY, j);
							_MapObjs [i , j] = null;
							////////////////////
							_Vertives [i - stepY, j]=_Vertives [i,j];
							_Vertives [i , j] = null;
							////////////////
						}
					}
				}
			}
		}

	
		string ttt=     "`MapObjs.name= ";

		Vector2Int  baseChunkPos = onchunk.GetComponent<drawJterrain> ().Vpos;
		//Debug.LogWarning ("baseChunkPos=" + baseChunkPos);
		for (int i = 0; i < NumChunk.y; i++) {
			for (int j = 0; j < NumChunk.x; j++) {
				if (_MapObjs [i, j] == null && pool.Count>0) {
					
					_MapObjs [i, j] = pool [0];
					//_MapObjs [i, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j);
					//////////////////////////////////////***
					/// float newClng = (j - 0) * 2 * main.stepLng+onchunkLng ;
					//float newClat = ( 0-i) * 2 * main.stepLat+onchunkLat ;
				
					float newClng = (j - baseChunkPos.y) * 2 * TerrainManager.stepLng+onchunkLng ;
					float newClat = (baseChunkPos.x - i) * 2 * TerrainManager.stepLat+onchunkLat ;
				//	if (newClat < 80 && newClat > -80) {
						//Debug.LogError  ("set with pool "+pool [0].name+"("+i+","+j+") base="+baseChunkPos  +"lat,lng="+onchunkLat +","+onchunkLng);
						_MapObjs [i, j].GetComponent<drawJterrain> ().loadNewLoc (newClat, newClng, new Vector2Int (i, j));//,main.stepLat,main.stepLng,main.s

						pool.RemoveAt (0);
					//} else {
					//	Debug.LogError ("*** out of 85");
					//}
				}

				ttt += _MapObjs [i, j].name+",";
			}
			ttt+="/";
		}
	//	Debug.Log ( ttt);


	}
	void setpos(Vector3 centerpos){
		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;

		for (int i = 0; i < NumChunk.y; i++) {
			for (int j = 0; j < NumChunk.x; j++) {
				Vector3 nv = new Vector3 (centerpos.x + TerrainManager.MeshSize.x * (j-cx), centerpos.y, centerpos.z +TerrainManager.MeshSize.z * (cy - i));
				_MapObjs [i, j] .transform.position=nv;
			}
			//	ttt+="/";

		}
	}





//	void initMapsTest(){
//		vpnow = new Vector2 (1,1);
//		vplast = vpnow;
//		_MapObjs=new GameObject[(int)NumChunk.x ,(int)NumChunk.y ] ;
//		pool = new List<GameObject> ();
//
//
//		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
//		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;
//		string ttt="";
//		for (int i = 0; i < NumChunk.x; i++) {
//			for (int j = 0; j < NumChunk.y; j++) {
//				Vector3 nv = new	Vector3 (0 + distanceV * (cx - j), 0, 0 -distanceV * (cy - i));
//				_MapObjs [i, j] = testObjs [i*(int)NumChunk.x+j];
//				_MapObjs [i, j] .transform.localPosition =nv;
//				_MapObjs [i, j].GetComponent<drawJterrain > ().Vpos = new Vector2 (i, j);
//				ttt += _MapObjs [i, j].name+nv;
//			}
//			ttt+="/";
//		}
//		Debug.Log (ttt);
//	}
//



}
