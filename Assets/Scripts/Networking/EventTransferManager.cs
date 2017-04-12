using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventTransferManager : Photon.MonoBehaviour {

	public static EventTransferManager instance = null;

	public GameObject boardPrefab;
	public GameObject settingsPrefab;
	public GameObject playerPrefab;
	public GameObject canvasPrefab;
	public GameObject ProgressCardsStackManagerPrefab;

	public int currentPlayerTurn = 0;
	public int currentActiveButton = -1;
	public bool setupPhase = true;
	public bool waitingForPlayer = false;
	public bool diceRolledThisTurn = false;
	public bool shipMovedThisTurn = false;
	public bool waitingforcards = true;
	public GameObject diceRollerPrefab;
	private GameObject diceRoller;

	//----------<Persistence>--------
	public int barbariansDistance = 7;
	public int defendersOfCatanLeft = 6;
	public bool barbariansAttackedIsland = false;
	public int vpNeededToWin = 13;
	//----------</Persistence>--------

	public bool waitingForPlayers = false;
	public bool[] playerChecks;

	void Awake() {
		if (instance == null)
			instance = this;

		playerChecks = new bool[PhotonNetwork.playerList.Length];
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnReadyToPlay() {
		Debug.Log (GameManager.instance.gameBoard);
		if (GameManager.instance.LoadGameMode == true) {
			GetComponent<PhotonView> ().RPC ("GenerateBoardForClientFromSavedGame", PhotonTargets.All, new object[] {
			});
			GetComponent<PhotonView> ().RPC ("GenerateHarborsForClient", PhotonTargets.All, new object[] { });
			GetComponent<PhotonView> ().RPC ("GenerateProgressCards", PhotonTargets.All, new object[] { });
		} else {
			GetComponent<PhotonView> ().RPC ("GenerateBoardForClient", PhotonTargets.All, new object[] { });
			GetComponent<PhotonView> ().RPC ("GenerateHarborsForClient", PhotonTargets.All, new object[] { });
			GetComponent<PhotonView> ().RPC ("GenerateProgressCards", PhotonTargets.All, new object[] { });
		}
		GetComponent<PhotonView> ().RPC ("CleanExtraInstances", PhotonTargets.All, new object[] { });

		if (photonView.isMine) {
			StartCoroutine (CatanSetupPhase ());
		}
	}

	public void OnEndTurn() {
		GetComponent<PhotonView> ().RPC ("EndTurn", PhotonTargets.All, new object[] { });
		currentPlayerTurn = (currentPlayerTurn + 1) % PhotonNetwork.playerList.Length;
		diceRolledThisTurn = false;
		shipMovedThisTurn = false;
	}

	public void OnEndGame() {
		GetComponent<PhotonView> ().RPC ("GameOver", PhotonTargets.All, new object[] { });
	}

	[PunRPC]
	void GameOver() {
		EventTransferManager.instance.waitingForPlayer = true;
		EventTransferManager.instance.waitingForPlayers = true;
		EventTransferManager.instance.currentActiveButton = -1;
	}

	public void OnOperationFailure() {
		GetComponent<PhotonView> ().RPC ("HandleOperationFailure", PhotonTargets.All, new object[] { });
	}

	public void OnTradeWithBank(int playerNum, bool gained, AssetTuple assetsTraded) {
		GetComponent<PhotonView> ().RPC ("ResourceChangeEvent", PhotonTargets.All, new object[] { 
			playerNum,
			gained,
			assetsTraded.resources.resourceTuple[ResourceType.Brick],
			assetsTraded.resources.resourceTuple[ResourceType.Grain],
			assetsTraded.resources.resourceTuple[ResourceType.Lumber],
			assetsTraded.resources.resourceTuple[ResourceType.Ore],
			assetsTraded.resources.resourceTuple[ResourceType.Wool],
			assetsTraded.commodities.commodityTuple[CommodityType.Paper],
			assetsTraded.commodities.commodityTuple[CommodityType.Coin],
			assetsTraded.commodities.commodityTuple[CommodityType.Cloth],
			assetsTraded.fishTokens.fishTuple[FishTokenType.One],
			assetsTraded.fishTokens.fishTuple[FishTokenType.Two],
			assetsTraded.fishTokens.fishTuple[FishTokenType.Three],
			assetsTraded.fishTokens.fishTuple[FishTokenType.OldBoot],
			assetsTraded.gold
		});
	}

	public void OnTradeOfferCounter(int senderNumber, int receiverNumber, AssetTuple offer, AssetTuple receive){
		StartCoroutine (TradeOffer (senderNumber, receiverNumber, offer, receive,true));
	}
	public void OnTradeOffer(int senderNumber, int receiverNumber, AssetTuple offer, AssetTuple receive) {
		Debug.Log (LevelManager.instance.players [senderNumber].playerName + " sends a trade offer to " + LevelManager.instance.players [receiverNumber].playerName);
		StartCoroutine (TradeOffer (senderNumber, receiverNumber, offer, receive,false));
	}

	public IEnumerator TradeOffer(int senderNumber, int receiverNumber, AssetTuple offer, AssetTuple receive,bool counter) {
		int[] offerArray = new int[offer.resources.resourceTuple.Values.Count + offer.commodities.commodityTuple.Values.Count];
		int[] receiveArray = new int[offer.resources.resourceTuple.Values.Count + offer.commodities.commodityTuple.Values.Count];

		for (int i = 0; i < offerArray.Length; i++) {
			offerArray [i] = offer.GetValueAtIndex (i);
		}
		for (int i = 0; i < offerArray.Length; i++) {
			receiveArray [i] = receive.GetValueAtIndex (i);
		}

		Debug.Log ("sending trade to: " + receiverNumber);
		GetComponent<PhotonView> ().RPC ("SignalTrade", PhotonTargets.All, new object[] {
			senderNumber,
			receiverNumber,
			offerArray,
			receiveArray,
			counter
		});

		int[] waitingForPlayersArray = new int[1];
		waitingForPlayersArray [0] = receiverNumber;
		OnWaitForPlayers (waitingForPlayersArray);

		for (int i = 0; i < playerChecks.Length; i++) {
			Debug.Log ("playerchecks[" + i + "] = " + playerChecks [i]);
		}

		while (EventTransferManager.instance.waitingForPlayers) {
			Debug.Log ("waiting..");
			CheckIfPlayersReady ();
			yield return new WaitForEndOfFrame ();
		}
	}
		
	[PunRPC]
	void SignalTrade(int senderNum, int receiverNum, int[] offer, int[] receive,bool counter) {
		Debug.Log ("Receiver number is " + receiverNum);
		if(PhotonNetwork.player.ID - 1 == receiverNum) {
			Debug.Log ("Receiver true number: " + receiverNum);
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			AssetTuple offerTuple = new AssetTuple();
			AssetTuple receiveTuple = new AssetTuple ();

			for (int i = 0; i < offer.Length; i++) {
				offerTuple.SetValueAtIndex (i, offer [i]);
			}
			for (int i = 0; i < receive.Length; i++) {
				receiveTuple.SetValueAtIndex (i, receive [i]);
			}
			if (counter) {
				Debug.Log ("Open Counter");
				clientCatanManager.uiManager.tradePlayerPanel.OpenRespondCounter(clientCatanManager.players[senderNum], offerTuple, receiveTuple);
				clientCatanManager.uiManager.tradePlayerPanel.waiting.text="Counter Offer Received Please Respond";
				clientCatanManager.uiManager.tradePlayerPanel.waiting.gameObject.SetActive (true);
			} else {
				clientCatanManager.uiManager.tradePlayerPanel.waiting.gameObject.SetActive (false);
				clientCatanManager.uiManager.tradePlayerPanel.OpenRespond(clientCatanManager.players[senderNum], offerTuple, receiveTuple);
			}
		}
	}

	public void OnTradeRespose(bool active) {
		GetComponent<PhotonView> ().RPC ("PlayerTradePanelActivation", PhotonTargets.All, new object[] {
			active
		});
	}

	public void OnTradeEnd(int to,bool result){
		Debug.Log ("sending notification to: " + to);
		GetComponent<PhotonView> ().RPC ("SignalEnd", PhotonTargets.All, new object[] {
			to,
			result
		});
	}
	[PunRPC]
	void SignalEnd(int to,bool result){
		if(PhotonNetwork.player.ID - 1 == to) {
			Debug.Log ("trade notified: " + to);
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			clientCatanManager.uiManager.tradePlayerPanel.notification.gameObject.SetActive (true);
			if (result) {
				clientCatanManager.uiManager.tradePlayerPanel.notificationText.text = "Trade Accepted";
			} else {
				clientCatanManager.uiManager.tradePlayerPanel.notificationText.text = "Trade Rejected";
			}
		}
		GetComponent<PhotonView> ().RPC ("PlayerTradePanelActivation", PhotonTargets.All, new object[] {
			false
		});
	}
	[PunRPC]
	void PlayerTradePanelActivation(bool active) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		TradePlayerPanel tradePanel = clientCatanManager.uiManager.tradePlayerPanel;

		for (int i = 0; i < tradePanel.getAssetSliders.Length; i++) {
			tradePanel.getAssetSliders [i].value = 0;
		}
		for (int i = 0; i < tradePanel.giveAssetSliders.Length; i++) {
			tradePanel.giveAssetSliders [i].value = 0;
		}

		tradePanel.confirm.onClick.RemoveAllListeners ();
		tradePanel.cancel.onClick.RemoveAllListeners ();
		tradePanel.counter.onClick.RemoveAllListeners ();

		clientCatanManager.uiManager.tradePlayerPanel.gameObject.SetActive (false);
	}
	public void GoldTrade(int player){
		GetComponent<PhotonView> ().RPC ("OnGoldTrade", PhotonTargets.All, new object[] {
			player
		});
	}
	[PunRPC]
	void OnGoldTrade(int reciever){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [reciever].goldCoins = clientCatanManager.players [reciever].goldCoins - 2;
	}
	public void OnMoveGamePiece(int boardPieceNum, int tileID, bool remove) {
		GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
			boardPieceNum,
			tileID,
			remove
		});
	}

	public void OnUpgradeCity(int playerNum, int upgradeType) {
		GetComponent<PhotonView> ().RPC ("UpgradeCity", PhotonTargets.All, new object[] {
			playerNum,
			upgradeType
		});
	}

	[PunRPC]
	void UpgradeCity(int playerNum, int upgradeType) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		clientCatanManager.players [playerNum].cityImprovements.ImproveCityOfType ((CityImprovementType)upgradeType);
	}

	public IEnumerator OnDiceRolled() {
		//int redDieRoll=1;
		//int yellowDieRoll=1;
		//EventDieFace eventDieRoll=EventDieFace.Green;
		if (!diceRolledThisTurn){
		  
			EventTransferManager.instance.waitingForPlayer = true;
			GetComponent<PhotonView> ().RPC ("RollDice", PhotonTargets.All, new object[] {Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),false});
			GameObject[] dice = GameObject.FindGameObjectsWithTag ("Dice");
			int redDieRoll=0;
			int yellowDieRoll=0;
			EventDieFace eventDieRoll=EventDieFace.Black;
			bool ready = true;		

			bool x = dice [0].GetComponent<FaceDetection> ().ready;
			bool y = dice [1].GetComponent<FaceDetection> ().ready;
			bool z = dice [2].GetComponent<FaceDetection> ().ready;
			//wait for dice to finish rolling
			while (ready) {
				dice = GameObject.FindGameObjectsWithTag ("Dice");
				if (x) {
					if (y) {
						if (z) {
							ready = false;
						}
					}
				}
				Debug.Log ("Dice Rolling");
				yield return new WaitForSeconds (3f);
				x = dice [0].GetComponent<FaceDetection> ().ready;
				y = dice [1].GetComponent<FaceDetection> ().ready;
				z = dice [2].GetComponent<FaceDetection> ().ready;
				if (!x || !y || !z) {
					//fault tolerance in event 1 of the dice got stuck
					Debug.Log("Re-roll required");
					GetComponent<PhotonView> ().RPC ("RollDice", PhotonTargets.All, new object[] {Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),false});
				}
				yield return new WaitForEndOfFrame ();
			}
			//get dice rolls
			GameObject temp = GameObject.FindGameObjectWithTag ("DiceRoller");
			redDieRoll=temp.GetComponent<DiceRoller> ().dice [0].GetComponent<DieHelper>().value;
			yellowDieRoll = temp.GetComponent<DiceRoller> ().dice [1].GetComponent<DieHelper>().value;;
			int temp2=temp.GetComponent<DiceRoller> ().dice [2].GetComponent<DieHelper>().value;;
			switch (temp2) {
			case 1:
				eventDieRoll = EventDieFace.Blue;
				break;
			case 2:
				eventDieRoll = EventDieFace.Black;
				break;
			case 3:
				eventDieRoll = EventDieFace.Green;
				break;
			case 4:
				eventDieRoll = EventDieFace.Black;
				break;
			case 5:
				eventDieRoll = EventDieFace.Black;
				break;
			case 6:
				eventDieRoll = EventDieFace.Yellow;
				break;
			}
			GetComponent<PhotonView> ().RPC ("DestroyDice", PhotonTargets.All, new object[] {});
			Debug.Log("Red: "+redDieRoll+" Yellow: "+yellowDieRoll+ " Event: "+eventDieRoll.ToString());

			if (eventDieRoll == EventDieFace.Black) {
				GetComponent<PhotonView> ().RPC ("BarbariansEvent", PhotonTargets.All, new object[] {});
			}

			if ((redDieRoll + yellowDieRoll) == 7) {
				redDieRoll = 1;
				yellowDieRoll = 5;
			}
			//draw progress cards
			yield return StartCoroutine(CardDrawEvent (eventDieRoll, redDieRoll));
			if (!setupPhase && redDieRoll + yellowDieRoll == 7) {
				//StartCoroutine(clientCatanManager.discardResourcesForPlayers());
				//StartCoroutine(clientCatanManager.moveRobberForCurrentPlayer());
				GetComponent<PhotonView> ().RPC ("EnforceDiceRollEvents", PhotonTargets.All, new object[] {});
			} else {
				if (!setupPhase) {
					//CommodityCollectionEvent (redDieRoll + yellowDieRoll);
					GetComponent<PhotonView> ().RPC ("CommodityCollectionEvent", PhotonTargets.All, new object[] {
						redDieRoll + yellowDieRoll
					});
				}

				GetComponent<PhotonView> ().RPC ("ResourceCollectionEvent", PhotonTargets.All, new object[] {
					redDieRoll + yellowDieRoll
				});

			}
			EventTransferManager.instance.waitingForPlayer = false;
			diceRolledThisTurn = true;
		}
	}
	[PunRPC]
	void DestroyDice(){
		Destroy(diceRoller);
	}
	[PunRPC]
	void EnforceDiceRollEvents() {
		StartCoroutine(DiceRollSevenEvents());
	}

	[PunRPC]
	void BarbariansEvent() {
		StartCoroutine (BarbarianEventCoroutine ());
	}

	IEnumerator BarbarianEventCoroutine() {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		EventTransferManager.instance.barbariansDistance--;
		Debug.Log ("barbarians are now " + EventTransferManager.instance.barbariansDistance + " steps away from Catan!");

		if (EventTransferManager.instance.barbariansDistance <= 0) {
			int barbariansStrength = 0;
			int knightsStrength = 0;

			List<int> lowestKnightContributingPlayerNums = new List<int>();
			int lowestKnightStrSum = int.MaxValue;

			List<int> highestKnightContributingPlayerNums = new List<int>();
			int highestKnightStrSum = -1;

			List<int> cityIDsDestroyed = new List<int> ();

			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				List<City> playerCities = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.City).Cast<City> ().ToList ();
				List<Metropolis> playerMetropolises = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Metropolis).Cast<Metropolis> ().ToList ();

				barbariansStrength += (playerCities.Count + playerMetropolises.Count);

				List<Knight> playerKnights = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().ToList ();
				int playerKnightsStr = 0;

				for(int j = 0; j < playerKnights.Count; j++) {
					if (playerKnights [j].isActive) {
						knightsStrength += (((int)playerKnights [j].rank) + 1);
						playerKnightsStr += (((int)playerKnights [j].rank) + 1);
					}
				}

				if (playerCities.Count == 0) {
					continue;
				}

				if (playerKnightsStr < lowestKnightStrSum) {
					lowestKnightContributingPlayerNums.Clear ();
					lowestKnightContributingPlayerNums.Add (i);
					lowestKnightStrSum = playerKnightsStr;
				} else if (playerKnightsStr == lowestKnightStrSum) {
					lowestKnightContributingPlayerNums.Add (i);
				}

				if (playerKnightsStr > highestKnightStrSum) {
					highestKnightContributingPlayerNums.Clear ();
					highestKnightContributingPlayerNums.Add (i);
					highestKnightStrSum = playerKnightsStr;
				} else if (playerKnightsStr == highestKnightStrSum) {
					highestKnightContributingPlayerNums.Add (i);
				}
			}
			Debug.Log ("BARBARIANS STRENGTH IS: " + barbariansStrength);
			Debug.Log ("KNIGHTS STRENGTH IS: " + knightsStrength);

			if (barbariansStrength > knightsStrength) {
				Debug.Log ("Lowest contribution count is: " + lowestKnightContributingPlayerNums.Count);
				for (int i = 0; i < lowestKnightContributingPlayerNums.Count; i++) {
					Debug.Log (clientCatanManager.players [lowestKnightContributingPlayerNums [i]].playerName + " get his city destroyed!");
					List<City> playerCities = clientCatanManager.players [lowestKnightContributingPlayerNums [i]].getOwnedUnitsOfType (UnitType.City).Cast<City> ().ToList ();

					City cityToDestroy = playerCities [0];

					if (cityToDestroy.cityWalls != null) {
						clientCatanManager.players [lowestKnightContributingPlayerNums [i]].removeOwnedUnit ((Unit)cityToDestroy.cityWalls, typeof(CityWall));
						Destroy (cityToDestroy.cityWalls.gameObject);
						cityToDestroy.cityWalls = null;
					} else {
						// Downgrade to settlement
						//if (lowestKnightContributingPlayerNums [i] == PhotonNetwork.player.ID - 1) {
						//OnDowngradeCity (lowestKnightContributingPlayerNums [i], cityToDestroy.id);
						cityIDsDestroyed.Add(cityToDestroy.id);
						//}
					}
				}
			} else {
				Debug.Log ("Highest contribution count is: " + lowestKnightContributingPlayerNums.Count);
				if (highestKnightContributingPlayerNums.Count == 1) {
					EventTransferManager.instance.defendersOfCatanLeft--;
					clientCatanManager.players [highestKnightContributingPlayerNums [0]].defenderOfCatans++;
					Debug.Log (clientCatanManager.players [highestKnightContributingPlayerNums [0]].playerName + " is crowned the defender of Catan!");
				} else {
					int[] playersToWait = new int[highestKnightContributingPlayerNums.Count];
					for (int i = 0; i < highestKnightContributingPlayerNums.Count; i++) {
						playersToWait [i] = highestKnightContributingPlayerNums [i];
					}

					EventTransferManager.instance.BeginWaitForPlayers (playersToWait);

					if (playersToWait.Contains(PhotonNetwork.player.ID - 1)) {
						clientCatanManager.uiManager.cardSelectPanel.gameObject.SetActive (true);

						while(!clientCatanManager.uiManager.cardSelectPanel.selectionMade){
							Debug.Log ("Waiting for card choice");
							yield return new WaitForEndOfFrame ();
						}

						clientCatanManager.uiManager.cardSelectPanel.selectionMade=false;
					}

					Debug.Log ("Waiting for players started");
					while (EventTransferManager.instance.waitingForPlayers) {
						EventTransferManager.instance.CheckIfPlayersReady ();
						Debug.Log ("Waiting...");
						yield return new WaitForEndOfFrame ();
					}
				}
			}

			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				List<Knight> playerKnights = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().ToList ();
				for (int j = 0; j < playerKnights.Count; j++) {
					playerKnights [j].activateKnight (false);
				}
			}

			if (PhotonNetwork.isMasterClient) {
				for (int i = 0; i < cityIDsDestroyed.Count; i++) {
					OnDowngradeCity (cityIDsDestroyed [i]);
				}
			}
			EventTransferManager.instance.barbariansAttackedIsland = true;
			EventTransferManager.instance.barbariansDistance = 7;
		}

		yield return null;
	}

	public void OnDrawWithChoice(List<Player> players){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
		EventTransferManager.instance.WaitOnDraw = false;
		for (int i = 0; i < players.Count; i++) {
			EventTransferManager.instance.WaitOnDraw = true;

			GetComponent<PhotonView> ().RPC ("drawWithChoiceRPC", PhotonTargets.All, new object[] {
				players[i].playerNumber-1
			});

		}
	}

	[PunRPC]
	void drawWithChoiceRPC(int receiverNum) {
		StartCoroutine (drawWithChoiceCoroutine (receiverNum));
	}

	IEnumerator drawWithChoiceCoroutine(int receiverNum){
		if(PhotonNetwork.player.ID-1== receiverNum) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

			clientCatanManager.uiManager.cardSelectPanel.gameObject.SetActive (true);
			while(!clientCatanManager.uiManager.cardSelectPanel.selectionMade){
				Debug.Log ("Waiting for card choice");
				yield return new WaitForEndOfFrame ();

			}
			clientCatanManager.uiManager.cardSelectPanel.selectionMade=false;
			GetComponent<PhotonView> ().RPC ("OnDrawCompleted", PhotonTargets.All, new object[] {
			});
		}
	}

	IEnumerator DiceRollSevenEvents() {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		int[] allPlayers = { 0, 1, 2, 3 };

		EventTransferManager.instance.BeginWaitForPlayers (allPlayers);

		int numDiscards = clientCatanManager.players [PhotonNetwork.player.ID - 1].getNumDiscardsNeeded ();
		yield return StartCoroutine(clientCatanManager.selectResourcesForPlayers(numDiscards, false));

		Debug.Log ("Waiting for players started");
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}

		if (PhotonNetwork.player.ID - 1 == clientCatanManager.currentPlayerTurn && EventTransferManager.instance.barbariansAttackedIsland) {
			yield return StartCoroutine(clientCatanManager.moveGamePieceForCurrentPlayer(0, false, true));
		}
	}

	void OnMoveRobberOrPirate(int gamePieceNumber, bool removeFromBoard, bool stealResource) {
		GetComponent<PhotonView> ().RPC ("MoveRobberOrPirate", PhotonTargets.All, new object[] {
			gamePieceNumber,
			removeFromBoard,
			stealResource
		});
	}

	[PunRPC]
	void MoveRobberOrPirate(int gamePieceNumber, bool removeFromBoard, bool stealResource) {
		StartCoroutine (MoveRobberPirateCoroutine (gamePieceNumber, removeFromBoard, stealResource));
	}

	IEnumerator MoveRobberPirateCoroutine(int gamePieceNumber, bool removeFromBoard, bool stealResource) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if (PhotonNetwork.player.ID - 1 == clientCatanManager.currentPlayerTurn) {
			yield return StartCoroutine(clientCatanManager.moveGamePieceForCurrentPlayer(gamePieceNumber, removeFromBoard, stealResource));
		}
	}

	void BeginWaitForPlayers(int[] playerNums) {
		for (int i = 0; i < playerChecks.Length; i++) {
			EventTransferManager.instance.playerChecks [i] = true;
		}
		for (int i = 0; i < playerNums.Length; i++) {
			if (playerNums [i] < playerChecks.Length) {
				EventTransferManager.instance.playerChecks [playerNums [i]] = false;
			}
		}
		EventTransferManager.instance.waitingForPlayers = true;
	}

	void WaitForAllPlayers() {
		for (int i = 0; i < playerChecks.Length; i++) {
			EventTransferManager.instance.playerChecks [i] = false;
		}
		EventTransferManager.instance.waitingForPlayers = true;
	}

	void WaitUntilPlayersDone() {
		StartCoroutine (WaitUntilPlayersDoneCoroutine ());
	}

	IEnumerator WaitUntilPlayersDoneCoroutine() {
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
			CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}
	}

	void OnWaitForPlayers(int[] playerNums) {
		GetComponent<PhotonView> ().RPC ("BeginWaitForPlayers", PhotonTargets.All, new object[] {
			playerNums
		});
	}

	void OnWaitForAllPlayers() {
		int[] allPlayers = new int[PhotonNetwork.playerList.Length];

		for (int i = 0; i < allPlayers.Length; i++) {
			allPlayers [i] = i;
		}

		GetComponent<PhotonView> ().RPC ("BeginWaitForPlayers", PhotonTargets.All, new object[] {
			allPlayers
		});
	}

	void CheckIfPlayersReady() {
		bool ready = true;
		for (int i = 0; i < playerChecks.Length; i++) {
			if (!playerChecks [i]) {
				ready = false;
			}
		}
		EventTransferManager.instance.waitingForPlayers = !ready;
		EventTransferManager.instance.waitingForPlayer = !ready;
	}

	[PunRPC]
	IEnumerator RollDice(float number, float number1, float number2,bool alchemist){
		Destroy(diceRoller);
		diceRoller = Instantiate(diceRollerPrefab , new Vector3(-1.0f,0,-5.7f),Quaternion.identity);

		if (alchemist) {
			GameObject temp = GameObject.FindGameObjectWithTag ("DiceRoller");
			Destroy (temp.GetComponent<DiceRoller> ().dice [0]);
			Destroy (temp.GetComponent<DiceRoller> ().dice [1]);

			DiePhysics physics = temp.GetComponent<DiceRoller> ().dice [2].GetComponent<DiePhysics> ();
			Debug.Log ("vector is " + number + " " + number1 + " " + number2);
			physics.init (new Vector3 (number, number1, number2));

		} else {
			GameObject[] dice = GameObject.FindGameObjectsWithTag ("Dice");
			foreach (GameObject go in dice) {
				DiePhysics physics = go.GetComponent<DiePhysics> ();
				Debug.Log ("vector is " + number + " " + number1 + " " + number2);
				physics.init (new Vector3 (number, number1, number2));
			}
		}
		
		yield return new WaitForSeconds (3.0f);
	}
		
	public void OnPlayerReady(int playerNum, bool ready) {
		GetComponent<PhotonView> ().RPC ("SignalReady", PhotonTargets.All, new object[] {
			playerNum,
			ready
		});
	}

	[PunRPC]
	void SignalReady(int playerNumber, bool ready) {
		EventTransferManager.instance.playerChecks [playerNumber] = ready;
	}

	public void OnHighlightForUser(int highlightTypeNumber, int playerNum, bool highlight, int[] ids) {
		// 0 = Intersection,
		// 1 = Edge,
		// 2 = Unit
		switch (highlightTypeNumber) {
		case 0:
			GetComponent<PhotonView> ().RPC ("HighlightIntersections", PhotonTargets.All, new object[] {
				playerNum,
				highlight,
				ids
			});
			break;
		case 1:
			GetComponent<PhotonView> ().RPC ("HighlightEdges", PhotonTargets.All, new object[] {
				playerNum,
				highlight,
				ids
			});
			break;
		case 2:
			GetComponent<PhotonView> ().RPC ("HighlightUnits", PhotonTargets.All, new object[] {
				playerNum,
				highlight,
				ids
			});
			break;
		default:
			break;
		}

	}

	public void OnBuildUnitForUser(UnitType unitType, int playerNumber, int id, bool paid, int metropolisType) {
		if (unitType == UnitType.Road || unitType == UnitType.Ship) {
			GetComponent<PhotonView> ().RPC ("BuildEdgeUnit", PhotonTargets.All, new object[] {
				playerNumber,
				(int)unitType,
				id,
				paid
			});
		} else if (unitType == UnitType.City && !EventTransferManager.instance.setupPhase && paid) {
			GetComponent<PhotonView> ().RPC ("UpgradeSettlement", PhotonTargets.All, new object[] {
				playerNumber,
				id
			});
		} else if (unitType == UnitType.Metropolis) {
			GetComponent<PhotonView> ().RPC ("UpgradeCity", PhotonTargets.All, new object[] {
				playerNumber,
				id,
				metropolisType
			});
		} else if (unitType == UnitType.CityWalls) {
			GetComponent<PhotonView> ().RPC ("BuildCityWalls", PhotonTargets.All, new object[] {
				playerNumber,
				id,
				paid
			});
		} else {
			GetComponent<PhotonView> ().RPC ("BuildIntersectionUnit", PhotonTargets.All, new object[] {
				playerNumber,
				(int)unitType,
				id,
				paid
			});
		}
	}

	public void OnMoveShipForUser(int playerNumber, int shipID, int newLocationEdgeID) {
		GetComponent<PhotonView> ().RPC ("MoveShip", PhotonTargets.All, new object[] {
			playerNumber,
			shipID,
			newLocationEdgeID
		});
	}

	public void OnKnightActionForUser(MoveType knightActionType, int playerNumber, int knightID, int otherID, bool active, bool paid) {
		if (knightActionType == MoveType.ActivateKnight) {
			GetComponent<PhotonView> ().RPC ("ActivateKnight", PhotonTargets.All, new object[] {
				playerNumber,
				knightID,
				active,
				paid
			});
		} else if (knightActionType == MoveType.PromoteKnight) {
			GetComponent<PhotonView> ().RPC ("PromoteKnight", PhotonTargets.All, new object[] {
				playerNumber,
				knightID,
				paid
			});
		} else if (knightActionType == MoveType.MoveKnight) {
			GetComponent<PhotonView> ().RPC ("MoveKnight", PhotonTargets.All, new object[] {
				playerNumber,
				knightID,
				otherID
			});
		} else if (knightActionType == MoveType.DisplaceKnight) {
			GetComponent<PhotonView> ().RPC ("DisplaceKnight", PhotonTargets.All, new object[] {
				playerNumber,
				knightID,
				otherID
			});
		} else if (knightActionType == MoveType.ChaseRobber) {
			GetComponent<PhotonView> ().RPC ("ChaseRobber", PhotonTargets.All, new object[] {
				playerNumber,
				knightID,
				otherID
			});
		} else if (knightActionType == MoveType.RemoveKnight) {
			GetComponent<PhotonView> ().RPC ("DestroyUnitRPC", PhotonTargets.All, new object[] {
				(int)UnitType.Knight,
				knightID
			});
		}

	}

	public void OnDowngradeCity(int unitID) {
		GetComponent<PhotonView> ().RPC ("DowngradeCity", PhotonTargets.All, new object[] {
			unitID
		});
	}

	public void OnDestroyUnit(UnitType type, int unitID) {
		GetComponent<PhotonView> ().RPC ("DestroyUnitRPC", PhotonTargets.All, new object[] {
			(int)type,
			unitID
		});
	}

	[PunRPC]
	public void DestroyUnitRPC(int unitType, int unitID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		Unit destroyedUnit = clientCatanManager.unitManager.unitsInPlay [unitID];

		switch ((UnitType)unitType) {
		case UnitType.Settlement:
			Settlement selectedSettlement = destroyedUnit as Settlement;
			selectedSettlement.owner.removeOwnedUnit (selectedSettlement, typeof(Settlement));
			selectedSettlement.locationIntersection.occupier = null;
			break;
		case UnitType.City:
			City selectedCity = destroyedUnit as City;
			selectedCity.owner.removeOwnedUnit (selectedCity, typeof(City));
			selectedCity.locationIntersection.occupier = null;
			break;
		case UnitType.CityWalls:
			CityWall selectedCityWall = destroyedUnit as CityWall;
			selectedCityWall.owner.removeOwnedUnit (selectedCityWall, typeof(CityWall));
			selectedCityWall.locationIntersection.occupier = null;
			break;
		case UnitType.Knight:
			Knight selectedKnight = destroyedUnit as Knight;
			selectedKnight.owner.removeOwnedUnit (selectedKnight, typeof(Knight));
			selectedKnight.locationIntersection.occupier = null;
			break;
		case UnitType.Road:
			Road selectedRoad = destroyedUnit as Road;
			selectedRoad.owner.removeOwnedUnit (selectedRoad, typeof(Road));
			selectedRoad.locationEdge.occupier = null;
			break;
		case UnitType.Ship:
			Ship selectedShip = destroyedUnit as Ship;
			selectedShip.owner.removeOwnedUnit (selectedShip, typeof(Ship));
			selectedShip.locationEdge.occupier = null;
			break;
		default:
			break;
		}

		Destroy (destroyedUnit.gameObject);
	}


	IEnumerator CatanSetupPhase() {
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			GetComponent<PhotonView> ().RPC ("SetPlayerTurn", PhotonTargets.Others, new object[] { i });
			yield return StartCoroutine(ClientBuildSettlement (i));
			yield return StartCoroutine(ClientBuildRoad (i));
			//GetComponent<PhotonView> ().RPC ("EndTurn", PhotonTargets.Others, new object[] { });
		}

		for (int i = PhotonNetwork.playerList.Length - 1; i >= 0; i--) {
			GetComponent<PhotonView> ().RPC ("SetPlayerTurn", PhotonTargets.Others, new object[] { i });
			yield return StartCoroutine(ClientBuildCity (i));
			// RESOURCE COLLECTION EVENT!!!

			GetComponent<PhotonView> ().RPC ("CollectResources", PhotonTargets.All, new object[] { i });
			//GetComponent<PhotonView> ().RPC ("ResourceCollectionEvent", PhotonTargets.All, new object[] {
			//	0
			//});
			yield return StartCoroutine(ClientBuildRoad (i));
			//GetComponent<PhotonView> ().RPC ("EndTurn", PhotonTargets.Others, new object[] { });
		}
		setupPhase = false;
		waitingForPlayer = false;

		currentPlayerTurn = 0;
	}

	[PunRPC]
	void PlayMove(int playerNumber, int moveType) {
		if (playerNumber == PhotonNetwork.player.ID - 1) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			switch ((MoveType)moveType) {
			case MoveType.BuildSettlement:
				StartCoroutine(clientCatanManager.unitManager.buildSettlement ());
				break;
			case MoveType.BuildRoad:
				StartCoroutine(clientCatanManager.unitManager.buildRoad (true));
				break;
			case MoveType.BuildCity:
				StartCoroutine(clientCatanManager.unitManager.buildCity ());
				break;
			case MoveType.BuildShip:
				StartCoroutine(clientCatanManager.unitManager.buildShip ());
				break;
			case MoveType.UpgradeSettlement:
				StartCoroutine(clientCatanManager.unitManager.upgradeSettlement ());
				break;
			case MoveType.TradeBank:
				clientCatanManager.tradeWithBankAttempt (4);
				break;
			case MoveType.TradePlayer:
				clientCatanManager.tradeWithPlayerAttempt (1);
				break;
			case MoveType.MoveShip:
				StartCoroutine(clientCatanManager.unitManager.moveShip ());
				break;
			case MoveType.BuildKnight:
				StartCoroutine(clientCatanManager.unitManager.buildKnight ());
				break;
			case MoveType.ActivateKnight:
				StartCoroutine(clientCatanManager.unitManager.activateKnight (true));
				break;
			case MoveType.PromoteKnight:
				StartCoroutine(clientCatanManager.unitManager.promoteKnight (true));
				break;
			case MoveType.MoveKnight:
				StartCoroutine(clientCatanManager.unitManager.moveKnight ());
				break;
			case MoveType.DisplaceKnight:
				StartCoroutine(clientCatanManager.unitManager.displaceKnight ());
				break;
			case MoveType.ChaseRobber:
				StartCoroutine(clientCatanManager.unitManager.chaseRobber ());
				break;
			case MoveType.BuildCityWall:
				StartCoroutine(clientCatanManager.unitManager.buildCityWall ());
				break;
			default:
				break;
			}
		}
	}

	public IEnumerator ClientBuildSettlementForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.BuildSettlement
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildSettlement(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.Others, new object[] {
			playerNumber,
			(int)MoveType.BuildSettlement
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildCity(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.Others, new object[] {
			playerNumber,
			(int)MoveType.BuildCity
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildRoad(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.Others, new object[] {
			playerNumber,
			(int)MoveType.BuildRoad
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildRoadForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.BuildRoad
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildShip(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.Others, new object[] {
			playerNumber,
			(int)MoveType.BuildShip
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientUpgradeSettlement(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.UpgradeSettlement
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildShipForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.BuildShip
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public void ClientTradeBank(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.TradeBank
		});
	}

	public void ClientTradePlayer(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.TradePlayer
		});
	}

	public IEnumerator ClientMoveShip(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.MoveShip
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientUpgradeCity(int playerNumber, int metropolisType) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("BuildMetropolis", PhotonTargets.All, new object[] {
			playerNumber,
			metropolisType
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildKnightForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.BuildKnight
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientActivateKnightForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.ActivateKnight
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientPromoteKnightForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.PromoteKnight
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientMoveKnightForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.MoveKnight
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientDisplaceKnightForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.DisplaceKnight
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientChaseRobberForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.ChaseRobber
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public IEnumerator ClientBuildWallForAll(int playerNumber) {
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("PlayMove", PhotonTargets.All, new object[] {
			playerNumber,
			(int)MoveType.BuildCityWall
		});
		while (EventTransferManager.instance.waitingForPlayer) {
			yield return new WaitForEndOfFrame ();
		}
	}

	[PunRPC]
	void BuildMetropolis(int playerNumber, int metropolisType) {
		if (playerNumber == PhotonNetwork.player.ID - 1) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			StartCoroutine(clientCatanManager.unitManager.upgradeCity (metropolisType));
		}
	}

	[PunRPC]
	void SetPlayerTurn(int currentTurnPlayer) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.SetCurrentTurn (currentTurnPlayer);

		EventTransferManager.instance.currentPlayerTurn = currentTurnPlayer;
	}

	[PunRPC]
	void EndTurn() {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.currentPlayerTurn = (clientCatanManager.currentPlayerTurn + 1) % PhotonNetwork.playerList.Length;
		//clientCatanManager.unitManager.destroyCancelledUnits ();
		Destroy(diceRoller);

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			clientCatanManager.players [i].cityImprovements.playedCraneThisTurn = false;
			clientCatanManager.players [i].playedRoadBuilding = false;

			List<Knight> playerKnights = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().ToList ();
			for(int j = 0; j < playerKnights.Count; j++) {
				playerKnights [j].actionPerformedThisTurn = false;
			}

			List<Ship> playerShips = clientCatanManager.players [i].getOwnedUnitsOfType (UnitType.Ship).Cast<Ship> ().ToList ();
			for(int j = 0; j < playerShips.Count; j++) {
				playerShips [j].builtThisTurn = false;
			}
		}

		if (clientCatanManager.players [clientCatanManager.currentPlayerTurn].hasOldBoot ()) {
			Player maxVPPlayer = clientCatanManager.players [clientCatanManager.currentPlayerTurn];

			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				if (clientCatanManager.players [i].getVpPoints() > maxVPPlayer.getVpPoints()) {
					maxVPPlayer = clientCatanManager.players [i];
				}
			}

			if (maxVPPlayer != clientCatanManager.players [clientCatanManager.currentPlayerTurn]) {
				maxVPPlayer.assets.fishTokens.OldBootReceive (true);
				clientCatanManager.players [clientCatanManager.currentPlayerTurn].assets.fishTokens.OldBootReceive (false);

				if (PhotonNetwork.player.ID == maxVPPlayer.playerNumber) {
					//clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
					clientCatanManager.uiManager.notificationpanel.gameObject.SetActive(true);
					clientCatanManager.uiManager.notificationtext.text = "Received Old Boot from " + 
						clientCatanManager.players [clientCatanManager.currentPlayerTurn].playerName;
				}
				else if (PhotonNetwork.player.ID == clientCatanManager.players [clientCatanManager.currentPlayerTurn].playerNumber) {
					//clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
					clientCatanManager.uiManager.notificationpanel.gameObject.SetActive(true);
					clientCatanManager.uiManager.notificationtext.text = "Gave Old Boot to " + maxVPPlayer.playerName;
				}
			}
		}
	}

	[PunRPC]
	void CleanExtraInstances() {
		GameObject[] boards = GameObject.FindGameObjectsWithTag ("Board");

		for (int i = 0; i < boards.Length; i++) {
			GameBoard board = boards [i].GetComponent<GameBoard> ();

			Intersection[] intersections = board.GetComponentsInChildren<Intersection> ();

			if (intersections == null) {
				Debug.Log ("Destroyed " + i + " named " + board.name);
				Destroy (board.gameObject);
			}
		}
	}

	[PunRPC]
	void ResourceChangeEvent(int playerNum, bool gained, int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth, int oneFish, int twoFish, int threeFish, int oldBoot, int gold) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		AssetTuple delta = new AssetTuple (brick, grain, lumber, ore, wool, paper, coin, cloth, oneFish, twoFish, threeFish, oldBoot, gold);

		if (gained) {
			clientCatanManager.players [playerNum].receiveAssets (delta);
			clientCatanManager.resourceManager.giveFishTokens (delta.fishTokens);
		} else {
			clientCatanManager.players [playerNum].spendAssets (delta);
			clientCatanManager.resourceManager.receiveFishTokens (delta.fishTokens);
		}
	}

	// MUST BE TURNED INTO A COROUTINE, RPC CALLS IT...
	[PunRPC]
	void ResourceCollectionEvent(int diceOutcome) {
		StartCoroutine(ResourceCollectionCoroutine(diceOutcome));
	}

	IEnumerator ResourceCollectionCoroutine(int diceOutcome) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		int collectionCount = 0;

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {

			if (EventTransferManager.instance.setupPhase && i != clientCatanManager.currentPlayerTurn && PhotonNetwork.player.ID - 1 != i) {
				continue;
			} else {
				int[] tileIDs = clientCatanManager.boardManager.getTileIDsWithDiceValue (i, diceOutcome, false);
				Debug.Log ("Called resource collection for player:" + clientCatanManager.players[i].playerName + " by " + clientCatanManager.players[PhotonNetwork.player.ID - 1].playerName);

				for (int j = 0; j < tileIDs.Length; j++) {
					if (clientBoard.GameTiles [tileIDs [j]].canProduce ()) {
						print (clientCatanManager.players [i].playerName + " gets " + "1 " + GameAsset.getResourceOfHex (clientBoard.GameTiles [tileIDs [j]].tileType));

						ResourceTuple resources = clientCatanManager.resourceManager.getResourceForTile (clientBoard.GameTiles [tileIDs [j]], 1);

						int gold = 0;
						if (clientBoard.GameTiles [tileIDs [j]].tileType == TileType.Gold) {
							gold = 1;
						}

						clientCatanManager.players [i].receiveAssets (new AssetTuple(resources, new CommodityTuple(), gold));
						collectionCount++;
					}

					if (PhotonNetwork.player.ID - 1 == i) {
						if (clientBoard.GameTiles [tileIDs [j]].tileType == TileType.Ocean && clientBoard.GameTiles [tileIDs [j]].fishTile != null) {
							if (clientBoard.GameTiles [tileIDs [j]].fishTile.diceValue == diceOutcome) {
								FishTuple fishTokens = clientCatanManager.resourceManager.getFishTokenForTile(clientBoard.GameTiles [tileIDs [j]], 1);
								fishTokens.printFishTuple ();
								if (clientCatanManager.players [i].assets.fishTokens.numTokens () < 7) {
									OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
								} else {
									int currentLowestIndex = clientCatanManager.players [i].assets.fishTokens.nextAvailableSmallestIndex ();
									int receivingLowestIndex = fishTokens.nextAvailableSmallestIndex ();
									if (currentLowestIndex < receivingLowestIndex) {
										FishTuple toRemove = new FishTuple ();
										toRemove.addFishTokenWithType ((FishTokenType)currentLowestIndex, 1);

										OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
										OnTradeWithBank (i, false, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), toRemove));
									}
								}
							}
						}
					}
				}

				if (PhotonNetwork.player.ID - 1 == i) {
					if (diceOutcome == 11 || diceOutcome == 12 || diceOutcome == 2 || diceOutcome == 3) {
						GameTile lakeTile = clientBoard.GameTiles [clientCatanManager.boardManager.getLakeTileID ()];

						foreach (var intersection in lakeTile.getIntersections()) {
							if (intersection.occupier != null && intersection.occupier.owner == clientCatanManager.players [i]) {
								FishTuple fishTokens = clientCatanManager.resourceManager.getFishTokenForTile (lakeTile, 1);
								fishTokens.printFishTuple ();
								if (clientCatanManager.players [i].assets.fishTokens.numTokens () < 7) {
									OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
								} else {
									int currentLowestIndex = clientCatanManager.players [i].assets.fishTokens.nextAvailableSmallestIndex ();
									int receivingLowestIndex = fishTokens.nextAvailableSmallestIndex ();
									if (currentLowestIndex < receivingLowestIndex) {
										FishTuple toRemove = new FishTuple ();
										toRemove.addFishTokenWithType ((FishTokenType)currentLowestIndex, 1);

										OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
										OnTradeWithBank (i, false, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), toRemove));
									}
								}
							}
						}
					}
				}
			}
		}

		int[] allPlayers = { 0, 1, 2, 3 };
		EventTransferManager.instance.BeginWaitForPlayers (allPlayers);

		if(clientCatanManager.players[PhotonNetwork.player.ID - 1].unlockedAqueduct() && collectionCount == 0 && diceOutcome != 7) {
			yield return StartCoroutine(clientCatanManager.receiveNResourceSelection(PhotonNetwork.player.ID - 1, 1));
		}
		else{
			EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
			Debug.Log (clientCatanManager.players [PhotonNetwork.player.ID - 1].playerName + " signals ready");
		}

		Debug.Log ("Waiting for players started");
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}
	}

	[PunRPC]
	void CollectResources(int playerNum) {
		if (PhotonNetwork.player.ID == playerNum + 1) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

			if(!clientCatanManager.players[playerNum].collectedThisTurn) {
				Intersection selectedIntersection = clientBoard.Intersections [clientCatanManager.players [playerNum].lastIntersectionSelection.id];
				for (int i = 0; i < selectedIntersection.adjacentTiles.Count; i++) {
					GameTile tile = clientBoard.GameTiles [selectedIntersection.adjacentTiles [i]];

					if (tile.canProduce ()) {
						ResourceTuple resources = clientCatanManager.resourceManager.getResourceForTile (tile, 1);
						FishTuple fishTokens = clientCatanManager.resourceManager.getFishTokenForTile (tile, 1);
						OnTradeWithBank (playerNum, true, new AssetTuple (resources, new CommodityTuple (0, 0, 0), fishTokens));
						clientCatanManager.players [playerNum].collectedThisTurn = true;
					}
				}
			}
		}
	}

	[PunRPC]
	void CommodityCollectionEvent(int diceOutcome) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		int collectionCount = 0;

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if (EventTransferManager.instance.setupPhase && i != clientCatanManager.currentPlayerTurn) {
				continue;
			} else {
				int[] tileIDs = clientCatanManager.boardManager.getTileIDsWithDiceValue (i, diceOutcome, true);

				for (int j = 0; j < tileIDs.Length; j++) {
					if (clientBoard.GameTiles [tileIDs [j]].canProduce ()) {
						print (clientCatanManager.players [i].playerName + " gets " + "1 " + GameAsset.getCommodityOfHex (clientBoard.GameTiles [tileIDs [j]].tileType));

						CommodityTuple commodities = clientCatanManager.resourceManager.getCommodityForTile (clientBoard.GameTiles [tileIDs [j]], 1);

						int gold = 0;
						if (clientBoard.GameTiles [tileIDs [j]].tileType == TileType.Gold) {
							gold = 1;
						}

						clientCatanManager.players [i].receiveAssets (new AssetTuple(new ResourceTuple(), commodities, gold));
					}

					if (PhotonNetwork.player.ID - 1 == i) {
						if (clientBoard.GameTiles [tileIDs [j]].tileType == TileType.Ocean && clientBoard.GameTiles [tileIDs [j]].fishTile != null) {
							if (clientBoard.GameTiles [tileIDs [j]].fishTile.diceValue == diceOutcome) {
								FishTuple fishTokens = clientCatanManager.resourceManager.getFishTokenForTile(clientBoard.GameTiles [tileIDs [j]], 1);
								fishTokens.printFishTuple ();
								if (clientCatanManager.players [i].assets.fishTokens.numTokens () < 7) {
									OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
								} else {
									int currentLowestIndex = clientCatanManager.players [i].assets.fishTokens.nextAvailableSmallestIndex ();
									int receivingLowestIndex = fishTokens.nextAvailableSmallestIndex ();
									if (currentLowestIndex < receivingLowestIndex) {
										FishTuple toRemove = new FishTuple ();
										toRemove.addFishTokenWithType ((FishTokenType)currentLowestIndex, 1);

										OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
										OnTradeWithBank (i, false, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), toRemove));
									}
								}
							}
						}
					}
				}

				if (PhotonNetwork.player.ID - 1 == i) {
					if (diceOutcome == 11 || diceOutcome == 12 || diceOutcome == 2 || diceOutcome == 3) {
						GameTile lakeTile = clientBoard.GameTiles [clientCatanManager.boardManager.getLakeTileID ()];

						foreach (var intersection in lakeTile.getIntersections()) {
							if (intersection.occupier != null && intersection.occupier.owner == clientCatanManager.players [i]) {
								FishTuple fishTokens = clientCatanManager.resourceManager.getFishTokenForTile (lakeTile, 1);
								fishTokens.printFishTuple ();
								if (clientCatanManager.players [i].assets.fishTokens.numTokens () < 7) {
									OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
								} else {
									int currentLowestIndex = clientCatanManager.players [i].assets.fishTokens.nextAvailableSmallestIndex ();
									int receivingLowestIndex = fishTokens.nextAvailableSmallestIndex ();
									if (currentLowestIndex < receivingLowestIndex) {
										FishTuple toRemove = new FishTuple ();
										toRemove.addFishTokenWithType ((FishTokenType)currentLowestIndex, 1);

										OnTradeWithBank (i, true, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), fishTokens));
										OnTradeWithBank (i, false, new AssetTuple (new ResourceTuple (0, 0, 0, 0, 0), new CommodityTuple (0, 0, 0), toRemove));
									}
								}
							}
						}
					}
				}
			}
		}


	}

	[PunRPC]
	void GenerateBoardForClient() {
		/*GameObject[] boards = GameObject.FindGameObjectsWithTag ("Board");

		if (boards != null) {
			for (int i = 0; i < boards.Length; i++) {
				Destroy (boards[i].gameObject);
			}
		}*/
		GameObject clientBoardGO = Instantiate (boardPrefab);
		GameBoard clientBoard = clientBoardGO.GetComponent<GameBoard>();

		GameObject clientSettingsGO = Instantiate (settingsPrefab);
		TileTypeSettings clientSettings = clientSettingsGO.GetComponent<TileTypeSettings> ();

		clientBoard.GenerateTiles (clientBoard.transform.GetChild(0));
		clientBoard.GenerateIntersections (clientBoard.transform.GetChild(1));
		clientBoard.GenerateEdges (clientBoard.transform.GetChild(2));

		if (PhotonNetwork.isMasterClient) {
			BoardDecorator clientBoardDecorator = new BoardDecorator (clientBoard, clientSettings);

			foreach(GameTile tile in clientBoard.GameTiles.Values) {
				Material mat = clientSettings.getMaterialsDictionary ()[tile.tileType];
				GetComponent<PhotonView> ().RPC ("PaintTile", PhotonTargets.Others, new object[] {
					tile.id,
					tile.diceValue,
					(int)tile.tileType,
					tile.atIslandLayer
				});
			}

			//int robberTileID = clientBoard.robber.occupyingTile.id;
			//int pirateTileID = clientBoard.pirate.occupyingTile.id;

			/*GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
				0,
				robberTileID,
				false
			});

			GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
				1,
				pirateTileID
			});*/
		}
	}

	[PunRPC]
	void GenerateBoardForClientFromSavedGame() {
		
		GameObject clientBoardGO = Instantiate (boardPrefab);
		GameBoard clientBoard = clientBoardGO.GetComponent<GameBoard>();
		Persistence.pe_GameBoard pe_gameBoard = GameManager.instance.gameBoard;
			
		// deseralize the setttings to generate the same shape
		clientBoard.mapShape = (MapShape)pe_gameBoard.mapShape;
		clientBoard.mapWidth = pe_gameBoard.mapWidth;
		clientBoard.mapHeight = pe_gameBoard.mapHeight;
		clientBoard.hexOrientation = (HexOrientation)pe_gameBoard.hexOrientation;
		clientBoard.hexRadius = pe_gameBoard.hexRadius;

		GameObject clientSettingsGO = Instantiate (settingsPrefab);
		TileTypeSettings clientSettings = clientSettingsGO.GetComponent<TileTypeSettings> (); // not needed

		clientBoard.GenerateTiles (clientBoard.transform.GetChild(0));
		clientBoard.GenerateIntersections (clientBoard.transform.GetChild(1));
		clientBoard.GenerateEdges (clientBoard.transform.GetChild(2));

		if (PhotonNetwork.isMasterClient) {
			BoardDecorator clientBoardDecorator = new BoardDecorator (clientBoard, pe_gameBoard);

			foreach(GameTile tile in clientBoard.GameTiles.Values) {
				Material mat = clientSettings.getMaterialsDictionary ()[tile.tileType];
				GetComponent<PhotonView> ().RPC ("PaintTile", PhotonTargets.All, new object[] {
					tile.id,
					tile.diceValue,
					(int)tile.tileType,
					tile.atIslandLayer
				});
			}
		}
	}
	public void swapDiceValues(GameTile tile1, GameTile tile2){
		int d1 = tile1.diceValue;
		int d2 = tile2.diceValue;
		GetComponent<PhotonView> ().RPC ("PaintTile", PhotonTargets.All, new object[] {
			tile1.id,
			d2,
			(int)tile1.tileType,
			tile1.atIslandLayer
		});
		GetComponent<PhotonView> ().RPC ("PaintTile", PhotonTargets.All, new object[] {
			tile2.id,
			d1,
			(int)tile2.tileType,
			tile2.atIslandLayer
		});
	}
	[PunRPC]
	void PaintTile(int id, int diceValue, int tileTypeNum, bool islandLayer) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		if (clientBoard != null) {
			GameTile tileToPaint = clientBoard.GameTiles [id];
			tileToPaint.setTileType (tileTypeNum);
			if (tileToPaint.tileType != TileType.Desert) {
				tileToPaint.setDiceValue (diceValue);
				tileToPaint.transform.FindChild ("Dice Values").gameObject.SetActive (false);
			} else {
				tileToPaint.transform.FindChild ("Dice Values").gameObject.SetActive (true);
			}

			tileToPaint.atIslandLayer = islandLayer;
		}
	}

	[PunRPC]
	void PaintFishTile(int fishTileID, int diceValue) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		Debug.Log("Fishtiles dictionary count = " + clientBoard.FishTiles.Values.Count);

		foreach (var key in clientBoard.FishTiles.Keys) {
			Debug.Log ("key = " + key);
		}
		if (clientBoard != null) {
			if (clientBoard.FishTiles.ContainsKey (fishTileID)) {
				FishTile fishTileToPaint = clientBoard.FishTiles [fishTileID];
				fishTileToPaint.setDiceValue (diceValue);
				fishTileToPaint.locationTile.diceValue = diceValue;
			}
		}
	}

	[PunRPC]
	void OldBootReceived(int playerNumber, bool received) {

	}


	[PunRPC]
	void ClearHarborsDictionary() {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		clientBoard.Harbors.Clear ();
	}

	[PunRPC]
	void GenerateHarbor(int harborType, int harborID, int intersection1ID, int intersection2ID) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		//if (!clientBoard.Harbors.ContainsKey (harborID)) {
			int index = 0;

			for (int i = 0; i < clientBoard.harborPrefabs.Length; i++) {
				if (clientBoard.harborPrefabs [i].GetComponent<Harbor> ().resourceType == (ResourceType)harborType) {
					Debug.Log ("Found " + (ResourceType)harborType + " at array index " + i);
					index = i;
				}
			}

			GameObject harborGO = Instantiate(clientBoard.harborPrefabs[index], clientBoard.transform.GetChild(3));
			Harbor harbor = harborGO.GetComponent<Harbor> ();
			harbor.id = harborID;

			Intersection intersection1 = clientBoard.Intersections [intersection1ID];
			Intersection intersection2 = clientBoard.Intersections [intersection2ID];
			intersection1.harbor = harbor;
			intersection2.harbor = harbor;

			harborGO.transform.position = intersection1.getCommonTileWith (intersection2, TileType.Ocean).transform.position
				+ 0.01f * Vector3.up;
			harborGO.transform.localScale *= 1.25f;
			harborGO.name = "Harbor " + harbor.id;

			Debug.Log("Creating harbor" + harborGO.name + " with resource type " + harbor.resourceType);
			foreach (var pair in clientBoard.Harbors) {
				Debug.Log ("Client dictionary already contains " + pair.Key + " , val = " + pair.Value);
			}

			harbor.locations.Add (intersection1);
			harbor.locations.Add (intersection2);

			SpriteRenderer[] arrows = harbor.GetComponentsInChildren<SpriteRenderer> ();

			int cornerNum1 = intersection1.getCommonTileWith (intersection2, TileType.Ocean).getCornerNumberOfIntersection (intersection1);
			arrows [1].transform.rotation = Quaternion.Euler(new Vector3 (90.0f, 0.0f, 30.0f + 60.0f * cornerNum1));
			arrows[1].transform.Translate(Vector3.right * (float)(5 * clientBoard.hexRadius / 8), Space.Self);

			int cornerNum2 = intersection1.getCommonTileWith (intersection2, TileType.Ocean).getCornerNumberOfIntersection (intersection2);
			arrows [2].transform.rotation = Quaternion.Euler(new Vector3 (90.0f, 0.0f, 30.0f + 60.0f * cornerNum2));
			arrows[2].transform.Translate(Vector3.right * (float)(5 * clientBoard.hexRadius / 8), Space.Self);

			clientBoard.Harbors.Add (harbor.id, harbor);
		//}
	}

	[PunRPC]
	void GenerateHarborsForClient() {
		Harbor[] previousHarbors = GameObject.FindObjectsOfType<Harbor> ();
		for (int i = 0; i < previousHarbors.Length; i++) {
			Destroy (previousHarbors [i].gameObject);
		}

		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		clientBoard.ClearHarbors ();
		clientBoard.GenerateHarbors (clientBoard.transform.GetChild (3));

		clientBoard.ClearFishTiles ();
		clientBoard.GenerateFishGroundTiles ();

		if (PhotonNetwork.isMasterClient) {
			List<int> fishDiceValues = new List<int> { 4, 5, 6, 8, 9, 10 };
			foreach (var fishtile in clientBoard.FishTiles.Values) {
				int randomDiceValue = fishDiceValues[Random.Range (0, fishDiceValues.Count)];

				GetComponent<PhotonView> ().RPC ("PaintFishTile", PhotonTargets.All, new object[] {
					fishtile.id,
					randomDiceValue
				});

				fishDiceValues.Remove (randomDiceValue);
			}
		}
	}

	[PunRPC]
	void PlaceBoardPieces(int boardPieceNum, int tileID, bool remove) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		if (clientBoard != null) {
			// Board Piece Num:
			// 0 -> Move Robber
			// 1 -> Move Pirate
			// 2 -> Move Merchant
			switch (boardPieceNum) {
			case 0:
				Robber[] robbers = GameObject.FindObjectsOfType<Robber> ();
				if (robbers != null) {
					for (int i = 0; i < robbers.Length; i++) {
						robbers [i].occupyingTile.occupier = null;
						Destroy (robbers [i].gameObject);
					}
				}
				clientBoard.robber = null;
				if (!remove) {
					clientBoard.MoveRobber (tileID);
				}
				break;
			case 1:
				Pirate[] pirates = GameObject.FindObjectsOfType<Pirate> ();
				if (pirates != null) {
					for (int i = 0; i < pirates.Length; i++) {
						pirates [i].occupyingTile.occupier = null;
						Destroy (pirates [i].gameObject);
					}
				}
				clientBoard.pirate = null;
				if (!remove) {
					clientBoard.MovePirate (tileID);
				}
				break;
			case 2:
				Merchant[] merchants = GameObject.FindObjectsOfType<Merchant> ();
				if (merchants != null) {
					for (int i = 0; i < merchants.Length; i++) {
						merchants [i].occupyingTile.occupier = null;
						Destroy (merchants [i].gameObject);
					}
				}
				clientBoard.merchant = null;
				if (!remove) {
					clientBoard.MoveMerchant (tileID);
				}
				break;
			default:
				break;
			}
		}
	}

	[PunRPC]
	void HighlightIntersections(int userNum, bool highlight, int[] intersectionIDs) {
		if (PhotonNetwork.player.ID == userNum + 1) {
			BoardManager clientBoardManager = GameObject.FindGameObjectWithTag ("BoardManager").GetComponent<BoardManager> ();
			clientBoardManager.highlightIntersectionsWithPlayerColor (intersectionIDs, highlight, userNum);
		}
	}

	[PunRPC]
	void HighlightEdges(int userNum, bool highlight, int[] edgeIDs) {
		if (PhotonNetwork.player.ID == userNum + 1) {
			BoardManager clientBoardManager = GameObject.FindGameObjectWithTag ("BoardManager").GetComponent<BoardManager> ();
			clientBoardManager.highlightEdgesWithPlayerColor (edgeIDs, highlight, userNum);
		}
	}

	[PunRPC]
	void HighlightUnits(int userNum, bool highlight, int[] unitIDs) {
		if (PhotonNetwork.player.ID == userNum + 1) {
			BoardManager clientBoardManager = GameObject.FindGameObjectWithTag ("BoardManager").GetComponent<BoardManager> ();
			clientBoardManager.highlightUnitsWithPlayerColor (unitIDs, highlight, userNum);
		}
	}

	[PunRPC]
	void ActivateKnight(int playerNum, int unitID, bool active, bool isPaid) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Knight knightToActivate = (Knight)clientCatanManager.unitManager.unitsInPlay [unitID];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Found knight to upgrade with id#: " + knightToActivate.id + ". Residing on intersection id#: " + knightToActivate.locationIntersection.id);

		knightToActivate.activateKnight (active);

		if (isPaid) {
			clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOf (MoveType.ActivateKnight));
		}
		clientCatanManager.boardManager.highlightKnightsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Knight), true, clientCatanManager.players [playerNum].playerColor);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void PromoteKnight(int playerNum, int unitID, bool isPaid) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Knight knightToPromote = (Knight)clientCatanManager.unitManager.unitsInPlay [unitID];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Found knight to promote with id#: " + knightToPromote.id + ". Residing on intersection id#: " + knightToPromote.locationIntersection.id);

		GameObject nextLevelKnight = (GameObject)Instantiate (clientCatanManager.unitManager.GetNextLevelKnightPrefab(knightToPromote.rank));
		Knight newKnight = nextLevelKnight.GetComponent<Knight> ();
		newKnight.id = knightToPromote.id;
		newKnight.name = newKnight.rank.ToString () + " Knight " + newKnight.id;

		clientCatanManager.unitManager.unitsInPlay [knightToPromote.id] = newKnight;

		knightToPromote.locationIntersection.occupier = newKnight;
		newKnight.locationIntersection = knightToPromote.locationIntersection;

		clientCatanManager.players [playerNum].removeOwnedUnit (knightToPromote, typeof(Knight));
		clientCatanManager.players [playerNum].addOwnedUnit (newKnight, typeof(Knight));

		newKnight.owner = clientCatanManager.players [playerNum];

		newKnight.transform.position = knightToPromote.transform.position;
		newKnight.transform.parent = knightToPromote.transform.parent;
		newKnight.transform.localScale = knightToPromote.transform.localScale;

		newKnight.GetComponentsInChildren<SpriteRenderer> ()[1].color = clientCatanManager.players [playerNum].playerColor;

		if (knightToPromote.isActive) {
			newKnight.activateKnight (true);
			newKnight.actionPerformedThisTurn = knightToPromote.actionPerformedThisTurn;
		}

		Destroy (knightToPromote.gameObject);

		if (isPaid) {
			clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit (UnitType.Knight));
		}
		clientCatanManager.boardManager.highlightKnightsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Knight), true, clientCatanManager.players [playerNum].playerColor);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void MoveKnight(int playerNum, int unitID, int newLocationID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Knight knightToMove = (Knight)clientCatanManager.unitManager.unitsInPlay [unitID];
		Intersection newLocationIntersection = clientGameBoard.Intersections [newLocationID];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Moving with id#: " + knightToMove.id + ". From current location with intersection id#: " + knightToMove.locationIntersection.id + 
			" to new intersection with id#: " + newLocationIntersection.id);

		knightToMove.locationIntersection.occupier = null;
		knightToMove.locationIntersection = newLocationIntersection;
		newLocationIntersection.occupier = knightToMove;

		knightToMove.transform.position = newLocationIntersection.transform.position + Vector3.up * 0.2f;
		knightToMove.transform.parent = newLocationIntersection.transform;

		knightToMove.activateKnight (false);
		knightToMove.actionPerformedThisTurn = true;

		clientCatanManager.boardManager.highlightKnightsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Knight), true, clientCatanManager.players [playerNum].playerColor);
		clientCatanManager.boardManager.highlightAllIntersections (false);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void DisplaceKnight(int playerNum, int unitID, int opponentKnightID) {
		StartCoroutine (DisplaceKnightCoroutine (playerNum, unitID, opponentKnightID));
	}

	IEnumerator DisplaceKnightCoroutine(int playerNum, int knightID, int opponentKnightID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Knight playerKnight = (Knight)clientCatanManager.unitManager.unitsInPlay [knightID];
		Knight opponentKnight = (Knight)clientCatanManager.unitManager.unitsInPlay [opponentKnightID];
		Intersection newLocationIntersection = opponentKnight.locationIntersection;

		bool opponentKnightActive = opponentKnight.isActive;

		print("Player knight with id#: " + playerKnight.id + " at intersection with id#: " + playerKnight.locationIntersection.id +
			" displaced opponent knight with id#: " + opponentKnight.id + " at intersection with id#: " + opponentKnight.locationIntersection.id);

		int[] opponentIDArray = { opponentKnight.owner.playerNumber - 1 };
		EventTransferManager.instance.BeginWaitForPlayers (opponentIDArray);

		if (PhotonNetwork.player.ID == opponentKnight.owner.playerNumber) {
			yield return StartCoroutine(clientCatanManager.moveKnight(opponentKnightID, true));

			if (clientCatanManager.unitManager.unitsInPlay [opponentKnightID] != null) {
				if (opponentKnightActive) {
					EventTransferManager.instance.OnKnightActionForUser (MoveType.ActivateKnight, opponentKnight.owner.playerNumber - 1, opponentKnightID, -1, true, false);
				}
			}
		}

		if (clientCatanManager.unitManager.unitsInPlay [opponentKnightID] != null) {
			Debug.Log ("Waiting for players started");
			while (EventTransferManager.instance.waitingForPlayers) {
				EventTransferManager.instance.CheckIfPlayersReady ();
				Debug.Log ("Waiting...");
				yield return new WaitForEndOfFrame ();
			}
		}

		playerKnight.locationIntersection.occupier = null;
		playerKnight.locationIntersection = newLocationIntersection;
		newLocationIntersection.occupier = playerKnight;

		playerKnight.transform.position = newLocationIntersection.transform.position + Vector3.up * 0.2f;
		playerKnight.transform.parent = newLocationIntersection.transform;

		playerKnight.activateKnight (false);
		playerKnight.actionPerformedThisTurn = true;

		clientCatanManager.boardManager.highlightKnightsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Knight), true, clientCatanManager.players [playerNum].playerColor);
		clientCatanManager.boardManager.highlightAllIntersections (false);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void ChaseRobber(int playerNum, int knightID, int gamePieceType) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Knight playerKnight = (Knight)clientCatanManager.unitManager.unitsInPlay [knightID];

		if (PhotonNetwork.player.ID - 1 == playerNum) {
			OnMoveRobberOrPirate (gamePieceType, false, true);
		}

		playerKnight.activateKnight (false);
		playerKnight.actionPerformedThisTurn = true;

		clientCatanManager.boardManager.highlightKnightsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Knight), true, clientCatanManager.players [playerNum].playerColor);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void BuildIntersectionUnit(int userNum, int unitType, int intersectionID, bool paid) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		print (clientCatanManager.players [userNum].playerName + " builds a " + ((UnitType)unitType).ToString() + " on intersection #" + intersectionID);

		GameObject intersectionUnitGO = null;
		IntersectionUnit intersectionUnit = null;

		switch ((UnitType)unitType) {
		case UnitType.Settlement:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Settlement));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			intersectionUnitGO.name = "Settlement " + clientCatanManager.unitManager.unitID;
			break;
		case UnitType.City:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.City));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			intersectionUnitGO.name = "City " + clientCatanManager.unitManager.unitID;
			break;
		case UnitType.Metropolis:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Metropolis));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			break;
		case UnitType.CityWalls:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.CityWalls));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			break;
		case UnitType.Knight:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Knight));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			intersectionUnitGO.name = "Basic Knight " + clientCatanManager.unitManager.unitID;
			break;
		default:
			return;
			break;
		}

		intersectionUnit.id = clientCatanManager.unitManager.unitID++;
		clientCatanManager.unitManager.unitsInPlay.Add (intersectionUnit.id, intersectionUnit);

		clientGameBoard.Intersections[intersectionID].occupier = intersectionUnit;
		intersectionUnit.locationIntersection = clientGameBoard.Intersections[intersectionID];
		clientCatanManager.players [userNum].addOwnedUnit(intersectionUnit, (UnitType)unitType);
		intersectionUnit.owner = clientCatanManager.players [userNum];

		intersectionUnit.transform.position = clientGameBoard.Intersections[intersectionID].transform.position;
		intersectionUnit.transform.parent = clientGameBoard.Intersections[intersectionID].transform;
		intersectionUnit.transform.localScale = intersectionUnit.transform.localScale * clientGameBoard.hexRadius;

		if ((UnitType)unitType == UnitType.Knight) {
			intersectionUnit.transform.localScale = intersectionUnit.transform.localScale * (1.5f / 3.33f);
			intersectionUnit.transform.position = intersectionUnit.transform.position + Vector3.up * 0.2f;
			intersectionUnit.GetComponentsInChildren<SpriteRenderer> ()[1].color = clientCatanManager.players [userNum].playerColor;
		} else {
			intersectionUnit.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [userNum].playerColor;
		}

		intersectionUnit.gameObject.SetActive (true);

		if (paid && !EventTransferManager.instance.setupPhase) {
			clientCatanManager.players [userNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit ((UnitType)unitType));
		}

		clientCatanManager.boardManager.highlightAllIntersections(false);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void UpgradeSettlement(int playerNum, int unitID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Settlement settlementToUpgrade = (Settlement)clientCatanManager.unitManager.unitsInPlay [unitID];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Found settlement with id#: " + settlementToUpgrade.id + ". Residing on intersection id#: " + settlementToUpgrade.locationIntersection.id);

		if (settlementToUpgrade != null) {
			GameObject cityGameObject = (GameObject)Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.City));
			City newCity = cityGameObject.GetComponent<City> ();
			newCity.id = settlementToUpgrade.id;

			clientCatanManager.unitManager.unitsInPlay [settlementToUpgrade.id] = newCity;

			settlementToUpgrade.locationIntersection.occupier = newCity;
			newCity.locationIntersection = settlementToUpgrade.locationIntersection;

			clientCatanManager.players [playerNum].removeOwnedUnit (settlementToUpgrade, typeof(Settlement));
			clientCatanManager.players [playerNum].addOwnedUnit (newCity, typeof(City));
			newCity.owner = clientCatanManager.players [playerNum];

			if (cityGameObject != null) {
				cityGameObject.transform.position = settlementToUpgrade.transform.position;
				cityGameObject.transform.parent = settlementToUpgrade.transform.parent;
				cityGameObject.transform.localScale = settlementToUpgrade.transform.localScale;

				newCity.name = "City " + newCity.id;

				cityGameObject.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [playerNum].playerColor;
			}

			Destroy (settlementToUpgrade.gameObject);

			clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit (UnitType.City));
			clientCatanManager.boardManager.highlightUnitsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Settlement), true, clientCatanManager.players [playerNum].playerColor);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
		}


		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void UpgradeCity(int playerNum, int unitID, int metropolisType) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		City cityToUpgrade = (City)clientCatanManager.unitManager.unitsInPlay [unitID];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Found city with id#: " + cityToUpgrade.id + ". Residing on intersection id#: " + cityToUpgrade.locationIntersection.id);

		int destroyedCityLocationID = -1;
		int destroyedCityOwnerNumber = -1;

		bool metropolisExists = false;

		Metropolis newMetropolis = null;
		Metropolis[] metropolisInPlay = GameObject.FindObjectsOfType<Metropolis> ();

		for (int i = 0; i < metropolisInPlay.Length; i++) {
			if (metropolisInPlay [i].metropolisType == (MetropolisType)metropolisType) {
				metropolisExists = true;
				newMetropolis = metropolisInPlay [i];
				destroyedCityLocationID = newMetropolis.locationIntersection.id;
				destroyedCityOwnerNumber = newMetropolis.owner.playerNumber;
				newMetropolis.owner.removeOwnedUnit (metropolisInPlay [i], typeof(Metropolis));
			}
		}

		if (!metropolisExists) {
			GameObject metropolisGameObject = (GameObject)Instantiate (clientCatanManager.unitManager.GetMetropolisOfType ((MetropolisType)metropolisType));
			newMetropolis = metropolisGameObject.GetComponent<Metropolis> ();
			newMetropolis.id = cityToUpgrade.id;
		}

		clientCatanManager.unitManager.unitsInPlay [newMetropolis.id] = newMetropolis;

		cityToUpgrade.locationIntersection.occupier = newMetropolis;
		newMetropolis.locationIntersection = cityToUpgrade.locationIntersection;

		clientCatanManager.players [playerNum].removeOwnedUnit (cityToUpgrade, typeof(City));
		clientCatanManager.players [playerNum].addOwnedUnit (newMetropolis, typeof(Metropolis));
		newMetropolis.owner = clientCatanManager.players [playerNum];

		newMetropolis.transform.position = cityToUpgrade.transform.position;
		newMetropolis.transform.parent = cityToUpgrade.transform.parent;
		newMetropolis.transform.localScale = cityToUpgrade.transform.localScale;

		//newMetropolis.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [playerNum].playerColor;

		Destroy (cityToUpgrade.gameObject);

		if (metropolisExists && PhotonNetwork.player.ID == destroyedCityOwnerNumber) {
			//BuildIntersectionUnit (destroyedCityOwnerNumber - 1, UnitType.City, destroyedCityLocationID);
			//clientCatanManager.players[destroyedCityOwnerNumber - 1].removeOwnedUnit (newMetropolis, typeof(Metropolis));
			OnBuildUnitForUser(UnitType.City, destroyedCityOwnerNumber - 1, destroyedCityLocationID, false, -1);
		}

		//clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit (UnitType.City));
		clientCatanManager.boardManager.highlightUnitsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.City), true, clientCatanManager.players [playerNum].playerColor);
		//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
		clientCatanManager.metropolisOwners.metropolisOwners [metropolisType] = clientCatanManager.players [playerNum];

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void DowngradeCity(int unitID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		City cityToDestroy = (City)clientCatanManager.unitManager.unitsInPlay [unitID];

		//if(cityToDestroy != null) {
		int destroyedCityLocationID = cityToDestroy.locationIntersection.id;
		int destroyedCityID = cityToDestroy.id;
		int playerNum = cityToDestroy.owner.playerNumber - 1;

		//clientCatanManager.unitManager.unitsInPlay.Remove (unitID);
		clientCatanManager.players [playerNum].removeOwnedUnit (cityToDestroy, typeof(City));

		Destroy (cityToDestroy.gameObject);
		//Debug.Log ("destroyed city id: " + newSettlement.id);
		if(PhotonNetwork.isMasterClient) {
			OnBuildUnitForUser (UnitType.Settlement, playerNum, destroyedCityLocationID, false, -1);
		}
		//}


		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void MoveShip(int playerNumber, int shipID, int newLocationEdgeID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		Ship ship = clientCatanManager.unitManager.Units [shipID] as Ship;

		ship.transform.position = clientGameBoard.Edges[newLocationEdgeID].transform.position;
		ship.transform.rotation = clientGameBoard.Edges[newLocationEdgeID].transform.rotation;
		ship.transform.parent = clientGameBoard.Edges[newLocationEdgeID].transform;

		ship.locationEdge.occupier = null;
		clientGameBoard.Edges [newLocationEdgeID].occupier = ship;
		ship.locationEdge = clientGameBoard.Edges [newLocationEdgeID];

		ship.transform.localScale = clientGameBoard.Edges[newLocationEdgeID].transform.localScale;
		ship.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
		EventTransferManager.instance.shipMovedThisTurn = true;
	}

	[PunRPC]
	void BuildCityWalls(int playerNum, int unitID, bool paid) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		City city = clientCatanManager.unitManager.Units [unitID] as City;

		GameObject cityWallGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.CityWalls));
		CityWall cityWall = cityWallGO.GetComponent<CityWall> ();
		cityWall.id = clientCatanManager.unitManager.unitID++;
		cityWall.name = "City Walls " + cityWall.id;

		city.cityWalls = cityWall;
		cityWall.owner = city.owner;
		clientCatanManager.players [playerNum].addOwnedUnit (cityWall, UnitType.CityWalls);

		cityWall.transform.rotation = city.transform.rotation;
		cityWall.transform.parent = city.transform;
		cityWall.transform.localScale = city.transform.localScale / 3.33f;
		cityWall.transform.position = city.transform.position + new Vector3 (0.25f, 0.5f, 0.25f);

		cityWall.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [playerNum].playerColor;
		cityWall.gameObject.SetActive (true);

		if (!EventTransferManager.instance.setupPhase && paid) {
			clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit (UnitType.CityWalls));
		}

		clientCatanManager.boardManager.highlightUnitsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.City), true, clientCatanManager.players [playerNum].playerColor);
		clientCatanManager.boardManager.highlightMetropolisWithOwnColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Metropolis));

		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}

	[PunRPC]
	void BuildEdgeUnit(int userNum, int unitType, int edgeID, bool paid) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		print (clientCatanManager.players [userNum].playerName + " builds a " + ((UnitType)unitType).ToString() + " on edge #" + edgeID);

		GameObject edgeUnitGo = null;
		EdgeUnit edgeUnit = null;

		switch ((UnitType)unitType) {
		case UnitType.Road:
			edgeUnitGo = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Road));
			edgeUnit = edgeUnitGo.GetComponent<EdgeUnit> ();
			edgeUnit.id = clientCatanManager.unitManager.unitID++;
			edgeUnit.name = "Road " + edgeUnit.id;
			break;
		case UnitType.Ship:
			edgeUnitGo = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Ship));
			edgeUnit = edgeUnitGo.GetComponent<EdgeUnit> ();
			edgeUnit.id = clientCatanManager.unitManager.unitID++;
			edgeUnit.name = "Ship " + edgeUnit.id;
			break;
		default:
			break;
		}

		clientCatanManager.unitManager.unitsInPlay.Add (edgeUnit.id, edgeUnit);

		clientGameBoard.Edges[edgeID].occupier = edgeUnit;
		edgeUnit.locationEdge = clientGameBoard.Edges[edgeID];
		clientCatanManager.players [userNum].addOwnedUnit (edgeUnit, (UnitType)unitType);
		edgeUnit.owner = clientCatanManager.players [userNum];

		edgeUnit.transform.position = clientGameBoard.Edges[edgeID].transform.position;
		edgeUnit.transform.rotation = clientGameBoard.Edges[edgeID].transform.rotation;
		edgeUnit.transform.localScale = clientGameBoard.Edges[edgeID].transform.localScale;
		edgeUnit.transform.parent = clientGameBoard.Edges[edgeID].transform;

		edgeUnit.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [userNum].playerColor;
		edgeUnit.gameObject.SetActive (true);

		if (!EventTransferManager.instance.setupPhase && paid) {
			clientCatanManager.players [userNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit ((UnitType)unitType));

			//uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			//uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
		}
		clientCatanManager.boardManager.highlightAllEdges(false);
		clientCatanManager.currentActiveButton = -1;
		clientCatanManager.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayer = false;
	}
		
	[PunRPC]
	void HandleOperationFailure() {
		EventTransferManager.instance.waitingForPlayer = false;
		EventTransferManager.instance.waitingForPlayers = false;
		EventTransferManager.instance.currentActiveButton = -1;
	}
	//initial progress card method creates 3 synced queues of cards for each client
	[PunRPC]
	public IEnumerator GenerateProgressCards(){
		Destroy (GameObject.FindGameObjectWithTag ("ProgressCardsStackManager"));
		GameObject clientcardsStackGO = Instantiate (ProgressCardsStackManagerPrefab);
		//ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
		ProgressCardStackManager clientcards = clientcardsStackGO.GetComponent<ProgressCardStackManager> ();
		//master sets everyones card orders
		if (PhotonNetwork.isMasterClient) {
			yield return new WaitForSeconds (5f);
			clientcards.shuffleCards ();
			for (int i = 0; i < clientcards.yellowCards.Length; i++) {
				GetComponent<PhotonView> ().RPC ("SetProgressCard", PhotonTargets.Others, new object[] {
					(int) ProgressCardColor.Yellow,
					(int) clientcards.yellowCards [i],
					i
				});
			}
			for (int i = 0; i < clientcards.blueCards.Length; i++) {
				GetComponent<PhotonView> ().RPC ("SetProgressCard", PhotonTargets.Others, new object[] {
					(int) ProgressCardColor.Blue,
					(int) clientcards.blueCards [i],
					i
				});
			}
			for (int i = 0; i < clientcards.greenCards.Length; i++) {
				GetComponent<PhotonView> ().RPC ("SetProgressCard", PhotonTargets.Others, new object[] {
					(int) ProgressCardColor.Green,
					(int) clientcards.greenCards [i],
					i
				});
			}
			GetComponent<PhotonView> ().RPC ("GenerateProgressCardQueues", PhotonTargets.All, new object[] { });
		}
	}
	//makes the queues once order has been synced
	[PunRPC]
	void GenerateProgressCardQueues(){
		ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
		//because doubles ??
		clientcards.yellowCardsQueue.Clear();
		clientcards.blueCardsQueue.Clear();
		clientcards.greenCardsQueue.Clear ();
		//generates the queues
		clientcards.generateQueues();
	}
	//sync progress card order across clients
	[PunRPC]
	void SetProgressCard(int color, int type, int position) {
		ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();

		if (clientcards != null) {
			switch ((ProgressCardColor)color) {
			case ProgressCardColor.Yellow:
				clientcards.yellowCards [position] = (ProgressCardType)type;
				break;
			case ProgressCardColor.Blue:
				clientcards.blueCards [position] = (ProgressCardType)type;
				break;
			case ProgressCardColor.Green:
				clientcards.greenCards [position] = (ProgressCardType)type;
				break;
			}
		}
	}
	public bool WaitOnDraw;
	public IEnumerator CardDrawEvent(EventDieFace eventdie,int redDieRoll){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		ProgressCardColor drawcolor=ProgressCardColor.Blue;
		switch (eventdie) {
		case EventDieFace.Blue:
			drawcolor=ProgressCardColor.Blue;
			break;
		case EventDieFace.Yellow:
			drawcolor=ProgressCardColor.Yellow;
			break;
		case EventDieFace.Green:
			drawcolor=ProgressCardColor.Green;
			break;
		}

		Debug.Log ("draw color is: " + drawcolor.ToString ());
		if (eventdie != EventDieFace.Black) {
			ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
			EventTransferManager.instance.WaitOnDraw = false;
			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				while (EventTransferManager.instance.WaitOnDraw) {
					Debug.Log ("Waiting for Players to draw");
					yield return new WaitForEndOfFrame ();
				}
				EventTransferManager.instance.WaitOnDraw = true;
				Debug.Log ("can draw: " + i + " t: " + clientCatanManager.players [i].canDrawProgressCard (eventdie, redDieRoll).ToString());
				if (clientCatanManager.players [i].canDrawProgressCard (eventdie, redDieRoll) && clientcards.checkQueue (drawcolor)) {
					GetComponent<PhotonView> ().RPC ("DrawCard", PhotonTargets.All, new object[] {
						(int)drawcolor,
						i
					});
				} else {
					WaitOnDraw = false;
				}
				Debug.Log ("Draw Completed");
			}
		}
	}
	[PunRPC]
	void OnDrawCompleted(){
		EventTransferManager.instance.WaitOnDraw = false;
	}
	//methods used for drawing cards this will be called by dice roll events player has no control over when he can draw the card himself
	[PunRPC]
	public void DrawCard(int color,int receiverNum){
		Debug.Log ("card color: " + color);
		if(PhotonNetwork.player.ID-1== receiverNum) {
			Debug.Log ("turn to draw: " + receiverNum);
			GetComponent<PhotonView> ().RPC ("SyncProgressCardDraw", PhotonTargets.Others, new object[] {
				color
			});
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
			clientCatanManager.uiManager.progressCardPanel.newCard ((ProgressCardColor)color,clientcards.drawCard ((ProgressCardColor)color));
			GetComponent<PhotonView> ().RPC ("OnDrawCompleted", PhotonTargets.All, new object[] {
			});
		}
	}
	//drawing will be done by client
	public void ReturnProgressCard(ProgressCardColor color, ProgressCardType type){
		GetComponent<PhotonView> ().RPC ("SyncProgressCardreturn", PhotonTargets.All, new object[] {
			(int) color,
			(int) type
		});
	}
	//functions for player progress cards
	//used for notifications of cards
	public void NotifyProgressCard(ProgressCardType type,string name){
		string message="";
		switch(type){
		case ProgressCardType.Constitution:
			message = name + " has played the Constitution card. Victory points permanently increased by 1";
			break;
		case ProgressCardType.Printer:
			message = name + " has played the Printer card. Victory points permanently increased by 1";
			break;
		case ProgressCardType.Warlord:
			message = name + " has played the Warlord card. All owned knights have been activated for free";
			break;
		case ProgressCardType.Inventor:
			message = name + " has played the Inventor card. 2 tiles have swapped dice values";
			break;
		case ProgressCardType.Merchant:
			message = name + " has played the Merchant card. Previous controller, if any, loses 1 victory point";
			break;
		}
		GetComponent<PhotonView> ().RPC ("sendNotification", PhotonTargets.Others, new object[] {
			message
		});
	}
	public void sendNotification(string message,int receiverNum){
		GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
			message,
			receiverNum
		});
	}
	[PunRPC]
	void sendNotificationSpecific(string message,int receiverNum){
		if (PhotonNetwork.player.ID - 1 == receiverNum) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			clientCatanManager.uiManager.notificationpanel.SetActive (true);
			clientCatanManager.uiManager.notificationtext.text = message;
		}
	}
	[PunRPC]
	void sendNotification(string message){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.uiManager.notificationpanel.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text = message;
	}
	//2 rpcs for drawing/returning progress cards
	[PunRPC]
	void SyncProgressCardreturn(int color,int type){
		ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
		clientcards.returnCard((ProgressCardColor)color,(ProgressCardType)type);
	}
	[PunRPC]
	void SyncProgressCardDraw(int color){
		ProgressCardStackManager clientcards = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();
		ProgressCardColor currentColor = (ProgressCardColor)color;
		clientcards.drawCard (currentColor);
	}
	public void addCardToHand(int reciever, ProgressCardType card){
		GetComponent<PhotonView> ().RPC ("addCardToHandRPC", PhotonTargets.All, new object[] {
			reciever,
			(int)card
		});
	}
	[PunRPC]
	void addCardToHandRPC(int reciever,int card){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [reciever].progressCards.Add ((ProgressCardType)card);
	}
	public void removeCardFromHand(int reciever, ProgressCardType card){
		GetComponent<PhotonView> ().RPC ("removeCardFromHandRPC", PhotonTargets.All, new object[] {
			reciever,
			(int)card
		});

	}
	[PunRPC]
	void removeCardFromHandRPC(int reciever,int card){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [reciever].progressCards.Remove ((ProgressCardType)card);
		if(PhotonNetwork.player.ID-1==reciever){
			for (int i = 0; i < clientCatanManager.uiManager.progressCardHolder.progressCardList.Count; i++) {
				if (clientCatanManager.uiManager.progressCardHolder.progressCardList[i].type == (ProgressCardType)card) {
					Debug.Log ("removal was initiated");
					Destroy (clientCatanManager.uiManager.progressCardHolder.progressCardList[i].gameObject);
					clientCatanManager.uiManager.progressCardHolder.progressCardList.RemoveAt(i);
				}
			}
		}
	}
	public void increaseVpAdder(int number, int player){
		GetComponent<PhotonView> ().RPC ("setVpAdder", PhotonTargets.All, new object[] {
			number,
			player
		});

	}

	[PunRPC]
	void setVpAdder(int number, int player){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.players [player].vpAdder = number;
	}
	public IEnumerator playSaboteur(int vplimit,int player,string sendername){
		List<int> toplayers=new List<int>();
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//figure out which players need to do it
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if (clientCatanManager.players[i].getVpPoints() >= vplimit && i!=PhotonNetwork.player.ID - 1) {
				toplayers.Add (i);
				GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
					sendername+" has played the Saboteur card, you must discard half your resources/commodities",
					i
				});
			}
		}
		EventTransferManager.instance.BeginWaitForPlayers (toplayers.ToArray());
		EventTransferManager.instance.waitingForPlayer = true;
		GetComponent<PhotonView> ().RPC ("saboteurRPC", PhotonTargets.Others, new object[] {
			toplayers.ToArray()
		});

		Debug.Log ("Waiting for players started");

		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}
	}
	[PunRPC]
	IEnumerator saboteurRPC(int[] allPlayers){
		for(int i=0;i<allPlayers.Length;i++){
			if (PhotonNetwork.player.ID - 1 == allPlayers[i]) {
				CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

				EventTransferManager.instance.BeginWaitForPlayers (allPlayers);

				int numDiscards = clientCatanManager.players [allPlayers[i]].getHalfAssetCount ();
				yield return StartCoroutine (clientCatanManager.selectResourcesForPlayers (numDiscards, false));
				Debug.Log ("Waiting for players started");
				while (EventTransferManager.instance.waitingForPlayers) {
					EventTransferManager.instance.CheckIfPlayersReady ();
					Debug.Log ("Waiting...");
					yield return new WaitForEndOfFrame ();
				}
			}
			Debug.Log ("events completed");
		}
	}
	public IEnumerator playWedding(int vplimit, int player,string sendername){
		List<int> toplayers=new List<int>();
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//figure out which players need to do it
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if (clientCatanManager.players[i].getVpPoints() >= vplimit && i!=PhotonNetwork.player.ID - 1) {
				toplayers.Add (i);
				GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
					sendername+" has played the Wedding card, you must give him 2 of your resources/commodities",
					i
				});
			}
		}

		EventTransferManager.instance.BeginWaitForPlayers (toplayers.ToArray());
		GetComponent<PhotonView> ().RPC ("weddingRPC", PhotonTargets.Others, new object[] {
			toplayers.ToArray(),
			player
		});

		Debug.Log ("Waiting for players started");
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}

	}
	[PunRPC]
	IEnumerator weddingRPC(int[] allPlayers, int sendto){
		for(int i=0;i<allPlayers.Length;i++){
			if (PhotonNetwork.player.ID - 1 == allPlayers[i]) {
				CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

				EventTransferManager.instance.BeginWaitForPlayers (allPlayers);

				yield return StartCoroutine (clientCatanManager.stealRessourcesWedding (sendto));

				Debug.Log ("Waiting for players started");
				while (EventTransferManager.instance.waitingForPlayers) {
					EventTransferManager.instance.CheckIfPlayersReady ();
					Debug.Log ("Waiting...");
					yield return new WaitForEndOfFrame ();
				}
			}
		}
	}
	//handles both trade and resource monopoly
	public void playMonopoly(int sender,int selection,bool trigger){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		AssetTuple tempasset = new AssetTuple ();
		string message = "";
		if (trigger) {
			tempasset.SetValueAtIndex (selection, 1);
			message = " has played the Trade Monopoly card, you must give him 1 "+ ((CommodityType)selection - 5).ToString ();
		} else {
			tempasset.SetValueAtIndex (selection, 2);
			message = " has played the Resource Monopoly card, you must give him 2 "+ ((ResourceType)selection).ToString ();
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if(i!=sender && clientCatanManager.players[i].hasAvailableAssets(tempasset)){
				EventTransferManager.instance.OnTradeWithBank (sender, true, tempasset);
				EventTransferManager.instance.OnTradeWithBank (i, false, tempasset);
				GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
					clientCatanManager.players [sender].playerName + message,
					i
				});
			}

		}
	}
	public IEnumerator playCommercialHarbor (List<Player> recievers, int[] selections){
		List<int> toplayers=new List<int>();
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		for (int i = 0; i < recievers.Count; i++) {
			//only if the player has commodities
			if (!recievers [i].hasZeroCommodities()) {
				GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
					clientCatanManager.players[PhotonNetwork.player.ID-1].playerName+" has played the Commercial Harbor, you must give him 1 of your commodities, you will recieve a resource of your choice",
					recievers[i].playerNumber-1
				});
				GetComponent<PhotonView> ().RPC ("commercialHarborRPC", PhotonTargets.Others, new object[] {
					recievers[i].playerNumber-1,
					PhotonNetwork.player.ID - 1,
					selections [i]
				});
				toplayers.Add (recievers[i].playerNumber-1);
			}
		}
		EventTransferManager.instance.BeginWaitForPlayers (toplayers.ToArray());
		Debug.Log ("Waiting for players started");
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}
	}
	[PunRPC]
	IEnumerator commercialHarborRPC(int receiver,int sender,int receiveselection){
		if (PhotonNetwork.player.ID - 1 == receiver) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			AssetTuple givetuple = new AssetTuple ();
			givetuple.SetValueAtIndex (receiveselection, 1);
			clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (true);
			//disabled resources
			for (int i = 0; i < 5; i++) {
				clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (false);
			}
			//wait for selection
			bool selectionMade = false;
			//get the selection
			while (!selectionMade) {
				if (!clientCatanManager.uiManager.fishresourcepanel.selectionMade) {
					yield return StartCoroutine (clientCatanManager.uiManager.fishresourcepanel.waitUntilButtonDown ());
				}
				if (clientCatanManager.uiManager.fishresourcepanel.selectionMade) {
					selectionMade = true;
				}
			}
			clientCatanManager.uiManager.fishresourcepanel.selectionMade = false;
			int selection = clientCatanManager.uiManager.fishresourcepanel.getSelection ();
			//reactivate resource options
			for (int i = 0; i < 5; i++) {
				clientCatanManager.uiManager.fishresourcepanel.resourceoptions [i].gameObject.SetActive (true);
			}
			clientCatanManager.uiManager.fishresourcepanel.gameObject.SetActive (false);
			AssetTuple temptuple = new AssetTuple ();
			temptuple.SetValueAtIndex (selection, 1);
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, false, temptuple);
			EventTransferManager.instance.OnTradeWithBank (sender, true, temptuple);

			EventTransferManager.instance.OnTradeWithBank (sender, false, givetuple);
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, true, givetuple);

			EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
		}
	}
	[PunRPC]
	void notifyAlchemist(int red, int yellow,int name){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.uiManager.notificationpanel.gameObject.SetActive (true);
		clientCatanManager.uiManager.notificationtext.text=clientCatanManager.players[name].playerName+" played the Alchemist Card red die value: "+red+" yellow die value: "+yellow;

	}
	public IEnumerator playDeserter(int selection){
		int[] toplayers={selection};
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		//first send notification
		GetComponent<PhotonView> ().RPC ("sendNotificationSpecific", PhotonTargets.Others, new object[] {
			clientCatanManager.players[PhotonNetwork.player.ID-1].playerName+" has played the Deserter card, you must destroy 1 of your knights",
			selection
		});
		EventTransferManager.instance.BeginWaitForPlayers (toplayers);

		GetComponent<PhotonView> ().RPC ("knightRPC", PhotonTargets.Others, new object[] {
			selection
		});

		Debug.Log ("Waiting for players started");
		while (EventTransferManager.instance.waitingForPlayers) {
			EventTransferManager.instance.CheckIfPlayersReady ();
			Debug.Log ("Waiting...");
			yield return new WaitForEndOfFrame ();
		}


	}
	[PunRPC]
	IEnumerator knightRPC(int reciever){
		if (PhotonNetwork.player.ID - 1 == reciever) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			yield return StartCoroutine (clientCatanManager.removeKnight ());
			OnPlayerReady(PhotonNetwork.player.ID-1,true);
		}
	}
	public void setMerchantController(){
		GetComponent<PhotonView> ().RPC ("merchantRPC", PhotonTargets.All, new object[] {
			PhotonNetwork.player.ID-1
		});
	}
	[PunRPC]
	void merchantRPC(int playerNum){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		clientCatanManager.merchantController = playerNum;
	}
	public void onDeserterBuild(KnightRank rank){
		GetComponent<PhotonView> ().RPC ("knightbuildRPC", PhotonTargets.Others, new object[] {
			(int)rank
		});
	}
	[PunRPC]
	public void knightbuildRPC(int rank){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if (clientCatanManager.players [clientCatanManager.currentPlayerTurn].playerNumber-1 == PhotonNetwork.player.ID - 1) {
			StartCoroutine(clientCatanManager.buildDeserter ((KnightRank)rank));
		}
	}
	public IEnumerator playAlchemist(int redDieRoll, int yellowDieRoll) {
		if (!diceRolledThisTurn) {
			
			GetComponent<PhotonView> ().RPC ("RollDice", PhotonTargets.All, new object[] {Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),true});
			GetComponent<PhotonView> ().RPC ("notifyAlchemist", PhotonTargets.Others, new object[] {
				redDieRoll,
				yellowDieRoll,
				PhotonNetwork.player.ID-1
			});

			GameObject dice = GameObject.FindGameObjectWithTag ("DiceRoller").GetComponent<DiceRoller> ().dice [2];
			EventDieFace eventDieRoll=EventDieFace.Black;
			bool ready = true;		
			//wait for dice to finish rolling
			while (ready) {
				dice = GameObject.FindGameObjectWithTag ("DiceRoller").GetComponent<DiceRoller> ().dice [2];
				yield return new WaitForSeconds (3f);
				bool z = dice.GetComponent<FaceDetection> ().ready;
				if (z) {
					ready = false;
				} else {
					Debug.Log("Re-roll required");
					GetComponent<PhotonView> ().RPC ("RollDice", PhotonTargets.All, new object[] {Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),Random.Range(-5.0f, 5.0f),true});
				}
				Debug.Log ("Dice Rolling");
				yield return new WaitForEndOfFrame ();
			}


			DieHelper temp = dice.GetComponent<DieHelper> ();
			switch (temp.value) {
			case 1:
				eventDieRoll = EventDieFace.Blue;
				break;
			case 2:
				eventDieRoll = EventDieFace.Black;
				break;
			case 3:
				eventDieRoll = EventDieFace.Green;
				break;
			case 4:
				eventDieRoll = EventDieFace.Black;
				break;
			case 5:
				eventDieRoll = EventDieFace.Black;
				break;
			case 6:
				eventDieRoll = EventDieFace.Yellow;
				break;
			}

			//let everyone look then destroy
			yield return new WaitForSeconds (5f);

			GetComponent<PhotonView> ().RPC ("DestroyDice", PhotonTargets.All, new object[] {});

			Debug.Log("Red: "+redDieRoll+" Yellow: "+yellowDieRoll+ " Event: "+eventDieRoll.ToString());

			if (!setupPhase && redDieRoll + yellowDieRoll == 7) {
				//StartCoroutine(clientCatanManager.discardResourcesForPlayers());
				//StartCoroutine(clientCatanManager.moveRobberForCurrentPlayer());
				GetComponent<PhotonView> ().RPC ("EnforceDiceRollEvents", PhotonTargets.All, new object[] {});
			} else {
				if (!setupPhase) {
					//CommodityCollectionEvent (redDieRoll + yellowDieRoll);
					GetComponent<PhotonView> ().RPC ("CommodityCollectionEvent", PhotonTargets.All, new object[] {
						redDieRoll + yellowDieRoll
					});
				}

				GetComponent<PhotonView> ().RPC ("ResourceCollectionEvent", PhotonTargets.All, new object[] {
					redDieRoll + yellowDieRoll
				});

			}
			diceRolledThisTurn = true;
		}
	}

	public void saveFile(string fileName){
		GetComponent<PhotonView> ().RPC ("saveFileEvent", PhotonTargets.All, new object[] {
			fileName
		});
	}
	[PunRPC]
	private void saveFileEvent(string fileName){
		SaveJson.saveJson (fileName);
	}

}

public enum MoveType {
	BuildSettlement = 0,
	BuildRoad,
	BuildCity,
	UpgradeSettlement,
	BuildShip,
	TradeBank,
	MoveShip,
	TradePlayer,
	BuildKnight,
	ActivateKnight,
	PromoteKnight,
	MoveKnight,
	DisplaceKnight,
	ChaseRobber,
	RemoveKnight,
	BuildCityWall
}

