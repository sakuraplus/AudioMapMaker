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
	private Quaternion m_CharacterTargetRot;


	void Start() {
		distanceH = TargetObj.position.z - followCamera.position.z;
		distanceV = followCamera.position.y-TargetObj.position.y ;

		TargetObj = gameObject.transform;
		m_CharacterTargetRot = TargetObj.localRotation;
		//cc = GetComponent<CharacterController> ();
	}
	
	void Update() {

		if (Input.GetKey (KeyCode.A )) {
			targetMove();
		}


		LookRotation ();
		CharSmoothRotation ();
	
	}
	void LateUpdate()
	{
		Vector3 nextpos = TargetObj.forward * -1 * distanceH + TargetObj.up * distanceV + TargetObj.position;
		if (cameragroundlimit) {
			groundLimit (nextpos);
		}
		followCamera.transform.position = nextpos;

		followCamera.transform.LookAt(TargetObj);
	}
	void groundLimit(Vector3 _nextpos){
		RaycastHit hit;
		if(Physics.Raycast (followCamera.position ,Vector3.down ,out hit ,LayerMask.NameToLayer("Ground")))
		{
			//Debug.Log("r "+hit.distance+"//"+hit.point+"//"+_nextpos );
			if (_nextpos.y-1 <hit.point.y ) {
				_nextpos.y = hit.point.y + 1f;
			}
		}

	}

	void targetMove()
	{


		TargetObj.Translate (Vector3.forward *moveSpeed * Time.deltaTime);
		Character.position=TargetObj.position;


//		Vector3 movementZ = Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime;
//
//		// Determine how much should move in the x-direction
//		Vector3 movementX = Input.GetAxis("Horizontal") * Vector3.right * moveSpeed * Time.deltaTime;
//
//		// Convert combined Vector3 from local space to world space based on the position of the current gameobject (player)
//		Vector3 movement = transform.TransformDirection(movementZ+movementX);
//		
//		// Apply gravity (so the object will fall if not grounded)
//		movement.y -= gravity * Time.deltaTime;
//
//		Debug.Log ("Movement Vector = " + movement);
//
//		// Actually move the character controller in the movement direction
//		myController.Move(movement);
	}



	void OnCollisionEnter (Collision newCollision)
	{
		Debug.Log ("aaaa"+newCollision.collider.name );
	}








	public void CharSmoothRotation()
	{
		Character.localRotation = Quaternion.Slerp (Character.localRotation, TargetObj.localRotation ,smoothTime * Time.deltaTime);//smoothTime * Time.deltaTime
	}

	public void LookRotation()
	{
		//get the y and x rotation based on the Input manager
		float yRot = Input.GetAxis("Mouse X") * XSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

		m_CharacterTargetRot *= Quaternion.Euler (-xRot, yRot, 0f);

		if(clampVerticalRotation)
		{
			m_CharacterTargetRot = ClampRotationAroundXAxis (m_CharacterTargetRot);
		}

		if(smooth) // if smooth, then slerp over time
		{
			TargetObj.localRotation = Quaternion.Slerp (TargetObj.localRotation, m_CharacterTargetRot,
			                                            smoothTime * Time.deltaTime);
		}
		else // not smooth, so just jump
		{
			TargetObj.localRotation = m_CharacterTargetRot;
		}
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
