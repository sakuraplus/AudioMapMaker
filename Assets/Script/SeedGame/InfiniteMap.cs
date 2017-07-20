using UnityEngine;
using System.Collections.Generic ;
//using UnityStandardAssets.CrossPlatformInput ;

public class InfiniteMap : MonoBehaviour {

	//public float distanceH = 7f;
	public float distanceV = 4f;
	public bool cameragroundlimit;
	public GameObject [] objs=new GameObject[9];
	Vector3[] posAddUp=new Vector3[9] ;
	GameObject[,] MapObjs=new GameObject[3,3] ;
//	Dictionary <pos,GameObject > DictMapObj;
	public Vector2 NumChunk;
	public  Transform charPos;

	GameObject onchunk;
	void Start() {
		posAddUp = new Vector3[(int)NumChunk.x * (int)NumChunk.y];
	


		MapObjs=new GameObject[(int)NumChunk.x ,(int) NumChunk.y] ;
		initMaps();
	
	}
	
	void Update() {
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
				if (g.tag =="Ground" && g.GetComponent<chunk > ()) {
					if(onchunk==g){
						return ;
					}
					onchunk =g;
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
			vpnow = onchunk.GetComponent<chunk> ().Vpos;
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
	Vector2 vplast;
	void replacechunk(Vector2 vp){

		Debug.Log(vp+"/"+vplast );
		int stepY =(int) vp.x -Mathf.FloorToInt( NumChunk .x/2);
		int stepX = (int)vp.y - Mathf.FloorToInt( NumChunk .x/2);
		vplast = vp;
		if (stepX> 0) {
			for (int i = 0; i < NumChunk.x; i++) {
				for (int j = 0; j < NumChunk.y; j++) {
					id = new Vector2 (i, j);
					if (MapObjs [i, j] != null) {
						if (j < stepX) {
							pool.Add (MapObjs [i, j]);
						} else {
							MapObjs [i, j - stepX] = MapObjs [i, j];
							MapObjs [i, j - stepX].GetComponent<chunk> ().Vpos = new Vector2 (i, j - stepX);
							MapObjs [i, j] = null;

						}
					}
				}
			}
		}else if(stepX < 0) {
			for (int i = 0; i < NumChunk.x; i++) {
				for (int j =(int)NumChunk.y; j >0 ; j--) {
					id = new Vector2 (i, j);
					if (MapObjs [i, j-1] != null) {
						if (j > (NumChunk.y + stepX)) {
							pool.Add (MapObjs [i, j - 1]);
						} else {

							MapObjs [i, j] = MapObjs [i, j + stepX];
							MapObjs [i, j].GetComponent<chunk> ().Vpos = new Vector2 (i, j);
							MapObjs [i, j + stepX] = null;
						}
					}
				}
			}
		}
		if (stepY> 0) {
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j = 0; j < NumChunk.x; j++) {
					id = new Vector2 (j, i);
					if (MapObjs [j, i] != null) {
						if (j < stepY) {
							pool.Add (MapObjs [j, i]);
						} else {
							MapObjs [j - stepY, i] = MapObjs [j, i];
							MapObjs [j - stepY, i].GetComponent<chunk> ().Vpos = new Vector2 (j - stepY, i);
							MapObjs [j, i] = null;
						}
					}
				}
			}
		}else if(stepY< 0) {
			for (int i = 0; i < NumChunk.y; i++) {
				for (int j =(int)NumChunk.x; j >0 ; j--) {
					id = new Vector2 (j, i);
					if (MapObjs [j-1, i] != null) {
						if (j > (NumChunk.y + stepY)) {
							pool.Add (MapObjs [j - 1, i]);
						} else {

							MapObjs [j, i] = MapObjs [j + stepY, i];
							MapObjs [j, i].GetComponent<chunk> ().Vpos = new Vector2 (j, i);
							MapObjs [j + stepY, i] = null;
						}
					}
				}
			}
		}
		string ttt="";
		for (int i = 0; i < NumChunk.x; i++) {
			for (int j = 0; j < NumChunk.y; j++) {
				if (MapObjs [i, j] == null && pool.Count>0) {
					MapObjs [i, j] = pool [0];
					MapObjs [i, j].GetComponent<chunk> ().Vpos = new Vector2 (i, j);

					pool.RemoveAt (0);
				}
				ttt += MapObjs [i, j].name+",";
			}
			ttt+="/";
		}
		Debug.Log ("ttt=" + ttt);


	}
	void setpos(Vector3 centerpos){
		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;

		//string ttt="";
		for (int i = 0; i < NumChunk.x; i++) {
			for (int j = 0; j < NumChunk.y; j++) {
				Vector3 nv = new	Vector3 (centerpos.x + distanceV * (cx - j), centerpos.y, centerpos.z -distanceV * (cy - i));
				MapObjs [i, j] .transform.position=nv;
				//		ttt += nv;
			}
			//	ttt+="/";

		}
		//Debug.Log (ttt);
	}


	List < GameObject> pool;
	void initMaps(){
		vpnow = new Vector2 (1,1);
		vplast = vpnow;
		MapObjs=new GameObject[(int)NumChunk.x ,(int)NumChunk.y ] ;
		pool = new List<GameObject> ();

	
		int cx = Mathf.FloorToInt (NumChunk.x / 2);// + 1;
		int cy = Mathf.FloorToInt (NumChunk.y / 2) ;//+ 1;
		string ttt="";
		for (int i = 0; i < NumChunk.x; i++) {
			for (int j = 0; j < NumChunk.y; j++) {
				Vector3 nv = new	Vector3 (0 + distanceV * (cx - j), 0, 0 -distanceV * (cy - i));
				MapObjs [i, j] = objs [i*(int)NumChunk.x+j];
				MapObjs [i, j] .transform.localPosition =nv;
				MapObjs [i, j].GetComponent<chunk> ().Vpos = new Vector2 (i, j);
				ttt += MapObjs [i, j].name+nv;
			}
			ttt+="/";
		}
		Debug.Log (ttt);
	}

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
