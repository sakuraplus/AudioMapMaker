using UnityEngine;
using System.Collections;
//using UnityStandardAssets.CrossPlatformInput ;

public class InfiniteMap : MonoBehaviour {

	//public float distanceH = 7f;
	public float distanceV = 4f;
	public bool cameragroundlimit;

	public  Transform TargetObj;
	public  Transform followCamera;


	void Start() {
		//distanceH = TargetObj.position.z - followCamera.position.z;
		distanceV = followCamera.position.y-TargetObj.position.y ;
		followCamera.LookAt (Vector3.down);

	
	}
	
	void Update() {


	
	}
	void LateUpdate()
	{
		Vector3 nextpos = new Vector3(TargetObj.position.x, TargetObj.position.y+distanceV ,TargetObj.position.z);

		followCamera.transform.position = nextpos;

	}
	Vector3 groundLimit(Vector3 _nextpos){
		RaycastHit hit;
		if(Physics.Raycast (followCamera.position ,Vector3.down ,out hit ))
		{
			//LayerMask.NameToLayer("Ground")
			if(hit.collider.tag =="Ground"){
				if (_nextpos.y-1 <hit.point.y ) {
					Vector3 oldpos = new Vector3 (_nextpos.x, _nextpos.y, _nextpos.z);
					_nextpos.y = hit.point.y + 1f;//方法需要根据体验效果调整****
					//Debug.Log("r "+hit.distance+"//hitp= "+hit.point+"//old= "+oldpos+"//new= "+_nextpos+"//"+hit.collider.name  );
				}
			}
		}
		return _nextpos;

	}

//	void targetMove()
//	{
//
//
//		TargetObj.Translate (Vector3.forward *moveSpeed * Time.deltaTime);
//		Character.position=TargetObj.position;
//
//
////		Vector3 movementZ = Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime;
////
////		// Determine how much should move in the x-direction
////		Vector3 movementX = Input.GetAxis("Horizontal") * Vector3.right * moveSpeed * Time.deltaTime;
////
////		// Convert combined Vector3 from local space to world space based on the position of the current gameobject (player)
////		Vector3 movement = transform.TransformDirection(movementZ+movementX);
////		
////		// Apply gravity (so the object will fall if not grounded)
////		movement.y -= gravity * Time.deltaTime;
////
////		Debug.Log ("Movement Vector = " + movement);
////
////		// Actually move the character controller in the movement direction
////		myController.Move(movement);
//	}
//
//
//
//	void OnCollisionEnter (Collision newCollision)
//	{
//		Debug.Log ("aaaa"+newCollision.collider.name );
//	}
//
//
//
//
//
//
//
//
//	public void CharSmoothRotation()
//	{
//		Character.localRotation = Quaternion.Slerp (Character.localRotation, TargetObj.localRotation ,smoothTime * Time.deltaTime);//smoothTime * Time.deltaTime
//	}
//
//	public void LookRotation()
//	{
//		//get the y and x rotation based on the Input manager
//		float yRot = Input.GetAxis("Mouse X") * XSensitivity;
//		float xRot = Input.GetAxis("Mouse Y") * YSensitivity;
//
//		m_CharacterTargetRot *= Quaternion.Euler (-xRot, yRot, 0f);
//
//		if(clampVerticalRotation)
//		{
//			m_CharacterTargetRot = ClampRotationAroundXAxis (m_CharacterTargetRot);
//		}
//
//		if(smooth) // if smooth, then slerp over time
//		{
//			TargetObj.localRotation = Quaternion.Slerp (TargetObj.localRotation, m_CharacterTargetRot,
//			                                            smoothTime * Time.deltaTime);
//		}
//		else // not smooth, so just jump
//		{
//			TargetObj.localRotation = m_CharacterTargetRot;
//		}
//		float ff=0; //= m_CharacterTargetRot.ToAngleAxis ();
//		Vector3 vv=Vector3.forward;// m_CharacterTargetRot.ToAngleAxis ();
//		m_CharacterTargetRot.ToAngleAxis(out ff,out vv);
//		ttt =ff+"/"+vv;
//	}
//	
//
//
//
//
//
//
//
//
//
//	// Some math ... eeck!
//	Quaternion ClampRotationAroundXAxis(Quaternion q)
//	{
//		q.x /= q.w;
//		q.y /= q.w;
//		q.z /= q.w;
//		q.w = 1.0f;
//		
//		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
//		
//		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
//		
//		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
//		
//		return q;
//	}
}
