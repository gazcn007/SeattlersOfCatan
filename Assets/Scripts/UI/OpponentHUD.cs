using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHUD : MonoBehaviour {

	public Text nametxt;
	public Text VpText;

	public Text resourcesText;
	public Text cardsText;
	public Text roadsText;
	public Text knightsText;

	public Image avatarpanel;
	public Image avatar;
	public Player displayingPlayer;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		UpdateUIForPlayer ();
	}

	private void UpdateUIForPlayer() { 

		resourcesText.text = displayingPlayer.getNumResources ().ToString();
		//cardsText.text = Player.METHODTOGTNUMRESOURCECARDSLOL ();
		roadsText.text = (displayingPlayer.getOwnedUnitsOfType(typeof(Road)).Count
			+ displayingPlayer.getOwnedUnitsOfType(typeof(Ship)).Count).ToString();
		knightsText.text = displayingPlayer.getOwnedUnitsOfType (typeof(Knight)).Count.ToString();
		VpText.text= displayingPlayer.victoryPoints.ToString();
	}
	public void setPlayer(Player p){
		displayingPlayer = p;

		// name
		nametxt.text = p.playerName;
		avatar.sprite = p.avatar;
		// color
		avatarpanel.color = p.playerColor;

	}
}
