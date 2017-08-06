using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	public int coinValue = 1;
	public bool taken = false;
	public GameObject explosion;

	// if the player touches the coin, it has not already been taken, and the player can move (not dead or victory)
	// then take the coin
//	void OnTriggerEnter2D (Collider2D other)
//	{
//		 bool CanMove = false;
//		if (other.gameObject.GetComponent<CharacterController2D>())
//        {
//            print("sparty");
//            CanMove = other.gameObject.GetComponent<CharacterController2D>().playerCanMove;
//        }
//        if (other.gameObject.GetComponent<CharacterControllerSari>())
//        {
//            print("sari");
//            CanMove = other.gameObject.GetComponent<CharacterControllerSari>().playerCanMove;
//        }

//		if ((other.tag == "Player" ) && (!taken) && (CanMove))
//		{
//			// mark as taken so doesn't get taken multiple times
//			taken=true;
//			GameManager.gm.checkpoint (transform.position);
//			print("sari-check");
//			// if explosion prefab is provide, then instantiate it
//			if (explosion)
//			{
//				Instantiate(explosion,transform.position,transform.rotation);
//			}
//			// destroy the coin
//			DestroyObject(this.gameObject);
//		}
//	}

}
