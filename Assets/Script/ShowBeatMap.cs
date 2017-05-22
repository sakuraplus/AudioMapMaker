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
	bool playSFX=false;
	bool showBeatObj=false;

//	float[] gameobjScale;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	//ArrayList MusicArrayList=new ArrayList() ;//存音乐信息


	void Start () {

		_audio = BeatAnalysisManager._audio;
	
		BeatArrayList = BeatAnalysisManager.BeatArrayList;
	
//		gameobjScale=new float[beatObj.Length ];

		if (beatObj.Length > 0) {
			showBeatObj = true;
			for (int i = 0; i < beatObj.Length; i++) {
//				gameobjScale [i] = 0;
				if (beatObj [i] == null) {
					beatObj [i] = beatObjDefault;
				}
			}
		}
		if (beatsoundFX .Length > 0) {
			playSFX  = true;
			for (int i = 0; i < beatsoundFX.Length; i++) {
				if (beatsoundFX [i] == null) {
					beatsoundFX [i] = beatsoundDefault;
				}
			}
		}
//		Debug.Log("2"+Time.frameCount );
//		Debug.Log("2"+Time.captureFramerate );


	}
	void onOnbeatDetected(int i){
//		if (i < beatsoundFX.Length && playSFX ) {
//			_audio.PlayOneShot (beatsoundFX[i]);
//		}
//		if (i < beatObj.Length && showBeatObj ) {
//			gameobjScale [i] = 10;
//		}
	}
	// Update is called once per frame
	void Update () {
//		for (int i = 0; i < beatObj.Length; i++) {
//			beatObj [i].transform.localScale = new Vector3 (gameobjScale [i], gameobjScale [i], gameobjScale [i]);
//			gameobjScale [i] -= gameobjScale [i] / 5;
//		}
	
	}


	//根据实时采集到数据生成map
	GameObject BeatMapContainer;
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	public float speed=2000;
	public GameObject BeatPfb;
	public void DrawBeatMap()
	{

		if (GameObjBeats!=null) {
			Debug.Log (BeatMapContainer.transform.childCount);
			for (int i = 0; i < GameObjBeats.Length; i++) {
				DestroyImmediate (GameObjBeats [i]);
			}

		} else {
			if (BeatArrayList.Count <= 0) {
				return;
			}
			BeatMapContainer = new GameObject ();
			BeatMapContainer.name="nonrealtime";

			//GameObjBeats = new GameObject[BeatArrayList.Count ];
			BeatMapContainer.transform.position=new Vector3(50,0-speed/100,0);
		}

		//savedBeatMap sbm=new savedBeatMap();
		savedBeatMap  sbm=new savedBeatMap();
		sbm.MD=new MusicData[BeatArrayList.Count ] ;

		GameObjBeats = new GameObject[BeatArrayList.Count ];
		BeatMapContainer.transform.position = new Vector3 (0,0-speed/100,0);
		float [] beattimes=new float[BeatArrayList.Count ] ;
		for (int i = 0; i < BeatArrayList.Count; i++) {
			MusicData md = (MusicData )BeatArrayList [i];
			sbm.MD [i] = md;

			GameObject beat= Instantiate (BeatPfb) as GameObject ;
			beat.GetComponent<Beat> ().Destorytime  = md.playtime;
			beat.transform.parent = BeatMapContainer.transform ;

			beat.transform.position=new Vector3 (md.BeatPos*10,md.playtime*speed,0);
			GameObjBeats[i]=beat;
			beattimes [i] = md.playtime;

		}


		string ttt = JsonUtility.ToJson (sbm );
		Debug.Log("ttt="+ttt);

		Save (ttt);
	}

	//保存json格式化的map
	void Save(string jsonstr) {  

		if(!Directory.Exists("Assets/save")) {  
			Directory.CreateDirectory("Assets/save");  
		}  
		string filename="Assets/save/NonRT"+BeatAnalysisRealtime.AudioName  +System.DateTime.Now.ToString ("dd-hh-mm-ss")+".json";
		FileStream file = new FileStream(filename, FileMode.Create);  
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonstr);  
		file.Write(bts,0,bts.Length);  
		if(file != null) {  
			file.Close();  
		}  
	} 


}
