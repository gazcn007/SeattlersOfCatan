using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishResourcePanel : MonoBehaviour {

	public Button[] resourceoptions;
	public Image glow;
	public int selection;
	public bool selectionMade = false;
	public bool valchange;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < resourceoptions.Length; i++) {
			//get instance
			FishResourcePanelButton inst = resourceoptions[i].GetComponentInChildren<FishResourcePanelButton> ();
			inst.instance=this;
		}
			
		glow.gameObject.SetActive (false);
	}
		
	public int getResourceChoiceInt() {
		return selection;
	}
	public void setGlow(FishResourcePanelButton button){
		glow.gameObject.SetActive (true);
		glow.gameObject.transform.position = button.gameObject.transform.position;
	}

	public void confirmSelection() {
		if (valchange) {
			selectionMade = true;
		}
	}

	public int getSelection() {
		return selection;
	}

	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		while (!selectionMade) {
			yield return new WaitForEndOfFrame ();
		}
	}
}
