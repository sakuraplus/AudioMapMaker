using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
[System. Serializable]
public class MusicData
{
	public float playtime = 0;
	public float Average = 0;
	public bool OnBeat = false;
	public int BeatPos=-1;
}
[System.Serializable]
public class savedBeatMap{
	public  MusicData[] MD;

}
/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
public class BeatAnalysisManager : MonoBehaviour {
	[HideInInspector ]
	public static  AudioSource _audio;

	public static string AudioName="";
	public static int SpecSize = 256;
	public static int bufferSize = 256;
	public static  int numBands =8;
	public static  float decay = 0.997f;//衰减?
	public static float enegryaddup = 1.2f;
	[SerializeField ]
	int _SpecSize =256;
	[SerializeField ]
	int _bufferSize = 256;
	[SerializeField ]
	int _numBands =8;


	public  static  ArrayList BeatArrayList=new ArrayList() ;//存beat信息
//	int beatArrindex=0;
	public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	[SerializeField ]
	[Range (0.5f,3)]
	//public 
	float _enegryaddup = 1.2f;
	[SerializeField ]
	float _decay = 0.997f;//衰减?

	[SerializeField ]
	int _bandlength = 32;//
	public static int bandlength = 32;//

	public float speed=2000;


	GameObject BeatMapContainer;// = new GameObject ();
	GameObject BeatMapContainer2;// = new GameObject ();
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
//	public GameObject BeatPfb;

	/// <summary>
	/// /////////////////////////////////////////
	/// </summary>
	/// 
	/// 
	/// 
	[SerializeField]
	AudioClip musicA;
	[SerializeField]
	AudioClip musicB;
	[SerializeField]
	AudioClip musicC;
	public  GameObject cubelow;
	public  GameObject cubemid;
	public  GameObject cubehigh;

	public AudioClip[] beatsoundFX;
	public AudioClip beatsoundDefault;
	public AudioClip mmmhigh;


//
//
//	//
//
//	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
//	float[] Bands;// = new float[8];//分成8个频段
//	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];



	//public  GameObject drawline;
	public  float  lastAvgInc;

	int timelast;
	public int timestep;


	public  string strvariance="";//测试用
	[TextArea ]
	public string BeatMapDataJson;
	/// <summary>
	/// ///////////////////////////////////
	/// </summary>
	// Use this for initialization
	void Awake () {
		bandlength = _bandlength;
		numBands = _numBands;
		bufferSize = _bufferSize;
		SpecSize = _SpecSize;
//		spectrum =new float[SpecSize ];
//		Bands=new float[numBands+1 ];//分8个频段,最后一位保存时间
		_audio=GetComponent<AudioSource> ();
		AudioName = _audio.name;
		decay = _decay;
		enegryaddup = _enegryaddup;
		///////////////////////////////

//		RecAvgInBandInc=new float[bufferSize ,numBands ]; 
//		RecAvgInBand=new float[bufferSize ,numBands ]; 
//
//	
//		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 

		//_audio.pitch = 2;

		Debug.Log("1"+Time.frameCount );
		Debug.Log("1"+Time.captureFramerate );


	}
	public void playmusic()
	{
//		if (BeatMapContainer != null) {
//			playmap = true;
//
//		}
//
		_audio.Play ();
	}
	public void StopAudio()
	{
//		bpmsetting = false;
//		lastSetbpmframe=-1;
//		Setbpmframe=-1;
		_audio.Stop ();
	}


//
	//根据实时采集到数据生成map
//	public void DrawBeatMap()
//	{
//		if (GameObjBeats!=null) {
//			Debug.Log (BeatMapContainer.transform.childCount);
//			for (int i = 0; i < GameObjBeats.Length; i++) {
//				DestroyImmediate (GameObjBeats [i]);
//			}
//
//		} else {
//			if (BeatArrayList.Count <= 0) {
//				return;
//			}
//			BeatMapContainer = new GameObject ();
//			//GameObjBeats = new GameObject[BeatArrayList.Count ];
//			BeatMapContainer.transform.position=new Vector3(0,0-speed/100,0);
//		}
//
//		savedBeatMap sbm=new savedBeatMap();
//		sbm.MD=new MusicData[BeatArrayList.Count ] ;
//
//		//BeatMapContainer = new GameObject ();
//		GameObjBeats = new GameObject[BeatArrayList.Count ];
//		BeatMapContainer.transform.position = new Vector3 (0,0-speed/100,0);
//		float [] beattimes=new float[BeatArrayList.Count ] ;
//		for (int i = 0; i < BeatArrayList.Count; i++) {
//			MusicData md = (MusicData )BeatArrayList [i];
//			sbm.MD [i] = md;
//
//			GameObject beat= Instantiate (BeatPfb) as GameObject ;
//			beat.GetComponent<Beat> ().Destorytime  = md.playtime;
//			beat.transform.parent = BeatMapContainer.transform ;
//
//			beat.transform.position=new Vector3 (md.BeatPos*10,md.playtime*speed,0);
//			GameObjBeats[i]=beat;
//			beattimes [i] = md.playtime;
//
//		}
//
//		string ttt = JsonUtility.ToJson (sbm);
//		Debug.Log("ttt="+ttt);
//
//		Save (ttt);
//	}
	//end根据实时采集到数据生成map


	//保存json格式化的map
	void Save(string jsonstr) {  

		if(!Directory.Exists("Assets/save")) {  
			Directory.CreateDirectory("Assets/save");  
		}  
		string filename="Assets/save/"+_audio.clip.name +System.DateTime.Now.ToString ("dd-hh-mm-ss")+".json";
		FileStream file = new FileStream(filename, FileMode.Create);  
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonstr);  
		file.Write(bts,0,bts.Length);  
		if(file != null) {  
			file.Close();  
		}  
	}  
	//end保存json格式化的map
//
//	public  void Btnload() {
//
//		load(BeatMapDataJson) ;
//	}
//
//	//从json生成map
//	void load(string jsonstr) {  
//		savedBeatMap  smdread = JsonUtility.FromJson<savedBeatMap> (jsonstr);
//		Debug.Log ("load smdread.md="+smdread.MD );
//
//		if (GameObjBeats!=null) {
//			Debug.Log (BeatMapContainer.transform.childCount);
//			for (int i = 0; i < GameObjBeats.Length; i++) {
//				DestroyImmediate (GameObjBeats [i]);
//			}
//
//		} else {
//			BeatMapContainer = new GameObject ();
//			GameObjBeats = new GameObject[smdread.MD.Length];
//			BeatMapContainer.transform.position=new Vector3(0,0-speed/100,0);
//		}	
//		float [] beattimes=new float[smdread.MD.Length ] ;
//		for (int i = 0; i < smdread.MD.Length; i++) {
//			MusicData md = (MusicData )smdread.MD [i];
//
//			GameObject beat= Instantiate (BeatPfb) as GameObject ;
//			beat.GetComponent<Beat> ().Destorytime  = md.playtime;
//			beat.transform.parent = BeatMapContainer.transform ;
//
//			beat.transform.position=new Vector3 (md.BeatPos*10,md.playtime*speed,0);
//			GameObjBeats[i]=beat;
//			beattimes [i] = md.playtime;
//		}
//	}  
//	//end从json生成map
//
//
//	//测试用
//	//beatmap下落
//	void PlayBeatMap()
//	{
//		BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
//	}
//	//beatmap下落
//	void PlayBeatMap2()
//	{
//		BeatMapContainer2.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
//	}
//	//按键
//	public void CheckBeatMap()
//	{
//		Debug.Log(MusicArrayList.Count);

//	}
//	//测试用
//	public void playmusicA()
//	{
//		playmap = false;
//		playmap2 = false;
//		MusicArrayList=new ArrayList() ;
//		BeatArrayList = new ArrayList ();
//		//		lastAverage=new float[bufferSize] ;//存前1024帧
//		//		lastAverageInc=new float[bufferSize] ;//存前1024帧
//		//_audio.PlayOneShot( musicA);
//		_audio.clip= musicA;
//	}
//	public void playmusicB()
//	{
//		playmap = false;
//		playmap2 = false;
//		MusicArrayList=new ArrayList() ;
//		BeatArrayList = new ArrayList ();
//		//		lastAverage=new float[bufferSize] ;//存前1024帧
//		//		lastAverageInc=new float[bufferSize] ;//存前1024帧
//		_audio.clip= musicB;
//	}
//	public void playmusicC()
//	{	
//		playmap = false;
//		playmap2 = false;
//		MusicArrayList=new ArrayList() ;
//		BeatArrayList = new ArrayList ();
//		//		lastAverage=new float[bufferSize] ;//存前1024帧
//		//		lastAverageInc=new float[bufferSize] ;//存前1024帧
//		_audio.clip= musicC;		
//	}

}
