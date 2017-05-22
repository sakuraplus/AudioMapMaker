using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;





/// <summary>
///实时显示节拍
/// 
/// </summary>
public class ShowBeatRealtime : MonoBehaviour {
	
	public AudioClip[] beatsoundFX;
	public AudioClip beatsoundDefault;
	AudioSource _audio;
	public GameObject[] beatObj;
	public  GameObject beatObjDefault;
	bool playSFX=false;
	bool showBeatObj=false;

	float[] gameobjScale;



	void Start () {

		_audio = BeatAnalysisManager._audio;
		BeatAnalysisRealtime  BAR = FindObjectOfType<BeatAnalysisRealtime > ();
		BAR.onBeat.AddListener  (onOnbeatDetected);


	
		gameobjScale=new float[beatObj.Length ];

		if (beatObj.Length > 0) {
			showBeatObj = true;
			for (int i = 0; i < beatObj.Length; i++) {
				gameobjScale [i] = 0;
				if (beatObj [i] == null) {
					beatObj [i] = beatObjDefault;
				}
			}
		}
		if (beatsoundFX .Length > 0) {
			playSFX  = true;
			for (int i = 0; i < beatsoundFX.Length; i++) {
				if (beatsoundFX [i] == null) {
					beatsoundFX [i] = beatsoundDefault;
				}
			}
		}
		Debug.Log("2"+Time.frameCount );
		Debug.Log("2"+Time.captureFramerate );


	}
	void onOnbeatDetected(int i){
		if (i < beatsoundFX.Length && playSFX ) {
			_audio.PlayOneShot (beatsoundFX[i]);
		}
		if (i < beatObj.Length && showBeatObj ) {
			gameobjScale [i] = 10;
		}
	}
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < beatObj.Length; i++) {
			beatObj [i].transform.localScale = new Vector3 (gameobjScale [i], gameobjScale [i], gameobjScale [i]);
			gameobjScale [i] -= gameobjScale [i] / 5;
		}
	
	}




}
