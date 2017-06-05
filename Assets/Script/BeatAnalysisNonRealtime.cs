using UnityEngine;
using System.Collections;
using System.IO;

public class BeatAnalysisNonRealtime : MonoBehaviour {
	AudioSource _audio;
	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>

	public int BufferSize = 256;//每节拍之间的帧数
	public int numBands =8;//分频段的数量
	public int numSubdivide=1;//细分的段数，针对变化bpm的音乐



	float[,] RecAvgInBandInc;//存分频段每帧增长值
	float[,] RecAvgInBand;//存musicarraylist
	float[] wavelengths;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//



	[SerializeField ]
	float decay = 0.997f;//衰减?
	[SerializeField ]
	int bandlength = 64;//衰减?
	[Range (0.5f,3)]
	public float enegryaddup = 1.2f;



	int beatArrindex=0;
	/// <summary>
	/// ///////////////////////////////////
	/// </summary>









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






	float[] spectrum =new float[64 ];// 每帧采样数组
	float[] output =new float[64 ];// 每帧采样数组
	void Start () {
		//	bandlength = _bandlength;
		InitSetting ();
		//_audio.pitch = 2;
		Debug.Log("2"+Time.frameCount );

	}
	void InitSetting(){

		//		_numBands = BeatAnalysisManager.numBands;//使用numband
		//
		//		_bufferSize = BeatAnalysisManager .bufferSize ;
		//		RecAvgInBandInc=new float[_bufferSize ,_numBands ]; 
		//		RecAvgInBand=new float[_bufferSize ,_numBands ]; 
		//		_SpecSize = BeatAnalysisManager .SpecSize;
		//		spectrum =new float[64 ];
		//		Bands=new float[_numBands+1 ];//频段
		//
		//		lastbeatindexInBand=new int[_numBands];//存各个频段上一次节拍的位置 
		_audio=BeatAnalysisManager ._audio ;//GetComponent<AudioSource> ();
		//		//_AL=BeatAnalysisManager._AL ;
		//		freqlength=(int)Mathf.Floor (_SpecSize *_audio.clip.frequency/AudioSettings.outputSampleRate  );
		//		betweenbeat=Mathf.Clamp (_bufferSize/4,10,256);
		//		betweenbeat = Mathf.Min (betweenbeat, _bufferSize);
		//		//	AudioName = _audio.name;
		//		decay = BeatAnalysisManager .decay;
		//		enegryaddup = BeatAnalysisManager .enegryaddup;
		//
		//		BeatAnalysisManager .BeatArrayList.Clear();
		//		BeatAnalysisManager. MusicArrayList.Clear ();
		//		beatArrindex=0;
		//		CurrentIndex = 0;


		///////////////////****
		//		largeenergys=new float[_numBands ];
		//Debug.Log ("xxxx"+largeenergys[1]);
	}


	//测试用
	//beatmap下落
	public  void step()
	{
		//_audio.play
		_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		//_audio .GetOutputData (output,0);

		string testS = "";
		//string testO="";
		for (int i = 0; i < 64; i++) {
			testS += ", " + spectrum [i].ToString ();
		//	testO += ", " + output [i].ToString ();
		}
		Debug.Log (_audio.timeSamples + "S-- " + testS);
		//Debug.Log ("O-- " + testO);
	//	BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);

		string[] st1 = { "A","B","C"};
		string[] st2 = { "1","2","3","",""};
		st1.CopyTo (st2, -1);
		string t = "";
		for (int i = 0; i < st2.Length; i++) {
			t += st2 [i];
		}
		Debug.Log ("-/-/-   " + t);
	}
	//按键
	public void CheckBeatMap()
	{
		if (_audio.isPlaying) {
			_audio .GetSpectrumData(output, 0, FFTWindow.Rectangular);
			//string testS = "";
			string testO="";
			for (int i = 0; i < 64; i++) {
				testO += ", " + output [i].ToString ();
				//	testO += ", " + output [i].ToString ();
			}
			//Debug.Log ("S-- " + testS);
			Debug.Log ("O-- " + testO);
			//	BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
			_audio.Pause  ();
			_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
			//_audio .GetOutputData (output,0);

			string testS = "";
			//string testO="";
			for (int i = 0; i < 64; i++) {
				testS += ", " + spectrum [i].ToString ();
				//	testO += ", " + output [i].ToString ();
			}
			Debug.Log ("S-- " + testS);

		} else {
			Debug.Log ("p"+_audio.outputAudioMixerGroup);
			_audio.Play ();
			//_audio.outputAudioMixerGroup 
		}

	}
	//测试用

}
