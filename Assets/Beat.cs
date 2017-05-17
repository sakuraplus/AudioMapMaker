using UnityEngine;
using System.Collections;

public class Beat : MonoBehaviour {
	public float Destorytime;
	public bool  CheckState;
//	[SerializeField ]
	AudioSource _audio;
	// Update is called once per frame
//	void Update () {
//	
//	}

	void Awake () {
		_audio = GetComponent<AudioSource> ();
		// invote the DestroyNow funtion to run after timeOut seconds
	//	Invoke ("DestroyNow", time);
	}
	void OnCollisionEnter (Collision newCollision)
	{
		Debug.Log ("ccc  "+newCollision.collider .name  );
	}
	void OnTriggerEnter(Collider collider) 
	{
		if (collider.tag == "destoryzone") {
			Destroy (gameObject);
		}
		if (collider.tag == "checkzone") {
			CheckState = true;
			_audio.Play ();
//			Debug.Log(GameObject.Find ("MCamera") );
//			GameObject ggg = GameObject.Find ("MCamera");
//			Debug.Log(ggg.GetComponent<maintest2>()._audio.time  );
			Debug.Log(Destorytime+"//"+GameObject.Find ("MCamera").GetComponent<BeatAnalysisRealtime>()._audio.time);
		}
	//	Debug.Log ("ttt  "+collider.name );

	}



	void DestroyNow ()
	{

		// destory the game Object
		Destroy(gameObject);
	}
}
