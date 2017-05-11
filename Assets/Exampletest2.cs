﻿/*
 * Copyright (c) 2015 Allan Pichardo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System;

public class Exampletest2 : MonoBehaviour
{
	public  GameObject ggg;
	float ii=0;
	AudioSource sd;
	void Start ()
	{
		sd=GetComponent<AudioSource>();
		//Select the instance of AudioProcessor and pass a reference
		//to this object
		AudioProcessor processor = FindObjectOfType<AudioProcessor> ();
//		maintest3ap processor = FindObjectOfType<maintest3ap > ();
		processor.onBeat.AddListener (onOnbeatDetected);
		processor.onSpectrum.AddListener (onSpectrum);
	}

	//this event will be called every time a beat is detected.
	//Change the threshold parameter in the inspector
	//to adjust the sensitivity
	void onOnbeatDetected ()
	{
		//Debug.Log ("Beat!!!");
		ii = 5;
		ggg.transform.localScale   = new Vector3 (5,5,5);
	}
	float playtime=0;
	float pausetime=0;
	float unpausetime=0;
	void Update () {
		
		if(ii>1){
			ii -= ii / 5;
			ggg.transform.localScale    = new Vector3 (ii,ii,ii);
		}
		if(Input.GetKeyDown (KeyCode.A)){
		//	sd.Stop();
			Debug.Log(sd.timeSamples+"/stop//"+sd.time+"///"+Time.time );
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			if (!sd.isPlaying) {
				sd.Play ();
			
				playtime = Time.time;
				Debug.Log (sd.timeSamples + "/Play//" + sd.time + "///" + Time.time);
			}
		}
		if(Input.GetKeyDown (KeyCode.S  )){
			sd.Pause  ();
			pausetime = Time.time;
			Debug.Log(sd.timeSamples+"/Pause//"+sd.time+"///"+Time.time );
		}
		if(Input.GetKeyDown (KeyCode.X   )){
			unpausetime = Time.time;
			sd.UnPause   ();
			Debug.Log(sd.timeSamples+"/UnPause//"+sd.time+"///"+Time.time );
		}
		if(Input.GetKeyDown (KeyCode.Q )){
			//sd.UnPause   ();
			Debug.Log((sd.time+playtime+unpausetime-pausetime  )+"/UnPause//"+sd.time+"///"+Time.time );
		}

	}
	//This event will be called every frame while music is playing
	void onSpectrum (float[] spectrum)
	{
		//The spectrum is logarithmically averaged
		//to 12 bands

		for (int i = 0; i < spectrum.Length; ++i) {
			Vector3 start = new Vector3 (i, 0, 0);
			Vector3 end = new Vector3 (i, spectrum [i], 0);
			Debug.DrawLine (start, end);
		}
	}
}