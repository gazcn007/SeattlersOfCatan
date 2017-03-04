using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberStealPanel : MonoBehaviour {

	public GameObject panel;
	public Dropdown opponentChoicesDropdown;
	public Button selectionMadeButton;

	public List<Player> choices;

	public bool selectionMade = false;

	// Use this for initialization
	void Start () {
		selectionMadeButton.onClick.AddListener (confirmSelection);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void displayPanelForChoices(List<Player> opponents) {
		opponentChoicesDropdown.ClearOptions ();

		List<string> playerNames = new List<string> ();
		for (int i = 0; i < opponents.Count; i++) {
			playerNames.Add (opponents [i].playerName);
		}

		opponentChoicesDropdown.AddOptions(playerNames);
		selectionMade = false;
		this.gameObject.SetActive (true);
	}

	public void confirmSelection() {
		selectionMade = true;
	}

	public int getSelection() {
		return opponentChoicesDropdown.value;
	}

	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
	
		while (!selectionMade) {
			yield return new WaitForEndOfFrame ();
		}

	}

}
