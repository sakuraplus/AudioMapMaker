using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // include so we can load new scenes
//using UnityStandardAssets.CrossPlatformInput ;

public class worldMap : MonoBehaviour {
	public  Vector2 x;
	public  Vector2Int xxx;
	public  Text ttt;
	public  Image  imm;
	public  Image  iLL;
	public Texture2D map;


	float slat,slng;
	bool selectLoc=true;
	public GameObject _MainMenu;
	void Update(){
		if (Input.GetMouseButton (0) &&selectLoc ) {
			
			 
			Vector3  imgScale = imm.transform.lossyScale;
			Vector3 imgCenter = imm.transform.position;
			Vector3 imgUpLeft = new Vector3 (imgCenter.x - 500 * imgScale.x, imgCenter.y + 256 * imgScale.y, 0);
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//Vector3 posOnImg = new Vector3 (mousePos.x-imgUpLeft.x,mousePos.y-imgUpLeft.y,0);
			float posonimgx = Input.mousePosition .x - imgUpLeft.x;
			float posonimgy = Input.mousePosition .y - imgUpLeft.y;
			Vector3 localPosOnImg =new Vector3 (0.5f* posonimgx / imgScale.x, 512+posonimgy / imgScale.y,0);
			//Vector3 localPosOnImg =new Vector3 ( posonimgx / imgScale.x, posonimgy / imgScale.y,0);
			string sttt = "mouse=" + Input.mousePosition;
			//sttt += "\nmousepos= " + mousePos;
//			sttt += "\nimgScale=" + imgScale + "\nimgUpLeft(" + imgUpLeft.x + "," + imgUpLeft.y + ") ";
//			sttt += "\nposOnImg=(" + posonimgx + "," + posonimgy + ")  \nlocalPosOnImg=" + posonimgx/imgScale.x+","+posonimgy/imgScale .y;
//			sttt += "\nposOnImg input=(" + (Input.mousePosition.x - imgUpLeft.x) + "," + (Input.mousePosition.y - imgUpLeft.y);
//			sttt += "\nlocalPosOnImg input=(" + 0.5f*(Input.mousePosition.x - imgUpLeft.x)/imgScale.x + "," + (Input.mousePosition.y - imgUpLeft.y)/imgScale.y;
//			sttt += "\nlocalPosOnImg input int=" + Mathf.FloorToInt (localPosOnImg.x) + "," + Mathf.FloorToInt (localPosOnImg.y);
//			sttt += "\n " + (localPosOnImg.y / 512) + "," + localPosOnImg.x / 512;
//			sttt += "\n getp=" + map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));
			Color c = map.GetPixel (Mathf.FloorToInt (localPosOnImg.x), Mathf.FloorToInt (localPosOnImg.y));


			if (c != Color.black) {
				iLL.transform.position = Input.mousePosition;
				slat=calcLatLng (localPosOnImg).x;
				slng=calcLatLng (localPosOnImg).y;
				ttt.text="you will start at"+ calcLatLng (localPosOnImg);
				//sttt += "\nlatlng=" + calcLatLng (localPosOnImg);
				showMenu();
			}

			//ttt.text = sttt;
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
		Debug.Log (pointlat + "," + pointlng);
		//Debug.Log((pp-1)/(pp+1)+">>srcs="+Mathf.Asin((pp-1)/(pp+1))+"deg="+calclat);
		return new Vector2 (-1*calclat, calclng);
	}

	void showMenu(){
		selectLoc = false;
		_MainMenu.SetActive (true);
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
		_MainMenu.SetActive (false);
	}
}
