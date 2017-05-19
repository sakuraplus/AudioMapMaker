﻿using UnityEngine;
using System.Collections;
using System.IO;

public class BeatAnalysisNonRealtime : MonoBehaviour {
	

	/// <summary>
	/// ////////////////////////////////////////////
	/// </summary>

	public int BufferSize = 256;//每节拍之间的帧数
	public int numBands =8;//分频段的数量
	public int numSubdivide=1;//细分的段数，针对变化bpm的音乐

	//MusicData [] md;//=new MusicData[bufferSize] ;//存音乐信息
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
	//	md=new MusicData[BufferSize] ;//存音乐信息
		lastAverage=new float[BufferSize] ;//存前1024帧
		lastAverageInc=new float[BufferSize] ;//存前1024帧
	
		lastframeBands =new float[numBands ];//分8个频段
		lastbeatindexInBand=new int[numBands];//存各个频段上一次节拍的位置 
	
		bandlength =(int)Mathf.Floor( BufferSize * 1.5f);

		Debug.Log (BufferSize+",,"+MusicArrayList.Count +",,,"+numBands);
		CalcIncrement ();
		CalcWavelength ();
		for (int j = 0; j < numBands; j++) {
			Debug.LogError (j);
			CheckBeatInClip (j);/////////////////////////
		}
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
		int numS = (int)Mathf.Floor (MusicArrayList.Count / (bandlength  * 4f));
		numSubdivide = (int)Mathf.Min (numSubdivide, numS);
		Debug.LogError ("bandlength="+bandlength);
		wavelengths = new float[numSubdivide ];
		for (int inddivide = 0; inddivide < numSubdivide; inddivide++) {
			wavelengths [inddivide] = DetectWavelength (inddivide * MusicArrayList.Count / numSubdivide);//将完整音乐细分，计算每段波长
			Debug.Log ("wavelengths[ ]  " + inddivide + " // " + wavelengths [inddivide]);

		}
	}	//end//将完整音乐细分，计算每段波长


	//

	int DetectWavelength(int index)
	{

		Debug.LogError ("DetectBeatMap     bandlength=buffersize*1.5="+bandlength +"/"+MusicArrayList.Count+ "index="+index);
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
						//float Avginclipframe =((float[])MusicArrayList [ind+peaktimelast])[i];// as float[];
					//	Debug.Log("RecAvgInBand index= "+ind+">-->"+peaktimelast+"  numbands="+i);
						float Avginclipframe =RecAvgInBand [ind,i];// as float[];

						if(peakvalue<Avginclipframe)
						{
							peakvalue=Avginclipframe*(bandlength-4)/(decay*bandlength) ;//*衰减
							//peaktimes[numPeak ]=((float[])MusicArrayList [ind+peaktimelast])[numBands ] ;
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
		Debug.LogWarning ("band="+indBand+"  BeatArrayList count "+BeatArrayList.Count );
		string temp="";

		int peaktimeindex = 0;
		int peaktimeindexlast = 0;//上一个峰值的位置
		//float peakvalue=RecAvgInBandInc  [0,indBand];//第一个用来比较的值
		float peakvalue=RecAvgInBand  [0,indBand];//第一个用来比较的值
		int startindex = -1;
		int endindex = -1;
		int _wavelength = -1;
		for (int i = 0; i < MusicArrayList.Count-5; i++) {
			float _wavelengthindex=i/(MusicArrayList.Count/numSubdivide*1f) ;
			_wavelengthindex = Mathf.Clamp (_wavelengthindex, 0, (numSubdivide - 1));
			_wavelength =(int) wavelengths [(int)Mathf.Floor (_wavelengthindex)];


			startindex = (int)Mathf.Max (0,( _wavelength/8+peaktimeindexlast), (i - _wavelength / 2));
			startindex = (int)Mathf.Min (startindex, MusicArrayList.Count - 1);
			endindex = (int)Mathf.Min ((startindex+_wavelength ),MusicArrayList.Count );


			int finallength = endindex - startindex;

			temp += i + " ,start=" + startindex + " ,end=" + endindex+" length="+finallength+" wavelength="+_wavelength ;

			//peakvalue=RecAvgInBandInc[startindex ,indBand ];
			peakvalue=RecAvgInBand[startindex ,indBand ];
			for (int ind = 0; (ind < finallength && ind+startindex<MusicArrayList.Count ); ind++) {
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
			if(peaktimeindex-peaktimeindexlast>_wavelength/4){
				//保存鼓点信息
				MusicData md=new MusicData();
				md.playtime = RecAvgInBand[peaktimeindex,numBands  ];//_audio.time;
				md.OnBeat = true;
				md.BeatPos = indBand;
				BeatArrayList.Add (md);
				temp+="   beat";
				//end 保存鼓点信息
			}
			temp+="\n";
			peaktimeindexlast = peaktimeindex;

			if (i < peaktimeindexlast) {
				i = peaktimeindexlast;
				//Debug.LogWarning (">>> i="+i+"  last="+peaktimeindexlast+"from"+startindex+" to "+endindex);
			}
		
		}
		Debug.LogWarning ("band="+indBand+"  BeatArrayList count "+BeatArrayList.Count +" ////"+temp);
	}
	//end单频段检测节拍







	//根据实时采集到数据生成map
	GameObject BeatMapContainer;
	GameObject[] GameObjBeats;// = new GameObject[beatlist.Count ];
	public float speed=2000;
	public GameObject BeatPfb;
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
			BeatMapContainer.name="nonrealtime";

			//GameObjBeats = new GameObject[BeatArrayList.Count ];
			BeatMapContainer.transform.position=new Vector3(50,0-speed/100,0);
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

	//保存json格式化的map
	void Save(string jsonstr) {  

		if(!Directory.Exists("Assets/save")) {  
			Directory.CreateDirectory("Assets/save");  
		}  
		string filename="Assets/save/NonRT"+BeatAnalysisRealtime.AudioName  +System.DateTime.Now.ToString ("dd-hh-mm-ss")+".json";
		FileStream file = new FileStream(filename, FileMode.Create);  
		byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonstr);  
		file.Write(bts,0,bts.Length);  
		if(file != null) {  
			file.Close();  
		}  
	} 

	public  void Btnload() {
		//load(BeatMapDataJson) ;
	}

	//从json生成map



	//测试用
	//beatmap下落
	void PlayBeatMap()
	{
	//	BeatMapContainer.transform.position-=new Vector3 ( 0, speed * Time.deltaTime,0);
	}
	//按键
	public void CheckBeatMap()
	{
		Debug.Log(MusicArrayList.Count);
		//DetectWavelength  ();

	}
	//测试用

}
