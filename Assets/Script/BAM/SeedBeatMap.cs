using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;




/// <summary>
///实时显示节拍
/// 
/// </summary>
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

	//根据实时采集到数据生成map
	GameObject BeatMapContainer;
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	[SerializeField ]
	 float speed=0.01f;
//	[SerializeField ]
//	GameObject BeatPfb;

//	bool playSFX=false;
//	bool showBeatObj=false;
	bool playmap=false;
	//bool beatmapauto=false;
	[SerializeField ]
	GameObject checkobject;
	List<MusicData >  BeatArrayList;//存beat信息
	//ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
//	[SerializeField ]
//	TextAsset[] jsonfileAsset;

	[SerializeField ]
	float offset=0.5f;

	void Start () {

		_audio = BeatAnalysisManager ._audio;
		readytoplay = true;
	}

	// Update is called once per frame
	void Update () {
		if (!playmap) {
			return;
		}
		PlayBeatMap  ();

//		if(Input.GetKeyDown( KeyCode.Z ) ||beatmapauto){
		CheckBeatMap();
//		}
//		else if (beatmapauto) {
//			CheckBeatMap ();
//		}
		if (!_audio.isPlaying) {
			playmap = false;
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
	//	} else {
			Debug.Log ("playmap ");
	//	}
	}
	public  void beatmapToseedMode()
	{
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
	float showtime=5;

	void PlayBeatMap()
	{
		//foreach(MusicData md in  BeatAnalysisManager.BAL) {

		for(int i=0;i<BeatAnalysisManager.BAL.Count ;i++){
			MusicData md = BeatAnalysisManager.BAL [i];
			if (md.playtime <_audio.time -killtime) {
				BeatAnalysisManager.BAL.Remove (md);
				i--;
			} else if (md.playtime > _audio.time+showtime/2 && md.playtime < _audio.time + showtime  ) {
				//md.OnBeat = true;
				Debug.Log("make seed"+md.playtime+" // "+_audio.time );
				DrawBeatMapSeed(md);
				BeatAnalysisManager.BAL.Remove (md);
				//Debug.Log ("4");
				i--;
			}
		}
		//}
	


//		//同步
//		if (CountToSync > 240) {
//			BeatMapContainer.transform.position=new Vector3 ( 0, 0-speed * _audio.time -speed*offset,0);
//			CountToSync = 0;
//			Debug.LogWarning  ("sync");
//		} else {
//			BeatMapContainer.transform.position -= new Vector3 (0, speed * Time.deltaTime, 0);
//		}
//		//Debug.Log (CountToSync);
//		CountToSync++;
	}
	[SerializeField ]
	float showNum=1;

	public void DrawBeatMapSeed(MusicData MD)
	{
		BeatAnalysisManager.BeatmapOffset=offset ;
	//	Debug.Log ("BeatAnalysisManager .BAL.Count" + BeatAnalysisManager.BAL.Count);
		GameObject tarpos = Instantiate (beatObj [MD.BeatPos]) as GameObject;
		tarpos.transform.position = charPos.position ;
		tarpos.transform.localScale = new Vector3 (2, 1, 2);//.y = 10;
		tarpos.transform.parent = BeatMapContainer.transform;
	//	GameObjBeats = new GameObject[BeatAnalysisManager .BAL.Count ];
	//	float [] beattimes=new float[BeatAnalysisManager .BAL.Count ] ;
		for (int i = 0; i < showNum; i++) {
			for (int j = 0; j < MD.Average; j++) {
				GameObject beat = Instantiate (beatObj [MD.BeatPos]) as GameObject;
				if (!beat.GetComponent <Beat > ()) {
					beat.AddComponent <Beat> ();
				}

				beat.GetComponent<Beat> ().Destorytime = MD.playtime+killtime ;
				if (beat.GetComponent<Beat> ().AC == null) {
					beat.GetComponent<Beat> ().AC = beatsoundFX [MD.BeatPos];
				}

				beat.transform.parent = BeatMapContainer.transform;
				beat.transform.localPosition = nextSeedPos (MD,0);
			//	beat.transform.localPosition = nextSeedPos (MD,j);
				// new Vector3 (BeatAnalysisManager.BAL [i].BeatPos * 10, BeatAnalysisManager.BAL [i].playtime * speed, 0);
		
			}
		}

		//GameObjBeats[i]=beat;
		//beattimes [i] = BeatAnalysisManager .BAL[i].playtime;

	}
	public string testtt;
	[SerializeField ]
	Transform  charPos;
	[SerializeField ]
	float angleWind=120;
	[SerializeField ]
	float angleRange=45;
	[SerializeField ]
	float yRange=2;
	Vector3  nextSeedPos(MusicData MD,float offset)
	{
		float R = speed*(MD.playtime - _audio.time + BeatAnalysisManager.BeatmapOffset);
		if (offset > 0) {
			R += speed * offset / 100;
		}
		testtt = "R=" + R + "(" + offset + ")  ";
		float A = angleWind + Random.Range (0,angleRange )-angleRange/2;
		testtt += "A= " + A;
		float newPosX = R * Mathf.Sin (Mathf.Deg2Rad *A);
		float newPosZ = R * Mathf.Sin (Mathf.Deg2Rad * A);
		newPosX += charPos.position.x;
		newPosZ += charPos.position.z;
		float newPosY = charPos.position.y + Random.Range (0, yRange) - yRange / 2;
		testtt += "A= " + A+" Y= "+newPosY ;
		return new Vector3 (newPosX, newPosY, newPosZ);
	}

	//	//按键
	public void CheckBeatMap()
	{
			

		int ic=BeatMapContainer.transform.childCount ;
		for (int i = 0; i < ic; i++) {
			GameObject  b = BeatMapContainer.transform.GetChild (i).gameObject ; //<Beat> ();
			if (b.GetComponent<Beat> ().CheckState||(b.GetComponent<Beat> ().Destorytime<_audio.time )) {
				b.transform.localScale = new Vector3 (10, 1, 1);
			//	Debug.Log (_audio.time +"///"+ b.GetComponent<Beat> ().Destorytime+">  "+(_audio.time -b.GetComponent<Beat> ().Destorytime ));
			//	_audio.PlayOneShot (b.GetComponent<Beat> ().AC);
				b.GetComponent<Beat> ().CheckState = false;
			//	Destroy (b );		
			}
		}
				

	}
}
