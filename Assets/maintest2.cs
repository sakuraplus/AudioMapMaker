using UnityEngine;
using System.Collections;
public class MusicData
{
	public float playtime = 0;
	public float Average = 0;
	public bool onbeat = false;
}
/// <summary>
/// 检测音量变化幅度
/// 
/// </summary>
public class maintest2 : MonoBehaviour {
	AudioSource _audio;
	public AudioClip mmm;
	public  GameObject cubelow;
	public  GameObject cubemid;
	public  GameObject cubehigh;
	float gggtestposlow=0;
	float gggtestposmid=0;
	float gggtestposhigh=0;
	float gggscale=1;

	public float  方差;//
	public float  AvgMid;
	public float AvgHigh;
	public float superhigh;
	public float Avgmax;
	public string strclips;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
	public int SpecSize = 256;
	public int bufferSize = 256;
	public int numclips =8;
	MusicData [] md;//=new MusicData[bufferSize] ;//存音乐信息
	float[] lastAverage;//=new float[bufferSize] ;//存前1024帧
	float[] lastAverageInc;//=new float[bufferSize] ;//存前1024帧增长值
	float[,] AvgInClipInc;
	float[,] AvgInClip;
	ArrayList beatlist=new ArrayList() ;//存音乐信息
	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
	float[] clips;// = new float[8];//分成8个频段
	int[] lastbeatindexInClip;//存各个频段上一次节拍的位置 =new int[8];
	float[] lastframeclips;// = new float[8];//分成8个频段
	//end存当前帧之前或，前1024帧增长值
	int lastbeatindex=0;
	bool  onbeat=false;
	bool  onbeatlow=false;
	bool  onbeatmid=false;
	bool  onbeathigh=false;

	float CurrentFrameAvg=0;
	float LastFrameAvg=0;
	/// <summary>
	/// ///////////////////////////////////
	/// </summary>
	// Use this for initialization
	void Start () {
		AvgInClipInc=new float[bufferSize ,numclips ]; 
		AvgInClip=new float[bufferSize ,numclips ]; 
		 md=new MusicData[bufferSize] ;//存音乐信息
		lastAverage=new float[bufferSize] ;//存前1024帧
		lastAverageInc=new float[bufferSize] ;//存前1024帧
		spectrum =new float[SpecSize ];
		clips=new float[numclips ];//分8个频段
		lastframeclips =new float[numclips ];//分8个频段
		lastbeatindexInClip=new int[numclips];//存各个频段上一次节拍的位置 
		_audio=GetComponent<AudioSource> ();
		_audio.Play();
		_audio.loop = true;
		//Application.targetFrameRate = 1;
	}

	// Update is called once per frame
	void Update () {
		if (_audio.isPlaying) {
			recordmusicdata ();
		} else {
			Debug.Log ("stop");
			//searchBeat ();
		}

		if (onbeatlow) {
			gggtestposlow = 10;//+(0.1f*gggscale);

		} else {
			if (gggtestposlow > 0) {
				gggtestposlow-=gggtestposlow/5;
			}
		}
		cubelow.transform.localScale = new Vector3 (gggtestposlow, gggtestposlow , gggtestposlow);
		if (onbeatmid) {
			gggtestposmid = 10;//+(0.1f*gggscale);

		} else {
			if (gggtestposmid > 0) {
				gggtestposmid-=gggtestposmid/5;
			}
		}
		cubemid.transform.localScale = new Vector3 (gggtestposmid, gggtestposmid , gggtestposmid);
		if (onbeathigh) {
			gggtestposhigh = 10;//+(0.1f*gggscale);

		} else {
			if (gggtestposhigh > 0) {
				gggtestposhigh-=gggtestposhigh/5;
			}
		}
		cubehigh.transform.localScale = new Vector3 (gggtestposhigh, gggtestposhigh , gggtestposhigh);


//		if (onbeathigh || onbeatlow || onbeatmid) {
//			_audio.PlayOneShot (mmm);
//		}
//		if (!onbeat) {
//			onbeatlow = false;
//			onbeatmid = false;
//			onbeathigh = false;
//		}
		//测试用
		if(Input.GetKeyDown (KeyCode.A)){
			//	_audio.Stop();
			Debug.Log(_audio.timeSamples+"/stop//"+_audio.time+"///"+Time.time );
		}
	

		//end测试用

	
	}

	void recordmusicdata()
	{
	////////////////////////////////////////
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		CurrentFrameAvg =CalcCurrentFrameAvg (spectrum);// musicenergy;
//		AvgLow = (clips [0] )/1;
//		AvgMid = (clips [2] + clips [3]+ clips [4]+clips [1])/4;
//		AvgHigh = ( clips [5] + clips [6] + clips [7])/3;
//		superhigh =( clips [5] + clips [6] + clips [7]);

		strclips=方差+","+AvgMid+","+AvgHigh;
		recordAvgInc ();
		recordAvgInClip ();
		//CheckBeat ();
		CheckBeatInClip ();


	}

	//计算当前帧的平均值
	float   CalcCurrentFrameAvg(float[] spec)
	{
		
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个
		int freqlength=(int)Mathf.Floor (SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
		//Debug.Log  ("freqlength="+_audio.clip.frequency+"/"+AudioSettings.outputSampleRate+"="+freqlength+"///"+Mathf.Log(1));
		float musicenergy=0;
		//float musicenergylow=0;
		int[] cliplength = new int[clips.Length ];

		for (int i = 0; i < clips.Length; i++) {
			clips [i] =0;
			cliplength [i] = 0;
		}

		for (int i =0; i < freqlength; i++)
		{
			musicenergy += spectrum [i];//总音量和，需要取平均数使用
			int icic =(int) Mathf.Floor (0.5f+Mathf.Log(i+2,freqlength )*clips.Length ) -1;//对数方式，将频率分为几段
			icic=Mathf.Clamp(icic,0,numclips-1 );
			//Debug.Log  ("icic="+icic);;
			clips [icic] += spectrum [i];//每个频段的音量和，可能需要求平均数再使用
			cliplength[icic]++;

		}
		for (int i = 0; i < clips.Length; i++) {
			clips [i] /= cliplength [i];
		}
		musicenergy /= freqlength;//当前帧 平均值
	//	musicenergylow=(clips[0]+clips[1]);
		return musicenergy ;
		//return musicenergylow ;
	}
	//end 计算当前帧的平均值

	//记录各频段前时间段的值
	int CurrentIndex=0;//存1024帧中的位置
	void  recordAvgInClip()
	{
		if (CurrentIndex < bufferSize) {
			//1024帧之前
			for (int i = 0; i < numclips ; i++) {
				AvgInClip [CurrentIndex,i] = clips[i];

				if (CurrentIndex == 0) {
					AvgInClipInc [0, i] = 0;
				} else {
					AvgInClipInc [CurrentIndex, i] = clips [i] - AvgInClip [CurrentIndex- 1,i];
				}
			}

		} else { 
			//lastAverage  [CurrentIndex - bufferSize] = currentframeavg-LastFrameAvg ;
			for (int indroll = 0; indroll < bufferSize-1; indroll++) {
				for (int i = 0; i < numclips ; i++) {
					AvgInClip [indroll,i] =  AvgInClip [indroll + 1,i];
					AvgInClipInc [indroll,i] =  AvgInClipInc [indroll + 1,i];
				}
				//AvgInClip [indroll] = AvgInClip [indroll + 1];
				//AvgInClipInc [indroll] = AvgInClipInc [indroll + 1];
			}
			for (int i = 0; i < numclips ; i++) {
				AvgInClip [bufferSize-1,i] =  clips[i];
				AvgInClipInc [bufferSize - 1,i] = clips [i] - AvgInClip [bufferSize  -2,i];// CurrentFrameAvg - LastFrameAvg;// Mathf.Sqrt  ( Mathf.Abs( CurrentFrameAvg-LastFrameAvg));
			}
			//Debug.DrawLine(new Vector3 (1,0,0),new Vector3 (1,100*lastAverageInc [bufferSize - 1],0));
		}

		if (CurrentIndex < bufferSize) {
			CurrentIndex++;
		}	
	}







	//存当前帧之前或，前1024帧增长值
	public  GameObject drawline;
	public  float  lastAvgInc;
	void  recordAvgInc()
	{
		if (CurrentIndex == 0) {
			lastAverageInc [0] = 0;
			//lastAverage = CurrentFrameAvg;
			for (int i = 0; i < numclips; i++) {
				AvgInClipInc [CurrentIndex, i] = 0;
			}

		}else if (CurrentIndex < bufferSize) {
			//1024帧之前
			lastAverageInc  [CurrentIndex] =CurrentFrameAvg-LastFrameAvg;//Mathf.Sqrt (Mathf.Abs( CurrentFrameAvg-LastFrameAvg)) ;

		} else { 
			//lastAverage  [CurrentIndex - bufferSize] = currentframeavg-LastFrameAvg ;
			for (int i = 0; i < bufferSize-1; ++i) {
				lastAverageInc [i] = lastAverageInc [i + 1];
			}
			lastAverageInc [bufferSize - 1] = CurrentFrameAvg - LastFrameAvg;// Mathf.Sqrt  ( Mathf.Abs( CurrentFrameAvg-LastFrameAvg));

			//Debug.DrawLine(new Vector3 (1,0,0),new Vector3 (1,100*lastAverageInc [bufferSize - 1],0));
		}



		lastAvgInc = lastAverageInc [bufferSize - 1];
		drawline.transform.localScale = new Vector3 (1,Mathf.Max(0.5f, 100 * lastAverageInc [bufferSize - 1]), 0);

		LastFrameAvg = CurrentFrameAvg;
		if (CurrentIndex < bufferSize) {
			CurrentIndex++;
		}	
	}

	void  recordAvgIncInClip()
	{
		if (CurrentIndex == 0) {
			
			for (int i = 0; i < numclips; i++) {
				AvgInClipInc [CurrentIndex, i] = 0;
			}

		}else if (CurrentIndex < bufferSize) {
			//1024帧之前
			lastAverageInc  [CurrentIndex] =CurrentFrameAvg-LastFrameAvg;//Mathf.Sqrt (Mathf.Abs( CurrentFrameAvg-LastFrameAvg)) ;

		} else { 
			//lastAverage  [CurrentIndex - bufferSize] = currentframeavg-LastFrameAvg ;
			for (int i = 0; i < bufferSize-1; ++i) {
				lastAverageInc [i] = lastAverageInc [i + 1];
			}
			lastAverageInc [bufferSize - 1] = CurrentFrameAvg - LastFrameAvg;// Mathf.Sqrt  ( Mathf.Abs( CurrentFrameAvg-LastFrameAvg));

			//Debug.DrawLine(new Vector3 (1,0,0),new Vector3 (1,100*lastAverageInc [bufferSize - 1],0));
		}



		lastAvgInc = lastAverageInc [bufferSize - 1];
		drawline.transform.localScale = new Vector3 (1,Mathf.Max(0.5f, 100 * lastAverageInc [bufferSize - 1]), 0);

		LastFrameAvg = CurrentFrameAvg;
		if (CurrentIndex < bufferSize) {
			CurrentIndex++;
		}	
	}

	//end存当前帧之前或，前1024帧增长值

	[Range(1,4)]
	public float enegryaddup = 1.2f;
	int timelast;
	public int timestep;
	//检查是否为beat
	void CheckBeat()
	{  
		if (CurrentIndex < bufferSize) {
			return;
		}
		int largeindex = 0;
		int largeindexF = 0;
		//int lastAverageInc = 0;
		float largeenergy = lastAverageInc [lastbeatindex];
		for (int i = lastbeatindex+1; i < CurrentIndex-2; i++) {
			if (lastAverageInc [i] > largeenergy) {
				largeindexF = largeindex;
				largeindex = i;
				largeenergy = lastAverageInc [i];
			}
		}

		float beatsincelast = CurrentIndex - lastbeatindex;
		if (beatsincelast > bufferSize / 4 ) {
			//&& bufferSize-largeindexF<4
			if (lastAverageInc [CurrentIndex - 1] > largeenergy * enegryaddup  ) {
				//if (CurrentFrameAvg > largeenergy*enegryaddup  && (CurrentIndex-lastbeatindex )>bufferSize /8) {

				onbeat = true;
				if (方差 > AvgMid && 方差 > AvgHigh) {
					onbeatlow = true;
				} else if (AvgMid > 方差 && AvgMid > AvgHigh) {
					onbeatmid = true;
				} else {
					onbeathigh = true;
				}
				gggscale = CurrentFrameAvg / largeenergy;
				lastbeatindex = CurrentIndex;//largeindex;
				timestep = _audio.timeSamples - timelast;
				timelast = _audio.timeSamples;
				Debug.LogError ("onbeat  " + _audio.timeSamples + "-" + lastbeatindex + "," + largeindex + "," + largeenergy + "---" + lastAverageInc [CurrentIndex - 1]);

					_audio.PlayOneShot (mmm);

			} else {
				onbeat = false;

			}
			Debug.Log ("oncheck  " + _audio.timeSamples + "-" + lastbeatindex + "," + largeindex + "," + largeenergy + "---" + lastAverageInc [CurrentIndex - 1]);

		}
		lastbeatindex--;//后退一位
		if (lastbeatindex < 0) {
			lastbeatindex = 0;
		}

	}



	/// <summary>
	/// ////////////////////////分频段检测是否为节拍
	/// </summary>
	public  string strvariance="";//测试用

	void CheckBeatInClip()
	{  
		 strvariance="";

		if (CurrentIndex < bufferSize) {
			return;
		}

		for (int ic = 0; ic < numclips; ic++) {
			//遍历所有频段
			int largeindex = 0;
			int largeindexF = 0;
			//int lastAverageInc = 0;
			float largeenergy = AvgInClipInc  [lastbeatindexInClip[ic],ic];//当前频段上一次节拍的位置
			strvariance+=">>"+Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2)+"////";
			float variance=0;//方差
			for (int i = lastbeatindexInClip[ic]+1; i < CurrentIndex-2; i++) {
				//遍历当前频段，上一节拍至当前帧的所有平均值增量
				if (AvgInClipInc [i,ic] > largeenergy) {
					
					largeindexF = largeindex;
					largeindex = i;//最大值在buffersize中所在的位置
					largeenergy = AvgInClipInc [i,ic];//最大

				}//获取平均数增量最大值，及最大值在buffersize中所在的位置
				variance += Mathf.Pow (AvgInClipInc [i, ic], 2);

				variance /= CurrentIndex - lastbeatindexInClip [ic] - 1;//上一个节拍至当前节拍之前的节拍，当前频段，音量的方差
				AvgHigh=variance;//测试
			}//end遍历当前频段，上一节拍至当前帧的所有平均值增量，获取最大值及其序号

			float tempVarInc;//计算之前帧的方差与当前帧平方的关系 
			if (variance != 0) {
				tempVarInc = Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2) / variance;//当前帧增量的平方/当前之前的方差
				方差=tempVarInc;//测试
			} else {
				tempVarInc = 1.1f;//Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2);
			}
			tempVarInc = Mathf.Clamp (tempVarInc, 0.9f, 1.1f);
			AvgMid  = tempVarInc;//测试
			strvariance+=variance+",";//显示方差
			float beatsincelast = CurrentIndex - lastbeatindexInClip[ic];//当前频段与上一节拍之间的帧数
			if (beatsincelast > bufferSize / 2) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//&& bufferSize-largeindexF<4
				if (AvgInClipInc [CurrentIndex - 1,ic]/Mathf.Abs( largeenergy * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值
					//if (CurrentFrameAvg > largeenergy*enegryaddup  && (CurrentIndex-lastbeatindex )>bufferSize /8) {

					//onbeat = true;

					//将频段分为高中低音，确定当前节拍属高中低音
					if (ic<Mathf .Floor(numclips/3)) {
						onbeatlow = true;
						//_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numclips/3)&&ic<Mathf .Floor(2*numclips/3)) {
						onbeatmid = true;
					} else {
						onbeathigh = true;
					}
					_audio.PlayOneShot (mmm);

					lastbeatindexInClip[ic] = CurrentIndex;//记录当前频段的上一个节拍位置，每帧-1

					//测试用
					gggscale = CurrentFrameAvg / largeenergy;//测试用
					timestep = _audio.timeSamples - timelast;
					timelast = _audio.timeSamples;
					//end测试用
					Debug.LogError ("onbeat time="+_audio.timeSamples +" ic=" + ic + " lastbeat=" + lastbeatindexInClip[ic] + ",largeindex=" + largeindex + "," + largeenergy + "---" + AvgInClipInc [CurrentIndex - 1,ic]);
			


				} else {
					////将频段分为高中低音，确定当前范围中没有节拍
					if (ic<Mathf .Floor(numclips/3)) {
						onbeatlow = false;
						//_audio.PlayOneShot (mmm);
					} else if (ic>=Mathf .Floor(numclips/3)&&ic<Mathf .Floor(2*numclips/3)) {
						onbeatmid = false;
					} else {
						onbeathigh = false;
					}

				}
				////end当前帧增量为buffersize中最大的，且远大于之前的最大值


			}
				
		}//end 遍历所有频段
		Debug.Log (strvariance);
		//Debug.LogError ("on");
		//Debug.Log ("oncheck  time=" + _audio.timeSamples );
		for (int icc = 0; icc < numclips; icc++) {
			lastbeatindexInClip [icc]--;
			if (lastbeatindexInClip [icc] < 0) {
				lastbeatindexInClip [icc] = 0;
			}
		}


	}
	//end分频段检测是否为节拍


	float[] avgs = new float[20];
	float[] avgIncreases = new float[20];




	float calcAverage(int index)
	{
		float sum = 0;
		MusicData mdnow;//= md [i];
		MusicData mdlast;
		if ( md.Length < 20) {
			//return -1;
			for (int i = 1; i <  md.Length ; i++) {
				mdnow = md [i] as MusicData;
				mdlast = md [i-1] as MusicData;
				sum +=(mdnow.Average -mdlast.Average);
				avgs [i] = (mdnow.Average - mdlast.Average);// md [i ];
			}
			return sum/ md.Length ;
		}
		int startindex;
		int endindex;
		if (index >  md.Length - 20) {
			startindex = md.Length  - 21;
			endindex =  md.Length;
		} else {
			startindex = index - 10;
			endindex = index + 9;
		}

		for (int i = 0; i < 20 ; i++) {
			//sum += md [i+startindex ];
			//avgs[i]= md [i+startindex ];
			if (startindex > 0) {
				mdnow = md [i + startindex] as MusicData;
				mdlast = md [i + startindex - 1] as MusicData;
				avgIncreases [i] = mdnow.Average - mdlast.Average;
				sum += avgIncreases [i];
			} else {
				avgIncreases [i] = 0;
				sum += 0;
			}
		}
		//float avg = sum / 20;
		return sum/20;
	}

	void searchBeat()
	{
//		for (int i = 20; i <  md.Length; i++) {
//			if (md [i] - md [i - 1] > calcAverage [i] * 2) {
//				md[i].on
//			}
//		}


	}
}
