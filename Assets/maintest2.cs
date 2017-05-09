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
	public  GameObject ggg;
	float gggtestpos=0;
	float gggscale=1;

	public float  low;
	public float  mid;
	public float high;
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

		if (onbeat) {
			gggtestpos = 10+(0.1f*gggscale);
			ggg.transform.localScale = new Vector3 (gggtestpos, gggtestpos , gggtestpos);
		} else {
			if (gggtestpos > 0) {
				gggtestpos-=gggtestpos/5;
				ggg.transform.localScale  = new Vector3 (gggtestpos, gggtestpos, gggtestpos);
			}
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
		if(Input.GetKeyDown (KeyCode.X   )){

			Debug.Log ( md.Length );
			Debug.Log("SampleRate= "+_audio .clip.frequency  );
			Debug.Log("outputSampleRate= "+AudioSettings .outputSampleRate );
			Debug.Log ((int)Mathf.Floor (64*_audio.clip.frequency/AudioSettings.outputSampleRate  ));
			float  iclips;
			float  iclipsint;
			int speclength=8;
			int cliplength=4;
			string ttt = "";
			string tttint = "";
			for (int i =1; i <= speclength; i++) {
				iclips = Mathf.Log (i+1, speclength) * cliplength;
				ttt+=iclips +",";
				iclipsint = Mathf.Floor (0.5f+iclips)-1;
				tttint += iclipsint+",";
			}
			Debug.Log ("ttt=" + ttt);
			Debug.Log ("tttint=" + tttint);
		

		}
		if(Input.GetKeyDown (KeyCode.Q )){
			float lastavg = 0;
			string stt="";
			for (int i = 0; i <  md.Length; i++) {
				//Debug.Log (md);
				MusicData mmm=(MusicData )md[i];
				stt+=mmm.Average +","+mmm.playtime +" **";

				Debug.DrawRay (new Vector3 (i - 1, mmm.Average * 100, 0), new Vector3 (1, lastavg, 0), Color.blue);
					lastavg=mmm.Average ;
			}
			Debug.Log (stt);

		}
		//end测试用

	
	}

	void recordmusicdata()
	{
	////////////////////////////////////////
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		CurrentFrameAvg = CalcCurrentFrameAvg (spectrum);
		recordAvgInc ();
		CheckBeat ();


	}

	//计算当前帧的平均值
	float CalcCurrentFrameAvg(float[] spec)
	{
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个
		int freqlength=(int)Mathf.Floor (bufferSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
		//Debug.Log  ("freqlength="+_audio.clip.frequency+"/"+AudioSettings.outputSampleRate+"="+freqlength+"///"+Mathf.Log(1));
		float musicenergy=0;
		float musicenergylow=0;
		for (int i =0; i < freqlength; i++)
		{
			musicenergy += spectrum [i];//总音量和，需要取平均数使用
			int icic =(int) Mathf.Floor (0.5f+Mathf.Log(i+2,freqlength )*clips.Length ) -1;//对数方式，将频率分为几段
			//Debug.Log  ("icic="+icic);;
			clips [icic] += spectrum [i];//每个频段的音量和，可能需要求平均数再使用

		}
		musicenergy /= freqlength;//当前帧 平均值
		musicenergylow=(clips[0]+clips[1]);
		//return musicenergy ;
		return musicenergylow ;
	}
	//end 计算当前帧的平均值


	int CurrentIndex=0;//存1024帧中的位置


	//存当前帧前面帧数，或前1024帧平均值
//	void recordAvg(float currentframeavg)
//	{
//		if (CurrentIndex < bufferSize) {
//			//1024帧之前
//			lastAverage  [CurrentIndex] = currentframeavg;
//		} else { 
//			lastAverage  [CurrentIndex - bufferSize] = currentframeavg;
//		}
//		CurrentIndex++;
//		if (CurrentIndex > bufferSize * 2) {
//			CurrentIndex = bufferSize;
//		}
//	}
	//end//存当前帧前面帧数，或前1024帧平均值

	//存当前帧之前或，前1024帧增长值
	float CurrentFrameAvg=0;
	float LastFrameAvg=0;
	void  recordAvgInc()
	{
		if (CurrentIndex == 0) {
			lastAverageInc [0] = 0;
			//lastAverage = CurrentFrameAvg;

		}else if (CurrentIndex < bufferSize) {
			//1024帧之前
			lastAverageInc  [CurrentIndex] = CurrentFrameAvg-LastFrameAvg ;

		} else { 
			//lastAverage  [CurrentIndex - bufferSize] = currentframeavg-LastFrameAvg ;
			for (int i = 0; i < bufferSize-1; ++i) {
				lastAverageInc [i] = lastAverageInc [i + 1];
			}
			lastAverageInc [bufferSize - 1] = CurrentFrameAvg-LastFrameAvg;
		}


		LastFrameAvg = CurrentFrameAvg;
		if (CurrentIndex < bufferSize) {
			CurrentIndex++;
		}	
	}
	//end存当前帧之前或，前1024帧增长值
	int lastbeatindex=0;
	bool  onbeat=false;
	[Range(1,4)]
	public float enegryaddup = 1.2f;
	//检查是否为beat
	void CheckBeat()
	{  
		int largeindex = 0;
		//int lastAverageInc = 0;
		float largeenergy = lastAverageInc [lastbeatindex];
		for (int i = lastbeatindex+1; i < CurrentIndex-1; i++) {
			if (lastAverageInc [i] > largeenergy) {
				largeindex = i;
				largeenergy = lastAverageInc [i];
			}
		}

		if (CurrentFrameAvg > largeenergy*enegryaddup  && (CurrentIndex-lastbeatindex )>bufferSize /4) {
		//if (CurrentFrameAvg > largeenergy*enegryaddup  && (CurrentIndex-lastbeatindex )>bufferSize /8) {
			onbeat = true;
			gggscale = CurrentFrameAvg / largeenergy;
			lastbeatindex = CurrentIndex;//largeindex;
			Debug.LogError   ("onbeat"+lastbeatindex+","+CurrentIndex +","+largeindex+","+largeenergy+"---"+CurrentFrameAvg);

		} else {
			onbeat = false;
			Debug.Log ("checkbeat "+lastbeatindex+","+CurrentIndex +","+largeindex+","+largeenergy+"---"+CurrentFrameAvg);
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
