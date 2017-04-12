using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemistPanel : MonoBehaviour {

	public Sprite[] redDie;
	public Sprite[] yellowDie;

	public Slider	redDieVal;
	public Slider	yellowDieVal;

	public Image	redDieImg;
	public Image	yellowDieImg;
	public	bool	selectionMade;

	// Update is called once per frame
	void Update () {
		yellowDieImg.sprite = yellowDie [(int)yellowDieVal.value - 1];
		redDieImg.sprite = redDie [(int)redDieVal.value - 1];
	}
	public void Open(){
		this.gameObject.SetActive (true);
		redDieVal.value = 1;
		yellowDieVal.value = 1;
		redDieImg.sprite = redDie [0];
		yellowDieImg.sprite = yellowDie [0];
		selectionMade = false;
	}
	public void confirmSelection(){
			selectionMade = true;
	}
	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		while (!selectionMade) {
			yield return new WaitForEndOfFrame ();
		}
	}
}
