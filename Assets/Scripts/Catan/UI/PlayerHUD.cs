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
	public Image avatar;
	public Image oldBoot;
	public Player displayingPlayer;

	//fish
	public Text oneFishText;
	public Text twoFishText;
	public Text threeFishText;

	public Button FishDisplay;
	public Button ResourceDisplay;
	public GameObject fishpanel;
	public GameObject resourcepanel;

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

			oneFishText.text = displayingPlayer.assets.fishTokens.fishTuple[FishTokenType.One].ToString();
			twoFishText.text = displayingPlayer.assets.fishTokens.fishTuple[FishTokenType.Two].ToString();
			threeFishText.text = displayingPlayer.assets.fishTokens.fishTuple[FishTokenType.Three].ToString();

			//resourcesText.text = displayingPlayer.getNumResources ().ToString();

			resourcesText.text = displayingPlayer.getNumAssets ().ToString();

			//cardsText.text = Player.METHODTOGTNUMRESOURCECARDSLOL ();
			roadsText.text=displayingPlayer.getNumFishTokens().ToString();
			//roadsText.text = (displayingPlayer.getOwnedUnitsOfType(typeof(Road)).Count
			//	+ displayingPlayer.getOwnedUnitsOfType(typeof(Ship)).Count).ToString();
			knightsText.text = displayingPlayer.getOwnedUnitsOfType (typeof(Knight)).Count.ToString();
			cardsText.text =""+ displayingPlayer.progressCards.Count;
			goldText.text =""+ displayingPlayer.getGoldCoinsCnt ();
			VpText.text= displayingPlayer.victoryPoints.ToString();
		}

	}

	public void setPlayer(Player p){
		displayingPlayer = p;

		// name
		nametxt.text = p.playerName;
		// color
		avatarpanel.color = p.playerColor;
		//avatar.sprite = p.avatar;

	}
	public void SetResources(){
		FishDisplay.gameObject.SetActive (true);
		ResourceDisplay.gameObject.SetActive (false);
		resourcepanel.gameObject.SetActive (true);
		fishpanel.gameObject.SetActive (false);
		
	}
	public void SetFish(){
		FishDisplay.gameObject.SetActive (false);
		ResourceDisplay.gameObject.SetActive (true);
		resourcepanel.gameObject.SetActive (false);
		fishpanel.gameObject.SetActive (true);
	}

}