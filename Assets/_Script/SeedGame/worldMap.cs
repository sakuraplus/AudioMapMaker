using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // include so we can load new scenes
//using UnityStandardAssets.CrossPlatformInput ;

public class worldMap : MonoBehaviour {
	/// <summary>
	/// The text of panel.
	/// </summary>
	public  Text txtPanel;
	/// <summary>
	/// The text of location icon.
	/// </summary>
	public  Text txtLoc;
	public  Text ttt;
	/// <summary>
	/// img of map
	/// </summary>
	public  Image  imgMap;
	/// <summary>
	/// location icon
	/// </summary>
	public  Image  imgIcon;
	/// <summary>
	/// the black/white map 
	/// </summary>
	public Texture2D map;

	/// <summary>
	/// height of imgMap
	/// </summary>
	public int imgH;
	/// <summary>
	/// width of imgMap
	/// </summary>
	public int imgW;

	float slat,slng;
	/// <summary>
	///是否允许选择地点 The select location.
	/// </summary>
	public 	bool selectLoc=true;
	public 	bool selectLevel=false;
	public 	bool showRecord=false;

	public GameObject _MenuPanel;
	Vector3 imgScale;// = imgMap.transform.lossyScale;
	Vector3 imgCenter;// = imgMap.transform.position;
	Vector3 imgUpLeft ;//= new Vector3 (imgCenter.x - imgW * imgScale.x, imgCenter.y + imgH * imgScale.y, 0);

	void Start()
	{	
		imgScale = imgMap.transform.lossyScale;
		imgCenter = imgMap.transform.position;
		imgUpLeft = new Vector3 (imgCenter.x - imgW * imgScale.x, imgCenter.y + imgH * imgScale.y, 0);
		Debug.Log ("imgScale="+imgScale+" , imgUpLeft="+imgUpLeft+"  imgCenter="+imgCenter);
		if (selectLevel ) {
			setLevelSelect (10);
		}else if (showRecord) {
			showRecordIcon();
		}
	}

	void Update(){
		if(Input.GetKey(KeyCode.Z)){
			Debug.Log(imgMap.preferredHeight +"/"+imgMap.preferredWidth );
			Debug.Log ("imgScale="+imgMap.transform.lossyScale+"center="+imgMap.transform.position);
			Debug.Log ("imgScale="+imgMap.transform.lossyScale+"center="+imgMap.transform.position);
		}
		Vector3 illsc = imgIcon.transform.localScale;
		illsc *= 2;
		if (illsc .x > 1) {
			illsc = Vector2.one;
		} 
		imgIcon.transform.localScale = illsc;

		if (Input.GetMouseButton (0)&&selectLoc ) {
			//
			float posonimgx = Input.mousePosition .x - imgUpLeft.x;
			float posonimgy = Input.mousePosition .y - imgUpLeft.y;
			Vector3 localPosOnImg =new Vector3 ( posonimgx / (imgScale.x*imgW*2), 1+posonimgy / (imgScale.y*imgH*2),0);

//			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			Vector3 posOnImg = new Vector3 (mousePos.x-imgUpLeft.x,mousePos.y-imgUpLeft.y,0);
			string sttt =">"+( posonimgx / (imgScale.x*imgW*2))+","+(posonimgy / (imgScale.y*imgH*2));// "mouse=" + Input.mousePosition;
//			sttt += "\nmousepos= " + mousePos;
//			sttt += "\nimgScale=" + imgScale + "\nimgUpLeft(" + imgUpLeft.x + "," + imgUpLeft.y + ") ";
//			sttt += "\nposOnImg=(" + posonimgx + "," + posonimgy + ")  \nlocalPosOnImg=" + posonimgx/imgScale.x+","+posonimgy/imgScale .y;
//			sttt += "\nposOnImg input=(" + (Input.mousePosition.x - imgUpLeft.x) + "," + (Input.mousePosition.y - imgUpLeft.y);
//			sttt += "\nlocalPosOnImg input=(" + 0.5f*(Input.mousePosition.x - imgUpLeft.x)/imgScale.x + "," + (Input.mousePosition.y - imgUpLeft.y)/imgScale.y;
//			sttt += "\nlocalPosOnImg input int=" + Mathf.FloorToInt (localPosOnImg.x) + "," + Mathf.FloorToInt (localPosOnImg.y);
//			sttt += "\n " + (localPosOnImg.y / 512) + "," + localPosOnImg.x / 512;
//			sttt += "\n getp=" + map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));

			Color c = map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));
			//Debug.Log ("color" + c);

			if (c != Color.black) {
				Vector2 latlng = calcLatLng (localPosOnImg);
				slat = latlng.x;
				slng = latlng.y;
				if (slat > -70 && slat < 70) {
					imgIcon.transform.position = Input.mousePosition;
					txtLoc.text = calcLatLng (localPosOnImg).ToString ();
					imgIcon.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

					txtPanel.text = "you will start at" + calcLatLng (localPosOnImg);
					sttt += "\nlatlng=" + calcLatLng (localPosOnImg);
					showMenu ();
				}
			} else {
				hideMenu ();
				imgIcon.transform.position = new Vector3 (-10,-10,-10);
			}
			if (ttt) {
				ttt.text = sttt;
			}
			}
	}

	Vector2 calcLatLng(Vector3 posonimg) {
		
		// 纬度所在完整地图上的位置（比例）北极0，南极1，赤道0.5
		//float pointlat=(0.5f - Mathf.Log ((1 + sinlat) / (1 - sinlat)) / (4 * Mathf.PI));
		float pointlat=(posonimg.y);
		float yy=(0.5f-pointlat)*4*Mathf.PI ;
		float pp = Mathf.Pow ((float)System.Math.E , yy);
		float calclat = Mathf.Asin ((pp - 1) / (pp + 1)) * Mathf.Rad2Deg;

		float pointlng=posonimg.x;
		float calclng=(pointlng-0.5f)*360 ;
		//Debug.Log (pointlat + "," + pointlng+">> "+(-1*calclat)+" , "+calclng);
		//Debug.Log((pp-1)/(pp+1)+">>srcs="+Mathf.Asin((pp-1)/(pp+1))+"deg="+calclat);
		return new Vector2 (-1*calclat, calclng);
	}

	Vector3 calcPosonImg(Vector2 LatLng) {
		
		float sinsouthlat=Mathf.Sin(LatLng.y  *Mathf.Deg2Rad );
		sinsouthlat = Mathf.Min (Mathf.Max (sinsouthlat, -0.999f), 0.999f);
		float pointsouthlat=(0.5f - Mathf.Log ((1 + sinsouthlat) / (1 - sinsouthlat)) / (4 * Mathf.PI));
		//GALL_PETERS_RANGE_X * (0.5 + latLng.lng() / 360),
		//GALL_PETERS_RANGE_Y * (0.5 - 0.5 * Math.sin(latRadians)));
		string ss=LatLng+">>>";
		float onImgX=(0.5f + LatLng.x / 360);
		float onImgY = pointsouthlat;//((0.5f - 0.5f * Mathf .Sin (LatLng.y*Mathf.Deg2Rad )));
		float fullIX = imgW * 2 * imgScale.x;
		float fullIY = imgH * 2 * imgScale.y;

		ss+="fullimg= "+fullIX+","+fullIY+"onimg%="+onImgX+" , "+onImgY;
		onImgX = onImgX * fullIX;// imgW * 2*imgScale.x;
		onImgY = onImgY * fullIY;//imgH * 2*imgScale.y;
		ss+="onfull = "+onImgX+" , "+onImgY;
		onImgX +=imgUpLeft .x ;
		onImgY =imgUpLeft.y- onImgY;
	
		ss += "  >>=" + onImgX + " , " + onImgY;
		Debug.Log(ss);
		return new Vector3 (onImgX,onImgY,0);
	}
	void hideMenu(){
		selectLoc = true ;
		_MenuPanel.SetActive (false);
	}
	void showMenu(){
		selectLoc = false;
		_MenuPanel.SetActive (true);
	}

	// load the specified Unity level
	public void BtnStartGame(string levelToLoad)
	{
		// start new game so initialize player state
		//PlayerPrefManager.ResetPlayerState(startLives,false);

		// load the specified level
		TerrainManagerStatics.Lat=slat;
		TerrainManagerStatics.Lng=slng;
		PlayerPrefManager.SetLat (slat);
		PlayerPrefManager.SetLng (slng);
		SceneManager.LoadScene(levelToLoad);
	}
	public void BtnCancel()
	{
		selectLoc = true;
		_MenuPanel.SetActive (false);
	}
	public GameObject panelbtn;
	public  GameObject LevelButtonPrefab;
	void setLevelSelect(int num) {
		Debug.Log ("num="+num);
		string recordstr = PlayerPrefManager.GetRecord ();
		//recordstr="0,0|30,30|-85,0|60,60|0,90|85,0";
		///>>12,34/12.34,34,56/1,2/4,5/aaa,ddd/ccc,ddd/6,6/7,77/8,88/9,999/0,00/q,w/e,r//
		if (recordstr.Length > 0) {
			
			string sss = "|";
			string[] records = recordstr.Split (  sss.ToCharArray()[0]);
			string t = ">>";
			for (int i=0;i < records.Length &&i<num; i++) {
				t+=records[i]+"/";
				Debug.Log ("i="+i+"//"+records.Length );
				GameObject levelButton = Instantiate (LevelButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;

				string[] s = records[i].Split (",".ToCharArray ());
				if (s.Length == 2) {
					Vector2 latlng = new Vector2 (float.Parse (s [1]), float.Parse (s [0]));

					levelButton.name = "lastpoint" + i;// levelname + " Button";
					levelButton.transform.SetParent (panelbtn.transform, false);
					levelButton.transform.position = calcPosonImg (latlng);
					Button levelButtonScript = levelButton.GetComponent<Button> ();
					levelButtonScript.onClick.RemoveAllListeners ();

					if (selectLevel) {
						Debug.Log ("records [i]="+records [i]);
						string stt =records [i];
						levelButtonScript.onClick.AddListener (() => loadLevelselect (stt));
						//levelButtonScript.onClick.AddListener (() => loadLevelselect (latlng));
					}
				}
//				else if(showRecord ){
//					Text levelButtonLabel = levelButton.GetComponentInChildren<Text> ();
//					levelButtonLabel.text = records[i];//"start at the last point";//levelname;	
//				}



			}
			Debug.Log (t);
		}
	}
	void showRecordIcon() {
		

		GameObject levelButton = Instantiate (LevelButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Vector2 latlng = new Vector2 (TerrainManagerStatics.Lng,TerrainManagerStatics.Lat );
		levelButton.name ="lastpointS";// levelname + " Button";
		levelButton.transform.SetParent (panelbtn.transform, false);
		levelButton.transform.position = calcPosonImg (latlng);
		levelButton.GetComponentInChildren<Text> ().text ="S";//"start at the last point";//levelname;	

		GameObject levelButton2 = Instantiate (LevelButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Vector2 latlng2 = new Vector2 (PlayerPrefManager.GetLng() ,PlayerPrefManager.GetLat() );
		levelButton2.name ="lastpointE";// levelname + " Button";
		levelButton2.transform.SetParent (panelbtn.transform, false);
		levelButton2.transform.position = calcPosonImg (latlng2);
		levelButton2.GetComponentInChildren<Text> ().text ="E";//"start at the last point";//levelname;	

			
	}
	public void loadLevelselect(string location)
	{
		string[] s = location.Split (",".ToCharArray ());
		float lat = float.Parse (s [0]);
		//// start new game so initialize player state
		//PlayerPrefManager.ResetPlayerState(startLives,false);

		// load the specified level
		//TerrainManager.lat
		slat=float.Parse (s [1]);//PlayerPrefManager.GetLat();
		//TerrainManager.lng
		slng=float.Parse (s [0]);//PlayerPrefManager.GetLng();
		Debug.LogError("loadLevelselect"+float.Parse (s [0])+","+float.Parse (s [1]));
		showMenu ();
		txtPanel.text= "you will start at <lng,lat>:" + location;

		//SceneManager.LoadScene("SelectMusic");
	}
//	public void loadLevelselect(Vector2 location)
//	{
//
//
//		// load the specified level
//		TerrainManager.lat=location.y;
//		TerrainManager.lng = location.x;
//		Debug.Log("loadLevelselect-V2="+location);
//		showMenu ();
//		txtPanel.text= "you will start at <lng,lat>:" + location;
//		//SceneManager.LoadScene("SelectMusic");
//	}
	public void LoadScene(string scenename)
	{
		//switch(name) {
		//case "MainMenu":
		SceneManager.LoadScene(scenename);
	}

}
