using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab ;



/// <summary>
///实时显示节拍
/// 
/// </summary>
//[System.Serializable]
//public class StartFall : UnityEngine.Events.UnityEvent
//{
//
//} 
[RequireComponent (typeof (BeatAnalysisManager ) )]//
public class SeedBeatMap : MonoBehaviour {
	[HideInInspector ]
	public bool readytoplay=false ;
	[SerializeField ]
	 AudioClip[] beatsoundFX;
	[SerializeField ]
	 AudioClip beatsoundDefault;
	AudioSource _audio;
	[SerializeField ]
	 GameObject[] beatObj;
	[SerializeField ]
	GameObject beatObjDefault;
//	public StartFall _startfall;
	//根据实时采集到数据生成map
	GameObject BeatMapContainer;
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	[SerializeField ]
	 float speed=0.01f;

	bool playmap=false;
	//bool beatmapauto=false;
	[SerializeField ]
	GameObject checkobject;
	List<MusicData >  BeatArrayList;//存beat信息
	//ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	[SerializeField ]
	TextAsset jsonfileAsset;

	[SerializeField ]
	float offset=0.5f;
	[SerializeField ]
	GameObject [] targetposS;

	void Start () {

		_audio = BeatAnalysisManager ._audio;
		readytoplay = true;
		targetVecS=new Vector3[targetposS.Length ] ;

		//initTargetPosS ();

//		PlayFab.ClientModels.GetTitleNewsRequest gtn=new PlayFab.ClientModels.GetTitleNewsRequest() ; 
// 		gtn="POST https://E7D7.playfabapi.com/Client/GetTitleNews Content-Type: application/json X-Authentication: <user_session_ticket_value>
//			{
//  "Count": 25
//}
//
//
//		PlayFabClientAPI.GetTitleNews ( );
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A )) {
			//*******************************
			LoadJsonAndPlay();
			//**********************

		}
		if (Input.GetKey (KeyCode.W)) {
//			
			Cursor.visible=true;
			Cursor.lockState=CursorLockMode.None ;
		}
		if (!playmap) {
			return;
		}
		PlayBeatMap  ();

//		if(Input.GetKeyDown( KeyCode.Z ) ||beatmapauto){
		CheckBeatMap();

		if (!_audio.isPlaying) {
			playmap = false;
			CharacterControllerSeed   CCS = FindObjectOfType<CharacterControllerSeed > ();
			CCS.startFall = true;
			CCS.canmove = false;
			Cursor.visible=true;
			Cursor.lockState=CursorLockMode.None ;
			GetComponent<GameManager> ().LevelCompete ();

		}

	}






	void initBeatMapContainer()
	{
//		if (GameObjBeats != null) {
//			if (GameObjBeats.Length > 0) {
//				Debug.Log (GameObjBeats);
//				Debug.Log (BeatMapContainer.transform.childCount);
//				for (int i = 0; i < GameObjBeats.Length; i++) {
//					DestroyImmediate (GameObjBeats [i]);
//				}
//			}
//
//		} 
		if (BeatMapContainer == null) {

			BeatMapContainer = new GameObject ();
			BeatMapContainer.name = "nonrealtime";
			Debug.Log ("new BeatMapContainer");
	
		}
		
		if (beatObj.Length < BeatAnalysisManager.numBands) {
			GameObject[] beattemp = new GameObject[BeatAnalysisManager.numBands ];//= beatObj;
			beatObj.CopyTo(beattemp ,0);
			beatObj = beattemp;
			//Beat//beat
		}
		if (beatObj.Length > 0) {
			//showBeatObj = true;
			for (int i = 0; i < beatObj.Length; i++) {

				if (beatObj [i] == null) {
					beatObj [i] = beatObjDefault;
				}
			}
		}
		if (beatsoundFX.Length < BeatAnalysisManager.numBands) {
			AudioClip [] beatsfxtemp = new AudioClip[BeatAnalysisManager.numBands ];//= beatObj;
			beatsoundFX.CopyTo(beatsfxtemp ,0);
			beatsoundFX = beatsfxtemp;
			//Beat//beat
		}
		if (beatsoundFX .Length > 0) {
			//playSFX  = true;
			for (int i = 0; i < beatsoundFX.Length; i++) {
				if (beatsoundFX [i] == null) {
					beatsoundFX [i] = beatsoundDefault;
				}
			}
		}


	//	BeatMapContainer.transform.position = new Vector3 (0,0-speed*offset,0);

	}





	public void LoadJsonMap(TextAsset ta ){
		if (ta!= null) {
			load (ta.text.ToString ());
		}
	}
	public void LoadJsonAndPlay(){
		if (BeatAnalysisManager.BAL.Count < 1) {
			Debug.Log ("qq");
			load (jsonfileAsset.text.ToString ());
		}
		beatmapToseedMode ();
		btnPlaymap ();
		GameManager.fullPoint = BeatAnalysisManager.BAL.Count;
		Cursor.visible=false;
		Cursor.lockState=CursorLockMode.Locked ;
		CharacterControllerSeed   CCS = FindObjectOfType<CharacterControllerSeed > ();
		CCS.canmove  = true;//.onBeat.AddListener  (onOnbeatDetected);
		CCS.startFall=  false ;
	}
	void load(string jsonstr) {  
		savedBeatMap  smdread = JsonUtility.FromJson<savedBeatMap> (jsonstr);
		Debug.Log ("load smdread.md="+smdread.MD );
		BeatAnalysisManager.BeatmapOffset  = smdread.offset;
		BeatAnalysisManager.numBands   = smdread.numband ;
		BeatAnalysisManager.BAL.Clear ();
	
		for (int i = 0; i < smdread.MD.Length; i++) {
			BeatAnalysisManager.BAL .Add ( (MusicData )smdread.MD [i]);

		}
	}  
	//end从json生成map



	//beatmap下落
	public  void btnPlaymap()
	{
		//if (BeatMapContainer != null) {
			
			//	beatmapauto = true;
			_audio.Play ();
			playmap = true;
		initBeatMapContainer ();
		speed = GetComponent<CharacterControllerSeed> ().moveSpeed;
	//	} else {
			Debug.Log ("playmap ");
	//	}
	}
	public  void beatmapToseedMode()
	{
		Debug.Log ("to seed mode" + BeatAnalysisManager.BAL.Count); 
		for (int i = 0; i < BeatAnalysisManager.BAL.Count; i++) {
			
			for (int j =  BeatAnalysisManager.BAL.Count-1; j >i; j--) {

				if (Mathf.Abs (BeatAnalysisManager.BAL[j].playtime - BeatAnalysisManager.BAL[i].playtime)<0.1f && (BeatAnalysisManager.BAL[j].BeatPos !=BeatAnalysisManager.BAL[i].BeatPos) ) {
					BeatAnalysisManager.BAL.RemoveAt (j);// (BeatAnalysisManager.BAL[j].playtime);
					BeatAnalysisManager.BAL[i].Average ++;
				} 

			}
		}



	}
//	int CountToSync=0;//
	[SerializeField ]
	float killtime=1;
	[SerializeField ]
	float showtimeMax=5;
	[SerializeField ]
	float showtimeMin=5;
	void PlayBeatMap()
	{
		//foreach(MusicData md in  BeatAnalysisManager.BAL) {

		for(int i=0;i<BeatAnalysisManager.BAL.Count ;i++){
			MusicData md = BeatAnalysisManager.BAL [i];
			if (md.playtime > _audio.time+showtimeMin && md.playtime < _audio.time + showtimeMax  ) {
				//md.OnBeat = true;

				DrawBeatMapSeed(md);
				BeatAnalysisManager.BAL.Remove (md);
				//Debug.Log ("4");
				i--;
			}else if (md.playtime <_audio.time -killtime) {
				BeatAnalysisManager.BAL.Remove (md);
				Debug.Log ("kill one beat"+i+"//"+md.playtime +"<"+_audio.time +"-"+killtime );
				i--;
			} 
		}
		//}
	


	}
	[SerializeField ]
	float showNum=1;

	public void DrawBeatMapSeed(MusicData MD)
	{
		BeatAnalysisManager.BeatmapOffset=offset ;

		for (int i = 0; i < showNum; i++) {
		//	for (int j = 0; j < MD.Average; j++) {
				GameObject beat = Instantiate (beatObj [MD.BeatPos]) as GameObject;
				if (!beat.GetComponent <Beat > ()) {
					beat.AddComponent <Beat> ();
				}

				beat.GetComponent<Beat> ().Beattime = MD.playtime ;
				beat.GetComponent<Beat> ().Borntime  = _audio.time  ;
				if (beat.GetComponent<Beat> ().AC == null) {
					beat.GetComponent<Beat> ().AC = beatsoundFX [MD.BeatPos];
				}

				beat.transform.parent = BeatMapContainer.transform;
				//beat.transform.localPosition = nextSeedPos (MD,0);
			Vector3 TarPos=nextSeedPosS (MD,0);
			if (TarPos != Vector3.zero ) {
				beat.GetComponent<Beat> ().TargPos = TarPos;
				Vector3 stPos = nextSeedStartPos (TarPos);// nextSeedPosS (MD,0);
				beat.transform.position = stPos;//TarPos;
				beat.GetComponent<Beat> ().StartPos = stPos;
			}
			//Debug.LogError ("!");
			//	beat.transform.localPosition = nextSeedPos (MD,j);
				// new Vector3 (BeatAnalysisManager.BAL [i].BeatPos * 10, BeatAnalysisManager.BAL [i].playtime * speed, 0);
		
		//	}
		}

		//GameObjBeats[i]=beat;
		//beattimes [i] = BeatAnalysisManager .BAL[i].playtime;

	}
	public string testtt;
	[SerializeField ]
	Transform  charPos;
//	[SerializeField ]
//	float angleWind=120;
	[SerializeField ]
	float angleRange=45;
	[SerializeField ]
	float yRange=0.2f;


	//***
	[SerializeField ]
	Vector3[] targetVecS;//=new Vector3[] ;

	[SerializeField ]
	GameObject[] goo;
	[SerializeField ]
	float maxHeight=2;
	//***
	public  Vector3  nextSeedPosS(MusicData MD,float offset)
	{
		RaycastHit hit;
		float offsetY=0;
		if(Physics.Raycast (charPos.transform.position  ,Vector3.down ,out hit ))
		{
			if(hit.collider.tag =="Ground"){
				if (hit.distance > maxHeight) {
					offsetY = Mathf.Min  ( hit.distance / maxHeight,1f);// (maxHeight - hit.distance)/hit.distance;// 
				}
			}
		}

		float R = speed *(MD.playtime - _audio.time + BeatAnalysisManager.BeatmapOffset-0.5f);
		if (offset > 0) {
			R += speed * offset / 100;
		}
		//R = 1;
	//	float charA = 0;
		Vector3 charAxis = charPos.forward;


		//**
		List<int> targetVecSInds=new List<int> ();

		string sttt=">>>";
		for(int i=0;i<targetposS.Length;i++){
			targetVecS [i].x =targetposS [i].transform.position.x - charPos.position.x;//targetDri[i];
			targetVecS [i].y =0-offsetY+targetposS [i].transform.position.y - charPos.position.y;;//0;//offsetY+Random.Range(-0.5f,0.5f)-charPos.position.y;//targetposS [i].transform.position.y - charPos.position.y;
			targetVecS [i].z = targetposS [i].transform.position.z - charPos.position.z;
			sttt+=targetVecS[i]+",";

			float rr = Mathf.Sqrt (targetVecS [i].x * targetVecS [i].x + targetVecS [i].z * targetVecS [i].z+targetVecS[i].y*targetVecS[i].y);
			//y=0，高度不变
			//+targetVecS[i].y*targetVecS[i].y);
			if (rr != 0) {
				targetVecS[i].x *=  R / rr;
				targetVecS [i].y *=  R / rr;
				targetVecS[i].z *=  R / rr;
			}
			targetVecS [i] += charPos.transform.position;
			if (Physics.Raycast (targetVecS [i], Vector3.down, out hit)) {
				//on ground
				if (hit.collider.tag == "Ground") {
					Debug.LogWarning ("add " + i+">>"+targetVecS [i].y+","+hit.point.y);
					targetVecS [i].y = Mathf.Min  (targetVecS [i].y, hit.point.y + maxHeight);
				
					targetVecSInds.Add (i);
					goo [i].transform.position = targetVecS [i];
				}
			} else {
				//Debug.Log ("---underground  "+targetVecS[i]);
			}


			//goo [i].transform.position.y = Mathf.Clamp (goo [i].transform.position.y, charPos.transform.position.y-0.5f,charPos.transform.position.y+0.5f);
		}
		if (targetVecSInds.Count > 0) {
			int ind = Mathf.FloorToInt (Random.Range (0, targetVecSInds.Count));
			ind = targetVecSInds [ind];
			Debug.Log (sttt);
			Vector3 newpos = charPos.transform.position + targetVecS [ind];
			return targetVecS [ind];
		} else {
			Debug.LogWarning ("no! "+targetVecSInds.Count );
			return Vector3.zero;
		}
	}

	Vector3  nextSeedStartPos(Vector3 _tarPos)
	{
		RaycastHit hit;

		if (Physics.Raycast (_tarPos, Vector3.down, out hit)) {
			if (hit.collider.tag == "Ground") {
				//offsetY = charPos.transform.position.y - hit.distance+targetFormGround;
				_tarPos = hit.point;
				//if(hit.distance 

			}
		} else {
			Debug.LogError ("underground");
			_tarPos.y = charPos.transform.position .y;
		}
		return _tarPos;
	}

	Vector3  nextSeedPos(MusicData MD,float offset)
	{
		float R = speed*(MD.playtime - _audio.time + BeatAnalysisManager.BeatmapOffset-0.5f);
		if (offset > 0) {
			R += speed * offset / 100;
		}

		//	float charA = 0;
		Vector3 charAxis = charPos.forward;


		/////////////////////////****
		// float newX=charAxis.x* Random.Range (0.7f,2f);
		float newX=charAxis.x* Random.Range (1-angleRange,1+angleRange);
		float newY= Random.Range (0-yRange,yRange);
		//float newZ=charAxis.z* Random.Range (0.7f,2f);
		float newZ=charAxis.z* Random.Range (1-angleRange,1+angleRange);
		float r = Mathf.Sqrt (newX*newX+newZ*newZ+newY*newY);
		if (r != 0) {
			newX = newX * R / r;
			newY = newY * R / r;
			newZ = newZ * R / r;
		}
		///////////////////////////////////////////////////
		//	Vector3 newPos=new Vector3 (newPosX, newPosY, newPosZ);
		Vector3 newPos=new Vector3 (newX+charPos.position.x, newY+charPos.position.y, newZ+charPos.position.z);
		//		if(charAxis.z/newlengthZ<0||charAxis.x/newlengthX<0)
		//		{
		//			Debug.LogError  ("wrong D "+charAxis+"/"+newlengthX+","+newlengthZ);
		//		}
		newPos = groundLimit (newPos);


		return newPos;//new Vector3 (newPosX, newPosY, newPosZ);
	}


	Vector3 groundLimit(Vector3 _nextpos){
		Vector3 oldpos = _nextpos;
		RaycastHit hit;
		if(Physics.Raycast (_nextpos ,Vector3.up ,out hit )){
			if(hit.collider.tag =="Ground"){
				_nextpos.y = hit.point.y + 3;
				
				Debug.LogError("rD "+hit.distance+"//hitp= "+hit.point+"//old= "+oldpos+"//new= "+_nextpos+"//"+hit.collider.name  );
			}
		}else if(Physics.Raycast (_nextpos ,Vector3.down ,out hit ))
		{
			if(hit.collider.tag =="Ground"){
				if (_nextpos.y-2 <hit.point.y ) {
					// = new Vector3 (_nextpos.x, _nextpos.y, _nextpos.z);
					_nextpos.y = hit.point.y +3f;//方法需要根据体验效果调整****
					Debug.LogError ("rIN "+hit.distance+"//hitp= "+hit.point+"//old= "+oldpos+"//new= "+_nextpos+"//"+hit.collider.name  );
				}
			}
		}
		return _nextpos;

	}
	//	//按键
	public void CheckBeatMap()
	{
			
	
		int ic=BeatMapContainer.transform.childCount ;
		for (int i = 0; i < ic; i++) {
			GameObject  b = BeatMapContainer.transform.GetChild (i).gameObject ; //<Beat> ();
			if (b.GetComponent<Beat> ().Beattime<_audio.time*1.2f) {
			//	b.transform.localScale = new Vector3 (0.5f, 0.4f, 0.4f);
					
			}
			if (b.GetComponent<Beat> ().CheckState||(b.GetComponent<Beat> ().Beattime<_audio.time-killtime )) {
				//b.transform.localScale = new Vector3 (4, 4, 4);
			//	Debug.Log (_audio.time +"///"+ b.GetComponent<Beat> ().Destorytime+">  "+(_audio.time -b.GetComponent<Beat> ().Destorytime ));
			//	_audio.PlayOneShot (b.GetComponent<Beat> ().AC);
				b.GetComponent<Beat> ().CheckState = false;
				Destroy (b );		
			}
		}
				

	}




//	Vector3  nextSeedPos(MusicData MD,float offset)
//	{
//		float R = speed*(MD.playtime - _audio.time + BeatAnalysisManager.BeatmapOffset-0.5f);
//		if (offset > 0) {
//			R += speed * offset / 100;
//		}
//
//		//	float charA = 0;
//		Vector3 charAxis = charPos.forward;
//
//
//		/////////////////////////****
//		// float newX=charAxis.x* Random.Range (0.7f,2f);
//		float newX=charAxis.x* Random.Range (1-angleRange,1+angleRange);
//		float newY= Random.Range (0-yRange,yRange);
//		//float newZ=charAxis.z* Random.Range (0.7f,2f);
//		float newZ=charAxis.z* Random.Range (1-angleRange,1+angleRange);
//		float r = Mathf.Sqrt (newX*newX+newZ*newZ+newY*newY);
//		if (r != 0) {
//			newX = newX * R / r;
//			newY = newY * R / r;
//			newZ = newZ * R / r;
//		}
//
//		/// 
//		/// //////////////////
//		//		charPos.localRotation.ToAngleAxis (out charA, out charAxis);
//		//		if (charAxis.x >= 0 && charAxis.z >= 0 && charAxis.y < 0) {
//		//			charA = 360 - charA;
//		//
//		//		}
//		//		float A = angleWind - charA;
//		//		if (A > 180) {
//		//			A = A-360;
//		//		} else if (A < -180) {
//		//			A = A + 360;
//		//		}
//		//		A = Mathf.Clamp (A / 5, -1, 1);
//		//
//		//		A+=charA+ Random.Range (0,angleRange )-angleRange/2;//风向范围
//		//		
//		//		float newlengthX = R * Mathf.Sin (Mathf.Deg2Rad *A);
//		//		float newlengthZ = R * Mathf.Cos  (Mathf.Deg2Rad * A);
//		//		float	newPosX = newlengthX +charPos.position.x;
//		//		float	newPosZ=newlengthZ+ charPos.position.z;
//		//		float newPosY = charPos.position.y + Random.Range (0, yRange) - yRange / 2;
//		//		
//		//		Debug.LogWarning ("make seed time="+MD.playtime+" // "+_audio.time +"pos="+new Vector3 (newPosX, newPosY, newPosZ)
//		//			+"//"+charPos .position +" A="+A+"//CA="+charA+"R="+R );
//		///////////////////////////////////////////////////
//		//	Vector3 newPos=new Vector3 (newPosX, newPosY, newPosZ);
//		Vector3 newPos=new Vector3 (newX+charPos.position.x, newY+charPos.position.y, newZ+charPos.position.z);
//		//		if(charAxis.z/newlengthZ<0||charAxis.x/newlengthX<0)
//		//		{
//		//			Debug.LogError  ("wrong D "+charAxis+"/"+newlengthX+","+newlengthZ);
//		//		}
//		newPos = groundLimit (newPos);
//
//
//		return newPos;//new Vector3 (newPosX, newPosY, newPosZ);
//	}
//
}
