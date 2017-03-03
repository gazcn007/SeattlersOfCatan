using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	public Text nametxt;
	public Text goldText;
	public Text VpText;

	// Resources
	public Text bricktxt;
	public Text graintxt;
	public Text lumbertxt;
	public Text oretxt;
	public Text wooltxt;

	// Commodities
	public Text papertxt;
	public Text cointxt;
	public Text clothtxt;

	public Text resourcesText;
	public Text cardsText;
	public Text roadsText;
	public Text knightsText;

	public Image avatarpanel;

	public Player displayingPlayer;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		UpdateUIForPlayer ();
	}

	private void UpdateUIForPlayer() { 
		// resources
		bricktxt.text= displayingPlayer.assets.resources.resourceTuple[ResourceType.Brick].ToString();
		graintxt.text= displayingPlayer.assets.resources.resourceTuple[ResourceType.Grain].ToString();
		lumbertxt.text= displayingPlayer.assets.resources.resourceTuple[ResourceType.Lumber].ToString();
		oretxt.text= displayingPlayer.assets.resources.resourceTuple[ResourceType.Ore].ToString();
		wooltxt.text= displayingPlayer.assets.resources.resourceTuple[ResourceType.Wool].ToString();

		// commodities
		papertxt.text= displayingPlayer.assets.commodities.commodityTuple[CommodityType.Paper].ToString();
		cointxt.text= displayingPlayer.assets.commodities.commodityTuple[CommodityType.Coin].ToString();
		clothtxt.text= displayingPlayer.assets.commodities.commodityTuple[CommodityType.Cloth].ToString();


		//resourcesText.text = displayingPlayer.getNumResources ().ToString();

		resourcesText.text = displayingPlayer.getNumResources ().ToString();

		//cardsText.text = Player.METHODTOGTNUMRESOURCECARDSLOL ();
		roadsText.text = (displayingPlayer.getOwnedUnitsOfType(typeof(Road)).Count
			+ displayingPlayer.getOwnedUnitsOfType(typeof(Ship)).Count).ToString();
		knightsText.text = displayingPlayer.getOwnedUnitsOfType (typeof(Knight)).Count.ToString();
		goldText.text= displayingPlayer.goldCoins.ToString();
		VpText.text= displayingPlayer.victoryPoints.ToString();

	}

	public void setPlayer(Player p){
		displayingPlayer = p;

		// name
		nametxt.text = p.playerName;
		// color
		avatarpanel.color = p.playerColor;

	}
}