using UnityEngine;
using System.Collections;
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

	public  Transform TargetObj;
	public  Transform followCamera;
	public  Transform Character;//显示的

	// internal private variables
	[SerializeField ]
	private Quaternion m_CharacterTargetRot;
	//private Quaternion m_CameraTargetRot;
	//Quaternion 	L_CharacterTargetRot;
	private bool canmove=false;
	void Start() {
		distanceH = TargetObj.position.z - followCamera.position.z;
		distanceV = followCamera.position.y-TargetObj.position.y ;

		//TargetObj = gameObject.transform;
		m_CharacterTargetRot = TargetObj.localRotation;
	//	L_CharacterTargetRot = TargetObj.rotation ;//.localRotation;
	//	m_CameraTargetRot = followCamera.localRotation;
		//cc = GetComponent<CharacterController> ();
	}
	
	void Update() {

	//	if (Input.GetKey (KeyCode.A )) {
			canmove = true ;
			targetMove();
	//	}

		if (canmove) {
		//targetMove();
		}

		LookRotation ();
		CharSmoothRotation ();
	
	}
	void LateUpdate()
	{
		Vector3 nextpos = TargetObj.forward * -1 * distanceH + TargetObj.up * distanceV + TargetObj.position;
		if (cameragroundlimit) {
			nextpos=groundLimit (nextpos);
		}
		followCamera.transform.position = nextpos;

		followCamera.transform.LookAt(TargetObj);
	}
	Vector3 groundLimit(Vector3 _nextpos){
		RaycastHit hit;
		if(Physics.Raycast (followCamera.position ,Vector3.down ,out hit ))
		{
			//LayerMask.NameToLayer("Ground")
			if(hit.collider.tag =="Ground"){
				if (_nextpos.y-1 <hit.point.y ) {
//					Vector3 oldpos = new Vector3 (_nextpos.x, _nextpos.y, _nextpos.z);
					_nextpos.y = hit.point.y + 1f;//方法需要根据体验效果调整****
				//	Debug.Log("r "+hit.distance+"//hitp= "+hit.point+"//old= "+oldpos+"//new= "+_nextpos+"//"+hit.collider.name  );
				}
			}
		}
		return _nextpos;

	}

	void targetMove()
	{


		TargetObj.Translate (Vector3.forward *moveSpeed * Time.deltaTime);
		Character.position=TargetObj.position;



	}



	void OnCollisionEnter (Collision newCollision)
	{
		Debug.Log ("aaaa"+newCollision.collider.name );
	}








	public void CharSmoothRotation()
	{
		Character.localRotation = Quaternion.Slerp (Character.localRotation, TargetObj.localRotation ,smoothTime * Time.deltaTime);//smoothTime * Time.deltaTime
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

		TargetObj.localRotation  = m_CharacterTargetRot;
		//TargetObj.RotateAround (TargetObj.position, Vector3.up , yRot);// ;
		TargetObj.RotateAround (TargetObj.position, TargetObj.right, -xRot);// ;
	

//		float ff=0; //= m_CharacterTargetRot.ToAngleAxis ();
//		Vector3 vv=Vector3.forward;// m_CharacterTargetRot.ToAngleAxis ();
//		Quaternion cc=Quaternion.Euler(m_CharacterTargetRot.x,m_CharacterTargetRot.y,0);
//		m_CharacterTargetRot.ToAngleAxis(out ff,out vv);
//		ttt =ff+"/"+vv;
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
