using UnityEngine;
using System.Collections;
using System.IO;

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
public class BeatAnalysisRealtime : MonoBehaviour {
	public  AudioSource _audio;
	public AudioClip mmm;
	public AudioClip mmmhigh;
	public  GameObject cubelow;
	public  GameObject cubemid;
	public  GameObject cubehigh;
	float gggtestposlow=0;
	float gggtestposmid=0;
	float gggtestposhigh=0;


	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
	[Header("最后帧/方差")]
	public float  AvgLow;//
	[Header("限制范围后的 最后帧/方差")]
	public float  AvgMid;
	[Header("方差")]
	public float AvgHigh;
	[Header("包含当前帧方差/不包含帧方差")]
	public float AvgSuperhigh;
	[Header("方差/前一帧方差")]
	public float Avgmax;
	public string strclips;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
	public int SpecSize = 256;
	public static int bufferSize = 256;
	public static  int numBands =8;
	MusicData [] md;//=new MusicData[bufferSize] ;//存音乐信息
	float[] lastAverage;//=new float[bufferSize] ;//存前1024帧
	float[] lastAverageInc;//=new float[bufferSize] ;//存前1024帧增长值
	float[,] RecAvgInBandInc;
	float[,] RecAvgInBand;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	public  static  ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//

	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
	float[] Bands;// = new float[8];//分成8个频段
	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];
	float[] lastframeBandss;// = new float[8];//分成8个频段
	//end存当前帧之前或，前1024帧增长值
	int lastbeatindex=0;
	bool  onbeat=false;
	bool  onbeatlow=false;
	bool  onbeatmid=false;
	bool  onbeathigh=false;
	int CurrentIndex=0;//存1024帧中的位置
	//float CurrentFrameAvg=0;
	float LastFrameAvg=0;
	bool playmap=false;
	public  GameObject drawline;
	public  float  lastAvgInc;
	[Range (0.5f,3)]
	public float enegryaddup = 1.2f;
	int timelast;
	public int timestep;
	[SerializeField ]
	float decay = 0.997f;//衰减?
	//[SerializeField ]
	public static int bandlength = 64;//

	public float speed=2000;
	GameObject BeatMapContainer;// = new GameObject ();
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	public GameObject BeatPfb;
	public  string strvariance="";//测试用
	[TextArea ]
	public string BeatMapDataJson;
	/// <summary>
	/// ///////////////////////////////////
	/// </summary>
	// Use this for initialization
	void Start () {
		RecAvgInBandInc=new float[bufferSize ,numBands ]; 
		RecAvgInBand=new float[bufferSize ,numBands ]; 
		 md=new MusicData[bufferSize] ;//存音乐信息
		lastAverage=new float[bufferSize] ;//存前1024帧
		lastAverageInc=new float[bufferSize] ;//存前1024帧
		spectrum =new float[SpecSize ];
		Bands=new float[numBands+1 ];//分8个频段
		lastframeBandss =new float[numBands ];//分8个频段
		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 
		_audio=GetComponent<AudioSource> ();
		//_audio.pitch = 6;
		//_audio.Play();
		//	_audio.loop = true;
		//Application.targetFrameRate = 30;
		Debug.Log(Time.frameCount );
		Debug.Log(Time.captureFramerate );

//		MusicArrayList.Add (new float[4]{1.9f, 2, 3, 4});
//		MusicArrayList.Add (new float[4]{1.2f, 1,1.1f, 1});
//		MusicArrayList.Add (new float[4]{1.1f, 2, 2.1f, 2});
	}

	// Update is called once per frame
	void Update () {
		
		if (_audio.isPlaying && !playmap && !bpmsetting) {
			recordmusicdata ();
		} 	else if (_audio.isPlaying && playmap &&!bpmsetting) {
			_audio.pitch = 1;
			PlayBeatMap ();
		}else if (_audio.isPlaying && bpmsetting ) {
			_audio.pitch = 1;
			//PlayBeatMap ();
		}

		if (onbeatlow) {
			gggtestposlow = 10;//+(;

		} else {
			if (gggtestposlow > 0) {
				gggtestposlow-=gggtestposlow/5;
			}
		}
		cubelow.transform.localScale = new Vector3 (gggtestposlow, gggtestposlow , gggtestposlow);
		if (onbeatmid) {
			gggtestposmid = 10;//+(0.1f*);

		} else {
			if (gggtestposmid > 0) {
				gggtestposmid-=gggtestposmid/5;
			}
		}
		cubemid.transform.localScale = new Vector3 (gggtestposmid, gggtestposmid , gggtestposmid);
		if (onbeathigh) {
			gggtestposhigh = 10;//+(0.1f*);

		} else {
			if (gggtestposhigh > 0) {
				gggtestposhigh-=gggtestposhigh/5;
			}
		}
		cubehigh.transform.localScale = new Vector3 (gggtestposhigh, gggtestposhigh , gggtestposhigh);



		//测试用
		if(Input.GetKeyDown (KeyCode.A)){

//			string sssss = "";
//			for(int i=0;i<spectrum.Length ;i++){
//				sssss+=spectrum[i]+" , ";
//			}
//			Debug.LogError ("----"+_audio.clip.frequency +" // "+_audio.time +" //   "+sssss );
//
//			playmap = true;
//			_audio.Play ();
		}
		if(Input.GetKeyDown (KeyCode.D   )){

			//DrawBeatMap ();

		}
		if(Input.GetKeyDown (KeyCode.C )){
			
			//CheckBeatMap ();

		}
		if(Input.GetKeyDown (KeyCode.Z  )){
			Debug.Log ("MusicArrayList="+MusicArrayList.Count);
			float[] ff =(float[]) MusicArrayList [1];
			Debug.Log ("key1L"+ff[0]+"//"+ff[1]+"//"+ff[2]);
			ff =(float[]) MusicArrayList [5];
			Debug.Log ("key2L"+ff[0]+"//"+ff[1]+"//"+ff[2]);
		}
		if(Input.GetKeyDown (KeyCode.S   )){

			//_audio.Play ();
			Debug.Log(MusicArrayList.Count);
			DetectBeatMap ();
		}
		//end测试用
	
	
	}
	public void playmusic()
	{
		if (BeatMapContainer != null) {
			playmap = true;
		}
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
		//Debug.Log  ("freqlength="+_audio.clip.frequency+"/"+AudioSettings.outputSampleRate+"="+freqlength+"///"+Mathf.Log(1));
		//float musicenergy=0;
		//float musicenergylow=0;
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
		 strvariance="";

		if (CurrentIndex < bufferSize) {
			return;
		}

		for (int ic = 0; ic < numBands; ic++) {
			//遍历所有频段
			int largeindex = 0;
			int largeindexF = 0;
			//int lastAverageInc = 0;
			float largeenergy = RecAvgInBandInc  [lastbeatindexInBand[ic],ic];//当前频段上一次节拍的位置
			strvariance+=">>"+Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2)+"////";
			float variance=0;//方差
			for (int i = lastbeatindexInBand[ic]+1; i < CurrentIndex-2; i++) {
				//遍历当前频段，上一节拍至当前帧的所有平均值增量
				if (RecAvgInBandInc [i,ic] > largeenergy) {
					
					largeindexF = largeindex;
					largeindex = i;//最大值在buffersize中所在的位置
					largeenergy = RecAvgInBandInc [i,ic];//最大

				}//获取平均数增量最大值，及最大值在buffersize中所在的位置
				variance += Mathf.Pow (RecAvgInBandInc [i, ic], 2);

				variance /= CurrentIndex - lastbeatindexInBand [ic] - 1;//上一个节拍至当前节拍之前的节拍，当前频段，音量的方差
				AvgHigh=variance;//测试
			}//end遍历当前频段，上一节拍至当前帧的所有平均值增量，获取最大值及其序号

			float tempVarInc;//计算之前帧的方差与当前帧平方的关系 
			if (variance != 0) {
				tempVarInc = Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2) / variance;//当前帧增量的平方/当前之前的方差
				AvgLow=tempVarInc;//测试
			} else {
				tempVarInc = 1.1f;//Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2);
			}
			tempVarInc = 0.5f + tempVarInc / 5000;
			tempVarInc = Mathf.Clamp (tempVarInc, 0.5f, 1.2f);
			AvgMid  = tempVarInc;//测试
			AvgSuperhigh =(Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2)-variance )/(2*variance);
			strclips = "当前帧平方" + Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, ic], 2);
			strvariance+=variance+",";//显示方差


			//判断节拍///////////////////////
			float beatsincelast = CurrentIndex - lastbeatindexInBand[ic];//当前频段与上一节拍之间的帧数
			if (beatsincelast > bufferSize / 2) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//&& bufferSize-largeindexF<4
				if (RecAvgInBandInc [CurrentIndex - 1,ic]/Mathf.Abs( largeenergy * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值
					

					//保存鼓点信息
					MusicData md=new MusicData();
					md.playtime = _audio.time;
					md.OnBeat = true;
					md.BeatPos = ic;
					BeatArrayList.Add (md);
					//end 保存鼓点信息


					//将频段分为高中低音，确定当前节拍属高中低音
					if (ic<Mathf .Floor(numBands/3)) {
						onbeatlow = true;
						_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numBands/3)&&ic<Mathf .Floor(2*numBands/3)) {
						onbeatmid = true;
					} else {
						onbeathigh = true;
					}
					if (!onbeatlow &&(onbeatmid || onbeathigh)) {
						_audio.PlayOneShot (mmmhigh );
					}
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
						onbeatlow = false;
						//_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numBands/3)&&ic<Mathf .Floor(2*numBands/3)) {
						onbeatmid = false;
					} else {
						onbeathigh = false;
					}

				}
				////end当前帧增量为buffersize中最大的，且远大于之前的最大值


			}
				
		}//end 遍历所有频段
		//Debug.Log (strvariance);
		//Debug.LogError ("on");
		//Debug.Log ("oncheck  time=" + _audio.timeSamples );
		for (int icc = 0; icc < numBands; icc++) {
			lastbeatindexInBand [icc]--;
			if (lastbeatindexInBand [icc] < 0) {
				lastbeatindexInBand [icc] = 0;
			}
		}


	}
	//end分频段检测是否为节拍
	int lastSetbpmframe=-1;
	int Setbpmframe=-1;
	int bpmframe=0;
	bool bpmsetting=false;
	//ArrayList bpmlist;
	public void setBPM()
	{
		//Debug.Log(Time.frameCount  );
		//Debug.Log(Time.captureFramerate );
		if (!_audio.isPlaying) {
			_audio.Play ();
			bpmsetting = true;
			return;
		}
		Debug.Log ("setBPM  last="+lastSetbpmframe+" this="+Setbpmframe+" --=" +(Setbpmframe - lastSetbpmframe) +"//bpmframe*2="+ (bpmframe * 2));
		if (lastSetbpmframe < 0 &&Setbpmframe < 0) {
			lastSetbpmframe = Time.frameCount  ;
			Debug.Log ("set 1");
			return;
		} 
		if (lastSetbpmframe >= 0&&Setbpmframe < 0) {
			Setbpmframe = Time.frameCount  ;
			bpmframe = Setbpmframe - lastSetbpmframe;
			Debug.Log ("set 2");
			return;
		} 
		lastSetbpmframe = Setbpmframe;
		Setbpmframe = Time.frameCount;
		if (lastSetbpmframe >= 0 && Setbpmframe > 0) {
			if (Mathf.Abs (Setbpmframe - lastSetbpmframe ) > (bpmframe * 2) ||Mathf.Abs (Setbpmframe - lastSetbpmframe ) < (bpmframe /2)) {
				lastSetbpmframe = -1;
				Setbpmframe = -1;
				bpmframe = 0;
				Debug.Log ("re set bpm " + (Setbpmframe - lastSetbpmframe) + "//" + (bpmframe * 2));
			} else {
				bpmframe = (bpmframe + Setbpmframe - lastSetbpmframe) / 2;
				bandlength = bpmframe;// (int)Mathf.Floor (bpmframe*Application.targetFrameRate);
				Debug.Log ("bpmframe *2=" + (bpmframe * 2));
			}
		}

	}

	public void StopAudio()
	{
		bpmsetting = false;
		lastSetbpmframe=-1;
		Setbpmframe=-1;
		_audio.Stop ();
	}

	//

	void DetectBeatMap()
	{
		//int bandlength =(int)Mathf.Floor( MusicArrayList.Count / 8);
		//bandlength =  Mathf.Clamp (bandlength, 64, 512);// ( MusicArrayList.Count / 32, 512, 128);
		Debug.LogError ("DetectBeatMap     bandlength="+bandlength+"/"+MusicArrayList.Count);
		float avgWavelength = 0;//全部频段的平均波长
		float[] avgwavelengths=new float[numBands ];
		for (int i = 0; i < numBands ; i++) {
		
		
			float[] peaktimes = new float[4];
			//int index = 0;
			int numPeak = 0; //已经
		
			int peaktimeindex = 0;
			int peaktimelast = 0;//上一个峰值的位置
			do {
				//取4个峰值
			
				float[] firstclipinframe = MusicArrayList [peaktimelast+1] as float[];
				float peakvalue=firstclipinframe[i];
				peaktimes[numPeak ]=firstclipinframe[numBands ];
				Debug.Log ("peakvalue1=" + peakvalue + "peaktimelast index=" + peaktimelast+" time="+firstclipinframe[numBands ]);


				for (int ind = 0; (ind < bandlength &&ind+peaktimelast <MusicArrayList.Count  ); ind++) {
					//在bandlength中找最高值

					float Avginclipframe =((float[])MusicArrayList [ind+peaktimelast])[i];// as float[];
//					string ssss="";
//					for(int iii=0;iii<=numclips;iii++){
//						ssss+=((float[])MusicArrayList [ind+peaktimelast])[iii]+",";
//					}
//					Debug.Log("clipinframe "+ssss +"// index="+(ind+peaktimelast)+"//value="+Avginclipframe);
					if(peakvalue<Avginclipframe)
					{
						peakvalue=Avginclipframe*(bandlength-4)/(decay*bandlength) ;//*衰减
						peaktimes[numPeak ]=((float[])MusicArrayList [ind+peaktimelast])[numBands ] ;
						peaktimeindex=ind+peaktimelast;						
					}
					///////////////////////
//
//					float[] clipinframe =MusicArrayList [ind+peaktimelast] as float[];
//					Debug.Log("clipinframe"+(float[])MusicArrayList [ind+peaktimelast] +"//"+(ind+peaktimelast)+"//"+clipinframe[i ]);
//					if(peakvalue<clipinframe[i])
//					{
//						peakvalue=clipinframe[i];
//						peaktimes[index ]=clipinframe[numclips ] ;
//						peaktimeindex=ind+peaktimelast;						
//					}
				}

				Debug.LogError ("peaktimes index="+numPeak +" time="+peaktimes[numPeak ]+" value="+peakvalue+" peaktimeindex="+peaktimeindex );
				peaktimelast=peaktimeindex+(int)Mathf.Floor (bandlength/8) ;
				numPeak++;
			} while(numPeak < 4);


			float avgWavelengthInclip = (peaktimes [3] -  peaktimes [0])/3;
			//avgWavelengthInclip /= 3;//单个频段波长
			avgWavelength += avgWavelengthInclip;//计算全部频段平均波长
			avgwavelengths [i] = avgWavelength;//存所有频段的波长
			Debug.LogError ("avgWavelengthInclip="+avgWavelengthInclip);
		}

		avgWavelength /= numBands;//全部频段平均波长
		Debug.LogError ("avgWavelength="+avgWavelength);
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
		Debug.LogError ("avgWavelength final="+avgWavelength);
//	


	}


	//根据实时采集到数据生成map
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
			//GameObjBeats = new GameObject[BeatArrayList.Count ];
			BeatMapContainer.transform.position=new Vector3(0,0-speed/100,0);
		}

		savedBeatMap sbm=new savedBeatMap();
		sbm.MD=new MusicData[BeatArrayList.Count ] ;
	
		//BeatMapContainer = new GameObject ();
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
	
		string ttt = JsonUtility.ToJson (sbm);
		Debug.Log("ttt="+ttt);

		Save (ttt);
	}
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

	public  void Btnload() {
		load(BeatMapDataJson) ;
	}

	//从json生成map
	 void load(string jsonstr) {  
		savedBeatMap  smdread = JsonUtility.FromJson<savedBeatMap> (jsonstr);
		Debug.Log ("load smdread.md="+smdread.MD );

		if (GameObjBeats!=null) {
			Debug.Log (BeatMapContainer.transform.childCount);
			for (int i = 0; i < GameObjBeats.Length; i++) {
				DestroyImmediate (GameObjBeats [i]);
			}

		} else {
			BeatMapContainer = new GameObject ();
			GameObjBeats = new GameObject[smdread.MD.Length];
			BeatMapContainer.transform.position=new Vector3(0,0-speed/100,0);
		}	
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


	//测试用
	//beatmap下落
	void PlayBeatMap()
	{
		BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
	}
	//按键
	public void CheckBeatMap()
	{
		Debug.Log(MusicArrayList.Count);
		DetectBeatMap  ();
//		Beat[] beats = BeatMapContainer.GetComponentsInChildren <Beat> ();
//		foreach(Beat b in beats){
//			if (b.CheckState) {
//				b.transform.localScale = new Vector3 (10, 1, 1);
//				Debug.Log (_audio.time +"///"+ b.Destorytime);
//			//	_audio.PlayOneShot (mmm);
//			}
//		}
	}
	//测试用


}
