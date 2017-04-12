using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
	public bool checkQueue(ProgressCardColor color){
		switch (color) {
		case ProgressCardColor.Yellow:
			return (yellowCardsQueue.Count > 0);
		case ProgressCardColor.Blue:
			return (blueCardsQueue.Count > 0);
		case ProgressCardColor.Green:
			return (greenCardsQueue.Count > 0);
		}
		return false;
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
	public void playCard(ProgressCardType type){

		switch (type) {
		case ProgressCardType.Merchant:
			StartCoroutine(playMerchant ());
			break;
		case ProgressCardType.CommercialHarbor:
			StartCoroutine(playCommercialHarbor ());
			break;
		case ProgressCardType.MerchantFleet:
			StartCoroutine(playMerchantFleet ());
			break;
		case ProgressCardType.MasterMerchant:
			StartCoroutine(playMasterMerchant ());
			break;
		case ProgressCardType.TradeMonopoly:
			StartCoroutine (playTradeMonopoly ());
			break;
		case ProgressCardType.ResourceMonopoly:
			StartCoroutine (playResourceMonopoly ());
			break;
		case ProgressCardType.Bishop:
			StartCoroutine(playBishop ());
			break;
		case ProgressCardType.Diplomat:
			StartCoroutine(playDiplomat ());
			break;
		case ProgressCardType.Warlord:
			playWarlord ();
			break;
		case ProgressCardType.Wedding:
			playWedding ();
			break;
		case ProgressCardType.Intrigue:
			StartCoroutine(playIntrigue ());
			break;
		case ProgressCardType.Saboteur:
			playSaboteur ();
			break;
		case ProgressCardType.Spy:
			StartCoroutine(playSpy ());
			break;
		case ProgressCardType.Deserter:
			StartCoroutine(playDeserter ());
			break;
		case ProgressCardType.Constitution:
			playConstitution ();
			break;
		case ProgressCardType.Alchemist:
			StartCoroutine(playAlchemist ());
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
			StartCoroutine(playInventor ());
			break;
		case ProgressCardType.Engineer:
			playEngineer ();
			break;
		case ProgressCardType.Medicine:
			StartCoroutine (playMedicine ());
			break;
		case ProgressCardType.Smith:
			StartCoroutine(playSmith ());
			break;
		case ProgressCardType.RoadBuilding:
			StartCoroutine(playRoadBuilding ());
			break;
		}
	}

	private IEnumerator playMerchant (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();	
		yield return StartCoroutine (clientCatanManager.moveGamePieceForCurrentPlayer (2, false, false));
		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text = "You now control the Merchant while doing so you can trade the resource the gamepiece is on at a 2:1 rate and get 1 Victory point";
		int adder = clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder;
		clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder=adder + 1;
		EventTransferManager.instance.increaseVpAdder (clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder, PhotonNetwork.player.ID - 1);
		if (clientCatanManager.merchantController>-1) {
			clientCatanManager.players [clientCatanManager.merchantController].vpAdder = clientCatanManager.players [clientCatanManager.merchantController].vpAdder - 1;
			EventTransferManager.instance.increaseVpAdder (clientCatanManager.players [clientCatanManager.merchantController].vpAdder,clientCatanManager.merchantController);
		}
		EventTransferManager.instance.setMerchantController();
		EventTransferManager.instance.NotifyProgressCard (ProgressCardType.Merchant, clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName);
		returnCardToStack (ProgressCardColor.Yellow, ProgressCardType.Merchant);
	}
	private IEnumerator playCommercialHarbor (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();		
		//get other players
		List<Player> tempPlayers=new List<Player>();
		int[] selections= new int[3];
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if (clientCatanManager.players[i].playerNumber-1 != PhotonNetwork.player.ID - 1) {
				tempPlayers.Add(clientCatanManager.players[i]);
			}
		}
		clientCatanManager.uiManager.commercialHarborPanel.open (tempPlayers);
		//wait for selection
		bool selectionMade = false;
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.commercialHarborPanel.selectionsMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.commercialHarborPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.commercialHarborPanel.selectionsMade) {
				selectionMade = true;
			}
		}
		//store temporary selections
		selections[0]=clientCatanManager.uiManager.commercialHarborPanel.selection1;
		selections[1]=clientCatanManager.uiManager.commercialHarborPanel.selection2;
		selections[2]=clientCatanManager.uiManager.commercialHarborPanel.selection3;

		//disabled panel
		clientCatanManager.uiManager.commercialHarborPanel.selectionsMade = false;
		clientCatanManager.uiManager.commercialHarborPanel.gameObject.SetActive (false);
		StartCoroutine(EventTransferManager.instance.playCommercialHarbor (tempPlayers, selections));
		returnCardToStack(ProgressCardColor.Yellow, ProgressCardType.CommercialHarbor);
	}
	private IEnumerator playMerchantFleet (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedMerchantFleet = true;

		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.fishresourcepanel.glow.gameObject.SetActive (false);
		//wait for selection
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.fishresourcepanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				selectionMade = true;
			}
		}
		clientCatanManager.players [PhotonNetwork.player.ID  - 1].merchantFleetSelection = clientCatanManager.uiManager.fishresourcepanel.getSelection();
		clientCatanManager.uiManager.fishresourcepanel.selectionMade = false;
		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (false);
		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text = "You may now trade the selected resource at a 2:1 ratio until the end of your turn";
		returnCardToStack(ProgressCardColor.Yellow, ProgressCardType.MerchantFleet);
	}
	private IEnumerator playMasterMerchant (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//first get possible players
		List <Player> possibleplayer=new List<Player>();
		for (int i = 0; i<PhotonNetwork.playerList.Length; i++) {
			if (PhotonNetwork.player.ID-1 != i && clientCatanManager.players [i].getVpPoints () >= clientCatanManager.players [PhotonNetwork.player.ID].getVpPoints ()) {
				possibleplayer.Add (clientCatanManager.players [i]);
			}
		}
		//make user select 1 of the players
		clientCatanManager.uiManager.robberStealPanel.displayPanelForChoices(possibleplayer);
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.robberStealPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
				selectionMade = true;
			}
		}
		//get player selection
		int selection = clientCatanManager.uiManager.robberStealPanel.selection;
		Debug.Log ("selection: " +selection);
		//take off robber steal panel
		clientCatanManager.uiManager.robberStealPanel.selectionMade = false;
		clientCatanManager.uiManager.robberStealPanel.gameObject.SetActive (false);
		//now show resource selection
		clientCatanManager.uiManager.discardPanel.title.text = "Select 2 resources to take";
		clientCatanManager.uiManager.discardPanel.displayPanelMasterMerchant(clientCatanManager.players [selection].assets, 2);

		selectionMade = false;

		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.discardPanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.discardPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.discardPanel.selectionMade) {
				selectionMade = true;
			}
		}
		AssetTuple temptuple = clientCatanManager.uiManager.discardPanel.discardTuple;
		clientCatanManager.uiManager.discardPanel.selectionMade = false;
		clientCatanManager.uiManager.discardPanel.gameObject.SetActive (false);
		clientCatanManager.uiManager.discardPanel.title.text = "Discard:";

		EventTransferManager.instance.OnTradeWithBank (selection, false, temptuple);
		EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID-1, true, temptuple);
		//build notification message
		string message = clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName + " has played the Master Merchant card.and forced you to give: ";
		for(int i =0;i < 8 ; i++){
			if(temptuple.GetValueAtIndex(i)>0){
				if (i < 5) {
					message = message + temptuple.GetValueAtIndex (i) +" "+ ((ResourceType)i).ToString()+" and ";

				} else {
					message = message + temptuple.GetValueAtIndex (i) +" "+  ((CommodityType)(i-5)).ToString()+" and ";
				}
			}
		}
		message = message.Substring (0, message.Length - 4);
		EventTransferManager.instance.sendNotification (message, selection);
		returnCardToStack (ProgressCardColor.Yellow, ProgressCardType.MasterMerchant);
	}
	private IEnumerator playTradeMonopoly (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.fishresourcepanel.glow.gameObject.SetActive (false);
		//deactivate resource options
		for (int i = 0; i < 5; i++) {
			clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (false);
		}
		//wait for selection
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.fishresourcepanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				selectionMade = true;
			}
		}
		clientCatanManager.uiManager.fishresourcepanel.selectionMade = false;
		int selection = clientCatanManager.uiManager.fishresourcepanel.getSelection();
		//reactivate resource options
		for (int i = 0; i < 5; i++) {
			clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (true);
		}
		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (false);
		//call to transfer manager to actually do the changes
		EventTransferManager.instance.playMonopoly(PhotonNetwork.player.ID-1,selection,true);
		//return card
		returnCardToStack(ProgressCardColor.Yellow, ProgressCardType.TradeMonopoly);
	}
	private IEnumerator playResourceMonopoly (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.fishresourcepanel.glow.gameObject.SetActive (false);
		//deactivate resource options
		for (int i = 5; i < 8; i++) {
			clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (false);
		}
		//wait for selection
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.fishresourcepanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
				selectionMade = true;
			}
		}
		clientCatanManager.uiManager.fishresourcepanel.selectionMade = false;
		int selection = clientCatanManager.uiManager.fishresourcepanel.getSelection();
		//reactivate resource options
		for (int i = 5; i < 8; i++) {
			clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (true);
		}
		clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (false);
		//call to transfer manager to actually do the changes
		EventTransferManager.instance.playMonopoly(PhotonNetwork.player.ID-1,selection,false);
		//return card
		returnCardToStack(ProgressCardColor.Yellow, ProgressCardType.TradeMonopoly);
	}
	private IEnumerator playBishop (){

		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedBishop = true;
		yield return StartCoroutine(clientCatanManager.moveGamePieceForCurrentPlayer(0, false, true));
		returnCardToStack(ProgressCardColor.Blue, ProgressCardType.Bishop);
		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedBishop = false;
	}
	private IEnumerator playDiplomat (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		yield return StartCoroutine(clientCatanManager.playDiplomat ());
		returnCardToStack (ProgressCardColor.Blue, ProgressCardType.Diplomat);
	}
	private void playWarlord (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		
		List<Knight> tempknights = clientCatanManager.players [PhotonNetwork.player.ID-1].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => !knight.isActive).ToList ();
		for (int i = 0; i < tempknights.Count; i++) {
			EventTransferManager.instance.OnKnightActionForUser (MoveType.ActivateKnight, PhotonNetwork.player.ID-1, tempknights[i].id, -1, true, false);
		}
		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text="All your knights have been activated for free";
		EventTransferManager.instance.NotifyProgressCard(ProgressCardType.Warlord,clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName);
		returnCardToStack(ProgressCardColor.Blue, ProgressCardType.Warlord);
	}
	private void playWedding (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		StartCoroutine(EventTransferManager.instance.playWedding (clientCatanManager.players [PhotonNetwork.player.ID - 1].getVpPoints(), PhotonNetwork.player.ID - 1, clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName ));
		returnCardToStack(ProgressCardColor.Blue, ProgressCardType.Wedding);
	}
	private IEnumerator playIntrigue (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		yield return StartCoroutine (clientCatanManager.playIntrigue ());
		returnCardToStack(ProgressCardColor.Blue, ProgressCardType.Intrigue);
	}
	private void playSaboteur (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		StartCoroutine(EventTransferManager.instance.playSaboteur (clientCatanManager.players [PhotonNetwork.player.ID - 1].getVpPoints(), PhotonNetwork.player.ID - 1, clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName ));
		returnCardToStack(ProgressCardColor.Blue, ProgressCardType.Saboteur);
	}
	private IEnumerator playSpy (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//first get possible players
		List <Player> possibleplayer=new List<Player>();
		for (int i = 0; i<PhotonNetwork.playerList.Length; i++) {
			if (PhotonNetwork.player.ID-1 != i && clientCatanManager.players [i].progressCards.Count>0) {
				possibleplayer.Add (clientCatanManager.players [i]);
			}
		}
		//make user select 1 of the players
		clientCatanManager.uiManager.robberStealPanel.displayPanelForChoices(possibleplayer);
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.robberStealPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
				selectionMade = true;
			}
		}
		//get player selection
		int selection = clientCatanManager.uiManager.robberStealPanel.selection;

		//take off robber steal panel
		clientCatanManager.uiManager.robberStealPanel.selectionMade = false;
		clientCatanManager.uiManager.robberStealPanel.gameObject.SetActive (false);

		selectionMade = false;
		//get the selection
		clientCatanManager.uiManager.spyPanel.openPanel(clientCatanManager.players[selection].progressCards,"Select Card To Steal: ");
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.spyPanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.spyPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.spyPanel.selectionMade) {
				selectionMade = true;
			}
		}
		ProgressCardType selectedcard = clientCatanManager.uiManager.spyPanel.selection;
		int temp = (int)selectedcard;
		ProgressCardColor selectedcolor;
		if (temp <= 10) {
			selectedcolor = ProgressCardColor.Green;
		}else if(temp<=16){
			selectedcolor = ProgressCardColor.Yellow;
		}
		else{
			selectedcolor=ProgressCardColor.Blue;
		}
		selectionMade = false;
		clientCatanManager.uiManager.spyPanel.selectionMade=false;
		clientCatanManager.uiManager.spyPanel.gameObject.SetActive (false);

		StartCoroutine(clientCatanManager.uiManager.progressCardHolder.SpawnCard (selectedcolor, selectedcard));
		EventTransferManager.instance.sendNotification (clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName + " has played the Spy card and stolen your " + selectedcard.ToString () + " card", selection);
		EventTransferManager.instance.removeCardFromHand (selection, selectedcard);
		returnCardToStack (ProgressCardColor.Blue, ProgressCardType.Spy);
	}
	private IEnumerator playDeserter (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//first get possible players
		List <Player> possibleplayer=new List<Player>();
		for (int i = 0; i<PhotonNetwork.playerList.Length; i++) {
			if (PhotonNetwork.player.ID-1 != i && clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().ToList().Count>0) {
				possibleplayer.Add (clientCatanManager.players [i]);
			}
		}
		if (possibleplayer.Count == 0) {
			clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = "No Players Have any Knights to destroy";
		} else {
			//make user select 1 of the players
			clientCatanManager.uiManager.robberStealPanel.displayPanelForChoices (possibleplayer);
			bool selectionMade = false;
			//get the selection
			while (!selectionMade) {
				if (!CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
					yield return StartCoroutine (CatanManager.instance.uiManager.robberStealPanel.waitUntilButtonDown ());
				}
				if (CatanManager.instance.uiManager.robberStealPanel.selectionMade) {
					selectionMade = true;
				}
			}
			//get player selection
			int selection = clientCatanManager.uiManager.robberStealPanel.selection;

			//take off robber steal panel
			clientCatanManager.uiManager.robberStealPanel.selectionMade = false;
			clientCatanManager.uiManager.robberStealPanel.gameObject.SetActive (false);
			yield return StartCoroutine (EventTransferManager.instance.playDeserter (selection));
			returnCardToStack (ProgressCardColor.Blue, ProgressCardType.Deserter);
		}

	}
	private void playConstitution (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].playedConstitution==false) {
			clientCatanManager.players [PhotonNetwork.player.ID - 1].playedConstitution = true;
			int adder = clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder;
			clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder=adder+1;
			EventTransferManager.instance.NotifyProgressCard (ProgressCardType.Constitution, clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName);
			EventTransferManager.instance.increaseVpAdder (clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder, PhotonNetwork.player.ID - 1);
		}
	}
	private IEnumerator playAlchemist (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//select dice rolls
		clientCatanManager.uiManager.alchemistPanel.Open();
		bool selectionMade = false;
		//get the selection
		while (!selectionMade) {
			if (!CatanManager.instance.uiManager.alchemistPanel.selectionMade) {
				yield return StartCoroutine (CatanManager.instance.uiManager.alchemistPanel.waitUntilButtonDown ());
			}
			if (CatanManager.instance.uiManager.alchemistPanel.selectionMade) {
				selectionMade = true;
			}
		}
		//get player selection
		int redDie = (int)clientCatanManager.uiManager.alchemistPanel.redDieVal.value;
		int yellowDie = (int)clientCatanManager.uiManager.alchemistPanel.yellowDieVal.value;
		//turn panel off
		clientCatanManager.uiManager.alchemistPanel.selectionMade = false;
		clientCatanManager.uiManager.alchemistPanel.gameObject.SetActive (false);
		StartCoroutine (EventTransferManager.instance.playAlchemist (redDie, yellowDie));
		returnCardToStack (ProgressCardColor.Green, ProgressCardType.Alchemist);
	}
	private void playCrane (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedCrane = true;
		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text = "The next city upgrade will cost 1 less commodity, effect expires at the end of the current turn";
		returnCardToStack (ProgressCardColor.Green, ProgressCardType.Crane);
	}
	private void playMining (){
		int count = 0;
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		count= clientCatanManager.boardManager.getAdjacentTilesOfType(PhotonNetwork.player.ID-1,TileType.Mountains).Count;
		AssetTuple tempasset = new AssetTuple ();
		Debug.Log ("Adjacent tiles: " +count);
		tempasset.SetValueAtIndex (3, 2 * count);
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].canPlayMining) {
			clientCatanManager.players [PhotonNetwork.player.ID - 1].canPlayMining= false;
			EventTransferManager.instance.OnTradeWithBank(PhotonNetwork.player.ID-1,true,tempasset);
		}
		returnCardToStack (ProgressCardColor.Green, ProgressCardType.Mining);
	}
	private void playIrrigation (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		int count= clientCatanManager.boardManager.getAdjacentTilesOfType(PhotonNetwork.player.ID-1,TileType.Fields).Count;
		AssetTuple tempasset = new AssetTuple ();
		tempasset.SetValueAtIndex (1, 2 * count);
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].canPlayIrrigation) {
			clientCatanManager.players [PhotonNetwork.player.ID - 1].canPlayIrrigation = false;
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, true, tempasset);
		}
		returnCardToStack (ProgressCardColor.Green, ProgressCardType.Irrigation);
	}
	private void playPrinter (){
		
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].playedPrinter==false) {
			clientCatanManager.players [PhotonNetwork.player.ID - 1].playedPrinter= true;
			int adder = clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder;
			clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder=adder+1;
			EventTransferManager.instance.NotifyProgressCard(ProgressCardType.Printer,clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName);
			EventTransferManager.instance.increaseVpAdder (clientCatanManager.players [PhotonNetwork.player.ID - 1].vpAdder, PhotonNetwork.player.ID - 1);
		}
	}
	private IEnumerator playInventor (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		yield return clientCatanManager.playInventor ();
		returnCardToStack (ProgressCardColor.Green, ProgressCardType.Inventor);
	}
	private IEnumerator playEngineer (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		List<CityWall> cityWalls = clientCatanManager.players [PhotonNetwork.player.ID - 1].getOwnedUnitsOfType (UnitType.CityWalls).Cast<CityWall> ().ToList();

		if (cityWalls.Count >= 3) {
			clientCatanManager.uiManager.notificationpanel.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = "Cannot have more then 3 City walls";
			yield break;
		}

		List<City> ownedCities = clientCatanManager.players [PhotonNetwork.player.ID - 1].getOwnedUnitsOfType (UnitType.City).Cast<City> ().Where(city => city.cityWalls == null).ToList();
		List<Metropolis> ownedMetropolises = clientCatanManager.players [PhotonNetwork.player.ID - 1].getOwnedUnitsOfType (UnitType.Metropolis).Cast<Metropolis> ().Where(m => m.cityWalls == null).ToList();

		List<Unit> allPossibleUnits = ownedCities.Cast<Unit> ().ToList ().Union (ownedMetropolises.Cast<Unit> ().ToList ()).ToList ();
		if (ownedCities.Count == 0 && ownedMetropolises.Count == 0) {
			clientCatanManager.uiManager.notificationpanel.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = "Nothing to upgrade";
			yield break;
		} else {
			yield return StartCoroutine (clientCatanManager.buildCityWall (false));
			returnCardToStack (ProgressCardColor.Green, ProgressCardType.Engineer);

		}

	}
	private IEnumerator playMedicine (){

		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		AssetTuple tempasset = new AssetTuple ();
		//set the cost tuple = 2 ore and 1 grain
		tempasset.SetValueAtIndex (1, 1);
		tempasset.SetValueAtIndex (3, 2);
		//check if player has the assets required
		if (clientCatanManager.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (tempasset) && clientCatanManager.players [PhotonNetwork.player.ID - 1].getOwnedUnitsOfType(UnitType.Settlement).Count>0){
			tempasset.SetValueAtIndex (1, 1);
			tempasset.SetValueAtIndex (3, 1);
			yield return StartCoroutine (EventTransferManager.instance.ClientUpgradeSettlement(PhotonNetwork.player.ID - 1));
			EventTransferManager.instance.OnTradeWithBank(PhotonNetwork.player.ID - 1, true,tempasset);
			returnCardToStack(ProgressCardColor.Green, ProgressCardType.Medicine);
		} else {
			clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = "Insufficient resources to play this card or no settlements to upgrade";
		}

	}
	private IEnumerator playSmith (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text="Select 2 Knights to promote";;
		for (int i = 0; i < 2; i++) {
			List<Knight> promotableKnights=new List<Knight>();
			List<Knight> ownedKnights = clientCatanManager.players [PhotonNetwork.player.ID-1].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => knight.rank != KnightRank.Mighty).ToList ();
			for (int j = 0; j < ownedKnights.Count; j++) {
				if (clientCatanManager.players[PhotonNetwork.player.ID-1].unlockedFortress () || ownedKnights [i].rank != KnightRank.Strong) {
					promotableKnights.Add (ownedKnights [i]);
				}
			}
			if (promotableKnights.Count == 0) {
				clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
				clientCatanManager.uiManager.notificationtext.text="No knights to promote";
				break;

			}
			if (promotableKnights.Count > 0) {
				yield return StartCoroutine (clientCatanManager.promoteKnight (false));
			}
		}
		returnCardToStack(ProgressCardColor.Green, ProgressCardType.Smith);
	}
	private IEnumerator playRoadBuilding (){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedRoadBuilding = true;

		yield return StartCoroutine(clientCatanManager.buildEdgeUnit (UnitType.Road, false));
		yield return StartCoroutine(clientCatanManager.buildEdgeUnit (UnitType.Road, false));

		clientCatanManager.players [PhotonNetwork.player.ID - 1].playedRoadBuilding = false;
		returnCardToStack(ProgressCardColor.Green, ProgressCardType.RoadBuilding);
	}
	public void returnCardToStack(ProgressCardColor color, ProgressCardType type){
		
		//delete function for later
		EventTransferManager.instance.removeCardFromHand(PhotonNetwork.player.ID - 1,type);
		EventTransferManager.instance.ReturnProgressCard (color, type);
	}
	public void loadCardState(int[] yellowCards,int[] greenCards,int[] blueCards){
		for (int i = 0; i < yellowCards.Length; i++) {
			yellowCardsQueue.Add ((ProgressCardType)yellowCards [i]);
		}
		for (int i = 0; i < greenCards.Length; i++) {
			greenCardsQueue.Add ((ProgressCardType)greenCards [i]);
		}
		for (int i = 0; i < blueCards.Length; i++) {
			blueCardsQueue.Add ((ProgressCardType)blueCards [i]);
		}
	}
}
