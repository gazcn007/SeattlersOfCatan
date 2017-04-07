using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public UIManager instance = null;

	public int offset = 0;

	public Image currentTurnColor;
	public Image currentTurnAvatar;

	public PlayerHUD playerHUD;
	public OpponentHUD[] opponentHUDs;

	public Button[] uiButtons;

	public ProgressCardHolder progressCardHolder;
	public ProgressCardPanel progressCardPanel;
	public TradePlayerPanel tradePlayerPanel;

	public BuildPanel buildPanel;
	public TradePanel tradePanel;
	public RobberStealPanel robberStealPanel;
	public DiscardPanel discardPanel;
	public FishTradePanel fishTradePanel;
	public FishResourcePanel fishresourcepanel;
	//no script needed for this
	public GameObject robberOrPiratePanel;
	public GameObject costspanel;
	public FlipChartPanel flipchart;
	public GameObject notificationpanel;
	public Text notificationtext;

	void Awake() {
		if (instance == null)
			instance = this;
	}

	// Use this for initialization
	void Start () {
		InitializeUIManager ();
		SetupPlayers ();
	}

	void Update() {
		currentTurnColor.color = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor;
		currentTurnAvatar.sprite = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].avatar;
	}

	#region Initializers

	void InitializeUIManager() {

		//progress card initiators
		progressCardHolder.UIinstance = this;
		progressCardPanel.cardHolder = progressCardHolder;

		//a few progress card tests
		/*
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Alchemist);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Diplomat);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Merchant);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Crane);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.ResourceMonopoly);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Smith);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Deserter);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Engineer);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.MasterMerchant);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Wedding);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.Warlord);
		progressCardHolder.SpawnCard(ProgressCardColor.Yellow,ProgressCardType.CommercialHarbor);
*/


		//just in case due to constant editing
		tradePanel.gameObject.SetActive (false);
		buildPanel.gameObject.SetActive (false);

	}

	void SetupPlayers() {
		int key1 = (1 + offset) % PhotonNetwork.playerList.Length;

		Player player1 = LevelManager.instance.players[(0 + offset) % PhotonNetwork.playerList.Length];
		Player player2 = LevelManager.instance.players[(1 + offset) % PhotonNetwork.playerList.Length];

		if (PhotonNetwork.playerList.Length >= 3) {
			Player player3 = LevelManager.instance.players [(2 + offset) % PhotonNetwork.playerList.Length];

			if (PhotonNetwork.playerList.Length == 4) {
				Player player4 = LevelManager.instance.players [(3 + offset) % PhotonNetwork.playerList.Length];

				opponentHUDs [2].GetComponent<OpponentHUD> ().setPlayer (player4);
			} else {
				opponentHUDs [2].gameObject.SetActive (false);
			}
			opponentHUDs [1].GetComponent<OpponentHUD> ().setPlayer (player3);
		} else {
			opponentHUDs [1].gameObject.SetActive (false);
			opponentHUDs [2].gameObject.SetActive (false);
		}
		opponentHUDs [0].GetComponent<OpponentHUD> ().setPlayer (player2);
		playerHUD.GetComponent<PlayerHUD> ().setPlayer (player1);
	}

	#endregion


	#region Button Action Listener Methods

	public void buildSettlementEvent() {
		int buttonId = 1;
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientBuildSettlementForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();

					BoardManager.instance.highlightAllIntersections (false);
					EventTransferManager.instance.OnOperationFailure ();
					//uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
					// SOME WAY TO UNHIGHLIGHT BUTTON
				}
			}
		}
	}

	public void buildRoadEvent() {
		int buttonId = 2;
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientBuildRoadForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();

					BoardManager.instance.highlightAllEdges (false);
					EventTransferManager.instance.OnOperationFailure ();
					//uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
					// SOME WAY TO UNHIGHLIGHT BUTTON
				}
			}
		}
	}

	public void buildShipEvent() {
		int buttonId = 3;
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientBuildShipForAll(PhotonNetwork.player.ID - 1));
			}else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();

					BoardManager.instance.highlightAllEdges (false);
					EventTransferManager.instance.OnOperationFailure ();
					//uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
					// SOME WAY TO UNHIGHLIGHT BUTTON
				}
			}

		}
	}

	public void diceRollEvent() {
		Debug.Log ("diceRollEvent()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer) {
				EventTransferManager.instance.OnDiceRolled ();
			}
		}
	}

	public void upgradeSettlementEvent() {
		int buttonId = 5;
		Debug.Log ("upgradeSettlementEvent()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientUpgradeSettlement(PhotonNetwork.player.ID - 1));
			}else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();

					BoardManager.instance.highlightUnitsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Settlement)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					EventTransferManager.instance.OnOperationFailure ();
					//uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
					// SOME WAY TO UNHIGHLIGHT BUTTON
				}
			}
		}
	}
	public void toggleFlipChartPanel (){
		int buttonId = 4;
		Debug.Log ("toggleFlipChartPanel()");
		if (flipchart.isActiveAndEnabled == false && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				int scienceLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Science];
				int politicsLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Politics];
				int tradeLevel = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].cityImprovements.cityImprovements [CityImprovementType.Trade];

				EventTransferManager.instance.waitingForPlayer = true;
				EventTransferManager.instance.currentActiveButton = buttonId;
				flipchart.openPanel (tradeLevel, politicsLevel, scienceLevel);
			}
		} else {
			if (EventTransferManager.instance.currentActiveButton == buttonId) {
				flipchart.gameObject.SetActive (false);
				EventTransferManager.instance.OnOperationFailure ();
			}
		}

	}
	public void togglerCostsPanel(){
		int buttonId = 6;
		if(costspanel.activeSelf == true){
			costspanel.SetActive (false);
		} else{
			costspanel.SetActive (true);
		}
	}
	public void robberPirateSelection(int selection){
		robberOrPiratePanel.SetActive (false);
		//nehir this will be called by panel 0=robber 1=pirate
	}

	public void fishResourceSelection(){
		//stub for method in fish resource panel bindings already done
		fishresourcepanel.gameObject.SetActive(true);
	}

	public void toggleFishTradePanel(){
		//nehir add however you need to call this eventtransfer manager
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.waitingForPlayer = true;
				fishTradePanel.OpenPanel (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getCurrentAssets ());
			}
		}
	}

	public void tradeWithBankEvent() {
		int buttonId = 7;
		Debug.Log ("tradeWithBankEvent()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;
				//tradePanel.gameObject.SetActive (true);
				EventTransferManager.instance.ClientTradeBank(PhotonNetwork.player.ID - 1);
			}
		}
	}

	public void moveShipEvent(){
		int buttonId = 8;
		Debug.Log ("moveShipEvent()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientMoveShip(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();

					BoardManager.instance.highlightUnitsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Ship)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					BoardManager.instance.highlightAllEdges (false);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void togglerPlayerTradePanel(){
		int buttonId = 9;
		Debug.Log ("tradeWithPlayerEvent()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
				EventTransferManager.instance.currentActiveButton = buttonId;
				EventTransferManager.instance.ClientTradePlayer(PhotonNetwork.player.ID - 1);
			}
		}
	}

	public void endTurn() {
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				//CatanManager.instance.currentPlayerTurn = (CatanManager.instance.currentPlayerTurn + 1) % PhotonNetwork.playerList.Length;
				//UnitManager.instance.destroyCancelledUnits ();
				EventTransferManager.instance.OnEndTurn();
				deactivateButtons ();
			}
		}
	}

	public void deactivateButtons() {
		if (buildPanel.enabled == true) {
			// SOME WAY TO MAKE THE BUTTON HIGHLIGHTED
			buildPanel.gameObject.SetActive (false);
			if (tradePanel.enabled) {
				//tradeCancelled ();
			}
		}
	}

	public void tradeDone() {
		int tradeRatio = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getMinimumTradeValue (tradePanel.getTradeChoiceInt ());
		if (tradePanel.getTradeChoiceInt () >= 5 && CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].unlockedTradingHouse ()) {
			tradeRatio = 2;
		}
		AssetTuple assetsToSpend = GameAsset.getAssetOfIndex (tradePanel.getTradeChoiceInt (), tradeRatio);
		AssetTuple assetsToReceive = GameAsset.getAssetOfIndex (tradePanel.getReceiveChoiceInt (), 1);

		if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].hasAvailableAssets (assetsToSpend)) { 
			//CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].spendAssets (assetsToSpend);
			//CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].receiveAssets (assetsToReceive);
			EventTransferManager.instance.OnTradeWithBank(CatanManager.instance.currentPlayerTurn, false, assetsToSpend);
			EventTransferManager.instance.OnTradeWithBank(CatanManager.instance.currentPlayerTurn, true, assetsToReceive);

			//print (players [currentPlayerTurn].playerName + " gives 4 " + tradePanel.getTradeChoiceInt ().ToString () + " to the bank and receives 1 " + assetsToReceive.ToString());
			tradePanel.hideErrorText ();
			tradePanel.gameObject.SetActive (false);

			EventTransferManager.instance.OnOperationFailure ();
		} else {
			print ("Insufficient resources! Please try again...");
			tradePanel.showNotEnoughError (tradePanel.getTradeChoiceInt ());
		}
	}

	public void offerTrade() {
		tradePlayerPanel.sendTrade ();
	}
	public void tradeCancelled() {
		StopAllCoroutines();

		if (tradePanel.enabled) {
			tradePanel.hideErrorText ();
			tradePanel.gameObject.SetActive (false);
		}

		EventTransferManager.instance.OnOperationFailure ();
	}

	public void Test(){
		//StartCoroutine(EventTransferManager.instance.CardDrawEvent());
		notificationpanel.SetActive (true);
		notificationtext.text = "test";
	}

	#endregion
}