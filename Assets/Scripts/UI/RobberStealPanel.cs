using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberStealPanel : MonoBehaviour {

	public GameObject panel;
	public Dropdown opponentChoicesDropdown;
	public Button selectionMadeButton;

	public List<Player> choices;

	// Use this for initialization
	void Start () {
		//selectionMadeButton.onClick.AddListener(
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
		this.gameObject.SetActive (true);
	}


}
