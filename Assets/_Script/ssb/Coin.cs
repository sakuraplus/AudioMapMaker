using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	public int coinValue = 1;
	public bool taken = false;
	public GameObject explosion;

	// if the player touches the coin, it has not already been taken, and the player can move (not dead or victory)
	// then take the coin
	void OnTriggerEnter2D (Collider2D other)
	{
		 bool CanMove = false;
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

		if ((other.tag == "Player" ) && (!taken) && (CanMove))
		{
			// mark as taken so doesn't get taken multiple times
			taken=true;

			// if explosion prefab is provide, then instantiate it
			if (explosion)
			{
				Instantiate(explosion,transform.position,transform.rotation);
			}

			// do the player collect coin thing
			
//			if (other.gameObject.GetComponent<CharacterController2D>())
//			{
//				print("sparty");
//				other.gameObject.GetComponent<CharacterController2D>().CollectCoin(coinValue);
//			}
//			if (other.gameObject.GetComponent<CharacterControllerSari>())
//			{
//				print("sari");
//			   other.gameObject.GetComponent<CharacterControllerSari>().CollectCoin(coinValue);
//			}
			// destroy the coin
			DestroyObject(this.gameObject);
		}
	}

}
