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
	ArrayList md=new ArrayList() ;
	//MusicData mdarr=new ArrayList 
	// Use this for initialization
	void Start () {
		_audio=GetComponent<AudioSource> ();
		_audio.PlayOneShot(mmm);
		Application.targetFrameRate = 1;
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown (KeyCode.A)){
			//	_audio.Stop();
			Debug.Log(_audio.timeSamples+"/stop//"+_audio.time+"///"+Time.time );
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			if (!_audio.isPlaying) {
				_audio.Play ();
				_audio.pitch = 1.5f;

				//playtime = Time.time;
				//Debug.Log (_audio.timeSamples + "/Play//" + _audio.time + "///" + Time.time);
			}
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
			//Debug.Log((_audio.time+playtime+unpausetime-pausetime  )+"/UnPause//"+_audio.time+"///"+Time.time );
		}


	
	}

	void recordmusicdata()
	{
		float[] spectrum = new float[64];
		//float[] spectrumlow = new float[64];
		low = 0;
		high = 0;
		mid = 0;
		float musicenergy = 0;

		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		//AudioListener.GetSpectrumData(spectrumlow, 0, FFTWindow.Rectangular);

		for (int i = 1; i < spectrum.Length - 1; i++)
		{
			Debug.DrawLine(new Vector3(i - 1, 100*spectrum[i], 0), new Vector3(i, 100*spectrum[i + 1], 0), Color.red);
			lll = Mathf.Min (lll, hhh);
			hhh = Mathf.Max (lll, hhh);
			if (i < lll) {
				low += spectrum [i];
			} else if (i > hhh) {
				high += spectrum [i];
			} else {
				mid += spectrum [i];
			}
			//low= spectrum[i]*100000;
			musicenergy += spectrum [i];
		}
		//low = musicenergy;
		musicenergy /= spectrum.Length;
		//high = musicenergy*100000;
		MusicData _md = new MusicData ();
		_md.Average = musicenergy;
		_md.playtime = _audio.time;
		md.Add (_md);
		if (low > mid && low > high) {
			ggg.transform.position = new Vector3 (0, 0, 0);
		} else if (mid > low && mid > high) {
			ggg.transform.position = new Vector3 (0, 50, 0);
		} else {
			ggg.transform.position=new Vector3 (0, 100, 0);
		}

	}

}
