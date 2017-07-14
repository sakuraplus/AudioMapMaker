using UnityEngine;
using System.Collections;
[RequireComponent(typeof (PhotonView))]
public class movelerp :  Photon.MonoBehaviour, IPunObservable
{

	public  Vector3 latestCorrectPos;
	public  Vector3 onUpdatePos;
	private float fraction;
	[SerializeField ]
	bool onctrl = false;
	[SerializeField ]
	bool isstreaming = true;
	public void Start()
	{
		this.latestCorrectPos = transform.position;
		this.onUpdatePos = transform.position;
	}


	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.localPosition;
		//	Quaternion rot = transform.localRotation;
			Debug.Log("!");
			stream.Serialize(ref pos);
			//stream.Serialize(ref rot);
			isstreaming = true;
		}
		else
		{
			isstreaming = false;
			// Receive latest state information
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;

			stream.Serialize(ref pos);
		//	stream.Serialize(ref rot);

			this.latestCorrectPos = pos;                // save this to move towards it in FixedUpdate()
			this.onUpdatePos = transform.localPosition; // we interpolate from here to latestCorrectPos
			this.fraction = 0;                          // reset the fraction we alreay moved. see Update()

			//transform.localRotation = rot;              // this sample doesn't smooth rotation
		}
	}


	public void Update()
	{
		if (this.photonView.isMine)
		{onctrl = true;
			return;     // if this object is under our control, we don't need to apply received position-updates 
		}


		this.fraction = this.fraction + Time.deltaTime * 9;
		transform.localPosition = Vector3.Lerp(this.onUpdatePos, this.latestCorrectPos, this.fraction); // set our pos between A and B
	}
}
