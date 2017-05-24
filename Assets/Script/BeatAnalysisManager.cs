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


//	GameObject BeatMapContainer;// = new GameObject ();
//	GameObject BeatMapContainer2;// = new GameObject ();
//	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
//	public GameObject BeatPfb;

	/// <summary>
	/// /////////////////////////////////////////
	/// </summary>
	/// 
	/// 
	/// 
//	[SerializeField]
//	AudioClip musicA;
//	[SerializeField]
//	AudioClip musicB;
//	[SerializeField]
//	AudioClip musicC;
//	public  GameObject cubelow;
//	public  GameObject cubemid;
//	public  GameObject cubehigh;

//	public AudioClip[] beatsoundFX;
//	public AudioClip beatsoundDefault;
//	public AudioClip mmmhigh;


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
	_audio.Play ();
	}
	public void StopAudio()
	{
//		bpmsetting = false;
//		lastSetbpmframe=-1;
//		Setbpmframe=-1;
		_audio.Stop ();
	}




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

}
