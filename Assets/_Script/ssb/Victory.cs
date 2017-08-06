using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

	public bool taken = false;
	public GameObject explosion;

	// if the player touches the victory object, it has not already been taken, and the player can move (not dead or victory)
	// then the player has reached the victory point of the level
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

			// do the player victory thing
//		if (other.gameObject.GetComponent<CharacterController2D>())
//        {
//            print("sparty");
//            other.gameObject.GetComponent<CharacterController2D>().Victory();
//        }
//        if (other.gameObject.GetComponent<CharacterControllerSari>())
//        {
//            print("sari");
//           other.gameObject.GetComponent<CharacterControllerSari>().Victory();
//        }
//		
//
			// destroy the victory gameobject
			DestroyObject(this.gameObject);
		}
	}

}
