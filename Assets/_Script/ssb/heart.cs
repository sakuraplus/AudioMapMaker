using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
public class heart : MonoBehaviour {

	// Use this for initialization
	public int HeartValue = 1;
	public bool taken = false;
	void OnTriggerEnter2D (Collider2D other)
	{
//		bool CanMove = false;
//		if (other.gameObject.GetComponent<CharacterController2D>())
//		{
//			print("sparty");
//			CanMove = other.gameObject.GetComponent<CharacterController2D>().playerCanMove;
//		}
//		if (other.gameObject.GetComponent<CharacterControllerSari>())
//		{
//			print("sari");
//			CanMove = other.gameObject.GetComponent<CharacterControllerSari>().playerCanMove;
//		}
//
//		if ((other.tag == "Player" ) && (!taken) && (CanMove))
//		{
//			// mark as taken so doesn't get taken multiple times
//			taken=true;
//			Analytics.CustomEvent("Take Heart", new Dictionary<string, object>{{ "Heart", System.DateTime .Now }});
//		
//			// do the player collect coin thing
//
//			if (other.gameObject.GetComponent<CharacterController2D>())
//			{
//				print("sparty");
//				//other.gameObject.GetComponent<CharacterController2D>().IncreaseHeart(HeartValue);
//			}
//			if (other.gameObject.GetComponent<CharacterControllerSari>())
//			{
//				print("sari");
//				other.gameObject.GetComponent<CharacterControllerSari>().IncreaseHeart(HeartValue);
//			}
//			// destroy the coin
//			DestroyObject(this.gameObject);
//		}
	}


}
