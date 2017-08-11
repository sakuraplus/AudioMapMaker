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
	bool selectLoc=true;
	public GameObject _MenuPanel;

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

		if (Input.GetMouseButton (0) ) {
			//&&selectLoc 
			 
			Vector3  imgScale = imgMap.transform.lossyScale;
			Vector3 imgCenter = imgMap.transform.position;
			Vector3 imgUpLeft = new Vector3 (imgCenter.x - imgW * imgScale.x, imgCenter.y + imgH * imgScale.y, 0);



			float posonimgx = Input.mousePosition .x - imgUpLeft.x;
			float posonimgy = Input.mousePosition .y - imgUpLeft.y;
			Vector3 localPosOnImg =new Vector3 (512* posonimgx / (imgScale.x*imgW*2), 512+512*posonimgy / (imgScale.y*imgH*2),0);

//			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			Vector3 posOnImg = new Vector3 (mousePos.x-imgUpLeft.x,mousePos.y-imgUpLeft.y,0);
//			string sttt = "mouse=" + Input.mousePosition;
//			sttt += "\nmousepos= " + mousePos;
//			sttt += "\nimgScale=" + imgScale + "\nimgUpLeft(" + imgUpLeft.x + "," + imgUpLeft.y + ") ";
//			sttt += "\nposOnImg=(" + posonimgx + "," + posonimgy + ")  \nlocalPosOnImg=" + posonimgx/imgScale.x+","+posonimgy/imgScale .y;
//			sttt += "\nposOnImg input=(" + (Input.mousePosition.x - imgUpLeft.x) + "," + (Input.mousePosition.y - imgUpLeft.y);
//			sttt += "\nlocalPosOnImg input=(" + 0.5f*(Input.mousePosition.x - imgUpLeft.x)/imgScale.x + "," + (Input.mousePosition.y - imgUpLeft.y)/imgScale.y;
//			sttt += "\nlocalPosOnImg input int=" + Mathf.FloorToInt (localPosOnImg.x) + "," + Mathf.FloorToInt (localPosOnImg.y);
//			sttt += "\n " + (localPosOnImg.y / 512) + "," + localPosOnImg.x / 512;
//			sttt += "\n getp=" + map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));

			Color c = map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));
			Debug.Log ("color" + c);

			if (c != Color.black) {
				slat = calcLatLng (localPosOnImg).x;
				slng = calcLatLng (localPosOnImg).y;

				imgIcon.transform.position = Input.mousePosition;
				txtLoc .text =  calcLatLng (localPosOnImg).ToString();
				imgIcon.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

				txtPanel.text = "you will start at" + calcLatLng (localPosOnImg);
//				sttt += "\nlatlng=" + calcLatLng (localPosOnImg);
				showMenu ();
			} else {
				hideMenu ();
				imgIcon.transform.position = new Vector3 (-10,-10,-10);
			}

//			ttt.text = sttt;
			}
	}

	Vector2 calcLatLng(Vector3 posonimg) {

		// 纬度所在完整地图上的位置（比例）北极0，南极1，赤道0.5
		float lat=30;
		float sinlat=Mathf.Sin(lat  *Mathf.PI /180);
		sinlat = Mathf.Min (Mathf.Max (sinlat, -0.99f), 0.99f);

		// 纬度所在完整地图上的位置（比例）北极0，南极1，赤道0.5
		//float pointlat=(0.5f - Mathf.Log ((1 + sinlat) / (1 - sinlat)) / (4 * Mathf.PI));
		float pointlat=(posonimg.y)/512;
		float yy=(0.5f-pointlat)*4*Mathf.PI ;
		float pp = Mathf.Pow ((float)System.Math.E , yy);
		float calclat = Mathf.Asin ((pp - 1) / (pp + 1)) * Mathf.Rad2Deg;

		float pointlng=posonimg.x/512;
		float calclng=(pointlng-0.5f)*360 ;
		//Debug.Log (pointlat + "," + pointlng+">> "+(-1*calclat)+" , "+calclng);
		//Debug.Log((pp-1)/(pp+1)+">>srcs="+Mathf.Asin((pp-1)/(pp+1))+"deg="+calclat);
		return new Vector2 (-1*calclat, calclng);
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
		TerrainManager.lat=slat;
		TerrainManager.lng=slng;
		PlayerPrefManager.SetLat (slat);
		PlayerPrefManager.SetLng (slng);
		SceneManager.LoadScene(levelToLoad);
	}
	public void BtnCancel()
	{
		selectLoc = true;
		_MenuPanel.SetActive (false);
	}
}
