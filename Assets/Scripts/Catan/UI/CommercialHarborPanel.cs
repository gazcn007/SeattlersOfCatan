using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommercialHarborPanel : MonoBehaviour {

	public Image glow1;
	public Image glow2;
	public Image glow3;

	public List<CommercialHarborPanelButton> sel1;
	public List<CommercialHarborPanelButton> sel2;
	public List<CommercialHarborPanelButton> sel3;
	public GameObject arr1;
	public GameObject arr2;
	public GameObject arr3;

	public int selection1;
	public int selection2;
	public int selection3;

	public bool selection1flag;
	public bool selection2flag;
	public bool selection3flag;
	public bool selectionsMade;

	public List<Image> avatars;
	public List<Image> panels;
	// Use this for initialization
	public void open(List<Player> opponents){
		this.gameObject.SetActive (true);

		selection1flag = false;
		selection2flag = false;
		selection3flag = false;
		selectionsMade = false;
		selection1 = 0;
		selection2 = 0;
		selection3 = 0;

		glow1.gameObject.SetActive (false);
		glow2.gameObject.SetActive (false);
		glow3.gameObject.SetActive (false);

		//set opponenet stuff
		for (int i = 0; i < 3; i++) {
			if (i < opponents.Count) {
				avatars [i].gameObject.SetActive (true);
				panels [i].gameObject.SetActive (true);
				avatars [i].sprite = opponents [i].avatar;
				panels [i].color = opponents [i].playerColor;
			} else {
				avatars [i].gameObject.SetActive (false);
				panels [i].gameObject.SetActive (false);
				switch (i) {
				case 1:
					arr1.gameObject.SetActive (false);
					selection1flag = true;
					break;
				case 2:
					arr2.gameObject.SetActive (false);
					selection2flag = true;
					break;
				case 3:
					arr3.gameObject.SetActive (false);
					selection3flag = true;
					break;
				}
			}
		}
		//pass instance to buttons
		for(int i=0;i<sel1.Count;i++){
			sel1 [i].instance = this;
			sel2 [i].instance = this;
			sel3 [i].instance = this;
		}
	}
	public void confirmSelections(){

		if (selection1flag && selection2flag && selection3flag) {
			selectionsMade = true;
		}

	}
	public void setGlow(CommercialHarborPanelButton button,int num){

		switch(num) {
		case 1:
			glow1.gameObject.SetActive (true);
			glow1.gameObject.transform.position = button.gameObject.transform.position;
			break;
		case 2:
			glow2.gameObject.SetActive (true);
			glow2.gameObject.transform.position = button.gameObject.transform.position;
			break;
		case 3:
			glow3.gameObject.SetActive (true);
			glow3.transform.position = button.gameObject.transform.position;
			break;
		}
	}
	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		while (!selectionsMade) {
			yield return new WaitForEndOfFrame ();
		}
	}
}
