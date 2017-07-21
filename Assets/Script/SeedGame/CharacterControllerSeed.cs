using UnityEngine;
using System.Collections;
using UnityEngine.UI ;
//using UnityStandardAssets.CrossPlatformInput ;

public class CharacterControllerSeed : MonoBehaviour {
	public string ttt;
	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	public float moveSpeed = 0.01f;
	public float distanceH = 7f;
	public float distanceV = 4f;
	public bool cameragroundlimit;
	public Text tttt;
	public  GameObject  TargetObj;
	public  GameObject followCamera;
	public  GameObject Character;//显示的

	// internal private variables
	[SerializeField ]
	private Quaternion m_CharacterTargetRot;

	public  bool canmove=false;
	void Start() {
		distanceH = TargetObj.transform .position.z - followCamera.transform.position.z;
		distanceV = followCamera.transform.position.y-TargetObj.transform.position.y ;
<<<<<<< HEAD
		m_CharacterTargetRot = TargetObj.transform.localRotation;

		#if UNITY_ANDROID
		Screen.sleepTimeout=SleepTimeout.NeverSleep ;
		if (!Input.gyro.enabled) {
			Input.gyro.updateInterval =0.5f;
			Input.gyro.enabled = true;
		}
		AttachGyro();
		#endif 

	}
	public void test(bool b){
		canmove  = b ;
	}
=======
>>>>>>> parent of c6f31f5... 陀螺仪ok 无限地图ok，need：缓存eledata，风向影响beatmap

		//TargetObj = gameObject.transform;
		m_CharacterTargetRot = TargetObj.transform.localRotation;

	}
	
	void Update() {

		if (Input.GetKey (KeyCode.A )) {
			canmove = true ;
			//targetMove();
		}

		if (canmove) {
			targetMove();
		}
		Character.transform.position=TargetObj.transform.position;
//		LookRotation ();
		LookRotationGyro ();
		CharSmoothRotation ();
	
	}
	void LateUpdate()
	{
		Vector3 nextpos = TargetObj.transform.forward * -1 * distanceH + TargetObj.transform.up * distanceV + TargetObj.transform.position;
		if (cameragroundlimit) {
			nextpos=groundLimit (nextpos);
		}
		followCamera.transform.position = nextpos;

		followCamera.transform.LookAt(TargetObj.transform);
	}
	Vector3 groundLimit(Vector3 _nextpos){
		RaycastHit hit;
		if(Physics.Raycast (followCamera.transform.position ,Vector3.down ,out hit ))
		{
<<<<<<< HEAD

			bool objFound = false;
			GameObject g=hit.collider.gameObject;
			do {
				if (g.tag =="Ground") {
					objFound=true;
					break ;
				}else if(g.transform.parent !=null){
					g=g.transform.parent.gameObject ;
				}else{
					return _nextpos    ;
				}
			} while(objFound == false);

			//if(hit.collider.tag =="Ground"){
=======
			//LayerMask.NameToLayer("Ground")
			if(hit.collider.tag =="Ground"){
>>>>>>> parent of c6f31f5... 陀螺仪ok 无限地图ok，need：缓存eledata，风向影响beatmap
				if (_nextpos.y-1 <hit.point.y ) {
//					Vector3 oldpos = new Vector3 (_nextpos.x, _nextpos.y, _nextpos.z);
					_nextpos.y = hit.point.y + 1f;//方法需要根据体验效果调整****
				//	Debug.Log("r "+hit.distance+"//hitp= "+hit.point+"//old= "+oldpos+"//new= "+_nextpos+"//"+hit.collider.name  );
				}
			}
		}
		return _nextpos;

	}
<<<<<<< HEAD


=======
	public bool startFall = false;
>>>>>>> parent of c6f31f5... 陀螺仪ok 无限地图ok，need：缓存eledata，风向影响beatmap
	void targetMove()
	{

		Vector3 nv = TargetObj.transform.position;
		Vector3 addv = TargetObj.transform.forward;
		RaycastHit hit;



		if (startFall) {
			
			addv.y = Mathf .Min (addv.y, -0.1f);
			Debug.Log ("End-addv=" + addv);
		}
		else if(addv.y>0 && Physics.Raycast (TargetObj.transform.position ,Vector3.down ,out hit ))
		{
			//LayerMask.NameToLayer("Ground")
			if(hit.collider.tag =="Ground"){
				float speedy = 5-TargetObj.transform.position.y + hit.point.y;
				if (speedy > 3) {
					speedy = 5;
				}
				speedy = Mathf.Clamp (speedy, -1,5);
				addv.y = addv.y * speedy /5;
			}
			Debug.Log (TargetObj.transform.forward+"--to addv="+addv);
		}
		nv += (addv * moveSpeed * Time.deltaTime);
<<<<<<< HEAD
=======
		//TargetObj.transform.Translate (TargetObj.transform .forward *moveSpeed * Time.deltaTime);
		//Character.transform.position=TargetObj.transform.position;
>>>>>>> parent of c6f31f5... 陀螺仪ok 无限地图ok，need：缓存eledata，风向影响beatmap
		TargetObj.transform.position=nv;
	}



//	void OnCollisionEnter (Collision newCollision)
//	{
//		Debug.Log ("aaaa"+newCollision.collider.name );
//	}








	public void CharSmoothRotation()
	{
		Character.transform.localRotation = Quaternion.Slerp (Character.transform.localRotation, TargetObj.transform.localRotation ,smoothTime * Time.deltaTime);//smoothTime * Time.deltaTime
	}

	[SerializeField ]
	float yRot ;
	[SerializeField ]
	float xRot ;
	public void LookRotation()
	{
		//get the y and x rotation based on the Input manager
		 yRot = Input.GetAxis("Mouse X") * XSensitivity;
		 xRot += Input.GetAxis("Mouse Y") * YSensitivity;

//		 yRot = Input.GetAxis("Horizontal") * XSensitivity;
//		 xRot += Input.GetAxis("Vertical") * YSensitivity;

		xRot = Mathf.Clamp (xRot, MinimumX, MaximumX);
	//	m_CharacterTargetRot *= Quaternion.Euler (-xRot, yRot, 0f);
		m_CharacterTargetRot *= Quaternion.Euler (0, yRot, 0f);
	//	L_CharacterTargetRot *= Quaternion.Euler (-xRot , 0, 0f);
	
		if(clampVerticalRotation)
		{
			m_CharacterTargetRot = ClampRotationAroundXAxis (m_CharacterTargetRot);
	
		}

		TargetObj.transform.localRotation  = m_CharacterTargetRot;
		//TargetObj.RotateAround (TargetObj.position, Vector3.up , yRot);// ;
		TargetObj.transform.RotateAround (TargetObj.transform.position, TargetObj.transform.right, -xRot);// ;
	

	}
	

	public void LookRotationGyro()
	{
		Gyroscope gy=Input.gyro ;
		tttt.text = gy.attitude.x+","+gy.attitude.y+","+gy.attitude.z+","+gy.attitude.w;
		//get the y and x rotation based on the Input manager
		yRot = gy.attitude .x * XSensitivity;
		xRot +=gy.attitude.y* YSensitivity;

		xRot = Mathf.Clamp (xRot, MinimumX, MaximumX);
		m_CharacterTargetRot *= Quaternion.Euler (0, yRot, 0f);

		if(clampVerticalRotation)
		{
			m_CharacterTargetRot = ClampRotationAroundXAxis (m_CharacterTargetRot);

		}

		TargetObj.transform.localRotation  = m_CharacterTargetRot;
		TargetObj.transform.RotateAround (TargetObj.transform.position, TargetObj.transform.right, -xRot);// ;


	}







	// Some math ... eeck!
	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

	

		return q;
	}
}
