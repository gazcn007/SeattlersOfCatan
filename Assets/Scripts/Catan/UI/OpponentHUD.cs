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
	public Image oldBoot;

	private bool foundAvatar;

	// Use this for initialization
	void Start () {
		avatar = this.transform.Find ("avatar").GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
		UpdateUIForPlayer ();

		if (!foundAvatar) {
			if (displayingPlayer != null && displayingPlayer.avatar != null) {
				avatar.sprite = displayingPlayer.avatar;
				foundAvatar = true;
			}
		}
		if (displayingPlayer.hasOldBoot ()) {
			oldBoot.gameObject.SetActive (true);
		} else {
			oldBoot.gameObject.SetActive (false);
		}
	}

	private void UpdateUIForPlayer() { 
		if(displayingPlayer != null) {
			resourcesText.text = displayingPlayer.getNumAssets ().ToString();
			//cardsText.text = Player.METHODTOGTNUMRESOURCECARDSLOL ();
			roadsText.text = (displayingPlayer.getOwnedUnitsOfType(typeof(Road)).Count
				+ displayingPlayer.getOwnedUnitsOfType(typeof(Ship)).Count).ToString();
			knightsText.text = displayingPlayer.getOwnedUnitsOfType (typeof(Knight)).Count.ToString();
			VpText.text= displayingPlayer.victoryPoints.ToString();
		}
		
	}
	public void setPlayer(Player p){
		displayingPlayer = p;

		// name
		nametxt.text = p.playerName;
		//avatar.sprite = p.avatar;
		// color
		avatarpanel.color = p.playerColor;

	}
}
