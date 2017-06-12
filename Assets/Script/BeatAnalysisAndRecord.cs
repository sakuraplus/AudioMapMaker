using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using AForge.Math ; 



/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
/// 
public class BeatAnalysisAndRecord : MonoBehaviour {
	[HideInInspector ]
	public static  AudioSource _audio;
	public static string AudioName="";
	public int BufferSize = 256;//每节拍之间的帧数
	public int numBands =8;//分频段的数量
	public int numSubdivide=1;//细分的段数，针对变化bpm的音乐



	float[,] RecAvgInBandInc;//存分频段每帧增长值
	float[,] RecAvgInBand;//存musicarraylist
	float[] wavelengths;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//

	public OnBeatrealtimeEventHandler onbeat;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
//	public int SpecSize = 256;
//	public static int bufferSize = 256;
//	public static  int numBands =8;
	[SerializeField ]
	  int _bufferSize = 256;
	[SerializeField ]
	   int _numBands =8;

//	float[,] RecAvgInBandInc;
//	float[,] RecAvgInBand;
//	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
//	int beatArrindex=0;
//	public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//

	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
	//float[] Bands;// = new float[8];//分成8个频段
	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];

	FFT fft=new FFT ();
	//int CurrentIndex=0;//存1024帧中的位置

	float[] samples;

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
		//Bands=new float[numBands+1 ];//分8个频段

		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 
		_audio=GetComponent<AudioSource> ();
		AudioName = _audio.name;
		Application.targetFrameRate = 10;
	//	_audio.pitch = 2;

	//	Debug.Log(Time.frameCount );
	//	Debug.Log(Time.captureFramerate );


		//_audio = GetComponent<AudioSource>();
		 samples = new float[_audio.clip.samples * _audio.clip.channels];
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


	int testi=0;
	public void test()
	{
		//samples = new float[_audio.clip.samples * _audio.clip.channels];
		//_audio.clip.GetData(samples, 0);

		string st=testi+" MAL = ";
		if (testi < BeatAnalysisManager.MAL.Count) {
			for (int i = 0; i < BeatAnalysisManager.MAL [testi].Length; i++) {
				st += ">>" + BeatAnalysisManager.MAL [testi] [i];
			}
			Debug.Log (st);
			testi++;
		}
		//aud.clip.SetData(samples, 0);
	}

	float[] spem;//=new float[length] ;

	public void getdatawhileplay()
	{
		Debug.LogError  ("---");
		int length = 512;
		spem=new float[length] ;
		//_audio.GetSpectrumData (spem, 0, FFTWindow.Blackman);
		string str=_audio.timeSamples +"spem=";
//		for (int i = 0; i < length; i++) {
//			str +="\n"+ spem [i];
//		}
//		Debug.Log (str);

		spem=new float[length*2] ;
		_audio.GetOutputData (spem, 0);
		str="output0=";
		for (int i = 0; i < length; i++) {
			str +="\n"+ spem [i];
		}
		Debug.Log (str);
		spem=new float[length*2] ;
		_audio.GetOutputData (spem, 1);
		str="output2=";
		for (int i = 0; i < length; i++) {
			str +="\n"+ spem [i];
		}
		Debug.Log (str);


		Debug.Log ( _audio.timeSamples +" / "+samples.Length +">>"+_audio.clip.channels);
		str="getdata-c1=";
		int startind = _audio.timeSamples * 2;//Mathf.Max (0, _audio.timeSamples*2 - length);
		for (int i = 0; i < length * 2; i++) {
			if (i + startind < samples.Length/2 ) {
				spem[i] = samples [2*(i + startind)+1];
			} else {
				spem [i] = 0;
			}
			str +="\n"+ spem [i];
		}
		Debug.Log (str);

		str="getdata-S=";
		int startind3 = _audio.timeSamples * 2;//Mathf.Max (0, _audio.timeSamples*2 - length);
		for (int i = 0; i < length * 2; i++) {
			if (i + startind3 < samples.Length/2 ) {
				spem[i] = samples [2*(i + startind)];
			} else {
				spem [i] = 0;
			}
			str +="\n"+ spem [i];
		}
		Debug.Log (str);


	}



	public void testfftclass()
	{
	//	FFT.datafilter df=new FFT.datafilter();

		fft.FFTManagerinit (1024,FFT.datafilter.unityspec);

		float [] result = fft.CalNFFT (fft.windowBlackman ( spem));
		string  str="testfft=";
		for (int i = 0; i < result.Length ; i++) {
			str +=">>"+ result [i];
		}
		Debug.Log (str);
	}






	public void testfftAF()
	{
		int length = spem.Length /2;
		//FFTManagerinit (test);
		Complex[] data=new Complex[spem.Length  ] ;
		fft.FFTManagerinit (length*2,FFT.datafilter.unityspec);
		float[] dataf =fft.windowBlackman(spem );
		//float[] dataf={-0.161499f,-0.1580811f,-0.1656189f,-0.1662292f,-0.1711426f,-0.1741943f,-0.1695557f,-0.16922f,-0.1759338f,-0.1733093f,-0.1805115f,-0.1799927f,-0.188385f,-0.1890869f,-0.2252808f,-0.225769f,-0.1920166f,-0.1920166f,-0.06939697f,-0.06695557f,-0.04464722f,-0.03701782f,-0.09243774f,-0.07995605f,-0.05203247f,-0.03964233f,-0.01220703f,-0.00390625f,-0.0234375f,-0.02056885f,-0.0004882813f,-0.004974365f,0.01062012f,0.001312256f,-0.002075195f,-0.007080078f,0.009613037f,0.01419067f,-0.002105713f,0.007232666f,-0.01922607f,-0.01556396f,-0.009063721f,-0.01831055f,-0.01202393f,-0.03405762f,0.02798462f,-0.002960205f,0.1096497f,0.07568359f,0.1078491f,0.07644653f,0.06225586f,0.0340271f,0.06585693f,0.0380249f,0.06488037f,0.03491211f,0.01953125f,-0.01226807f,-0.05203247f,-0.08172607f,-0.1195374f,-0.1438904f,-0.1325684f,-0.1508484f,-0.1199036f,-0.1314697f,-0.1522522f,-0.1585388f,-0.1902161f,-0.1951599f,-0.174469f,-0.1810303f,-0.1566162f,-0.1639709f,-0.1800232f,-0.1847839f,-0.1879578f,-0.1920166f,-0.1622314f,-0.1718445f,-0.1547241f,-0.1699219f,-0.1560669f,-0.1716919f,-0.1422424f,-0.1552124f,-0.1336365f,-0.1428528f,-0.1207886f,-0.1289673f,-0.1054688f,-0.1181641f,-0.08862305f,-0.1082764f,-0.01376343f,-0.03747559f,0.07693481f,0.05737305f,0.08218384f,0.07345581f,0.05670166f,0.05484009f,0.07183838f,0.06832886f,0.08776855f,0.07830811f,0.09533691f,0.07803345f,0.1154785f,0.09100342f,0.1279602f,0.1022644f,0.1197815f,0.1002502f,0.1037292f,0.09182739f,0.07348633f,0.06558228f,-0.003204346f,-0.0100708f,-0.09185791f,-0.100769f,-0.1042786f,-0.1174011f,-0.07949829f,-0.09207153f,-0.108429f,-0.1166382f,-0.145752f,-0.1535645f,-0.1470337f,-0.1577148f,-0.151123f,-0.1645508f,-0.1513672f,-0.1660156f,-0.1339722f,-0.1445618f,-0.1288452f,-0.131012f,-0.1290894f,-0.1246338f,-0.1254272f,-0.1179504f,-0.1611023f,-0.1505127f,-0.2285461f,-0.2125549f,-0.2411804f,-0.2206116f,-0.1985779f,-0.1772461f,-0.1903076f,-0.1705627f,-0.2024231f,-0.1865234f,-0.1827393f,-0.170166f,-0.1757202f,-0.1601257f,-0.1854553f,-0.1644287f,-0.1781311f,-0.1569519f,-0.1860352f,-0.1701965f,-0.1907349f,-0.1838989f,-0.1611328f,-0.1634521f,-0.1530457f,-0.1601257f,-0.1669312f,-0.1724243f,-0.1708374f,-0.1669617f,-0.1792908f,-0.1633606f,-0.1445923f,-0.1244202f,-0.06484985f,-0.05007935f,-0.05075073f,-0.04705811f,-0.0831604f,-0.09042358f,-0.06759644f,-0.0769043f,-0.05401611f,-0.05447388f,-0.08743286f,-0.07495117f,-0.105835f,-0.08377075f,-0.09579468f,-0.07244873f,-0.09078979f,-0.07339478f};
		//float[] dataf={0.1041129f,0.1036635f,0.1052419f,0.1057416f,0.1049703f,0.1060077f,0.1066843f,0.1068769f,0.1069715f,0.106446f,0.1060993f,0.1058643f,0.1052831f,0.1045907f,0.1040952f,0.1033654f,0.1027249f,0.1018707f,0.1002725f,0.09851809f,0.09635115f,0.09444541f,0.09250216f,0.09093487f,0.09013779f,0.08648563f,0.07699686f,0.0684679f,0.06997196f,0.07236649f,0.06776825f,0.06665458f,0.06779321f,0.06811523f,0.06939336f,0.07090615f,0.07112982f,0.0695218f,0.06641477f,0.0634374f,0.06353404f,0.0655506f,0.06681231f,0.06757781f,0.06642818f,0.06414999f,0.06356682f,0.06647063f,0.07204661f,0.07719097f,0.08004987f,0.08211096f,0.08488354f,0.08613461f,0.08472483f,0.0825659f,0.0822165f,0.0832589f,0.08342052f,0.08352762f,0.0838558f,0.08331896f,0.08442751f,9.09E-02f,0.09804964f,9.68E-02f,0.09359796f,0.09928968f,0.1057f,0.1081425f,0.1081396f,0.1051908f,0.1012756f,0.09948105f,0.09987226f,0.1003696f,0.1001419f,0.09833466f,0.09322369f,0.08840115f,0.08784933f,0.09013937f,0.09414241f,0.09655434f,0.09495078f,0.09109037f,0.08713128f,0.08349441f,0.08062419f,0.07912382f,0.07703352f,0.07480041f,0.0753051f,0.07575846f,0.07516395f,0.0727569f,0.06446429f,0.04023963f,-0.01404434f,-0.09771536f,-0.1798149f,-0.2184406f,-0.2098463f,-0.1938514f,-0.1959543f,-0.204425f,-0.2101964f,-0.2120482f,-0.2104314f,-0.2066818f,-0.2008072f,-0.1918499f,-0.1808497f,-0.1714373f,-0.1652101f,-0.1591265f,-0.1503226f,-0.1399147f,-0.1299506f,-0.1201643f,-0.1097554f,-0.1009724f,-0.09630208f,-0.09407067f,-0.09056147f,-0.08570269f,-0.08308703f,-0.08341282f};
		//		float[] dataf={-0.4671015f,-0.4489561f,-0.3506886f,-0.2940752f,-0.3443512f,-0.4261942f,-0.4216127f,-0.3579025f,-0.3367245f,-0.3881326f,-0.4468037f,-0.4519297f,-0.4175656f,-0.4057757f,-0.4490566f,-0.507338f,-0.499439f,-0.4025084f,-0.3418621f,-0.391583f,-0.4290631f,-0.3686579f,-0.2874111f,-0.2791567f,-0.3333546f,-0.3771489f,-0.3679349f,-0.3115831f,-0.2361692f,-0.205928f,-0.2674995f,-0.3523813f,-0.4108945f,-0.4480685f,-0.45561f,-0.4329845f,-0.3998931f,-0.3648664f,-0.326448f,-0.3084148f,-0.3359526f,-0.33668f,-0.2227335f,-0.1375056f,-0.2047283f,-0.2817824f,-0.2117366f,-0.03067099f,0.1325354f,0.2251397f,0.2866853f,0.3419436f,0.3563427f,0.3396175f,0.4052128f,0.5655062f,0.6076923f,0.4897988f,0.3881745f,0.4003267f,0.4486199f,0.4566019f,0.4570129f,0.5007881f,0.5452156f,0.5369127f,0.5236734f,0.535898f,0.5251644f,0.5212737f,0.5734285f,0.6484504f,0.6725816f,0.6145085f,0.5170207f,0.4608375f,0.4909478f,0.5601056f,0.5599791f,0.4437896f,0.3080614f,0.2813075f,0.3916111f,0.5352585f,0.590978f,0.5650865f,0.5587434f,0.6163322f,0.667022f,0.6452882f,0.5881106f,0.5290624f,0.4674966f,0.4610982f,0.4935057f,0.4774015f,0.4313717f,0.4537602f,0.5604827f,0.659514f,0.6754787f,0.6291142f,0.5559838f,0.4669501f,0.4266595f,0.4407771f,0.4143518f,0.3279061f,0.2440245f,0.2008781f,0.1926336f,0.2014508f,0.2089205f,0.1989259f,0.1982872f,0.2744169f,0.387326f,0.3910131f,0.3256539f,0.3157545f,0.3592375f,0.3694392f,0.3150217f,0.2463091f,0.2212083f,0.2382626f,0.2424251f,0.1793114f};
		//.windowBlackman  (spem);

		for (int i = 0; i <dataf.Length; i++) {
			//data [ i].Re  =(double ) Mathf.Abs ( dataf [i]);
			data [ i].Re  =(double )( dataf [i]);
			data [ i].Im   =(double )( dataf [i]);
		}
	
		//Debug.Log ("AF datalength="+data.Length+"  dataf length="+dataf.Length );
		FourierTransform.FFT (data, FourierTransform.Direction.Forward);


		//		CalNFFT(test, data);
		string str="AF RE";
		string sta="AF REabs";
		string sti="AF IM";
		for (int i = 0; i < length; i++) {
			str += ">>"+data  [i].Re ;
			sta += ">>"+Mathf.Abs ( (float)data  [i].Re) ;
			sti += ">>"+data  [i].Im  ;
		}
		Debug.Log (str);
		Debug.Log (sta);
		Debug.Log (sti);
	}
//	void Update()
//	{
//		test ();
//	}




	public void testseparatedata()
	{
		BeatAnalysisManager.MAL.Clear ();
		GetMusicData (1);

		string st="sample = ";
		for (int ii = 0; ii < 50; ii++) {
			st += ">>" + samples  [ii];
		}
		Debug.LogError  (st);

		SeparateData (100,128);
		Debug.Log (BeatAnalysisManager.MAL.Count);

	}

	public void GetMusicData(int channel=0)
	{		
		//获取每帧的数据，取单声道存入samples，channel 获取的声道
		samples=new float[_audio.clip.samples ] ;

		if (channel >= _audio.clip.channels) {
			Debug.Log ("channel out of range");
			channel = _audio.clip.channels - 1;
		}
		if (_audio.clip.channels < 2) {
			_audio.clip.GetData (samples , 0);
			Debug.LogError ("channel<2");
		} else {
			int sourcelength=_audio.clip.samples * _audio.clip.channels;
			int NumChannels=_audio.clip.channels;

			float [] sourcesamples = new float[sourcelength];
			_audio.clip.GetData (sourcesamples , 0);
			for (int i = 0; i < samples.Length ; i ++) {
				samples [i] = sourcesamples [i * NumChannels + channel];
			}
		}
	}

	public void SeparateData(int time=100,int samplesize=128 )
	{	
		//拆分数据，time即采样间隔时间，单位毫秒，samplesize为采样数据数量，取每个间隔分段的钱samplesize位
		int SamplePerFrame = (int )Mathf.Floor ( time*_audio.clip.frequency / 1000);//每帧间隔的数据数量

		//从第一个非0数据开始
		int si=0;
		bool offsetfound = false;
		while (!offsetfound && si<samples.Length ) {
			si++;
			if (samples [si] != 0) {
				offsetfound = true;
			}
		}
		float offset = si * _audio.clip.frequency;/////////////////第一个非0数据所在的时间

		//int startindex =si;
		int NumOfFrame = (int)Mathf.Floor ((samples.Length-si) / SamplePerFrame);//拆分出的数据数量，相当于帧数
		//if(NumOfFrame<100
		//float[,] MusicDataInFreq = new float[NumOfFrame, samplesize/2+1];
		freqlength=(int)Mathf.Floor (samplesize *_audio.clip.frequency/AudioSettings.outputSampleRate  );//有效频率数量
		fft.FFTManagerinit (samplesize*2,FFT.datafilter.unityspec);//初始化fft
		Debug.LogError ("offset="+si+"  =  "+offset +"SamplePerFrame= "+SamplePerFrame);
		for (int i = 0; i < NumOfFrame; i++) {
			//帧数
			float[] separatedData = new float[ samplesize*2];//每次采样的数据数量，为了保证fft结果数量为samplesize，数量取两倍
			int startindex = si+i * SamplePerFrame;//开始采样的编号
			for (int j = 0; j < samplesize*2; j++) {
				//采样samplesize*2次
				if (startindex + j < samples.Length) {
					//编号有效
					separatedData [j] = samples [startindex + j];
				} else {
					//超出范围则置0
					separatedData [j] = 0;
				}
			}
	
			float [] result = fft.CalNFFT (fft.windowBlackman ( separatedData));//将采样数据做fft，长度为samplesize
			//MusicDataInFreq [i,].ToString ();



			if (i < 3) {
				string st = "result = ";
				for (int ii = 0; ii < result.Length; ii++) {
					st += ">>" + result [ii];
				}

				Debug.Log (st);

			}


			float[] bands =new float[_numBands+1];
			bands =CalcCurrentFrameAvg (result);//按频段拆分
			bands [_numBands] = startindex * _audio.clip.frequency;//startindex所在时间，单位为秒	
			BeatAnalysisManager.MAL.Add (bands);//
		
		}

	
	}

	//float[] Bands;// 保存当前帧各个频段能量，最后一位保存当前时间
	int freqlength;

	//计算当前帧的平均值，及各个频段平均值
	float[]   CalcCurrentFrameAvg(float[] spec)
	{	
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个，单声道和立体声似乎没有区别
		float[] _Bands = new float [_numBands + 1];

		//计算平均数及分频段时，只计算有效部分，忽略超出的freqlength//for (int i =0; i < _SpecSize ; i++)
		//遍历数据，根据DictBandlength将每个数据分配搭配对应频段
		for (int i =0; i < freqlength; i++)
		{	
			int ind = BeatAnalysisManager.DictBandRange [i];
			if(ind>=0 && ind <_numBands){
				_Bands[ind]+=spectrum [i];//每个频段的音量和，可能需要求平均数再使用
			}
		}//end遍历数据，根据DictBandlength将每个数据分配搭配对应频段

		//根据每频段计数计算平均值
		for (int i = 0; i < _numBands ; i++) {			
			_Bands [i] /= BeatAnalysisManager.DictBandlength  [i];
		}

		return _Bands;
		//end根据每频段计数计算平均值
		//Bands [_numBands] = _audio.time;//存当前帧的时间
	}
	//end 计算当前帧的平均值，及各个频段平均值


	/////////////////////////////////////////////////////
	/// ////////////////////////
	/// 
	/// 
	/// //////////////////////////////////////////////


	public  void StartAnaBeat () {
		BufferSize = BeatAnalysisManager.bufferSize;
		//		MusicArrayList=BeatAnalysisManager.MusicArrayList ;
		numBands = BeatAnalysisManager .numBands;
		RecAvgInBandInc=new float[BeatAnalysisManager.MAL .Count  ,numBands*2+1 ]; 
		RecAvgInBand=new float[BeatAnalysisManager.MAL .Count  ,numBands *2+1]; 





		bandlength =(int)Mathf.Floor( BufferSize * 1.5f);
		BeatArrayList.Clear ();
		beatArrindex=0;
		Debug.Log (BufferSize+",,"+BeatAnalysisManager.MAL .Count +",,,"+numBands);
		CalcIncrement ();
		CalcWavelength ();
		for (int j = 0; j < numBands; j++) {
			Debug.LogError (j);
			CheckBeatInClip (j);/////////////////////////
		}
		if(beatArrindex < BeatArrayList.Count-1)
		{
			BeatArrayList.RemoveRange (beatArrindex + 1, BeatArrayList.Count - beatArrindex-1);
		}

	}

	void CalcIncrement()
	{//计算并保存增长值，将arraylist存为数组
		for (int j = 0; j <= numBands; j++) {
			for (int i = 0; i < BeatAnalysisManager.MAL .Count; i++) {
				RecAvgInBand[i,j]=BeatAnalysisManager.MAL [i][j];
				if (i > 0 &&j<numBands ) {
					//计算每帧增加值，j=numbands位存储时间，不计算增加值
					RecAvgInBandInc [i, j] = RecAvgInBand [i, j] - RecAvgInBand [i - 1, j];
				}
			}
		}
	}

	void CalcWavelength()
	{		//将完整音乐细分，计算每段波长
		int numS = (int)Mathf.Floor (BeatAnalysisManager.MAL .Count / (bandlength  * 4f));
		numSubdivide = (int)Mathf.Min (numSubdivide, numS);
		Debug.LogError ("bandlength="+bandlength);
		wavelengths = new float[numSubdivide ];
		for (int inddivide = 0; inddivide < numSubdivide; inddivide++) {
			wavelengths [inddivide] = DetectWavelength (inddivide * BeatAnalysisManager.MAL .Count / numSubdivide);//将完整音乐细分，计算每段波长
			Debug.Log ("wavelengths[ ]  " + inddivide + " // " + wavelengths [inddivide]);

		}
	}	//end//将完整音乐细分，计算每段波长


	//

	int DetectWavelength(int index)
	{

		Debug.LogError ("DetectBeatMap     bandlength=buffersize*1.5="+bandlength +"/"+BeatAnalysisManager.MAL .Count+ "index="+index);
		int avgWavelength = 0;//全部频段的平均波长
		int[] avgwavelengths=new int[numBands ];
		for (int i = 0; i < numBands ; i++) {


			int[] peaktimes = new int[4];
			//int index = 0;
			int numPeak = 0; //已经

			int peaktimeindex = index;//0;
			int peaktimelast =index;// 0;//上一个峰值的位置
			do {
				//取4个峰值
				float peakvalue=RecAvgInBand [peaktimelast+1,i];
				peaktimes[numPeak ]=peaktimelast+1;//RecAvgInBand [peaktimelast+1,numBands ];

				for (int ind = peaktimelast; ind <( bandlength+peaktimelast) ; ind++) {
					if(ind+peaktimelast <RecAvgInBand.Length ){
						//在bandlength中找最高值

						float Avginclipframe =RecAvgInBand [ind,i];// as float[];

						if(peakvalue<Avginclipframe)
						{
							peakvalue=Avginclipframe*(bandlength-4)/(decay*bandlength) ;//*衰减

							peaktimes[numPeak ]=ind;//+peaktimelast;//RecAvgInBand[ind+peaktimelast,numBands ] ;
							peaktimeindex=ind;//+peaktimelast;						
						}
					}else{
						break;
					}

				}

				//Debug.LogError ("peaktimes index="+numPeak +" time="+peaktimes[numPeak ]+" value="+peakvalue+" peaktimeindex="+peaktimeindex );
				peaktimelast=peaktimeindex+(int)Mathf.Floor (bandlength/8) ;
				numPeak++;
			} while(numPeak < 4);


			int avgWavelengthInclip =(int)Mathf.Round ( (peaktimes [3] -  peaktimes [0])/3);
			//avgWavelengthInclip /= 3;//单个频段波长
			avgWavelength += avgWavelengthInclip;//计算全部频段平均波长
			avgwavelengths [i] = avgWavelength;//存所有频段的波长
			//	Debug.LogError ("avgWavelengthInclip="+avgWavelengthInclip);
		}

		avgWavelength=(int)Mathf.Round(avgWavelength / numBands);//全部频段平均波长
		//Debug.LogError ("avgWavelength="+avgWavelength);
		if (numBands >= 2) {
			//如果频段较多，则取与平均最接近的频段的波长
			int avgind = 0;
			float avgdiff = Mathf.Abs (avgwavelengths [1] - avgWavelength);
			for (int i = 1; i < numBands; i++) {
				if (avgdiff > Mathf.Abs (avgwavelengths [i] -avgWavelength)) {
					avgdiff = Mathf.Abs (avgwavelengths [i] - avgWavelength);
					avgind = i;
				}
			}
			avgWavelength = avgwavelengths [avgind];//如果频段较多，则取与平均最接近的频段的波长
		}
		Debug.LogError ("avgWavelength final "+index+" >>> "+avgWavelength);
		//	
		return (int)Mathf.Floor ( avgWavelength) ;
	}

	//单频段检测节拍
	void CheckBeatInClip(int indBand)
	{  
		Debug.LogError  ("band start="+indBand+"  BeatArrayList count "+BeatArrayList.Count );
		string temp="";

		int peaktimeindex = 0;
		int peaktimeindexlast = 0;//上一个峰值的位置
		//float peakvalue=RecAvgInBandInc  [0,indBand];//第一个用来比较的值
		float peakvalue=RecAvgInBand  [0,indBand];//第一个用来比较的值
		int startindex = -1;
		int endindex = -1;
		int _wavelength = -1;
		for (int i =5; i < BeatAnalysisManager.MAL .Count-5; i++) {
			float _wavelengthindex=i/(BeatAnalysisManager.MAL .Count/numSubdivide*1f) ;
			_wavelengthindex = Mathf.Clamp (_wavelengthindex, 0, (numSubdivide - 1));
			_wavelength =(int) wavelengths [(int)Mathf.Floor (_wavelengthindex)];


			startindex = (int)Mathf.Max (0,( _wavelength/8+peaktimeindexlast), (i - _wavelength / 2));
			startindex = (int)Mathf.Min (startindex, BeatAnalysisManager.MAL .Count - 1);
			endindex = (int)Mathf.Min ((startindex+_wavelength ),BeatAnalysisManager.MAL .Count );


			int finallength = endindex - startindex;

			temp += i + " ,start=" + startindex + " ,end=" + endindex+" length="+finallength+" wavelength="+_wavelength ;

			//peakvalue=RecAvgInBandInc[startindex ,indBand ];
			peakvalue=RecAvgInBand[startindex ,indBand ];
			for (int ind = 0; (ind < finallength && ind+startindex<BeatAnalysisManager.MAL .Count ); ind++) {
				//float newvalue=RecAvgInBandInc[startindex +ind,indBand ];
				float newvalue=RecAvgInBand[startindex +ind,indBand ];
				if (peakvalue < newvalue) {
					peakvalue = newvalue;
					peaktimeindex = startindex + ind;
				}

				peakvalue *= decay;//衰减
				//outputs [i] =(1-衰减)*(当前-前帧)+outputs [i] *衰减

			}
			temp += " peaktimeindex=" + peaktimeindex;
			if (peaktimeindex - peaktimeindexlast > _wavelength / 4) {
				//保存鼓点信息
				MusicData md = new MusicData();
				//md=
				md.playtime = RecAvgInBand [peaktimeindex, numBands];//_audio.time;
				md.OnBeat = true;
				md.BeatPos = indBand;
				if (beatArrindex < BeatArrayList.Count) {

					(BeatArrayList [beatArrindex]) = md;//.playtime = RecAvgInBand [peaktimeindex, numBands];//_audio.time;

				} else {
					//MusicData 

					BeatArrayList.Add (md);
				}
				beatArrindex++;
				temp += "   beat";
				//end 保存鼓点信息
				peaktimeindexlast = peaktimeindex;
			} else {
				peaktimeindexlast += _wavelength/2;
			}
			temp+="\n";


			if (i < peaktimeindexlast) {
				i = peaktimeindexlast;
				//Debug.LogWarning (">>> i="+i+"  last="+peaktimeindexlast+"from"+startindex+" to "+endindex);
			}

		}
		Debug.LogError  ("band end="+indBand+"  BeatArrayList count "+BeatArrayList.Count +" ////"+temp);
	}
	//end单频段检测节拍













}
