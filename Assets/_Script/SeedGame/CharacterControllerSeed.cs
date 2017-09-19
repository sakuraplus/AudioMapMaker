using UnityEngine;
using System.Collections;
using UnityEngine.UI ;
//using UnityStandardAssets.CrossPlatformInput ;

public class CharacterControllerSeed : MonoBehaviour {
	[SerializeField ]
	Text tttt;//测试
	string sttt="";//测试
	string sttd="";//测试
	private float yRot ;//鼠标控制时，存xy角度
	private float xRot ;
	[SerializeField ]
	private float xSensitivity = 1f;//鼠标控制，xy灵敏度
	[SerializeField ]
	private float ySensitivity = 1f;
	[SerializeField ]
	private Quaternion m_CharacterTargetRot;//鼠标/加速度控制，保存target角度

	public bool clampVerticalRotation = true;//限制俯仰
	public float MinimumX = -90F;//限制俯仰角度
	public float MaximumX = 90F;//限制俯仰角度

	[SerializeField ]
	float smoothTime = 5f;//char跟随target旋转平滑的时间

	public float moveSpeed = 0.01f;//前进速度

	public float distanceH = 7f;//摄像机跟随距离
	public float distanceV = 4f;//摄像机跟随距离

	public bool cameragroundlimit;//仿真摄像机入地
	public  bool canmove=false;//开始前进
	public bool startFall = false;//歌曲结束，开始下落

	public  GameObject  TargetObj;//移动位置
	public  GameObject followCamera;//跟随摄像机
	public  GameObject Character;//显示的




	void Start() {
		distanceH = TargetObj.transform .position.z - followCamera.transform.position.z;
		distanceV = followCamera.transform.position.y-TargetObj.transform.position.y ;
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

	public void testR(Toggle t){
		Debug.Log ("mmmm" + t);
		clampVerticalRotation =t.isOn;

	}
	public void testsave(){
		sttt=Input.gyro .attitude.x+","+Input.gyro.attitude.y+","+Input.gyro.attitude.z+","+Input.gyro.attitude.w;
	}


	void Update() {

		if (Input.GetKey (KeyCode.A )) {
			canmove = true ;
			//targetMove();
		}

		if (canmove) {
			targetMove();
		}

		if (gameover) {
		
		}
		Character.transform.position=TargetObj.transform.position;

		#if UNITY_ANDROID

		if (!Input.gyro.enabled) {
		Input.gyro.enabled = true;
		}

		LookRotationGyro ();

		//LookRotationAcc (Input.acceleration);

		//#endif 
		//#if UNITY_STANDALONE
		#else 
		LookRotation ();
		#endif 
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
	[SerializeField ]
	LayerMask layerGround;
	Vector3 groundLimit(Vector3 _nextpos){
		RaycastHit hit;
	
		if(Physics.Raycast (followCamera.transform.position ,Vector3.down ,out hit,Mathf.Infinity,layerGround  ))
		{
		//	Debug.Log ("glglgl"+hit.distance );
	
			GameObject g=hit.collider.gameObject;

			if (_nextpos.y-1 <hit.point.y ) {
				_nextpos.y = hit.point.y + 1f;//方法需要根据体验效果调整****

			//	Debug.Log("r "+hit.distance+"//hitp= "+hit.point+"//new= "+_nextpos+"//"+hit.collider.name  );
			}
			//}
		}else if(Physics.Raycast (followCamera.transform.position ,Vector3.up ,out hit,Mathf.Infinity,layerGround  )){
			_nextpos.y = hit.point.y + 1f;
			Debug.LogError ("zzz");
		}
		return _nextpos;

	}
	//bool onground=false;
	public void resetPlayerPos(){
		RaycastHit hit;

		if( Physics.Raycast (TargetObj.transform.position ,Vector3.down ,out hit  )||Physics.Raycast (TargetObj.transform.position ,Vector3.up ,out hit  ))
		{	
			Debug.LogWarning ("resetPlayerPos>>"+TargetObj.transform.position+"//"+hit.point );
			TargetObj.transform.position =hit.point+new Vector3(0, maxHeight,0);
		}
	}
	bool gameover=false;
	void targetMove()
	{

		Vector3 nv = TargetObj.transform.position;
		Vector3 addv = TargetObj.transform.forward;
		RaycastHit hit;



		if (startFall) {

			addv.y = Mathf .Min (addv.y, -0.3f);
			if(Mathf .Abs( addv.x)>0.05f){
				addv.x *= 0.8f;
			}
			if(Mathf .Abs( addv.z)>0.05f){
				addv.z *= 0.8f;
			}
		

		}
		//else if(addv.y>0 && Physics.Raycast (TargetObj.transform.position ,Vector3.down ,out hit,Mathf.Infinity ,layerGround  ))

		 if( Physics.Raycast (TargetObj.transform.position ,Vector3.down ,out hit  ))
		{	//LayerMask.NameToLayer("Ground")

		
			bool objFound = false;
			GameObject g=hit.collider.gameObject;
			do {
				if (g.tag =="Ground") {
					objFound=true;
					break ;
				}else if(g.transform.parent !=null){
					g=g.transform.parent.gameObject ;
				}else{
					break ;
				}
			} while(objFound == false);

			if (objFound) {

				sttd = "distance=" + hit.distance;
//				if(hit.distance>maxHeight){
//					addv.y =Mathf.Max(addv.y, 0);// addv.y * speedy / 5;
//				}		
			}
		}
		nv += (addv * moveSpeed * Time.deltaTime);
		if (hit.distance > maxHeight) {
			nv.y-=(hit.distance - maxHeight)/maxHeight;//(hit.distance - maxHeight);
			//Debug.Log ("high!");
		}
		TargetObj.transform.position=nv;
	}

	[SerializeField ]
	 float maxHeight=5;
	/// <summary>
	/// char平滑跟随target.
	/// </summary>
	public void CharSmoothRotation()
	{
		Character.transform.localRotation = Quaternion.Slerp (Character.transform.localRotation, TargetObj.transform.localRotation ,smoothTime * Time.deltaTime);//smoothTime * Time.deltaTime
	}

	/// <summary>
	/// 鼠标控制
	/// </summary>
	public void LookRotation()
	{
		//get the y and x rotation based on the Input manager

		yRot = Input.GetAxis("Mouse X") * xSensitivity;
		xRot += Input.GetAxis("Mouse Y") * ySensitivity;

		tttt.text =sttt+"\n"+sttd+"\nxx    "+  Input.GetAxis("Mouse X")+","+ Input.GetAxis("Mouse Y")+"xy="+xRot+","+yRot ;
		xRot = Mathf.Clamp (xRot, MinimumX, MaximumX);

		m_CharacterTargetRot *= Quaternion.Euler (0, yRot, 0f);

		if(clampVerticalRotation)
		{
			m_CharacterTargetRot = ClampRotationAroundXAxis (m_CharacterTargetRot);

		}
		TargetObj.transform.localRotation  = m_CharacterTargetRot;
		TargetObj.transform.RotateAround (TargetObj.transform.position, TargetObj.transform.right, -xRot);// ;


	}

	/// <summary>
	/// 陀螺仪控制
	/// </summary>
	public void LookRotationGyro()
	{
		Gyroscope gy=Input.gyro ;
		tttt.text =sttt+"\n"+sttd+"\nA    "+ gy.attitude;

		Quaternion targetrot = cameraBase * (ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix());
		if (clampVerticalRotation) {

			targetrot = ClampRotationAroundXAxis (targetrot);
		} else {
			sttt = "";
		}

		TargetObj.transform.localRotation =targetrot;
	}



	#region [math]
	private Quaternion cameraBase = Quaternion.identity;
	private Quaternion calibration = Quaternion.identity;
	private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix = Quaternion.identity;

	Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);
	#if UNITY_STANDALONE || UNITY_WEBGL 
	Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);
	Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);
	Quaternion upsideDown = Quaternion.Euler(0, 0, 180);
	#endif 

	private Quaternion referanceRotation = Quaternion.identity;


	private void AttachGyro()
	{
		//gyroEnabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}


	/// <summary>
	/// Update the gyro calibration.
	/// </summary>
	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}


	/// <summary>
	/// Update the camera base rotation.
	/// </summary>
	/// <param name='onlyHorizontal'>
	/// Only y rotation.
	/// </param>
	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = TargetObj.transform  .forward;
			//			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = TargetObj.transform  .rotation;
			//			cameraBase = transform.rotation;
		}
	}


	/// <summary>
	/// Converts the rotation from right handed to left handed.
	/// </summary>
	/// <returns>
	/// The result rotation.
	/// </returns>
	/// <param name='q'>
	/// The rotation to convert.
	/// </param>
	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}


	/// <summary>
	/// Gets the rot fix for different orientations.
	/// </summary>
	/// <returns>
	/// The rot fix.
	/// </returns>
	private Quaternion GetRotFix()
	{
		#if UNITY_STANDALONE || UNITY_WEBGL 
		if (Screen.orientation == ScreenOrientation.Portrait)
		return Quaternion.identity;

		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
		return landscapeLeft;

		if (Screen.orientation == ScreenOrientation.LandscapeRight)
		return landscapeRight;

		if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
		return upsideDown;
		return Quaternion.identity;
		#else
		return Quaternion.identity;
		#endif
	}


	/// <summary>
	/// Recalculates reference system.
	/// </summary>
	private void ResetBaseOrientation()
	{
		baseOrientationRotationFix = GetRotFix();
		baseOrientation = baseOrientationRotationFix * baseIdentity;
	}


	/// <summary>
	/// Recalculates reference rotation.
	/// </summary>
	private void RecalculateReferenceRotation()
	{
		referanceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
	}



	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		sttt = "limit"+q;
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);


		sttt += ">>" + q;
		return q;
	}
	#endregion
}

