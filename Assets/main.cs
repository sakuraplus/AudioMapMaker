using UnityEngine;
using System.Collections;
public class MusicData
{
	public float playtime = 0;
	public float Average = 0;
}

public class main : MonoBehaviour {
	AudioSource _audio;
	public AudioClip mmm;
	public  GameObject ggg;

	[Range (0,64)]
	public int hhh;
	[Range (0,64)]
	public int lll;
	public float  low;
	public float  mid;
	public float high;
	public float superhigh;
	public string strclips;


	ArrayList md=new ArrayList() ;//存音乐信息
	ArrayList lastAverage=new ArrayList() ;//存前10帧
	//MusicData mdarr=new ArrayList 
	// Use this for initialization
	void Start () {
		_audio=GetComponent<AudioSource> ();
		_audio.Play();
		_audio.loop = true;
		Application.targetFrameRate = 1;
	}

	// Update is called once per frame
	void Update () {
		//测试用
		if(Input.GetKeyDown (KeyCode.A)){
			//	_audio.Stop();
			Debug.Log(_audio.timeSamples+"/stop//"+_audio.time+"///"+Time.time );
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			//if (!_audio.isPlaying) {
				_audio.Play ();
				_audio.pitch = 1.5f;



				//playtime = Time.time;
				//Debug.Log (_audio.timeSamples + "/Play//" + _audio.time + "///" + Time.time);
			//}
		}
		if (_audio.isPlaying) {
			recordmusicdata ();
		} else {
			Debug.Log ("stop");
		}
		if(Input.GetKeyDown (KeyCode.S  )){
			_audio.Pause  ();
			//pausetime = Time.time;
			//Debug.Log(_audio.timeSamples+"/Pause//"+_audio.time+"///"+Time.time );
		}
		if(Input.GetKeyDown (KeyCode.X   )){

			Debug.Log (md.Count );
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
			//Debug.Log("outputSampleRate= "+_audio..outputSampleRate );
			//unpausetime = Time.time;
			//_audio.UnPause   ();
			//Debug.Log(_audio.timeSamples+"/UnPause//"+_audio.time+"///"+Time.time );

		}
		if(Input.GetKeyDown (KeyCode.Q )){
			float lastavg = 0;
			string stt="";
			for (int i = 0; i < md.Count; i++) {
				//Debug.Log (md);
				MusicData mmm=(MusicData )md[i];
				stt+=mmm.Average +","+mmm.playtime +" **";

				Debug.DrawRay (new Vector3 (i - 1, mmm.Average * 100, 0), new Vector3 (1, lastavg, 0), Color.blue);
					lastavg=mmm.Average ;
			}
			Debug.Log (stt);
			//for (int i = 0; i < md.Count; i++) {
				
			//}
			//_audio.UnPause   ();

		}
		//end测试用

	
	}

	void recordmusicdata()
	{
		float[] spectrum = new float[64];//每帧采样64个
		float[] clips = new float[8];//分成8个频段
		//float[] spectrumlow = new float[64];
		low = 0;//测试用
		high = 0;//测试用
		mid = 0;//测试用
		float musicenergy = 0;
		strclips = "";//测试用

		_audio .GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		//AudioListener.GetSpectrumData(spectrumlow, 0, FFTWindow.Rectangular);
		//	int recfreq =spectrum.Length;// (int)Mathf.Floor (spectrum.Length*_audio.clip.frequency/AudioSettings.outputSampleRate  );// 
		//int recfreq =(int)Mathf.Floor (spectrum.Length*_audio.clip.frequency/AudioSettings.outputSampleRate  );// 

		//当音频频率没有达到48000时，根据音频频率取全部采样中的前几个
		int freqlength=(int)Mathf.Floor (spectrum.Length*_audio.clip.frequency/AudioSettings.outputSampleRate  );
		//for (int i = 1; i < spectrum.Length - 1; i++)
		for (int i = 1; i < freqlength - 1; i++)
		{
			Debug.DrawLine(new Vector3(i - 1, 100*spectrum[i], 0), new Vector3(i, 100*spectrum[i + 1], 0), Color.red);
			//int icic =(int) Mathf.Floor (Mathf.Log((i*recfreq/spectrum.Length)+1,spectrum.Length )*clips.Length ) ;

			int icic =(int) Mathf.Floor (0.5f+Mathf.Log(i+1,freqlength )*clips.Length ) -1;//对数方式，将频率分为几段
			//Debug.Log  ("icic="+icic);
			clips [icic] += spectrum [i-1];//每个频段的音量和，可能需要求平均数再使用
			//low= spectrum[i]*100000;
			musicenergy += spectrum [i];//总音量和，需要取平均数使用
		}
		low = clips [0];
		mid = (clips [1] + clips [2]);
		high =  (clips [3] + clips [4]);
		superhigh =( clips [5] + clips [6] + clips [7]);

		float c=clips[0];
		int maxind = 0;
		for (int i = 0; i < clips.Length; i++) {
			if (clips [i] > c) {
				c = clips [i ];
				maxind = i;
			}
			//strclips += clips[i].ToString ()+"/";
		}
		strclips = maxind+"   "+c.ToString ();

		//low = musicenergy;
		musicenergy /= spectrum.Length;//总音量和，需要取平均数使用
		//high = musicenergy*100000;

		MusicData _md = new MusicData ();//存播放时间与当前音量总和
		_md.Average = musicenergy;
		_md.playtime = _audio.time;
		md.Add (_md);

		//测试用
		float tt = low + high + mid;
		low /= tt;
		high /= tt;
		mid /= tt;
		if (low > mid && low > high) {
			ggg.transform.position = new Vector3 (0, -10, 0);
		} else if (mid > low && mid > high) {
			ggg.transform.position = new Vector3 (32, -10, 0);
		} else {
			ggg.transform.position=new Vector3 (64, -10, 0);
		}
		//测试用end
	}


	void calcAverage()
	{

		int lastindex = md.Count - 20;
		for (int i = lastindex; i < md.Count; i++) {
		
		}

	}

}
