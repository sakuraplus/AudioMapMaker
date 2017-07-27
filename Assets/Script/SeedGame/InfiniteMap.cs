using UnityEngine;
using System.Collections.Generic ;
//using UnityStandardAssets.CrossPlatformInput ;

public class InfiniteMap : MonoBehaviour {

	//public float distanceH = 7f;
	public float distanceV = 4f;
	public bool cameragroundlimit;
	public GameObject [] objs=new GameObject[9];
	//Vector3[] posAddUp=new Vector3[9] ;
	//[SerializeField ]
	GameObject[,] _MapObjs=new GameObject[3,3] ;
	//List< Vector3 []> _Vertives=new List<Vector3[]> ();
	public static  Vector3 [,][] _Vertives;



	//	Dictionary <pos,GameObject > DictMapObj;
	public Vector2 NumChunk;
	public  Transform charPos;
	//public static Vector3[,] Vertives;

	[SerializeField]
	GameObject onchunk;
	float onchunkLat,onchunkLng;
	void Start() {

		_MapObjs = main.MapObjs;//new GameObject[(int)NumChunk.x ,(int) NumChunk.y] ;
		_Vertives= main.Vertives ;

		//Debug.Log("_MapObjs-"+_MapObjs.Length +_MapObjs[0,0].name );
		NumChunk = main.Pieces;
		initMaps();

	}

	void Update() {
//		_MapObjs = main.MapObjs;
		onwhichchunk ();

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


			//			if(hit.collider.tag =="Ground"){
			//				//Debug.Log ("onwhich "+hit.collider.gameObject.name);
			//				if (hit.collider.gameObject  != onchunk) {
			//
			//
			//			onchunk = hit.collider.gameObject;
		
			Debug.LogWarning  ("onwhich "+hit.collider.gameObject.name+onchunk.transform.localPosition);
			vpnow = onchunk.GetComponent<drawJterrain> ().Vpos;
			//if (vpnow != vplast) {
			replacechunk (vpnow );
			setpos (onchunk.transform.localPosition );

			//}
			//		
			//				}
			//			}
		}
	}


	/////////////////////////
	//[SerializeField ]
	Vector2 vpnow;
	Vector2 vplast;//测试用
	void replacechunk(Vector2 vp){
//		if (vplast == vp) {
//			Debug.Log ("vplast == vp");
//			return;
//		}

//		int stepY =(int) vp.x -(int)vplast.x;
//		int stepX = (int)vp.y - (int)vplast.y;
		int stepX =(int) vp.y -Mathf.FloorToInt( NumChunk .x/2);
		int stepY = (int)vp.x - Mathf.FloorToInt( NumChunk .y/2);
		Debug.LogWarning (vp+"/"+vplast +" stepxy="+stepX+","+stepY );
		vplast = vp;//测试用
		if (stepX> 0) {
			//右,j
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j = 0; j < NumChunk.x; j++) {
					
					if (_MapObjs [i, j] != null) {
						if (j < stepX) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i, j - stepX] = _MapObjs [i, j];
							_MapObjs [i, j - stepX].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j - stepX);

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
							_MapObjs [i, j-stepX].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j-stepX);
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
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i - stepY, j);
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
							_MapObjs [i - stepY, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i - stepY, j);
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
//		string ixix="infmap  main.v=";
//		for (int iii = 0; iii < main.Pieces.y; iii++) {
//			for (int jjj = 0; jjj < main.Pieces.x; jjj++) {
//				if (main.Vertives [iii, jjj] != null) {
//					ixix += " / " + iii + "," + jjj;
//				} else {
//					ixix += " / [ x ]";
//				}
//			}
//			ixix += "\n";
//		}
//		Debug.LogError (ixix);
//		//************
	
		string ttt=     "`MapObjs.name= ";

		Vector2  baseChunkPos = onchunk.GetComponent<drawJterrain> ().Vpos;
		//Debug.LogWarning ("baseChunkPos=" + baseChunkPos);
		for (int i = 0; i < NumChunk.y; i++) {
			for (int j = 0; j < NumChunk.x; j++) {
				if (_MapObjs [i, j] == null && pool.Count>0) {
					
					_MapObjs [i, j] = pool [0];
					//_MapObjs [i, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j);
					//////////////////////////////////////***
					/// float newClng = (j - 0) * 2 * main.stepLng+onchunkLng ;
					//float newClat = ( 0-i) * 2 * main.stepLat+onchunkLat ;
				
					float newClng = (j - baseChunkPos.y) * 2 * main.stepLng+onchunkLng ;
					float newClat = (baseChunkPos.x - i) * 2 * main.stepLat+onchunkLat ;
					//Debug.LogError  ("set with pool "+pool [0].name+"("+i+","+j+") base="+baseChunkPos  +"lat,lng="+onchunkLat +","+onchunkLng);
					_MapObjs [i, j].GetComponent<drawJterrain> ().loadNewLoc(newClat,newClng, new Vector2 (i, j));//,main.stepLat,main.stepLng,main.s

				//	g.GetComponent <drawJterrain>().loadNewLoc(_clat ,_clng ,stepLat ,stepLng,size,new Vector2 (i,j));

					//////////////////////////////////////***

					pool.RemoveAt (0);
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

		//string ttt="";
		for (int i = 0; i < NumChunk.y; i++) {
			for (int j = 0; j < NumChunk.x; j++) {
				Vector3 nv = new Vector3 (centerpos.x + main.MeshSize.x * (j-cx), centerpos.y, centerpos.z +main.MeshSize.z * (cy - i));
				_MapObjs [i, j] .transform.position=nv;
				//		ttt += nv;
			}
			//	ttt+="/";

		}
		//Debug.Log (ttt);
	}


	List < GameObject> pool;
	void initMaps(){

//		main.stepLng = 4;//***
//		main.stepLat = 3;//***

		int sy =Mathf.FloorToInt( NumChunk .x/2);
		int sx = Mathf.FloorToInt( NumChunk .x/2);
		vpnow = new Vector2 (sy,sx);
		//vplast = vpnow;//测试用
		_MapObjs = main.MapObjs;// new GameObject[(int)NumChunk.x ,(int)NumChunk.y ] ;
		pool = new List<GameObject> ();


		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;
		//测试***
//		string ttt="";
//		for (int i = 0; i < NumChunk.y; i++) {
//			for (int j = 0; j < NumChunk.x; j++) {
////				Vector3 nv = new	Vector3 (0 + distanceV * (j-cx), 0, 0 +distanceV * (cy - i));
////				_MapObjs [i, j] = objs [i*(int)(NumChunk.y)+j];
////				_MapObjs [i, j] .transform.localPosition =nv;
////				float newClng = (j - 0) * 2 * main.stepLng+onchunkLng ;
////				float newClat = ( 0-i) * 2 * main.stepLat+onchunkLat ;
////				_MapObjs [i, j].GetComponent<drawJterrain> ().loadNewLoc(newClat,newClng, new Vector2 (i, j));//,main.stepLat,main.stepLng,main.s
//
//	
//				ttt += _MapObjs [i, j].name;
//			}
//			ttt+="/";
//		}
//		Debug.Log (ttt);
		//测试***
	}
//	void initMaps(){
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
//				_MapObjs [i, j] = objs [i*(int)NumChunk.x+j];
//				_MapObjs [i, j] .transform.localPosition =nv;
//				_MapObjs [i, j].GetComponent<drawJterrain > ().Vpos = new Vector2 (i, j);
//				ttt += _MapObjs [i, j].name+nv;
//			}
//			ttt+="/";
//		}
//		Debug.Log (ttt);
//	}




}
