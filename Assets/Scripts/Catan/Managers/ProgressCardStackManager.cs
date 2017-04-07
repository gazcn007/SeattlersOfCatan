using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCardStackManager : MonoBehaviour {

	public ProgressCardType[] yellowCards;
	public ProgressCardType[] blueCards;
	public ProgressCardType[] greenCards;

	public List<ProgressCardType> yellowCardsQueue;
	public List<ProgressCardType> blueCardsQueue;
	public List<ProgressCardType> greenCardsQueue;

	// Use this for returning cards to deck
	public void returnCard(ProgressCardColor color, ProgressCardType type){
		switch (color) {
		case ProgressCardColor.Yellow:
			yellowCardsQueue.Add (type);
			break;
		case ProgressCardColor.Blue:
			blueCardsQueue.Add (type);
			break;
		case ProgressCardColor.Green:
			greenCardsQueue.Add (type);
			break;
		}
	}
	//use this for drawing cards from deck
	public ProgressCardType drawCard(ProgressCardColor color){
		ProgressCardType temp;
		switch (color) {
		case ProgressCardColor.Yellow:
			if (yellowCardsQueue.Count > 0) {
				temp = yellowCardsQueue [0];
				yellowCardsQueue.RemoveAt (0);
			} else {
				temp = ProgressCardType.None;
			}
			return temp;
		case ProgressCardColor.Blue:
			if (blueCardsQueue.Count > 0) {
				temp = blueCardsQueue [0];
				blueCardsQueue.RemoveAt (0);
			} else {
				temp = ProgressCardType.None;
			}
			return temp;
		case ProgressCardColor.Green:
			if (greenCardsQueue.Count > 0) {
				temp = greenCardsQueue [0];
				greenCardsQueue.RemoveAt (0);
			} else {
				temp = ProgressCardType.None;
			}
			return temp;
		}
		return ProgressCardType.None;
	}
	//this method is called to actually make the queues
	public void generateQueues(){
		for (int i = 0; i < yellowCards.Length; i++) {
			yellowCardsQueue.Add(yellowCards[i]);

		}
		for (int i = 0; i < blueCards.Length; i++) {
			blueCardsQueue.Add(blueCards[i]);
		}
		for (int i = 0; i < greenCards.Length; i++) {
			greenCardsQueue.Add(greenCards[i]);
		}
	}
	//used by masterclient to shuffled the cards
	public void shuffleCards(){
		yellowCards = shuffle (yellowCards);
		blueCards = shuffle (blueCards);
		greenCards = shuffle (greenCards);
	}
	//mixes order of cards
	private ProgressCardType[] shuffle(ProgressCardType[] arr){
		for (int i = arr.Length - 1; i > 0; i--) {
			int r = Random.Range (0, i);
			ProgressCardType tmp = arr [i];
			arr [i] = arr [r];
			arr [r] = tmp;
		}
		return arr;
	}
	public void playCard(ProgressCard Color, ProgressCardType type){

		switch (type) {
		case ProgressCardType.Merchant:
			playMerchant ();
			break;
		case ProgressCardType.CommercialHarbor:
			playCommercialHarbor ();
			break;
		case ProgressCardType.MerchantFleet:
			playMerchantFleet ();
			break;
		case ProgressCardType.MasterMerchant:
			playMasterMerchant ();
			break;
		case ProgressCardType.TradeMonopoly:
			playTradeMonolpoly ();
			break;
		case ProgressCardType.ResourceMonopoly:
			playResourceMonopoly ();
			break;
		case ProgressCardType.Bishop:
			playBishop ();
			break;
		case ProgressCardType.Diplomat:
			playDiplomat ();
			break;
		case ProgressCardType.Warlord:
			playWarlord ();
			break;
		case ProgressCardType.Wedding:
			playWedding ();
			break;
		case ProgressCardType.Intrigue:
			playIntrigue ();
			break;
		case ProgressCardType.Saboteur:
			playSaboteur ();
			break;
		case ProgressCardType.Spy:
			playSpy ();
			break;
		case ProgressCardType.Deserter:
			playDeserter ();
			break;
		case ProgressCardType.Constitution:
			playConstitution ();
			break;
		case ProgressCardType.Alchemist:
			playAlchemist ();
			break;
		case ProgressCardType.Crane:
			playCrane ();
			break;
		case ProgressCardType.Mining:
			playMining ();
			break;
		case ProgressCardType.Irrigation:
			playIrrigation ();
			break;
		case ProgressCardType.Printer:
			playPrinter ();
			break;
		case ProgressCardType.Inventor:
			playInventor ();
			break;
		case ProgressCardType.Engineer:
			playEngineer ();
			break;
		case ProgressCardType.Medicine:
			playMedicine ();
			break;
		case ProgressCardType.Smith:
			playSmith ();
			break;
		case ProgressCardType.RoadBuilding:
			playRoadBuilding ();
			break;
		}
	}

	private void playMerchant (){

	}
	private void playCommercialHarbor (){

	}
	private void playMerchantFleet (){

	}
	private void playMasterMerchant (){

	}
	private void playTradeMonolpoly (){

	}
	private void playResourceMonopoly (){

	}
	private void playBishop (){

	}
	private void playDiplomat (){

	}
	private void playWarlord (){

	}
	private void playWedding (){

	}
	private void playIntrigue (){

	}
	private void playSaboteur (){

	}
	private void playSpy (){

	}
	private void playDeserter (){

	}
	private void playConstitution (){

	}
	private void playAlchemist (){

	}
	private void playCrane (){

	}
	private void playMining (){

	}
	private void playIrrigation (){

	}
	private void playPrinter (){

	}
	private void playInventor (){

	}
	private void playEngineer (){

	}
	private IEnumerator playMedicine (){
		
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		AssetTuple tempasset = new AssetTuple ();
		//set the cost tuple = 2 ore and 1 grain
		tempasset.SetValueAtIndex (1, 1);
		tempasset.SetValueAtIndex (3, 2);
		//check if player has the assets required
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (tempasset) && clientCatanManager.players [PhotonNetwork.player.ID - 1].getOwnedUnitsOfType(UnitType.Settlement).Count>0){
			yield return StartCoroutine (EventTransferManager.instance.ClientUpgradeSettlement(PhotonNetwork.player.ID - 1));
			//refund standard cost-actualcost
		} else {
			clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = "Insufficient resources to play this card or no settlements to upgrade";
		}

	}
	private void playSmith (){

	}
	private void playRoadBuilding (){

	}
}
