using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;





/// <summary>
///实时显示节拍
/// 
/// </summary>
public class ShowBeatMap : MonoBehaviour {
	
	public AudioClip[] beatsoundFX;
	public AudioClip beatsoundDefault;
	AudioSource _audio;
	public GameObject[] beatObj;
	public  GameObject beatObjDefault;
//	bool playSFX=false;
//	bool showBeatObj=false;
	bool playmap=false;
	bool beatmapauto=true;
	public  GameObject checkobject;
	ArrayList BeatArrayList;//存beat信息
	//ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	[SerializeField ]
	float offset=0.5f;

	void Start () {

		_audio = BeatAnalysisManager ._audio;

	


		if (beatObj.Length > 0) {
			//showBeatObj = true;
			for (int i = 0; i < beatObj.Length; i++) {

				if (beatObj [i] == null) {
					beatObj [i] = beatObjDefault;
				}
			}
		}
		if (beatsoundFX .Length > 0) {
			//playSFX  = true;
			for (int i = 0; i < beatsoundFX.Length; i++) {
				if (beatsoundFX [i] == null) {
					beatsoundFX [i] = beatsoundDefault;
				}
			}
		}
//		Debug.Log("2"+Time.frameCount );
//		Debug.Log("2"+Time.captureFramerate );


	}

	// Update is called once per frame
	void Update () {
		if (!playmap) {
			return;
		}
		PlayBeatMap  ();

		if(Input.GetKeyDown( KeyCode.A) ||beatmapauto){
			CheckBeatMap();
		}
//		else if (beatmapauto) {
//			CheckBeatMap ();
//		}
		if (!_audio.isPlaying) {
			playmap = false;
		}
	}


	//根据实时采集到数据生成map
	GameObject BeatMapContainer;
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	public float speed=2000;
	public GameObject BeatPfb;
	public void DrawBeatMap()
	{
		if (BeatAnalysisManager .BeatArrayList.Count <= 0) {
			return;
		}
		
		initBeatMapContainer ();
		GameObjBeats = new GameObject[BeatAnalysisManager .BeatArrayList.Count ];
		float [] beattimes=new float[BeatAnalysisManager .BeatArrayList.Count ] ;
		for (int i = 0; i < BeatAnalysisManager .BeatArrayList.Count; i++) {
			MusicData md = (MusicData )BeatAnalysisManager .BeatArrayList [i];
			//sbm.MD [i] = md;

			GameObject beat= Instantiate (BeatPfb) as GameObject ;
			beat.GetComponent<Beat> ().Destorytime  = md.playtime;
			beat.transform.parent = BeatMapContainer.transform ;

			beat.transform.position=new Vector3 (md.BeatPos*10,md.playtime*speed,0);
			GameObjBeats[i]=beat;
			beattimes [i] = md.playtime;

		}



	}


	void initBeatMapContainer()
	{
		if (GameObjBeats!=null ) {
			Debug.Log (BeatMapContainer.transform.childCount);
			for (int i = 0; i < GameObjBeats.Length; i++) {
				DestroyImmediate (GameObjBeats [i]);
			}

		} else {

			BeatMapContainer = new GameObject ();
			BeatMapContainer.name="nonrealtime";
			Debug.Log ("new BeatMapContainer");
			//GameObjBeats = new GameObject[BeatArrayList.Count ];

		}


		//savedBeatMap  sbm=new savedBeatMap();
		//sbm.MD=new MusicData[BeatArrayList.Count ] ;


		BeatMapContainer.transform.position = new Vector3 (0,0-speed*offset,0);

	}



	public TextAsset[] TAs;

	string strmusic;

	public void btnChangemusic(int i){
		if (i < TAs .Length) {
			if (TAs  [i] == null) {
				Debug.LogError ("wrong music!");
				return;
			}
			strmusic = TAs [i].text.ToString();
			load (strmusic);
		}
	}
	//从json生成map
	void load(string jsonstr) {  
		savedBeatMap  smdread = JsonUtility.FromJson<savedBeatMap> (jsonstr);
		Debug.Log ("load smdread.md="+smdread.MD );
		//GameObject GameObjBeats = GameObject.Find ("nonrealtime");
		initBeatMapContainer();
		GameObjBeats = new GameObject[smdread.MD.Length ];
		float [] beattimes=new float[smdread.MD.Length ] ;
		for (int i = 0; i < smdread.MD.Length; i++) {
			MusicData md = (MusicData )smdread.MD [i];

			GameObject beat= Instantiate (BeatPfb) as GameObject ;
			beat.GetComponent<Beat> ().Destorytime  = md.playtime;
			beat.transform.parent = BeatMapContainer.transform ;

			beat.transform.position=new Vector3 (md.BeatPos*10,md.playtime*speed,0);
			GameObjBeats[i]=beat;
			beattimes [i] = md.playtime;
		}
	}  
	//end从json生成map




	public void setbeatmapauto(Toggle  t){
		
		beatmapauto = t.isOn;
		if (beatmapauto) {
			t.GetComponentInChildren <Text> ().text = "The beats will check auto";
			checkobject.transform.position = new Vector3 (0,-5f,0);
		} else {
			t.GetComponentInChildren <Text> ().text = "click key 'A' to check beats";
			checkobject.transform.position = new Vector3 (0,0,0);
		}
		Debug.Log (beatmapauto);
	}
	//beatmap下落
	public  void btnPlaymap()
	{
		if(BeatMapContainer!=null){
		//	beatmapauto = true;
		_audio.Play ();
		playmap = true;
		
		}
	}
//	public  void btnPlaymapCheck()
//	{
//		if(BeatMapContainer!=null){
//			beatmapauto = false;
//			_audio.Play ();
//			playmap = true;

//		}
//	}
	int CountToSync=0;//
	void PlayBeatMap()
	{
		
		if (CountToSync > 240) {
			BeatMapContainer.transform.position=new Vector3 ( 0, 0-speed * _audio.time ,0);
			CountToSync = 0;
			Debug.LogWarning  ("sync");
		} else {
			BeatMapContainer.transform.position -= new Vector3 (0, speed * Time.deltaTime, 0);
		}
		//Debug.Log (CountToSync);
		CountToSync++;
	}

	//	//按键
	public void CheckBeatMap()
	{
			
//			Beat[] beats = BeatMapContainer.GetComponentsInChildren <Beat> ();
//			foreach(Beat b in beats){
//				if (b.CheckState) {
//					b.transform.localScale = new Vector3 (10, 1, 1);
//					Debug.Log (_audio.time +"///"+ b.Destorytime+">  "+(_audio.time -b.Destorytime ));
//
//					_audio.PlayOneShot (b.AC);
//					b.CheckState = false;
//					Destroy (b.transform );			
//				}
//			}
		int ic=BeatMapContainer.transform.childCount ;
		for (int i = 0; i < ic; i++) {
			GameObject  b = BeatMapContainer.transform.GetChild (i).gameObject ; //<Beat> ();
			if (b.GetComponent<Beat> ().CheckState) {
				b.transform.localScale = new Vector3 (10, 1, 1);
				Debug.Log (_audio.time +"///"+ b.GetComponent<Beat> ().Destorytime+">  "+(_audio.time -b.GetComponent<Beat> ().Destorytime ));
				_audio.PlayOneShot (b.GetComponent<Beat> ().AC);
				b.GetComponent<Beat> ().CheckState = false;
				Destroy (b );		
			}
		}
				

	}
}
