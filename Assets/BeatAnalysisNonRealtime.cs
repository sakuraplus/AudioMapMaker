using UnityEngine;
using System.Collections;

public class BeatAnalysisNonRealtime : MonoBehaviour {
	

	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>

	public int BufferSize = 256;//每节拍之间的帧数
	public int numBands =8;//分频段的数量
	public int numSubdivide=1;//细分的段数，针对变化bpm的音乐

	MusicData [] md;//=new MusicData[bufferSize] ;//存音乐信息
	float[] lastAverage;//=new float[bufferSize] ;//存前1024帧
	float[] lastAverageInc;//=new float[bufferSize] ;//存前1024帧增长值
	float[,] RecAvgInBandInc;//存分频段每帧增长值
	float[,] RecAvgInBand;//存musicarraylist
	float[] wavelengths;
	ArrayList BeatArrayList=new ArrayList() ;//存beat信息
	ArrayList MusicArrayList=new ArrayList() ;//存音乐信息
	//


	int[] lastbeatindexInBand;//存各个频段上一次节拍的位置 =new int[8];
	float[] lastframeBands;// = new float[8];//分成8个频段
	//end存当前帧之前或，前1024帧增长值
	int lastbeatindex=0;
	bool  onbeat=false;
	bool  onbeatlow=false;
	bool  onbeatmid=false;
	bool  onbeathigh=false;
	int CurrentIndex=0;//存1024帧中的位置
	//float CurrentFrameAvg=0;
	float LastFrameAvg=0;
	[SerializeField ]
	float decay = 0.997f;//衰减?
	[SerializeField ]
	int bandlength = 64;//衰减?
	[Range (0.5f,3)]
	public float enegryaddup = 1.2f;

	int lastSetbpmframe=-1;
	int Setbpmframe=-1;
	int bpmframe=0;
	bool bpmsetting=false;
	//ArrayList bpmlist;

	/// <summary>
	/// ///////////////////////////////////
	/// </summary>

	public  void StartAnaBeat () {
		BufferSize = BeatAnalysisRealtime.bandlength;//.bufferSize;
		MusicArrayList=BeatAnalysisRealtime.MusicArrayList ;
		numBands = BeatAnalysisRealtime.numBands;
		RecAvgInBandInc=new float[MusicArrayList.Count  ,numBands*2+1 ]; 
		RecAvgInBand=new float[MusicArrayList.Count  ,numBands *2+1]; 
		md=new MusicData[BufferSize] ;//存音乐信息
		lastAverage=new float[BufferSize] ;//存前1024帧
		lastAverageInc=new float[BufferSize] ;//存前1024帧
	
		lastframeBands =new float[numBands ];//分8个频段
		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 
	
		bandlength =(int)Mathf.Floor( BufferSize * 1.5f);
	}

	void CalcIncrement()
	{//计算并保存增长值，将arraylist存为数组
		for (int j = 0; j <= numBands; j++) {
			for (int i = 0; i < MusicArrayList.Count; i++) {
				RecAvgInBand[i,j]=((float[])MusicArrayList[i])[j];
				if (i > 0 &&j<numBands ) {
					//计算每帧增加值，j=numbands位存储时间，不计算增加值
					RecAvgInBandInc [i, j] = RecAvgInBand [i, j] - RecAvgInBand [i - 1, j];
				}
			}
		}
	}

	void CalcWavelength()
	{		//将完整音乐细分，计算每段波长
		int numS = (int)Mathf.Floor (MusicArrayList.Count / (BufferSize * 4));
		numSubdivide = (int)Mathf.Min (numSubdivide, numS);
		wavelengths = new float[numSubdivide ];
		for (int inddivide = 0; inddivide < numSubdivide; inddivide++) {
			wavelengths [inddivide] = DetectWavelength (inddivide * MusicArrayList.Count / numSubdivide);//将完整音乐细分，计算每段波长
			Debug.Log ("wavelengths[ ]  " + inddivide + " // " + wavelengths [inddivide]);

		}
	}	//end//将完整音乐细分，计算每段波长

	

	//分频段，实时检测节拍
	void CheckBeatInClip(int indBand)
	{  
		float[] peaktimes = new float[4];
		//int index = 0;
		//int numPeak = 0; //已经
		int peaktimeindex = 0;
		int peaktimeindexlast = 0;//上一个峰值的位置
		float peakvalue=RecAvgInBandInc  [0,indBand];//第一个用来比较的值
		int startindex = -1;
		int endindex = -1;
		int _wavelength = -1;
		for (int i = 0; i < MusicArrayList.Count; i++) {
			_wavelength =(int) wavelengths [(int)Mathf.Floor (i/(MusicArrayList.Count/numSubdivide ))];
			//			if (i < wavelengths [0] / 2 || startindex < 0) {
			//				startindex = 0;
			//				endindex = _wavelength;
			//			} else if (i >= wavelengths [0] / 2) {
			//			
			//			}

			startindex = (int)Mathf.Max (0,( _wavelength/8+peaktimeindexlast), (i - _wavelength / 2));
			endindex = (int)Mathf.Min ((startindex+_wavelength ),MusicArrayList.Count );
			//int peakindex = startindex;
			int finallength = endindex - startindex;
			peakvalue=RecAvgInBandInc[startindex ,indBand ];
			for (int ind = 0; (ind < finallength && ind+startindex<MusicArrayList.Count ); ind++) {
				float newvalue=RecAvgInBandInc[startindex +ind,indBand ];
				if (peakvalue < newvalue) {
					peakvalue = newvalue;
					peaktimeindex = startindex + ind;
				}
			}
			peaktimeindexlast = peaktimeindex;

			i = peaktimeindexlast;


		}

//		for(int i=0;i<buffersize;i++)


		//peaktimes[numPeak ]=RecAvgInBand [peaktimeindexlast+1,numBands ];
		Debug.Log ("peakvalue1=" + peakvalue + "peaktimelast index=" + peaktimeindexlast);
		//int _buffersize=wavelengths[Mathf.Floor()];
			//for (int ind = index; ((ind < bandlength+index) &&(ind+peaktimelast <MusicArrayList.Count)  ); ind++) {
		for (int ind = peaktimeindexlast; ((ind < bandlength+peaktimeindexlast) &&(ind+peaktimeindexlast <RecAvgInBand.Length )  ); ind++) {
				//在bandlength中找最高值
				//float Avginclipframe =((float[])MusicArrayList [ind+peaktimelast])[i];// as float[];
				//float Avginclipframe =RecAvgInBand [ind+peaktimeindexlast,i];// as float[];
				//					string ssss="";
				//					for(int iii=0;iii<=numclips;iii++){
				//						ssss+=((float[])MusicArrayList [ind+peaktimelast])[iii]+",";
				//					}
				//					Debug.Log("clipinframe "+ssss +"// index="+(ind+peaktimelast)+"//value="+Avginclipframe);
//				if(peakvalue<Avginclipframe)
//				{
//					peakvalue=Avginclipframe*(bandlength-4)/(decay*bandlength) ;//*衰减
//					//peaktimes[numPeak ]=((float[])MusicArrayList [ind+peaktimelast])[numBands ] ;
//					//peaktimes[numPeak ]=RecAvgInBand[ind+peaktimeindexlast,numBands ] ;
//					peaktimeindex=ind+peaktimeindexlast;						
//				}

			}

			//Debug.LogError ("peaktimes index="+numPeak +" time="+peaktimes[numPeak ]+" value="+peakvalue+" peaktimeindex="+peaktimeindex );
			peaktimeindexlast=peaktimeindex+(int)Mathf.Floor (bandlength/8) ;
			//numPeak++;
	//	} while(numPeak < 4);


		//}
		/////////////////


		//for (int ic = 0; ic < numBands; ic++) {
		//遍历所有频段
		int largeindex = 0;
		int largeindexF = 0;
		//int lastAverageInc = 0;
		float largeenergy = RecAvgInBandInc  [lastbeatindexInBand[indBand],indBand];//当前频段上一次节拍的位置

		float variance=0;//方差
		for (int i = lastbeatindexInBand[indBand]+1; i < CurrentIndex-2; i++) {
			//遍历当前频段，上一节拍至当前帧的所有平均值增量
			if (RecAvgInBandInc [i,indBand] *decay> largeenergy) {

				largeindexF = largeindex;
				largeindex = i;//最大值在buffersize中所在的位置
				largeenergy = RecAvgInBandInc [i,indBand];//最大

			}//获取平均数增量最大值，及最大值在buffersize中所在的位置
			variance += Mathf.Pow (RecAvgInBandInc [i, indBand], 2);

			variance /= CurrentIndex - lastbeatindexInBand [indBand] - 1;//上一个节拍至当前节拍之前的节拍，当前频段，音量的方差

		}//end遍历当前频段，上一节拍至当前帧的所有平均值增量，获取最大值及其序号

		float tempVarInc;//计算之前帧的方差与当前帧平方的关系 
		if (variance != 0) {
			tempVarInc = Mathf.Pow (RecAvgInBandInc [CurrentIndex - 1, indBand], 2) / variance;//当前帧增量的平方/当前之前的方差

		} else {
			tempVarInc = 1.1f;//Mathf.Pow (AvgInClipInc [CurrentIndex - 1, ic], 2);
		}
		tempVarInc = 0.5f + tempVarInc / 5000;
		tempVarInc = Mathf.Clamp (tempVarInc, 0.5f, 1.2f);



		//判断节拍///////////////////////
		float beatsincelast = CurrentIndex - lastbeatindexInBand[indBand];//当前频段与上一节拍之间的帧数
		if (beatsincelast > BufferSize / 2) {//与上一节拍之间的帧数不够大则不认为此处是节拍
			//&& bufferSize-largeindexF<4
			if (RecAvgInBandInc [CurrentIndex - 1,indBand]/Mathf.Abs( largeenergy * enegryaddup*tempVarInc)>1 ) {//当前帧增量为buffersize中最大的，且远大于之前的最大值


				//保存鼓点信息
				MusicData md=new MusicData();
				//md.playtime = _audio.time;
				md.OnBeat = true;
				md.BeatPos = indBand;
				BeatArrayList.Add (md);
				//end 保存鼓点信息


				//将频段分为高中低音，确定当前节拍属高中低音
				if (indBand<Mathf .Floor(numBands/3)) {
					onbeatlow = true;

				} else if (indBand>=Mathf .Floor(numBands/3)&&indBand<Mathf .Floor(2*numBands/3)) {
					onbeatmid = true;
				} else {
					onbeathigh = true;
				}
				if (!onbeatlow &&(onbeatmid || onbeathigh)) {

				}
				lastbeatindexInBand[indBand] = CurrentIndex;//记录当前频段的上一个节拍位置，每帧-1

				//测试用



			} else {
				////将频段分为高中低音，确定当前范围中没有节拍
				if (indBand<Mathf .Floor(numBands/3)) {
					onbeatlow = false;
					//_audio.PlayOneShot (mmm);
				} else if (indBand>=Mathf .Floor(numBands/3)&&indBand<Mathf .Floor(2*numBands/3)) {
					onbeatmid = false;
				} else {
					onbeathigh = false;
				}

			}
			////end当前帧增量为buffersize中最大的，且远大于之前的最大值


		}

		//}//end 遍历所有频段
		//Debug.Log (strvariance);

		for (int icc = 0; icc < numBands; icc++) {
			lastbeatindexInBand [icc]--;
			if (lastbeatindexInBand [icc] < 0) {
				lastbeatindexInBand [icc] = 0;
			}
		}


	}
	//end分频段检测是否为节拍





	//

	int DetectWavelength(int index)
	{

		Debug.LogError ("DetectBeatMap     bandlength=buffersize*1.5="+bandlength +"/"+MusicArrayList.Count);
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

				//float[] firstclipinframe = RecAvgInBand [peaktimelast+1] as float[];
				//float[] firstclipinframe = MusicArrayList [peaktimelast+1] as float[];
				float peakvalue=RecAvgInBand [peaktimelast+1,i];
				peaktimes[numPeak ]=RecAvgInBand [peaktimelast+1,numBands ];
				Debug.Log ("peakvalue1=" + peakvalue + "peaktimelast index=" + peaktimelast+" time="+peaktimes[numPeak ]);

				//for (int ind = index; ((ind < bandlength+index) &&(ind+peaktimelast <MusicArrayList.Count)  ); ind++) {
				for (int ind = index; ((ind < bandlength+index) &&(ind+peaktimelast <RecAvgInBand.Length )  ); ind++) {
					//在bandlength中找最高值
					//float Avginclipframe =((float[])MusicArrayList [ind+peaktimelast])[i];// as float[];
					float Avginclipframe =RecAvgInBand [ind+peaktimelast,i];// as float[];
					//					string ssss="";
					//					for(int iii=0;iii<=numclips;iii++){
					//						ssss+=((float[])MusicArrayList [ind+peaktimelast])[iii]+",";
					//					}
					//					Debug.Log("clipinframe "+ssss +"// index="+(ind+peaktimelast)+"//value="+Avginclipframe);
					if(peakvalue<Avginclipframe)
					{
						peakvalue=Avginclipframe*(bandlength-4)/(decay*bandlength) ;//*衰减
						//peaktimes[numPeak ]=((float[])MusicArrayList [ind+peaktimelast])[numBands ] ;
						peaktimes[numPeak ]=RecAvgInBand[ind+peaktimelast,numBands ] ;
						peaktimeindex=ind+peaktimelast;						
					}

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
		return (int)Mathf.Floor ( avgWavelength) ;
	}


	//根据实时采集到数据生成map
	public void DrawBeatMap()
	{
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
	}
	//end根据实时采集到数据生成map


	//保存json格式化的map
	void Save(string jsonstr) {  

//		if(!Directory.Exists("Assets/save")) {  
//			Directory.CreateDirectory("Assets/save");  
//		}  
//		string filename="Assets/save/"+_audio.clip.name +System.DateTime.Now.ToString ("dd-hh-mm-ss")+".json";
//		FileStream file = new FileStream(filename, FileMode.Create);  
//		byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonstr);  
//		file.Write(bts,0,bts.Length);  
//		if(file != null) {  
//			file.Close();  
//		}  
	}  
	//end保存json格式化的map

	public  void Btnload() {
		//load(BeatMapDataJson) ;
	}

	//从json生成map



	//测试用
	//beatmap下落
	void PlayBeatMap()
	{
		//BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
	}
	//按键
	public void CheckBeatMap()
	{
		Debug.Log(MusicArrayList.Count);
		//DetectWavelength  ();

	}
	//测试用

}
