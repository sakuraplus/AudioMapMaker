﻿using UnityEngine;
using System.Collections.Generic ;
//using UnityStandardAssets.CrossPlatformInput ;

public class InfiniteMap : MonoBehaviour {

	//public float distanceH = 7f;
	public float distanceV = 4f;
	public bool cameragroundlimit;
	public GameObject [] objs=new GameObject[9];
	Vector3[] posAddUp=new Vector3[9] ;
	[SerializeField ]
	GameObject[,] _MapObjs=new GameObject[3,3] ;
	//	Dictionary <pos,GameObject > DictMapObj;
	public Vector2 NumChunk;
	public  Transform charPos;
	//public static Vector3[,] Vertives;
	int numEachChunkx;
	int numEachChunky;
	[SerializeField]
	GameObject onchunk;
	float onchunkLat,onchunkLng;
	void Start() {

		numEachChunkx = (int)main.SegmentInPiece.x;
		numEachChunky =(int) main.SegmentInPiece.y;
		_MapObjs = main.MapObjs;//new GameObject[(int)NumChunk.x ,(int) NumChunk.y] ;
		Debug.Log("_MapObjs-"+_MapObjs.Length +_MapObjs[0,0].name );
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
					onchunkLat=onchunk.GetComponent<drawJterrain>().centerlat ;
					onchunkLat=onchunk.GetComponent<drawJterrain>().centerlng ;
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
		
			Debug.Log ("onwhich "+hit.collider.gameObject.name+onchunk.transform.localPosition);
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
	[SerializeField ]
	Vector2 id;
	Vector2 vpnow;
	Vector2 vplast;//测试用？
	void replacechunk(Vector2 vp){
		if (vplast == vp) {
			Debug.Log ("vplast == vp");
			return;
		}
		Debug.Log(vp+"/"+vplast +_MapObjs[0,0].name);
//		int stepY =(int) vp.x -(int)vplast.x;
//		int stepX = (int)vp.y - (int)vplast.y;
		int stepY =(int) vp.x -Mathf.FloorToInt( NumChunk .x/2);
		int stepX = (int)vp.y - Mathf.FloorToInt( NumChunk .x/2);
		vplast = vp;
		if (stepX> 0) {
			//右
			for (int i = 0; i < NumChunk.x; i++) {
				for (int j = 0; j < NumChunk.y; j++) {
					id = new Vector2 (i, j);
					if (_MapObjs [i, j] != null) {
						if (j < stepX) {
							pool.Add (_MapObjs [i, j]);
						} else {
							_MapObjs [i, j - stepX] = _MapObjs [i, j];
							_MapObjs [i, j - stepX].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j - stepX);
							_MapObjs [i, j] = null;
							////////////////////
							//main.Vertives [
							////////////////
						}
					}
				}
			}
		}else if(stepX < 0) {
			//左
			for (int i = 0; i < NumChunk.x; i++) {
				for (int j =(int)NumChunk.y; j >0 ; j--) {
					id = new Vector2 (i, j);
					if (_MapObjs [i, j-1] != null) {
						if (j > (NumChunk.y + stepX)) {
							pool.Add (_MapObjs [i, j - 1]);
						} else {

							_MapObjs [i, j] = _MapObjs [i, j + stepX];
							_MapObjs [i, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j);
							_MapObjs [i, j + stepX] = null;
						}
					}
				}
			}
		}
		if (stepY> 0) {
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j = 0; j < NumChunk.x; j++) {
					id = new Vector2 (j, i);
					if (_MapObjs [j, i] != null) {
						if (j < stepY) {
							pool.Add (_MapObjs [j, i]);
						} else {
							_MapObjs [j - stepY, i] = _MapObjs [j, i];
							_MapObjs [j - stepY, i].GetComponent<drawJterrain> ().Vpos = new Vector2 (j - stepY, i);
							_MapObjs [j, i] = null;
						}
					}
				}
			}
		}else if(stepY< 0) {
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j =(int)NumChunk.x; j >0 ; j--) {
					id = new Vector2 (j, i);
					if (_MapObjs [j-1, i] != null) {
						if (j > (NumChunk.y + stepY)) {
							pool.Add (_MapObjs [j - 1, i]);
						} else {

							_MapObjs [j, i] = _MapObjs [j + stepY, i];
							_MapObjs [j, i].GetComponent<drawJterrain> ().Vpos = new Vector2 (j, i);
							_MapObjs [j + stepY, i] = null;
						}
					}
				}
			}
		}
		string ttt="";
		Vector2  baseChunkPos = onchunk.GetComponent<drawJterrain> ().Vpos;

		for (int i = 0; i < NumChunk.x; i++) {
			for (int j = 0; j < NumChunk.y; j++) {
				if (_MapObjs [i, j] == null && pool.Count>0) {
					Debug.LogError  ("set with pool "+pool [0].name );
					_MapObjs [i, j] = pool [0];
					//_MapObjs [i, j].GetComponent<drawJterrain> ().Vpos = new Vector2 (i, j);
					//////////////////////////////////////***
					float newClng = (i - baseChunkPos.x) * 2 * main.stepLng+onchunkLng ;
					float newClat = (baseChunkPos.y - j) * 2 * main.stepLat+onchunkLat ;
					_MapObjs [i, j].GetComponent<drawJterrain> ().loadNewLoc(newClat,newClng, new Vector2 (i, j));//,main.stepLat,main.stepLng,main.s

				//	g.GetComponent <drawJterrain>().loadNewLoc(_clat ,_clng ,stepLat ,stepLng,size,new Vector2 (i,j));

					//////////////////////////////////////***

					pool.RemoveAt (0);
				}
				ttt += _MapObjs [i, j].name+",";
			}
			ttt+="/";
		}
		Debug.Log ("ttt=" + ttt);


	}
	void setpos(Vector3 centerpos){
		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;

		//string ttt="";
		for (int i = 0; i < NumChunk.y; i++) {
			for (int j = 0; j < NumChunk.x; j++) {
				Vector3 nv = new	Vector3 (centerpos.x + main.MeshSize.x * (j-cx), centerpos.y, centerpos.z -main.MeshSize.z * (cy - i));
				_MapObjs [i, j] .transform.position=nv;
				//		ttt += nv;
			}
			//	ttt+="/";

		}
		//Debug.Log (ttt);


//		for (int i = 0; i < Pieces.y; i++) {
//			for (int j = 0; j < Pieces.x; j++) {
//				
//				g.transform .Translate(new Vector3((j-offsetx)*MeshSize.x, 0, (offsety -i)*MeshSize.z));
//
//			}
//		}
	}


	List < GameObject> pool;
	void initMaps(){
		int sy =Mathf.FloorToInt( NumChunk .x/2);
		int sx = Mathf.FloorToInt( NumChunk .x/2);
		vpnow = new Vector2 (sy,sx);
		vplast = vpnow;
		_MapObjs = main.MapObjs;// new GameObject[(int)NumChunk.x ,(int)NumChunk.y ] ;
		pool = new List<GameObject> ();


		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;
		string ttt="";
		for (int i = 0; i < NumChunk.x; i++) {
			for (int j = 0; j < NumChunk.y; j++) {
//				Vector3 nv = new	Vector3 (0 + distanceV * (cx - j), 0, 0 -distanceV * (cy - i));
//				_MapObjs [i, j] = objs [i*(int)NumChunk.x+j];
//				_MapObjs [i, j] .transform.localPosition =nv;
//				_MapObjs [i, j].GetComponent<drawJterrain > ().Vpos = new Vector2 (i, j);
				ttt += _MapObjs [i, j].name;
			}
			ttt+="/";
		}
		Debug.Log (ttt);
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


	public void bbbb(int vv){
		Vector2 vx=new Vector2(1,1) ;

		switch (vv){
		case 0:
			vx=new Vector2(0,0);
			break;
		case 1:
			vx=new Vector2(0,1);
			break;
		case 2 :
			vx=new Vector2(0,2);;
			break;
		case 3  :
			vx=new Vector2(1,0);
			break;
		case 4  :
			vx=new Vector2(1,1);
			break;
		case 5 :
			vx=new Vector2(1,2);
			break;
		case 6 :
			vx=new Vector2(2,0);
			break;
		case 7 :
			vx=new Vector2(2,1);
			break;
		case 8:
			vx=new Vector2(2,2);
			break;
		}
		vpnow = vx;
		if (vpnow != vplast) {
			replacechunk (vpnow );
			vplast = new Vector2(1,1);
			setpos (Vector3.zero );// (onchunk.transform.position);
		}

	}


}
