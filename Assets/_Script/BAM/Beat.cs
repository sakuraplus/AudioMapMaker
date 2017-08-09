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
	public GameObject ExpoPartPerfect;
	public GameObject ExpoPartCool;
	public GameObject ExpoPartGood;
	public GameObject ExpoPartMiss;

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
	[SerializeField ]
	GameObject  fbx;
	// Update is called once per frame
	[SerializeField ]
	float timerRot=0;
	void Update () {
		if (onPos) {
			
			if (speedRotate == 0) {
				_animator.SetTrigger ("posReady");
				speedRotate =360/(Beattime-_audio.time ) ;
		
			}
		//	this.transform .LookAt (lookatCamera.position);
			float rotA= Time.deltaTime*speedRotate ;
			Timer.transform.RotateAround (Timer.transform.position, Timer.transform.forward  , rotA );
			timerRot += rotA;
		} else {
			sttt = Time.deltaTime;
			this.transform.position+=new Vector3(0,speedMove*Time.deltaTime ,0);//Translate(new Vector3(0,speedMove*Time.deltaTime ,0));
			speedMove*=1.1f;
			if (this.transform.position .y >= TargPos.y) {
				
				onPos = true;
				if (MoveUpPart) {
					Debug.LogError ("T!!!");
					Debug.Log ("Destroy (MoveUpPart)");
					Destroy (MoveUpPart);
				}
			}
		}
	}
	[SerializeField ]
	float sttt;
	void Awake () {
		_animator =fbx. GetComponent<Animator> ();
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
		//Debug.LogError ("B!!");
		// invote the DestroyNow funtion to run after timeOut seconds
		//Invoke ("DestroyNow", time);


	}

	void OnTriggerEnter(Collider collider) 
	{
		
		if (collider.tag == "destoryzone") {
			Destroy (gameObject);

		}
		if(timerRot>100){
		if (collider.tag == "checkzone" ||collider.tag == "Player" ) {
			Debug.Log ("kkkkkkkkk  "+collider.name );
			CheckState = true;
			//AudioClip ac = GameObject.Find ("MCamera").GetComponent<BeatAnalysisManager > ().beatsoundDefault  as AudioClip;
			_audio.PlayOneShot  (AC);


			//GameObject.Find ("_script").GetComponent < GameManager>()
			calcPoints ();
			DestroyNow ();
			Debug.Log ("ttt  "+collider.name );
		}

		//Debug.Log ("ttt  "+collider.name );

		}
	}

	/// <summary>
	/// 计算得分Calculates the points.
	/// </summary>
	/// <returns>The points.</returns>
	void calcPoints(){
		float ii = 360 - timerRot;
		int point;
		if (Mathf.Abs (ii) <= GameManager.gm.RotationRangePerfect) {
			point = 10;
			if (ExpoPartPerfect) {
				Instantiate (ExpoPartPerfect, transform.position, transform.rotation);
			}
		} else if (ii < 0 - GameManager.gm.RotationRangePerfect) {
			if (ExpoPartMiss) {
				Instantiate (ExpoPartMiss, transform.position, transform.rotation);
			}
			point = 1;
		} else if (ii > GameManager.gm.RotationRangePerfect && ii <= GameManager.gm.RotationRangeCool) {
			if (ExpoPartCool) {
				Instantiate (ExpoPartCool, transform.position, transform.rotation);
			}
			point = 5;
		} else if (ii > GameManager.gm.RotationRangeCool && ii <= GameManager.gm.RotationRangeGood) {
			if (ExpoPartGood) {
				Instantiate (ExpoPartGood, transform.position, transform.rotation);
			}
			point = 3;
		} else  {
			point = 0;
		}
		Debug.LogError ("point>>"+ii+">>"+point);
		if (point > 0) {
			GameManager.gm.AddPoints (point);
		}
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
