﻿using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;




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
		_audio.pitch = 2;

		Debug.Log(Time.frameCount );
		Debug.Log(Time.captureFramerate );


	}

//	// Update is called once per frame
//	void Update () {		
//
//
//
//		//测试用
//
//
//		if(Input.GetKeyDown (KeyCode.S   )){
//
//			//_audio.Play ();
//			Debug.Log(MusicArrayList.Count);
//			//DetectBeatMap ();
//		}
//		//end测试用
//
//	
//	
//	}
	void recordmusicdata()
	{
	////////////////////////////////////////
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		//CurrentFrameAvg =
		CalcCurrentFrameAvg (spectrum);// musicenergy;

		recordAvgInBand ();
		//CheckBeat ();
		CheckBeatInBand ();
		recordAllData();

	}

	void recordAllData(){
		float[] ff = new float[numBands+1] ;//clips;
		ff=Bands;
		MusicArrayList.Add (ff);
		//////////////
		string s="";
		float[] f = MusicArrayList [MusicArrayList.Count - 1] as float[];
		for (int i = 0; i <= numBands; i++) {
			s += "~" + f [i];
		}
		print ("s="+s);
	}


	//计算当前帧的平均值，及各个频段平均值
	void   CalcCurrentFrameAvg(float[] spec)
	{		
		
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个，单声道和立体声似乎没有区别
		int freqlength=(int)Mathf.Floor (SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );

		int[] ArrBandlength = new int[numBands ];//存每个频段的数据数量

		for (int i = 0; i <numBands; i++) {
			//初始化各频段和各频段的数据数量
			//clips [i] =0;
			ArrBandlength [i] = 0;
		}
		Bands = new float [numBands + 1];

		//计算平均数及分频段时，只计算有效部分，忽略超出的freqlength
		for (int i =0; i < freqlength; i++)
		{
			//musicenergy += spectrum [i];//总音量和，需要取平均数使用
			int icic =(int) Mathf.Floor (0.5f+Mathf.Log(i+2,freqlength )*numBands) -1;//对数方式，将频率分为几段
			icic=Mathf.Clamp(icic,0,numBands-1 );//限制频段编号范围
			//Debug.Log  ("icic="+icic);;
			Bands [icic] += spectrum [i];//每个频段的音量和，可能需要求平均数再使用
			ArrBandlength[icic]++;//计数增加

		}
		for (int i = 0; i < numBands ; i++) {
			//根据每频段计数计算平均值
			Bands [i] /= ArrBandlength [i];
		}
		Bands [numBands] = _audio.time;//存当前帧的时间
	
	}
	//end 计算当前帧的平均值，及各个频段平均值


	//记录各频段前时间段的平均值RecAvgInClip，及增长值RecAvgInClipInc
	void  recordAvgInBand()
	{
		if (CurrentIndex < bufferSize) {
			//1024帧之前。直接存入数组
			for (int i = 0; i < numBands ; i++) {
				//分频段存入，各频段记录平均值
				RecAvgInBand [CurrentIndex,i] = Bands[i];

				//分频段存，各频段平均值增长值
				if (CurrentIndex == 0) {
					RecAvgInBandInc [0, i] = 0;//第一次数据的增长值为0
				} else {
					RecAvgInBandInc [CurrentIndex, i] = Bands [i] - RecAvgInBand [CurrentIndex- 1,i];//根据前一帧各频段平均值计算增长值
				}
			}

		} else { 
			//计数超过buffersize后，所有数据前移一位
			for (int indroll = 0; indroll < bufferSize-1; indroll++) {
				for (int i = 0; i < numBands ; i++) {
					RecAvgInBand [indroll,i] =  RecAvgInBand [indroll + 1,i];
					RecAvgInBandInc [indroll,i] =  RecAvgInBandInc [indroll + 1,i];
				}
			}
			//最后一位为最新一帧的平均值及增长值
			for (int i = 0; i < numBands ; i++) {
				RecAvgInBand [bufferSize-1,i] =  Bands[i];
				RecAvgInBandInc [bufferSize - 1,i] = Bands [i] - RecAvgInBand [bufferSize  -2,i];// 
			}
			//Debug.DrawLine(new Vector3 (1,0,0),new Vector3 (1,100*lastAverageInc [bufferSize - 1],0));
		}
		//没填满buffersize则计数增加
		if (CurrentIndex < bufferSize) {
			CurrentIndex++;
		}	
	}
	//end记录各频段前时间段的平均值RecAvgInClip，及增长值RecAvgInClipInc














	/// <summary>
	/// ////////////////////////分频段检测是否为节拍
	/// </summary>



	//分频段，实时检测节拍
	void CheckBeatInBand()
	{  
		 //strvariance="";

		if (CurrentIndex < bufferSize) {
			return;
		}

		for (int ic = 0; ic < numBands; ic++) {
			//遍历所有频段
			int largeindex = 0;
//			int largeindexF = 0;
			//int lastAverageInc = 0;
			float largeenergy = RecAvgInBandInc  [lastbeatindexInBand[ic],ic];//当前频段上一次节拍的位置
			//strvariance+=">>"+Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2)+"////";
			float variance=0;//方差
			for (int i = lastbeatindexInBand[ic]+1; i < CurrentIndex-2; i++) {
				//遍历当前频段，上一节拍至当前帧的所有平均值增量
				if (RecAvgInBandInc [i,ic] > largeenergy ) {
					
//					largeindexF = largeindex;
					largeindex = i;//最大值在buffersize中所在的位置
					largeenergy = RecAvgInBandInc [i,ic] ;//最大

				}//获取平均数增量最大值，及最大值在buffersize中所在的位置
				variance += Mathf.Pow (RecAvgInBandInc [i, ic], 2);
				largeenergy *= decay;//衰减
				variance /= CurrentIndex - lastbeatindexInBand [ic] - 1;//上一个节拍至当前节拍之前的节拍，当前频段，音量的方差

			}//end遍历当前频段，上一节拍至当前帧的所有平均值增量，获取最大值及其序号

			float tempVarInc;//计算之前帧的方差与当前帧平方的关系 
			if (variance != 0) {
				tempVarInc = Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2) / variance;//当前帧增量的平方/当前之前的方差

			} else {
				tempVarInc = 1.1f;//Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2);
			}
			tempVarInc = 0.5f + tempVarInc / 5000;
			tempVarInc = Mathf.Clamp (tempVarInc, 0.5f, 1.2f);
		

			//strvariance+=variance+",";//显示方差


			//判断节拍///////////////////////
			float beatsincelast = CurrentIndex - lastbeatindexInBand[ic];//当前频段与上一节拍之间的帧数
			if (beatsincelast > bufferSize / 2) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//&& bufferSize-largeindexF<4
				if (RecAvgInBandInc [CurrentIndex - 1,ic]/Mathf.Abs( largeenergy  * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值
					




						//保存鼓点信息
						if (beatArrindex < BeatArrayList.Count) {
							((MusicData)BeatArrayList [beatArrindex]).playtime = _audio.time;
							((MusicData)BeatArrayList [beatArrindex]).OnBeat = true;
							((MusicData)BeatArrayList [beatArrindex]).BeatPos = ic;
						} else {
							MusicData md = new MusicData ();
							md.playtime = _audio.time;
							md.OnBeat = true;
							md.BeatPos = ic;
							BeatArrayList.Add (md);
						}
						beatArrindex++;
					//保存鼓点信息
				
					//end 保存鼓点信息

					onbeat.Invoke (ic);
					//将频段分为高中低音，确定当前节拍属高中低音
					if (ic<Mathf .Floor(numBands/3)) {
						onbeat.Invoke (0);
//						onbeatlow = true;
//						_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numBands/3)&&ic<Mathf .Floor(2*numBands/3)) {
//						onbeatmid = true;
						onbeat.Invoke (1);
					} else {
						onbeat.Invoke (2);
//						onbeathigh = true;
					}
//					if (!onbeatlow &&(onbeatmid || onbeathigh)) {
//						//_audio.PlayOneShot (mmmhigh );
////					}
					lastbeatindexInBand[ic] = CurrentIndex;//记录当前频段的上一个节拍位置，每帧-1

					//测试用
				
					timestep = _audio.timeSamples - timelast;
					timelast = _audio.timeSamples;
					//end测试用
					Debug.LogWarning  ("onbeat time="+_audio.timeSamples +" ic=" + ic + " lastbeat=" + lastbeatindexInBand[ic] + ",largeindex=" + largeindex + "," + largeenergy + "---" + RecAvgInBandInc [CurrentIndex - 1,ic]);
					//Debug.LogError ("on   AvgSuperhigh="+AvgSuperhigh);


				} else {
					////将频段分为高中低音，确定当前范围中没有节拍
					if (ic<Mathf .Floor(numBands/3)) {
//						onbeatlow = false;
						//_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numBands/3)&&ic<Mathf .Floor(2*numBands/3)) {
//						onbeatmid = false;
					} else {
//						onbeathigh = false;
					}

				}
				////end当前帧增量为buffersize中最大的，且远大于之前的最大值


			}
				
		}//end 遍历所有频段

		for (int icc = 0; icc < numBands; icc++) {
			lastbeatindexInBand [icc]--;
			if (lastbeatindexInBand [icc] < 0) {
				lastbeatindexInBand [icc] = 0;
			}
		}


	}






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


}