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
public class BeatAnalysisNonRT : MonoBehaviour {
	[SerializeField]
	Text TxTProcess;

	AudioSource _audio;
	int _sampletime=15;
	public int channel = 0;
	public bool isinited=false;
	int _bufferSize = 256;//每节拍之间的帧数
	int _numBands =1;//分频段的数量
	int numSubdivide=1;//细分的段数，针对变化bpm的音乐



	float[,] RecAvgInBandInc;//存分频段每帧增长值
	float[,] RecAvgInBand;//存musicarraylist
	float[] wavelengths;
	int _SpecSize = 0;// = BeatAnalysisManager .SpecSize;


	int freqlength;//在fft结果中，有效频率的数
	FFT fft=new FFT ();

	float[] samples;//存getdata

	float _decay = 0.997f;//衰减?
	public static int bandlength = 32;//节拍间隔



	int outputindex=0;//测试
	void Start () {
		ParaInit();
	}

//	public void playmusic()
//	{
//		_audio.Play ();	
//	}
//
//	public void stopmusic()
//	{
//		_audio.Stop ();
//	}


	int testi=0;
	public void test()
	{

		GetMusicData (0);
		string st=" All = ";
		for (int i=0;i+testi < samples.Length &&i<100000 ;i+=100) {
			st+="\n"+(i+testi )+" , "+samples[i];

		}
		testi += 100000;
		if(!Directory.Exists("Assets/save")) {  
			Directory.CreateDirectory("Assets/save");  
		}  
		string filename="Assets/save/"+testi+_audio.clip.name +System.DateTime.Now.ToString ("dd-hh-mm-ss")+".txt";
		FileStream file = new FileStream(filename, FileMode.Create);  
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(st);  
		file.Write(bts,0,bts.Length);  
		if(file != null) {  
			file.Close();  
		}  
		Debug.Log ("ssssss"+testi+"--"+samples.Length );
		//aud.clip.SetData(samples, 0);
	}




		

	public void Btnseparatedata()
	{
		//BeatAnalysisManager.MAL.Clear ();//初始化mal
		ParaInit ();
		GetMusicData (channel);//获取——audiosource，声道1	
		DataProcess (_sampletime,_SpecSize);
		Debug.Log ("sanple="+samples.Length +"spt="+_sampletime+"bfs="+_bufferSize +"dec="+_decay +"Mal=" + BeatAnalysisManager.MAL.Count);

	}

	public void GetMusicData(int channel=0)
	{		
		
		//获取每帧的数据，取单声道存入samples，channel 获取的声道
		samples=new float[_audio.clip.samples ] ;
		BeatAnalysisManager.MAL.Clear ();//初始化mal


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
	int processbar=0;
	public void DataProcess(int time=100,int samplesize=128 )
	{	
		processbar=10;
		//拆分数据，time即采样间隔时间，单位毫秒，samplesize为fft结果的数据数量=_spec，采样数据数量为sanplesize*2，取每个间隔分段的钱samplesize位
		int SamplePerFrame = time;//Mathf.FloorToInt ( time*_audio.clip.frequency / 1000);//每帧间隔的数据数量

		//从第一个非0数据开始
		int si=0;
		bool offsetfound = false;
		while (!offsetfound && si<samples.Length ) {
			si++;
			if (samples [si] != 0) {
				offsetfound = true;
			}
		}

		if (TxTProcess != null) {
			TxTProcess.text="processing  10 %";
		}
		float offset = si * _audio.clip.frequency;/////////////////第一个非0数据所在的时间

		int NumOfFrame =Mathf.FloorToInt ((samples.Length-si) / SamplePerFrame);//拆分出的数据数量，相当于帧数

		freqlength=Mathf.FloorToInt (samplesize *_audio.clip.frequency/AudioSettings.outputSampleRate  );//有效频率数量
		fft.FFTManagerinit (samplesize*2,FFT.datafilter.unityspec);//初始化fft
		Debug.LogError ("offset="+si+"  =  "+offset +"SamplePerFrame= "+SamplePerFrame+"--systemtime="+Time.realtimeSinceStartup);
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


			float[] bands =new float[_numBands +1];
			bands =CalcCurrentFrameAvg (result);//按频段拆分，计算当前帧平均值，存入band		
			bands [_numBands] = (float)startindex / _audio.clip.frequency;//startindex所在时间，单位为秒	


			BeatAnalysisManager.MAL.Add (bands);//存入MAL
			if (TxTProcess != null) {
				
				processbar= Mathf.Min ( Mathf.RoundToInt  ( 90f*i / NumOfFrame)+ 10, 100);
				TxTProcess.text="processing  "+processbar+" %";
			}

		}//end 采样samplesize*2次	
		Debug.LogError ("end process--systemtime="+Time.realtimeSinceStartup );
	}//end dataprocess ，band存入MAL



	//计算各个频段平均值，存入band
	float[]   CalcCurrentFrameAvg(float[] spec)
	{	
		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个，单声道和立体声似乎没有区别
		float[] _Bands = new float [_numBands + 1];

		//计算平均数及分频段时，只计算有效部分，忽略超出的freqlength
		//遍历数据，根据DictBandlength将每个数据分配搭配对应频段
		for (int i =0; i < freqlength; i++)
		{	
			int ind = BeatAnalysisManager.DictBandRange [i];
			if(ind>=0 && ind <_numBands){
				_Bands[ind]+=spec [i];//每个频段的音量和，可能需要求平均数再使用
			}
		}//end遍历数据，根据DictBandlength将每个数据分配搭配对应频段

		//根据每频段计数计算平均值
		for (int i = 0; i < _numBands ; i++) {			
			_Bands [i] /= BeatAnalysisManager.DictBandlength  [i];
		}

		return _Bands;
		//end根据每频段计数计算平均值
		//Bands [numBands] = _audio.time;//存当前帧的时间
	}
	//end 计算当前帧的平均值，及各个频段平均值




	int beatArrindex=0;
	public void ParaInit()
	{
		isinited = true;
		BeatAnalysisManager.BAL.Clear ();
		_audio=BeatAnalysisManager ._audio ;//GetComponent<AudioSource> ();
		_sampletime=_audio.clip.frequency/ BeatAnalysisManager.samplePerSecond ;
		_decay = BeatAnalysisManager .decay;//衰减
		_SpecSize = BeatAnalysisManager .SpecSize;//fft后数据数量

		_bufferSize = BeatAnalysisManager.bufferSize;
		_numBands = BeatAnalysisManager .numBands;
		RecAvgInBandInc=new float[BeatAnalysisManager.MAL .Count  ,_numBands+1 ]; 
		RecAvgInBand=new float[BeatAnalysisManager.MAL .Count  ,_numBands+1]; 
		bandlength =Mathf.FloorToInt( _bufferSize * 1.5f);//节拍检测时，实际取出的数据量

	}
	public  void StartAnaBeat () {
		//同步manager中的各参数
		ParaInit();
		beatArrindex=0;//池，暂时不用

		CalcIncrement ();//计算并保存增长值，将增长值及绝对值list存为数组
		CalcWavelength ();//将完整音乐细分，计算每段波长
		Debug.Log ("Buffer="+_bufferSize+",,MAl.count="+BeatAnalysisManager.MAL .Count +",,,numBands="+_numBands+"  numSub="+numSubdivide);

		for (int j = 0; j < _numBands; j++) {
		//	Debug.LogError (j);
			CheckBeatInClip (j);///////单频段检测节拍
		}


	}

	void CalcIncrement()
	{//计算并保存增长值，将arraylist存为数组
		for (int j = 0; j <= _numBands; j++) {
			for (int i = 0; i < BeatAnalysisManager.MAL .Count; i++) {
				RecAvgInBand[i,j]=BeatAnalysisManager.MAL [i][j];
				if (i > 0 && j < _numBands) {
					//计算每帧增加值，j=numbands位存储时间，不计算增加值
					RecAvgInBandInc [i, j] = RecAvgInBand [i, j] - RecAvgInBand [i - 1, j];
				} else {
					RecAvgInBandInc [i, j] = RecAvgInBand [i, j];//最后一位，存储时间
				}
			}
		}
	}//end计算并保存增长值，将arraylist存为数组

	void CalcWavelength()
	{		//将完整音乐细分，计算每段波长wavelengths
		int numS = Mathf.FloorToInt (BeatAnalysisManager.MAL .Count / (bandlength  * 4f));
		//numS = Mathf.Min (1, numS);
		numSubdivide = (int)Mathf.Max  (numSubdivide, numS);//至少为1
		//Debug.LogError ("bandlength="+bandlength);
		wavelengths = new float[numSubdivide ];
		for (int inddivide = 0; inddivide < numSubdivide; inddivide++) {
			//计算每段波长
			wavelengths [inddivide] = DetectWavelength (inddivide * BeatAnalysisManager.MAL .Count / numSubdivide);//将完整音乐细分，计算每段波长
			//Debug.Log ("wavelengths[ ]  " + inddivide + " // " + wavelengths [inddivide]);

		}
	}	//end//将完整音乐细分，计算每段波长


	//

	int DetectWavelength(int index)
	{//计算每段波长，使用绝对值计算

		//Debug.LogError ("DetectBeatMap     bandlength=buffersize*1.5="+bandlength +"/"+BeatAnalysisManager.MAL .Count+ "index="+index);
		int avgWavelength = 0;//全部频段的平均波长
		int[] avgwavelengths=new int[_numBands ];
		for (int i = 0; i < _numBands ; i++) {


			int[] peaktimes = new int[4];
			//int index = 0;
			int numPeak = 0; //已经找到的峰值个数

			int peaktimeindex = index;//0;
			int peaktimelast =index;// 0;//上一个峰值的位置
			do {
				//取4个峰值
				float peakvalue=RecAvgInBand [peaktimelast+1,i];//使用绝对值计算
				peaktimes[numPeak ]=peaktimelast+1;//
			
				for (int ind = peaktimelast; ind <( bandlength+peaktimelast) ; ind++) {
					if(ind <BeatAnalysisManager.MAL.Count ){
						//在bandlength中找最高值
						//Debug.Log("ind="+ind+" i="+i);
				
						float Avginclipframe =RecAvgInBand [ind,i];// as float[];

						if(peakvalue<Avginclipframe)
						{
							peakvalue=Avginclipframe*(bandlength-4)/(_decay*bandlength) ;//*衰减

							peaktimes[numPeak ]=ind;//+ ;
							peaktimeindex=ind;//+peaktimelast;						
						}
					}else{
						Debug.Log ("length out of range  i="+i);
						numPeak=3;
						avgwavelengths [i]=_bufferSize ;
						//超出范围则当前频段波长为BufferSize
						break;
					}

				}

			//	Debug.LogError ("peaktimes index="+numPeak +" time="+peaktimes[numPeak ]+" value="+peakvalue+" peaktimeindex="+peaktimeindex );
				peaktimelast=peaktimeindex+Mathf.FloorToInt (bandlength/8) ;
				peaktimelast=Mathf.Min (peaktimelast,BeatAnalysisManager.MAL.Count-2 );
					numPeak++;
			} while(numPeak < 4);


			int avgWavelengthInclip =Mathf.RoundToInt  ( (peaktimes [3] -  peaktimes [0])/3);
			//avgWavelengthInclip /= 3;//单个频段波长
			avgWavelength += avgWavelengthInclip;//计算全部频段平均波长
			avgwavelengths [i] = avgWavelength;//存所有频段的波长
			//	Debug.LogError ("avgWavelengthInclip="+avgWavelengthInclip);
		}

		avgWavelength=Mathf.RoundToInt(avgWavelength / _numBands);//全部频段平均波长
		//Debug.LogError ("avgWavelength="+avgWavelength);
		if (_numBands >= 2) {
			//如果频段较多，则取与平均最接近的频段的波长
			int avgind = 0;
			float avgdiff = Mathf.Abs (avgwavelengths [1] - avgWavelength);
			for (int i = 1; i < _numBands; i++) {
				if (avgdiff > Mathf.Abs (avgwavelengths [i] -avgWavelength)) {
					avgdiff = Mathf.Abs (avgwavelengths [i] - avgWavelength);
					avgind = i;
				}
			}
			avgWavelength = avgwavelengths [avgind];//如果频段较多，则取与平均最接近的频段的波长
		}
		//Debug.LogError ("avgWavelength final "+index+" >>> "+avgWavelength);
		//	
		return  Mathf.FloorToInt ( avgWavelength) ;
	}//end计算每段波长









	//单频段检测节拍
	void CheckBeatInClip(int indBand)
	{  
		//Debug.LogError  ("band start="+indBand );
		string temp="";//测试

		int peaktimeindex = 0;//当前峰值位置
		int peaktimeindexlast = 0;//上一个峰值的位置
		float peakvalue;
		if(BeatAnalysisManager.CheckWithInc ){
			 peakvalue=RecAvgInBandInc  [0,indBand];//第一个用来比较的值
		}else{
			 peakvalue=RecAvgInBand  [0,indBand];//第一个用来比较的值
		}

		int startindex = -1;
		int endindex = -1;
		int _wavelength = -1;

		for (int i =5; i < BeatAnalysisManager.MAL .Count-5; i++) {
			float _wavelengthindex=i/(BeatAnalysisManager.MAL .Count/numSubdivide*1f) ;//当前区域所在的范围，获取波长
			_wavelengthindex = Mathf.Clamp (_wavelengthindex, 0, (numSubdivide - 1));
			_wavelength =(int) wavelengths [Mathf.FloorToInt (_wavelengthindex)];//当前区域波长，即每次采样数量


			startindex = (int)Mathf.Max (0,( _wavelength/8+peaktimeindexlast), (i - _wavelength / 2));
			startindex = (int)Mathf.Min (startindex, BeatAnalysisManager.MAL .Count - 1);
			endindex = (int)Mathf.Min ((startindex+_wavelength ),BeatAnalysisManager.MAL .Count );
			//取当前帧前后一段范围

			int finallength = endindex - startindex;//实际采样的数量

			temp += i + " ,start=" + startindex + " ,end=" + endindex+" length="+finallength+" wavelength="+_wavelength ;//测试
			if(BeatAnalysisManager.CheckWithInc ){
				peakvalue=RecAvgInBandInc[startindex ,indBand ];//根据每帧增长值判断
			}else{
				peakvalue=RecAvgInBand[startindex ,indBand ];//根据每帧绝对值判断
			}
			for (int ind = 0; (ind < finallength && ind+startindex<BeatAnalysisManager.MAL .Count ); ind++) {
				float newvalue;
				if (BeatAnalysisManager.CheckWithInc) {
					newvalue=RecAvgInBandInc[startindex +ind,indBand ];//根据每帧增长值判断
				} else {
					newvalue=RecAvgInBand[startindex +ind,indBand ];//根据每帧绝对值判断
				}


				if (peakvalue < newvalue) {
					peakvalue = newvalue;
					peaktimeindex = startindex + ind;
				}
				peakvalue *= _decay;//衰减
				//outputs [i] =(1-衰减)*(当前-前帧)+outputs [i] *衰减

			}
			temp += " peaktimeindex=" + peaktimeindex;//测试

			if (peaktimeindex - peaktimeindexlast > _wavelength / 4) {//与上一节拍之间的帧数不够大则不认为此处是节拍
				//保存鼓点信息
				if (beatArrindex < BeatAnalysisManager. BAL.Count) {

					BeatAnalysisManager. BAL [beatArrindex].playtime =RecAvgInBand [peaktimeindex, _numBands ];//时间保存在绝对值数组中
					BeatAnalysisManager. BAL [beatArrindex].OnBeat = true;
					BeatAnalysisManager. BAL [beatArrindex].BeatPos = indBand;
				} else {
					MusicData md = new MusicData ();
					md.playtime =RecAvgInBand [peaktimeindex, _numBands ];
					md.OnBeat = true;
					md.BeatPos = indBand;

					BeatAnalysisManager.BAL.Add (md);
				}
				beatArrindex++;

				temp += "   beat";//测试

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
		//Debug.LogError  ("band end="+indBand+" ///"+temp);

	}
	//end单频段检测节拍













}
