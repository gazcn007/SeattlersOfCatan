using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour {
	public GameObject instance;
	public Text hovertext;

	//this holds generic functions for UI movement of buttons
	public void onMouseEnter(){
		//Vector3 position = new Vector3 (hovertext.transform.position.x-10, instance.transform.position.y, 0);
		//hovertext.transform.position = position;
		hovertext.gameObject.SetActive (true);
		instance.GetComponent<RectTransform> ().sizeDelta = new Vector2(82,82);

	}
	public void onMouseExit(){
		//Vector3 position = new Vector3 (hovertext.transform.position.x+10, instance.transform.position.y, 0);
		//hovertext.transform.position = position;
		hovertext.gameObject.SetActive (false);
		instance.GetComponent<RectTransform> ().sizeDelta = new Vector2(50,50);

	}
}
