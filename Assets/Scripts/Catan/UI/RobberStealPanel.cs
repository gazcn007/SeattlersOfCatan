using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberStealPanel : MonoBehaviour {

	public GameObject panel;
	public Button selectionMadeButton;
	public Image selectionGlow;
	public int selection;
	public List<Button> optionsPanel;
	public bool selectionMade = false;

	// Use this for initialization
	void Start () {
		selectionMadeButton.onClick.AddListener (confirmSelection);
		//optionsPanel = this.transform.FindChild("optionsPanel").gameObject.GetComponentsInChildren<Button> ();
		selectionGlow.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void displayPanelForChoices(List<Player> opponents) {
		this.gameObject.SetActive (true);
		selectionGlow.gameObject.SetActive (false);

		for (int i = 0; i<3; i++) {
			Debug.Log ("i:"+i);
			if ((i+1)>opponents.Count) {
				Debug.Log ("i");
				optionsPanel [i].gameObject.SetActive (false);
			} else {
				optionsPanel [i].gameObject.SetActive (true);
				optionsPanel [i].image.color = opponents [i].playerColor;
				RobberStealPanelButton current= optionsPanel [i].GetComponentInChildren<RobberStealPanelButton>();

				//set values for the buttons 
				current.instance = this;
				current.playernumber = opponents [i].playerNumber - 1;
				current.avatar.sprite = opponents [i].avatar;
		
				//set button
				optionsPanel[i].onClick.AddListener (current.UpdateSelection);
			}
		}
		//opponentChoicesDropdown.AddOptions(playerNames);
		selectionMade = false;
	}

	public void confirmSelection() {
		selectionMade = true;
	}

	public int getSelection() {
		return selection;
	}
	public void setSelectionGlow(RobberStealPanelButton button){
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}
	public IEnumerator waitUntilButtonDown() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
	
		while (!selectionMade) {
			yield return new WaitForEndOfFrame ();
		}
	}

}
