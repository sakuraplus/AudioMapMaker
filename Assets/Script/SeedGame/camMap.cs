using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityStandardAssets.CrossPlatformInput ;

public class camMap : MonoBehaviour {
	public  Vector2 x;
	public  Vector2Int xxx;
	public  Text ttt;
	public  Image  imm;

	void Update(){
		if (Input.GetMouseButton (0)) {
			Debug.Log (Input.mousePosition);
			Vector3 vv = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10);
			string sttt = "mouse=" + Input.mousePosition;
			sttt += "\n ScreenToWorldPoint=" + Camera.main.ScreenToWorldPoint (Input.mousePosition);
			sttt += "\n ScreenToWorldPoint=" + Camera.main.ScreenToWorldPoint (vv);
			sttt += "\n ScreenToViewportPoint=" + Camera.main.ScreenToViewportPoint (Input.mousePosition);
			sttt += "\n ViewportPointToRay=" + Camera.main.ViewportPointToRay (vv);
			sttt += "\n ViewportToWorldPoint=" + Camera.main.ViewportToWorldPoint (Input.mousePosition);
			sttt+="\n";
			sttt += "\n imm=" + imm.transform.position;//+"--"+imm.rectTransform ;
			//sttt += "\n imm=" + imm.transform.worldToLocalMatrix ;
			sttt += "\n ScreenToWorldPoint=" + Camera.main.ScreenToWorldPoint (imm.transform.position);

			sttt += "\n ViewportPointToRay=" + Camera.main.ViewportPointToRay (imm.transform.position);
			sttt += "\n ViewportToWorldPoint=" + Camera.main.ViewportToWorldPoint (imm.transform.position);
			ttt.text = sttt;
			}
	}
}
