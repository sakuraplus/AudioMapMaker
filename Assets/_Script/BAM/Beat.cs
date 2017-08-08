using UnityEngine;
using System.Collections;

public class Beat : MonoBehaviour {
	/// <summary>
	/// 节拍所在时间
	/// </summary>
	public float Beattime;
	/// <summary>
	/// 生成时间，audio。time
	/// </summary>
	public float Borntime;
	public bool  CheckState;
	/// <summary>
	/// 最终位置
	/// </summary>
	public Vector3 TargPos;
	/// <summary>
	/// 起始位置
	/// </summary>
	public Vector3 StartPos;
	//[SerializeField ]

	 AudioSource _audio;
	//[SerializeField ]
	public  AudioClip AC;
	/// <summary>
	/// 碰撞后特效
	/// </summary>
	public GameObject ExpoPart;
	public GameObject MoveUpPart;
	 Animator _animator;
	/// <summary>
	/// 计时器
	/// </summary>
	public GameObject Timer;
	/// <summary>
	/// 保持面朝camera
	/// </summary>
	public Transform  lookatCamera;
	/// <summary>
	/// 是否到达指定位置
	/// </summary>
	public bool onPos=false;
	[SerializeField ]
	float speedRotate=0;
	[SerializeField ]
	float speedMove=0;
	// Update is called once per frame
	void Update () {
		if (onPos) {
			if (speedRotate == 0) {
				speedRotate =360/(_audio.time -Beattime) ;
				if (MoveUpPart) {
					Debug.Log ("Destroy (MoveUpPart)");
					Destroy (MoveUpPart);
				}
			//	if (_animator) {
					_animator.SetTrigger ("posReady");
				//}
			}
		//	this.transform .LookAt (lookatCamera.position);
			//float A = 1; //(Destorytime- _audio .time )*360 / speedRotate;
		//	Debug.Log(A+" / "+speedRotate );
		//	A++;
			Timer.transform.RotateAround (Timer.transform.position, Timer.transform.forward  ,  Time.deltaTime*speedRotate );
		} else {
			sttt = Time.deltaTime;
			this.transform.position+=new Vector3(0,speedMove*Time.deltaTime ,0);//Translate(new Vector3(0,speedMove*Time.deltaTime ,0));
			if (this.transform.position .y >= TargPos.y) {
			
				onPos = true;
			}
		}
	}
	[SerializeField ]
	float sttt;
	void Awake () {
		_animator = GetComponent<Animator> ();
		//StartPos = transform.position;
		if(	GameObject.Find("_script")!=null){
			_audio=GameObject.Find("_script"). GetComponent<AudioSource> ();
		}
		if(	GameObject.Find("_script")!=null){
			lookatCamera =GameObject.Find("_script"). GetComponent<CharacterControllerSeed > ().followCamera.transform  ;
		}
		//speedMove =0.2f* (TargPos.y -StartPos.y) / (Beattime-Borntime ); 
		if (MoveUpPart ) {
			// Instantiate an explosion effect at the gameObjects position and rotation
			Instantiate (MoveUpPart, transform.position,transform.rotation  );
		}

		// invote the DestroyNow funtion to run after timeOut seconds
		//Invoke ("DestroyNow", time);
	}

	void OnTriggerEnter(Collider collider) 
	{
		
		if (collider.tag == "destoryzone") {
			Destroy (gameObject);

		}
		if (collider.tag == "checkzone" ||collider.tag == "Player" ) {
			Debug.Log ("kkkkkkkkk  "+collider.name );
			CheckState = true;
			//AudioClip ac = GameObject.Find ("MCamera").GetComponent<BeatAnalysisManager > ().beatsoundDefault  as AudioClip;
			_audio.PlayOneShot  (AC);
			if (ExpoPart) {
				// Instantiate an explosion effect at the gameObjects position and rotation
				Instantiate (ExpoPart, transform.position, transform.rotation);
			}
			if (MoveUpPart) {
				Debug.Log ("Destroy (MoveUpPart)");
				DestroyImmediate (MoveUpPart);
			}
			GameObject.Find ("_script").GetComponent < GameManager>().AddPoints (1);
			Destroy (gameObject);
			Debug.Log ("ttt  "+collider.name );
		}

		//Debug.Log ("ttt  "+collider.name );

	}



	void DestroyNow ()
	{

		// destory the game Object
		if (MoveUpPart) {
			Debug.Log ("Destroy (MoveUpPart)");
			Destroy (MoveUpPart);
		}
		Destroy(gameObject);
	}
}
