﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCard: MonoBehaviour  {

	public ProgressCardType type;
	public ProgressCardColor color;
	public Sprite cardSprite;
	public Image DisplayCard;
	public UIManager UIinstance;

	//mouseover effect functions
	public void OnMouseEnter(){
		if (DisplayCard.isActiveAndEnabled == false) {
			DisplayCard.gameObject.SetActive (true);
		}

		Vector3 position = new Vector3 (this.transform.position.x, DisplayCard.transform.position.y, 0);
		DisplayCard.transform.position = position;
		DisplayCard.sprite = cardSprite;
	}
	public void OnMouseExit(){
		if (DisplayCard.isActiveAndEnabled == true) {
			DisplayCard.gameObject.SetActive (false);
		}
	}
	//handler for triggering cards
	public void OnMouseClick(){
		//check if player can play card at this moment
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if (clientCatanManager.currentPlayerTurn == PhotonNetwork.player.ID - 1) {
			//alchemist check
			if (EventTransferManager.instance.diceRolledThisTurn && type == ProgressCardType.Alchemist) {
				clientCatanManager.uiManager.notificationpanel.SetActive (true);
				clientCatanManager.uiManager.notificationtext.text="you must play this card before rolling the dice";
			}else{
				if (EventTransferManager.instance.diceRolledThisTurn || type == ProgressCardType.Alchemist) {
					DisplayCard.gameObject.SetActive (false);
					UIinstance.progressCardPanel.SubmitCard (color, type);
				}
			}
		}
	}
}
public enum ProgressCardColor{
	Green = 0,
	Yellow,
	Blue
}
public enum ProgressCardType {
	None=0,
	Alchemist,
	Crane,
	Mining,
	Irrigation,
	Printer,
	Inventor,
	Engineer,
	Medicine,
	Smith,
	RoadBuilding,
	Merchant,
	CommercialHarbor,
	MerchantFleet,
	MasterMerchant,
	TradeMonopoly,
	ResourceMonopoly,
	Bishop,
	Diplomat,
	Warlord,
	Wedding,
	Intrigue,
	Saboteur,
	Spy,
	Deserter,
	Constitution
}