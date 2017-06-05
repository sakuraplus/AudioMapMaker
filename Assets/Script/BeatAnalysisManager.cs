using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;


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
	public static 	AudioListener _AL;

	public static string AudioName="";
	public static int SpecSize = 256;//采样数量
	public static int bufferSize = 256;//记录的帧数
	public static  int numBands =8;//分频段数量
	public static  float decay = 0.997f;//衰减?
	public static float enegryaddup = 1.2f;

	[SerializeField ]
	int _SpecSize =256;
	[SerializeField ]
	int _bufferSize = 256;
	[SerializeField ]
	int _numBands =8;
	[SerializeField ]
	[Range (0.5f,3)]	 
	float _enegryaddup = 1.2f;
	[SerializeField ]
	float _decay = 0.997f;//衰减?

	public static bool CheckWithInc;//使用增长值或能量值计算节拍
	[SerializeField ]
	bool _checkwithInc =true;//使用增长值或能量值计算节拍
	[SerializeField ]
	  Vector2[] FreqRange;//使用的频率范围，x为低频y为高频

	//Vector2[] _FreqRange;
	[SerializeField ]
	  bool useFreq=false;//是否根据频率范围分段，false则根据对数方式区分高频和低频
	public static int[] DictBandRange;//dictionary存采样数组中每位对应的频段
	public static int[] DictBandlength;//存每个频段的长度，计算平均值用

	//public  static  ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	//public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息

	public  static	List<MusicData > BAL;
	public  static	List<float[]  > MAL;
//	[SerializeField ]
//	int _bandlength = 32;//非实时计算时使用
//	public static int bandlength = 32;//非实时计算时使用

	public float speed=2000;




//	public  string strvariance="";//测试用
//	[TextArea ]
//	public string BeatMapDataJson;
	/// <summary>
	/// ///////////////////////////////////
	/// </summary>
	// Use this for initialization
	void Awake () {
		CheckWithInc  = _checkwithInc;
		//bandlength = _bandlength;
	
		SpecSize = _SpecSize;

		_audio=GetComponent<AudioSource> ();
		AudioName = _audio.name;
		_AL = GetComponent<AudioListener > ();

		BAL=new List<MusicData>();
		MAL=new List<float[]>();
		initDictOfBand ();
	//	FreqRange = _FreqRange;
//		if (BAM  == null)
//		{
//			BAM = this.GetComponent<BeatAnalysisManager >();
//		}else{
//			print("awake BAM--"+BAM);
//		}
		///////////////////////////////



		//_audio.pitch = 2;

		Debug.Log("1"+Time.frameCount );
		Debug.Log("1"+Time.captureFramerate );


	}

	public   void initDictOfBand()
	{
		if (!useFreq) {
			numBands = _numBands;
			//FreqRange.Initialize ();//=new Vector2[0];
		} else {
			numBands = FreqRange.Length;
		}
		bufferSize = _bufferSize;
		decay = _decay;
		enegryaddup = _enegryaddup;


		Debug.Log (">>frequency= " + _audio.clip.frequency + "SampleRate=" + AudioSettings.outputSampleRate);//(int)Mathf.Floor (_SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  
		float freqlength= (SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
		Debug.Log (">> length=" + freqlength);
		string ssss = "";
		DictBandRange = new int[SpecSize ];
		DictBandlength = new int[numBands ];
		for (int i =0; i < SpecSize; i++)
		{	int IndexInBand=-99;
			if(!useFreq){
				//musicenergy += spectrum [i];//总音量和，需要取平均数使用
				IndexInBand =(int) Mathf.Floor (0.5f+Mathf.Log(i+2,SpecSize )*numBands) -1;//对数方式，将频率分为几段
				IndexInBand=Mathf.Clamp(IndexInBand,0,numBands-1 );//限制频段编号范围
				ssss += "/ " + IndexInBand;
			}else{
				for (int ir = 0; ir < FreqRange.Length ; ir++) {
					int FreqToIndex = i * 24000 / SpecSize;
					if (FreqToIndex >= FreqRange [ir].x && FreqToIndex < FreqRange [ir].y) {
						IndexInBand = ir;
						break;
						//Debug.Log ("FreqToIndex= "+FreqToIndex+" ir="+ir);
					}		
				}	
				ssss += "~ " + IndexInBand;
			}
			DictBandRange [i] = IndexInBand;
			//if(IndexInBand<numBands && IndexInBand>=0  ){
			if(IndexInBand<numBands && IndexInBand>=0 && i<freqlength )
			{
				DictBandlength [IndexInBand]++;//计数增加
			}

		}
		Debug.Log (">>> " + ssss);
		string sxs = "";
		for (int iii = 0; iii < numBands; iii++) {
			sxs += " , " + DictBandlength [iii];
		}
		Debug.Log (">>length= "+sxs);
		//_audio.Stop ();
	}
	public void settodefault()
	{
		_bufferSize = 128;
		_numBands = 3;
		_enegryaddup = 1;
		_decay = 0.998f;
		initDictOfBand ();
	}
//	public void StopAudio()
//	{
////		bpmsetting = false;
////		lastSetbpmframe=-1;
////		Setbpmframe=-1;
//		_audio.Stop ();
//	}
//




	//保存json格式化的map


	//根据实时采集到数据 save map
	public  void Save() {  
		savedBeatMap  sbm=new savedBeatMap();
		sbm.MD=new MusicData[BeatAnalysisManager.BAL.Count ] ;


		for (int i = 0; i < BeatAnalysisManager.BAL.Count; i++) {
			//MusicData md =BeatAnalysisManager.BAL [i];
			sbm.MD [i] = BeatAnalysisManager.BAL [i];
		}

		string jsonstr = JsonUtility.ToJson (sbm );
		Debug.Log("ttt="+jsonstr);

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
