using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
//using 



/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
/// 
public class BeatAnalysisAndRecord : MonoBehaviour {
	[HideInInspector ]
	public static  AudioSource _audio;
	public static string AudioName="";


	public OnBeatrealtimeEventHandler onbeat;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
	public int SpecSize = 256;
	public static int bufferSize = 256;
	public static  int numBands =8;
	[SerializeField ]
	  int _bufferSize = 256;
	[SerializeField ]
	   int _numBands =8;

	float[,] RecAvgInBandInc;
	float[,] RecAvgInBand;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	int beatArrindex=0;
	public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//

	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
	float[] Bands;// = new float[8];//分成8个频段
	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];


	int CurrentIndex=0;//存1024帧中的位置



	public  float  lastAvgInc;
	[Range (0.5f,3)]
	public float enegryaddup = 1.2f;
	int timelast;
	public int timestep;
	[SerializeField ]
	float decay = 0.997f;//衰减?
	[SerializeField ]
	 int _bandlength = 32;//
	public static int bandlength = 32;//

	public float speed=2000;

	[TextArea ]
	public string BeatMapDataJson;

	int outputindex=0;
	void Start () {
		bandlength = _bandlength;
		numBands = _numBands;
		bufferSize = _bufferSize;
		RecAvgInBandInc=new float[bufferSize ,numBands ]; 
		RecAvgInBand=new float[bufferSize ,numBands ]; 
	
		spectrum =new float[SpecSize ];
		Bands=new float[numBands+1 ];//分8个频段

		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 
		_audio=GetComponent<AudioSource> ();
		AudioName = _audio.name;
		Application.targetFrameRate = 10;
	//	_audio.pitch = 2;

	//	Debug.Log(Time.frameCount );
	//	Debug.Log(Time.captureFramerate );


		//_audio = GetComponent<AudioSource>();
		float[] samples = new float[_audio.clip.samples * _audio.clip.channels];
		_audio.clip.GetData(samples, 0);
		Debug.Log ("length="+samples.Length );
		string sttt = "getdata---";
		int i = 0;
		while (i +outputindex < samples.Length && i<100) {
			sttt += samples [i +outputindex ]+",";
			//samples[i +outputindex ] = samples[i +outputindex ] * 0.5F;
			++i;

		}
		outputindex = 100;
		Debug.Log (sttt);
		//aud.clip.SetData(samples, 0);


	}
	public void playmusic()
	{
		_audio.Play ();	
	}

	public void stopmusic()
	{
		_audio.Stop ();
	}
	public void test()
	{
		float[] samples = new float[_audio.clip.samples * _audio.clip.channels];
		_audio.clip.GetData(samples,1);

		string sttt = outputindex+"getdata---";
		int i = 0;
		Vector3 lastp=new Vector3(0,0,0);

		while (i +outputindex < samples.Length && i<200) {
			sttt += ","+samples [i +outputindex ];
			//samples[i +outputindex ] = samples[i +outputindex ] * 0.5F;
			Vector3  thisp=new Vector3(i*2,samples [i +outputindex ]*20,0);
			Debug.DrawLine (lastp, thisp,Color.red );
			lastp = thisp;
			i++;


		}
		outputindex += 200;
		Debug.Log (sttt);
		//aud.clip.SetData(samples, 0);
	}



	public void getdatawhileplay()
	{
		int length = 128;
		float[] spem=new float[length] ;
		_audio.GetSpectrumData (spem, 0, FFTWindow.Rectangular);
		string str="getdatawhileplay spem=";
		for (int i = 0; i < length; i++) {
			str +=">>"+ spem [i];
		}
		Debug.Log (str);

		_audio.GetOutputData (spem, 0);
		str="getdatawhileplay output=";
		for (int i = 0; i < length; i++) {
			str +=">>"+ spem [i];
		}
		Debug.Log (str);
	}
//	void Update()
//	{
//		test ();
//	}
}
