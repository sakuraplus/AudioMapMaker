using UnityEngine;
using System.Collections;
public class MusicData
{
	public float playtime = 0;
	public float Average = 0;
	public bool onbeat = false;
}

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

	public float  AvgLow;
	public float  AvgMid;
	public float AvgHigh;
	public float superhigh;
	public string strclips;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>
	public int bufferSize = 256;
	MusicData [] md;//=new MusicData[bufferSize] ;//存音乐信息
	float[] lastAverage;//=new float[bufferSize] ;//存前1024帧
	float[] lastAverageInc;//=new float[bufferSize] ;//存前1024帧增长值
	ArrayList beatlist=new ArrayList() ;//存音乐信息
	float[] spectrum ;// new float[bufferSize ];//每帧采样64个
	float[] clips = new float[8];//分成8个频段

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

		 md=new MusicData[bufferSize] ;//存音乐信息
		lastAverage=new float[bufferSize] ;//存前1024帧
		lastAverageInc=new float[bufferSize] ;//存前1024帧
		spectrum =new float[bufferSize ];

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

		if (!onbeat) {
			onbeatlow = false;
			onbeatmid = false;
			onbeathigh = false;
		} else {
			_audio.PlayOneShot (mmm);
		}
		//测试用
		if(Input.GetKeyDown (KeyCode.A)){
			//	_audio.Stop();
			Debug.Log(_audio.timeSamples+"/stop//"+_audio.time+"///"+Time.time );
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			//if (!_audio.isPlaying) {
				_audio.Play ();
				_audio.pitch = 1.5f;

		}
	
		if(Input.GetKeyDown (KeyCode.S  )){
			_audio.Pause  ();

		}


		//end测试用

	
	}

	void recordmusicdata()
	{
	////////////////////////////////////////
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		CurrentFrameAvg =CalcCurrentFrameAvg (spectrum);// musicenergy;
		AvgLow = (clips [0] )/1;
		AvgMid = (clips [2] + clips [3]+ clips [4]+clips [1])/4;
		AvgHigh = ( clips [5] + clips [6] + clips [7])/3;
		superhigh =( clips [5] + clips [6] + clips [7]);

		strclips=AvgLow+","+AvgMid+","+AvgHigh;
		recordAvgInc ();
		CheckBeat ();


	}

	//计算当前帧的平均值
	float   CalcCurrentFrameAvg(float[] spec)
	{
		
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个
		int freqlength=(int)Mathf.Floor (bufferSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
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


	int CurrentIndex=0;//存1024帧中的位置



	//存当前帧之前或，前1024帧增长值
	public  GameObject drawline;
	public  float  lastAvgInc;
	void  recordAvgInc()
	{
		if (CurrentIndex == 0) {
			lastAverageInc [0] = 0;
			//lastAverage = CurrentFrameAvg;

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
		//int lastAverageInc = 0;
		float largeenergy = lastAverageInc [lastbeatindex];
		for (int i = lastbeatindex+1; i < CurrentIndex-2; i++) {
			if (lastAverageInc [i] > largeenergy) {
				largeindex = i;
				largeenergy = lastAverageInc [i];
			}
		}

		float beatsincelast = CurrentIndex - lastbeatindex;
		if (beatsincelast > bufferSize / 4) {
			if (lastAverageInc [CurrentIndex - 1] > largeenergy * enegryaddup) {
				//if (CurrentFrameAvg > largeenergy*enegryaddup  && (CurrentIndex-lastbeatindex )>bufferSize /8) {

				onbeat = true;
				if (AvgLow > AvgMid && AvgLow > AvgHigh) {
					onbeatlow = true;
				} else if (AvgMid > AvgLow && AvgMid > AvgHigh) {
					onbeatmid = true;
				} else {
					onbeathigh = true;
				}
				gggscale = CurrentFrameAvg / largeenergy;
				lastbeatindex = CurrentIndex;//largeindex;
				timestep = _audio.timeSamples - timelast;
				timelast = _audio.timeSamples;
				Debug.LogError ("onbeat  " + _audio.timeSamples + "-" + lastbeatindex + "," + largeindex + "," + largeenergy + "---" + lastAverageInc [CurrentIndex - 1]);

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
