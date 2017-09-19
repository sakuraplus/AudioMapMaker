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
	public  AudioClip AC2;
	public  AudioClip AC3;
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
	public  float timerRot=0;
	[SerializeField ]
	public  float timerOut=10;
	void Update () {
		if (Timer) {
			if (onPos) {
			
				if (speedRotate == 0) {
					_animator.SetTrigger ("posReady");

		
				}
				//speedRotate =(360-timerRot)/(Beattime-_audio.time ) ;
				speedRotate = (360) / (Beattime - Borntime);
				this.transform.LookAt (lookatCamera.position);
				float rotA = Time.deltaTime * speedRotate;

				Timer.transform.RotateAround (Timer.transform.position, Timer.transform.forward, rotA);
				timerRot += rotA;
			} else {
				//sttt = Time.deltaTime;
				//this.transform.position+=new Vector3(0,speedMove*Time.deltaTime ,0);//Translate(new Vector3(0,speedMove*Time.deltaTime ,0));
				Vector3 sc = this.transform.localScale;
				if (sc.x * 3f > 1) {
					sc = Vector3.one;
					onPos = true;
				} else {
					sc *= 3f;
				}
				this.transform.localScale = sc;//+=new Vector3(0,speedMove*Time.deltaTime ,0);//

				speedMove *= 1.1f;
				if (this.transform.position.y >= TargPos.y) {
				
					onPos = true;
					if (MoveUpPart) {
						Debug.LogError ("T!!!");
						Debug.Log ("Destroy (MoveUpPart)");
						Destroy (MoveUpPart);
					}
				}
			}
		}
		
	}
	//[SerializeField ]
	//float sttt;
	void Awake () {
		if (fbx) {
			_animator = fbx.GetComponent<Animator> ();
		}
		//StartPos = transform.position;
		if(	GameObject.Find("_script")!=null){
			_audio=GameObject.Find("_script"). GetComponent<AudioSource> ();
		}
		if(	GameObject.Find("_script")!=null){
			if (GameObject.Find ("_script").GetComponent<CharacterControllerSeed > ()) {
				lookatCamera = GameObject.Find ("_script").GetComponent<CharacterControllerSeed > ().followCamera.transform;
			}
		}
		//speedMove =0.2f* (TargPos.y -StartPos.y) / (Beattime-Borntime ); 
		if (MoveUpPart ) {
			// Instantiate an explosion effect at the gameObjects position and rotation
			Instantiate (MoveUpPart, transform.position,transform.rotation  );
		}


//			if (ExpoPartPerfect) {
//				Instantiate (ExpoPartPerfect, transform.position, transform.rotation);
//			}
//			DestroyNow ();

		//Debug.LogError ("B!!");
		// invote the DestroyNow funtion to run after timeOut seconds
		if (Timer) {
			Invoke ("DestroyNow", timerOut);

		}
	}

	void OnTriggerEnter(Collider collider) 
	{
		
		if (collider.tag == "destoryzone") {
			Destroy (gameObject);

			_audio.PlayOneShot  (AC);
		}
		if(timerRot>100){
		if (collider.tag == "checkzone" ||collider.tag == "Player" ) {
		//	Debug.Log ("kkkkkkkkk  "+collider.name );
			//CheckState = true;
			//AudioClip ac = GameObject.Find ("MCamera").GetComponent<BeatAnalysisManager > ().beatsoundDefault  as AudioClip;
			//_audio.PlayOneShot  (AC);


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
		int point=1;
		if (Mathf.Abs (ii) <= GameManager.gm.RotationRangePerfect) {
			point = 10;
			_audio.PlayOneShot  (AC);
//			if (ExpoPartPerfect) {
//				Instantiate (ExpoPartPerfect, transform.position, transform.rotation);
//			}
		} else if (ii < 0 - GameManager.gm.RotationRangePerfect) {
			//_audio.PlayOneShot  (AC3);
//			if (ExpoPartMiss) {
//				Instantiate (ExpoPartMiss, transform.position, transform.rotation);
//			}
			point = 1;
		} else if (ii > GameManager.gm.RotationRangePerfect && ii <= GameManager.gm.RotationRangeCool) {
			_audio.PlayOneShot  (AC2);
//			if (ExpoPartCool) {
//				Instantiate (ExpoPartCool, transform.position, transform.rotation);
//			}
			point = 5;
		} else if (ii > GameManager.gm.RotationRangeCool && ii <= GameManager.gm.RotationRangeGood) {
			_audio.PlayOneShot  (AC3);
//			if (ExpoPartGood) {
//				Instantiate (ExpoPartGood, transform.position, transform.rotation);
//			}
			point = 3;
		} else  {
			point = 0;
		}
		//Debug.LogError ("point>>"+ii+">>"+point+"<"+GameManager.gm.RotationRangePerfect+","+GameManager.gm.RotationRangeGood+">");
//
//		if (point > 0) {
//			CheckState = true;
//			//_audio.PlayOneShot  (AC);
//			DestroyNow ();
//
//			GameManager.gm.AddPoints (point);
//		}
	}

	void DestroyNow ()
	{
		//Debug.Log ("Destroy (born "+Borntime +"- beat "+Beattime+"="+(Beattime-Borntime));

		Destroy(gameObject);
	}
}
