﻿using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;



[System.Serializable]
public class OnBeatLowEventHandler : UnityEngine.Events.UnityEvent
{

}
[System.Serializable]
public class OnBeatHighEventHandler : UnityEngine.Events.UnityEvent
{

}
[System.Serializable]
public class OnBeatrealtimeEventHandler : UnityEngine.Events.UnityEvent< int >
{

}
/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
public class BeatAnalysisRealtime : MonoBehaviour {
	[HideInInspector ]
	AudioSource _audio;
	[SerializeField ]
	bool useFreq=false;//是否根据频率范围分段，false则根据对数方式区分高频和低频
	[SerializeField ]
	Vector2[] FreqRange;//使用的频率范围，x为低频y为高频
	public static string AudioName="";
	[SerializeField ]
	bool checkwithInc=true;//使用增长值或能量值计算节拍
	public OnBeatrealtimeEventHandler onBeat;//节拍事件


	//从manager读取的
	int _SpecSize = 256;//采样数量
	int _bufferSize = 256;//记录的帧数
	int _numBands =0;//分频段数量
	//end从manager读取的


	//记录每帧增长值和能量值，需要初始化
	float[,] RecAvgInBandInc;
	float[,] RecAvgInBand;

	int beatArrindex=0;//用于重复使用array中元素
	//public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息



	//每帧更新的
	float[] spectrum ;// 每帧采样数组
	float[] Bands;// 保存当前帧各个频段能量，最后一位保存当前时间
	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];
	int CurrentIndex=0;//存1024帧中的位置


	bool playRealtime=false;//播放一次后设为false，限制只有使用playmusic按钮时才分析谱面

	float enegryaddup = 1.2f;//比较最大值时使用的
	float decay = 0.997f;//衰减


	int timelast;//测试用
	public int timestep;//测试用


//	[SerializeField ]
//	 int _bandlength = 32;//
//	public static int bandlength = 32;//

	//public float speed=2000;
	//GameObject BeatMapContainer;// = new GameObject ();
//	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];




	void Start () {
	//	bandlength = _bandlength;
		InitSetting ();
		//_audio.pitch = 2;
		Debug.Log("2"+Time.frameCount );
	
	}
	void InitSetting(){
		if (!useFreq) {
			_numBands = BeatAnalysisManager.numBands;//使用numband
		} else {
			_numBands = FreqRange.Length;//使用频率区间
		}
		_bufferSize = BeatAnalysisManager .bufferSize ;
		RecAvgInBandInc=new float[_bufferSize ,_numBands ]; 
		RecAvgInBand=new float[_bufferSize ,_numBands ]; 
		_SpecSize = BeatAnalysisManager .SpecSize;
		spectrum =new float[_SpecSize ];
		Bands=new float[_numBands+1 ];//频段

		lastbeatindexInBand=new int[_numBands];//存各个频段上一次节拍的位置 
		_audio=BeatAnalysisManager ._audio ;//GetComponent<AudioSource> ();
		AudioName = _audio.name;
		decay = BeatAnalysisManager .decay;
		enegryaddup = BeatAnalysisManager .enegryaddup;

		BeatAnalysisManager .BeatArrayList.Clear();
		BeatAnalysisManager. MusicArrayList.Clear ();
		beatArrindex=0;
		CurrentIndex = 0;
	}

	// Update is called once per frame
	void Update () {
		

		if (_audio.isPlaying && playRealtime  ) {
		//if (_audio.isPlaying   ) {
			recordmusicdata ();
			return;
		} 

		if (!_audio.isPlaying) {
			playRealtime = false;
		}
	
	}
	public void playmusic()
	{
		InitSetting ();
		playRealtime = true;
		_audio.Play ();
	}

	void recordmusicdata()
	{
	////////////////////////////////////////
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		//CurrentFrameAvg =
		CalcCurrentFrameAvg (spectrum);// musicenergy;

		recordAvgInBand ();
		//CheckBeat ();
		if (checkwithInc) {
			CheckBeatIncInBand ();
		} else {
			CheckBeatInBand ();
		}
		recordAllData();

	}

	void recordAllData(){
		float[] ff = new float[_numBands+1] ;//clips;
		ff=Bands;
		BeatAnalysisManager. MusicArrayList.Add (ff);
		//////////////
//		string s="";
//		float[] f = BeatAnalysisManager.MusicArrayList [BeatAnalysisManager.MusicArrayList.Count - 1] as float[];
//		for (int i = 0; i <= _numBands; i++) {
//			s += "~" + f [i];
//		}
		//print ("s="+s);
	}


	//计算当前帧的平均值，及各个频段平均值
	void   CalcCurrentFrameAvg(float[] spec)
	{		
		
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个，单声道和立体声似乎没有区别
		int freqlength=(int)Mathf.Floor (_SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
	
		int[] ArrBandlength = new int[_numBands ];//存每个频段的数据数量

		for (int i = 0; i <_numBands; i++) {
			//初始化各频段和各频段的数据数量
			//clips [i] =0;
			ArrBandlength [i] = 0;
		}
		Bands = new float [_numBands + 1];

		//计算平均数及分频段时，只计算有效部分，忽略超出的freqlength
		for (int i =0; i < freqlength; i++)
		{	int IndexInBand=-1;
			if(!useFreq){
			//musicenergy += spectrum [i];//总音量和，需要取平均数使用
				IndexInBand =(int) Mathf.Floor (0.5f+Mathf.Log(i+2,freqlength )*_numBands) -1;//对数方式，将频率分为几段
				IndexInBand=Mathf.Clamp(IndexInBand,0,_numBands-1 );//限制频段编号范围
			}else{
				for (int ir = 0; ir < FreqRange.Length; ir++) {
					int FreqToIndex = i * 24000 / _SpecSize;
					if (FreqToIndex >= FreqRange [ir].x && FreqToIndex < FreqRange [ir].y) {
						IndexInBand = ir;
						//Debug.Log ("FreqToIndex= "+FreqToIndex+" ir="+ir);
					}		
				}	
			}
			if(IndexInBand<Bands.Length && IndexInBand>=0){
				Bands [IndexInBand] += spectrum [i];//每个频段的音量和，可能需要求平均数再使用
				ArrBandlength[IndexInBand]++;//计数增加
			}

		}
		for (int i = 0; i < _numBands ; i++) {
			//根据每频段计数计算平均值
			Bands [i] /= ArrBandlength [i];
		}
		Bands [_numBands] = _audio.time;//存当前帧的时间
	
	}
	//end 计算当前帧的平均值，及各个频段平均值


	//记录各频段前时间段的平均值RecAvgInClip，及增长值RecAvgInClipInc
	void  recordAvgInBand()
	{
		if (CurrentIndex < _bufferSize) {
			//1024帧之前。直接存入数组
			for (int i = 0; i < _numBands ; i++) {
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
			for (int indroll = 0; indroll < _bufferSize-1; indroll++) {
				for (int i = 0; i < _numBands ; i++) {
					RecAvgInBand [indroll,i] =  RecAvgInBand [indroll + 1,i];
					RecAvgInBandInc [indroll,i] =  RecAvgInBandInc [indroll + 1,i];
				}
			}
			//最后一位为最新一帧的平均值及增长值
			for (int i = 0; i < _numBands ; i++) {
				RecAvgInBand [_bufferSize-1,i] =  Bands[i];
				RecAvgInBandInc [_bufferSize - 1,i] = Bands [i] - RecAvgInBand [_bufferSize  -2,i];// 
			}
			//Debug.DrawLine(new Vector3 (1,0,0),new Vector3 (1,100*lastAverageInc [_bufferSize - 1],0));
		}
		//没填满buffersize则计数增加
		if (CurrentIndex < _bufferSize) {
			CurrentIndex++;
		}	
	}
	//end记录各频段前时间段的平均值RecAvgInClip，及增长值RecAvgInClipInc






	//分频段，实时检测节拍
	void CheckBeatIncInBand()
	{  
//		 strvariance="";

		if (CurrentIndex < _bufferSize) {
			return;
		}

		for (int ic = 0; ic < _numBands; ic++) {
			//遍历所有频段
			int largeindex = 0;

			float largeenergy = RecAvgInBandInc  [lastbeatindexInBand[ic],ic];//当前频段上一次节拍的位置
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




			//判断节拍///////////////////////
			float beatsincelast = CurrentIndex - lastbeatindexInBand[ic];//当前频段与上一节拍之间的帧数
			if (beatsincelast > _bufferSize / 2) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//&& _bufferSize-largeindexF<4
				if (RecAvgInBandInc [CurrentIndex - 1,ic]/Mathf.Abs( largeenergy  * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值
					



					ArrayList BeatArrayList = BeatAnalysisManager .BeatArrayList;//存beat信息
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


					//将频段分为高中低音，确定当前节拍属高中低音
					onBeat.Invoke (ic);

					lastbeatindexInBand[ic] = CurrentIndex;//记录当前频段的上一个节拍位置，每帧-1

					//测试用
				
					timestep = _audio.timeSamples - timelast;
					timelast = _audio.timeSamples;
					//end测试用
					Debug.LogWarning  ("onbeat time="+_audio.timeSamples +" ic=" + ic + " lastbeat=" + lastbeatindexInBand[ic] + ",largeindex=" + largeindex + "," + largeenergy + "---" + RecAvgInBandInc [CurrentIndex - 1,ic]);
					//Debug.LogError ("on   AvgSuperhigh="+AvgSuperhigh);


				} else {


				}
				////end当前帧增量为buffersize中最大的，且远大于之前的最大值


			}
				
		}//end 遍历所有频段
		//Debug.Log (strvariance);
		//Debug.LogError ("on");
		//Debug.Log ("oncheck  time=" + _audio.timeSamples );
		for (int icc = 0; icc < _numBands; icc++) {
			lastbeatindexInBand [icc]--;
			if (lastbeatindexInBand [icc] < 0) {
				lastbeatindexInBand [icc] = 0;
			}
		}


	}









	//分频段，实时检测节拍
	void CheckBeatInBand()
	{  
		//		 strvariance="";

		if (CurrentIndex < _bufferSize) {
			return;
		}

		for (int ic = 0; ic < _numBands; ic++) {
			//遍历所有频段
			int largeindex = 0;
			//			int largeindexF = 0;
			//int lastAverageInc = 0;
			float largeenergy = RecAvgInBand  [lastbeatindexInBand[ic],ic];//当前频段上一次节拍的位置
			//			strvariance+=">>"+Mathf.Pow (RecAvgInBand [CurrentIndex - 1, ic], 2)+"////";
			float variance=0;//方差
			for (int i = lastbeatindexInBand[ic]+1; i < CurrentIndex-2; i++) {
				//遍历当前频段，上一节拍至当前帧的所有平均值
				if (RecAvgInBand [i,ic] > largeenergy ) {

					//					largeindexF = largeindex;
					largeindex = i;//最大值在buffersize中所在的位置
					largeenergy = RecAvgInBand [i,ic] ;//最大

				}//获取平均数增量最大值，及最大值在buffersize中所在的位置
				variance += Mathf.Pow (RecAvgInBand [i, ic], 2);
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

			//			strvariance+=variance+",";//显示方差


			//判断节拍///////////////////////
			float beatsincelast = CurrentIndex - lastbeatindexInBand[ic];//当前频段与上一节拍之间的帧数
			if (beatsincelast > _bufferSize / 4) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//&& _bufferSize-largeindexF<4
				if (RecAvgInBand [CurrentIndex - 1,ic]/Mathf.Abs( largeenergy  * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值




					ArrayList BeatArrayList = BeatAnalysisManager .BeatArrayList;//存beat信息
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


					//将频段分为高中低音，确定当前节拍属高中低音
					onBeat.Invoke (ic);

					lastbeatindexInBand[ic] = CurrentIndex;//记录当前频段的上一个节拍位置，每帧-1

					//测试用

					timestep = _audio.timeSamples - timelast;
					timelast = _audio.timeSamples;
					//end测试用
					Debug.LogWarning  ("onbeat time="+_audio.timeSamples +" ic=" + ic + " lastbeat=" + lastbeatindexInBand[ic] + ",largeindex=" + largeindex + "," + largeenergy + "---" + RecAvgInBandInc [CurrentIndex - 1,ic]);
					//Debug.LogError ("on   AvgSuperhigh="+AvgSuperhigh);


				} else {


				}
				////end当前帧增量为buffersize中最大的，且远大于之前的最大值


			}

		}//end 遍历所有频段
		//Debug.Log (strvariance);
		//Debug.LogError ("on");
		//Debug.Log ("oncheck  time=" + _audio.timeSamples );
		for (int icc = 0; icc < _numBands; icc++) {
			lastbeatindexInBand [icc]--;
			if (lastbeatindexInBand [icc] < 0) {
				lastbeatindexInBand [icc] = 0;
			}
		}


	}




	//end分频段检测是否为节拍
//	int lastSetbpmframe=-1;
//	int Setbpmframe=-1;
//	int bpmframe=0;
//	[SerializeField]
//	//Text  bpmtxt;
//	bool bpmsetting=false;
	//ArrayList bpmlist;
//	public void setBPM()
//	{
//
//		if (!_audio.isPlaying) {
//			_audio.Play ();
//			bpmsetting = true;
//			return;
//		}
//		Debug.Log ("setBPM  last="+lastSetbpmframe+" this="+Setbpmframe+" --=" +(Setbpmframe - lastSetbpmframe) +"//bpmframe*2="+ (bpmframe * 2));
//		if (lastSetbpmframe < 0 &&Setbpmframe < 0) {
//			lastSetbpmframe = Time.frameCount  ;
//			Debug.Log ("set 1");
//			return;
//		} 
//		if (lastSetbpmframe >= 0&&Setbpmframe < 0) {
//			Setbpmframe = Time.frameCount  ;
//			bpmframe = Setbpmframe - lastSetbpmframe;
//			Debug.Log ("set 2");
//			return;
//		} 
//		lastSetbpmframe = Setbpmframe;
//		Setbpmframe = Time.frameCount;
//		if (lastSetbpmframe >= 0 && Setbpmframe > 0) {
//			if (Mathf.Abs (Setbpmframe - lastSetbpmframe ) > (bpmframe * 2) ||Mathf.Abs (Setbpmframe - lastSetbpmframe ) < (bpmframe /2)) {
//				lastSetbpmframe = -1;
//				Setbpmframe = -1;
//				bpmframe = 0;
//				Debug.Log ("re set bpm " + (Setbpmframe - lastSetbpmframe) + "//" + (bpmframe * 2));
//			} else {
//				bpmframe = (bpmframe + Setbpmframe - lastSetbpmframe) / 2;
//				bandlength = bpmframe;// (int)Mathf.Floor (bpmframe*Application.targetFrameRate);
//				Debug.Log ("bpmframe *2=" + (bpmframe * 2));
//			}
//		}
//		//bpmtxt.text = "bpm="+bpmframe;
//
//	}

	public void StopAudio()
	{
//		bpmsetting = false;
//		lastSetbpmframe=-1;
//		Setbpmframe=-1;
		_audio.Stop ();
	}





//	//测试用
//	//beatmap下落
//	void PlayBeatMap()
//	{
//				BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
//	}
//
//	//按键
//	public void CheckBeatMap()
//	{
//		Debug.Log(MusicArrayList.Count);
//	//	DetectBeatMap  ();
////		Beat[] beats = BeatMapContainer.GetComponentsInChildren <Beat> ();
////		foreach(Beat b in beats){
////			if (b.CheckState) {
////				b.transform.localScale = new Vector3 (10, 1, 1);
////				Debug.Log (_audio.time +"///"+ b.Destorytime);
////			//	_audio.PlayOneShot (mmm);
////			}
////		}
//	}

}