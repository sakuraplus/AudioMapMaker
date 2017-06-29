using UnityEngine;
using System.Collections;

public class Beat : MonoBehaviour {
	public float Destorytime;
	public float Borntime;
	public bool  CheckState;
	//[SerializeField ]
	 AudioSource _audio;
	//[SerializeField ]
	public  AudioClip AC;
	// Update is called once per frame
//	void Update () {
//	
//	}

	void Awake () {
		_audio =GameObject.Find("_script"). GetComponent<AudioSource> ();
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
			Debug.Log ("kkkkkkkkk  "+collider.name );
		}
		if (collider.tag == "checkzone" ||collider.tag == "Player" ) {
			CheckState = true;
			//AudioClip ac = GameObject.Find ("MCamera").GetComponent<BeatAnalysisManager > ().beatsoundDefault  as AudioClip;
			_audio.PlayOneShot  (AC);
			Destroy (gameObject);
			Debug.Log ("ttt  "+collider.name );
		}

		//Debug.Log ("ttt  "+collider.name );

	}



	void DestroyNow ()
	{

		// destory the game Object
		Destroy(gameObject);
	}
}
