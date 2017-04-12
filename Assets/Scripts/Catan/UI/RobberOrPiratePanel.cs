using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberOrPiratePanel : MonoBehaviour {

	public int selection;
	public bool selectionMade = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RobberChoice() {
		selection = 0;
		confirmSelection ();
	}

	public void PirateChoice() {
		selection = 1;
		confirmSelection ();
	}

	public void confirmSelection() {
		selectionMade = true;
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
