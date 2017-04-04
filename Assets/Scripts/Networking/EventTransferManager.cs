﻿using System.Collections;
using System.Collections.Generic;
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

	public bool waitingForPlayers = false;
	public bool[] playerChecks;

	void Awake() {
		if (instance == null)
			instance = this;

		playerChecks = new bool[LevelManager.instance.players.Count];
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnReadyToPlay() {
		GetComponent<PhotonView> ().RPC ("GenerateBoardForClient", PhotonTargets.All, new object[] { });
		GetComponent<PhotonView> ().RPC ("GenerateHarborsForClient", PhotonTargets.All, new object[] { });
		GetComponent<PhotonView> ().RPC ("GenerateProgressCards", PhotonTargets.All, new object[] { });
		//GetComponent<PhotonView> ().RPC ("CleanExtraInstances", PhotonTargets.All, new object[] { });
		StartCoroutine (CatanSetupPhase());
	}

	public void OnEndTurn() {
		GetComponent<PhotonView> ().RPC ("EndTurn", PhotonTargets.All, new object[] { });
		currentPlayerTurn = (currentPlayerTurn + 1) % PhotonNetwork.playerList.Length;
		diceRolledThisTurn = false;
		shipMovedThisTurn = false;
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
			assetsTraded.commodities.commodityTuple[CommodityType.Cloth]
		});
	}


	public void OnMoveGamePiece(int boardPieceNum, int tileID) {
		GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
			boardPieceNum,
			tileID
		});
	}

	public void OnDiceRolled() {
		if (!diceRolledThisTurn) {
			CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
			//int redDieRoll = Random.Range (1, 7);
			//int yellowDieRoll = Random.Range (1, 7);
			GameObject[] dice = GameObject.FindGameObjectsWithTag ("Dice");
			//int redDieRoll = dice[0].GetComponent<FaceDetection>().getNumber;
			int redDieRoll = 3;
			int yellowDieRoll = 4;
			print ("Red die rolled: " + redDieRoll);
			print ("Yellow die rolled: " + yellowDieRoll);

			GetComponent<PhotonView> ().RPC ("RollDice", PhotonTargets.All, new object[] {redDieRoll, redDieRoll, redDieRoll});

			if (!setupPhase && redDieRoll + yellowDieRoll == 7) {
				//StartCoroutine(clientCatanManager.discardResourcesForPlayers());
				//StartCoroutine(clientCatanManager.moveRobberForCurrentPlayer());
				GetComponent<PhotonView> ().RPC ("EnforceDiceRollEvents", PhotonTargets.All, new object[] {});
			} else {
				GetComponent<PhotonView> ().RPC ("ResourceCollectionEvent", PhotonTargets.All, new object[] {
					redDieRoll + yellowDieRoll
				});
				//ResourceCollectionEvent (redDieRoll + yellowDieRoll);

				if (!setupPhase) {
					//CommodityCollectionEvent (redDieRoll + yellowDieRoll);
					GetComponent<PhotonView> ().RPC ("CommodityCollectionEvent", PhotonTargets.All, new object[] {
						redDieRoll + yellowDieRoll
					});
				}
			}
			diceRolledThisTurn = true;
		}
	}

	[PunRPC]
	void EnforceDiceRollEvents() {
		StartCoroutine(DiceRollSevenEvents());
	}

	IEnumerator DiceRollSevenEvents() {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		yield return StartCoroutine(clientCatanManager.discardResourcesForPlayers());

		if (PhotonNetwork.player.ID - 1 == clientCatanManager.currentPlayerTurn) {
			yield return StartCoroutine(clientCatanManager.moveRobberForCurrentPlayer());
		}
	}

	[PunRPC]
	void RollDice(int number, int number1, int number2){
		diceRoller = Instantiate(diceRollerPrefab , new Vector3(-1.0f,0,-5.7f),Quaternion.identity);
		GameObject[] dice = GameObject.FindGameObjectsWithTag ("Dice");
		foreach(GameObject go in dice) {
			DiePhysics physics = go.GetComponent<DiePhysics> ();
			Debug.Log ("vector is " + number + " " + number1 + " " + number2);
			physics.init (new Vector3 ((float)number+1, (float)number1+1, (float)number2+1));
		}
		StartCoroutine (ShowDiceResult ());
	}

	IEnumerator ShowDiceResult(){
		yield return new WaitForSeconds (3.0f);
		GameObject[] dices = GameObject.FindGameObjectsWithTag ("Dice");
		foreach(GameObject go in dices) {
			FaceDetection faceDetection = go.GetComponent<FaceDetection> ();
			faceDetection.showNumber ();
		}
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

	public void OnBuildUnitForUser(UnitType unitType, int playerNumber, int id) {
		if (unitType == UnitType.Road || unitType == UnitType.Ship) {
			GetComponent<PhotonView> ().RPC ("BuildEdgeUnit", PhotonTargets.All, new object[] {
				playerNumber,
				(int)unitType,
				id
			});
		} else if (unitType == UnitType.City && !EventTransferManager.instance.setupPhase) {
			GetComponent<PhotonView> ().RPC ("UpgradeSettlement", PhotonTargets.All, new object[] {
				playerNumber,
				id
			});
		} else {
			GetComponent<PhotonView> ().RPC ("BuildIntersectionUnit", PhotonTargets.All, new object[] {
				playerNumber,
				(int)unitType,
				id
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
				StartCoroutine(clientCatanManager.unitManager.buildRoad ());
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
			case MoveType.MoveShip:
				StartCoroutine(clientCatanManager.unitManager.moveShip ());
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
	void ResourceChangeEvent(int playerNum, bool gained, int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();

		AssetTuple delta = new AssetTuple (brick, grain, lumber, ore, wool, paper, coin, cloth);

		if (gained) {
			clientCatanManager.players [playerNum].receiveAssets (delta);
		} else {
			clientCatanManager.players [playerNum].spendAssets (delta);
		}
	}

	[PunRPC]
	void ResourceCollectionEvent(int diceOutcome) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

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
						//CommodityTuple commodities = new CommodityTuple (0, 0, 0);

						clientCatanManager.players [i].receiveResources (resources);
						//OnTradeWithBank(i, true, new AssetTuple(resources, commodities));
					}
				}
			}
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
						OnTradeWithBank (playerNum, true, new AssetTuple (resources, new CommodityTuple (0, 0, 0)));
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

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			if (EventTransferManager.instance.setupPhase && i != clientCatanManager.currentPlayerTurn) {
				continue;
			} else {
				int[] tileIDs = clientCatanManager.boardManager.getTileIDsWithDiceValue (i, diceOutcome, true);

				for (int j = 0; j < tileIDs.Length; j++) {
					if (clientBoard.GameTiles [tileIDs [j]].canProduce ()) {
						print (clientCatanManager.players [i].playerName + " gets " + "1 " + GameAsset.getCommodityOfHex (clientBoard.GameTiles [tileIDs [j]].tileType));

						//ResourceTuple resources = new ResourceTuple (0, 0, 0, 0, 0);
						CommodityTuple commodities = clientCatanManager.resourceManager.getCommodityForTile (clientBoard.GameTiles [tileIDs [j]], 1);
						clientCatanManager.players [i].receiveCommodities (commodities);
						//OnTradeWithBank(i, true, new AssetTuple(resources, commodities));
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

			int robberTileID = clientBoard.robber.occupyingTile.id;
			int pirateTileID = clientBoard.pirate.occupyingTile.id;
			//clientBoard.robber.occupyingTile.occupier = null;
			//clientBoard.robber = null;
			//Destroy (clientBoard.robber.gameObject);

			GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
				0,
				robberTileID
			});
			GetComponent<PhotonView> ().RPC ("PlaceBoardPieces", PhotonTargets.All, new object[] {
				1,
				pirateTileID
			});
		}
	}

	[PunRPC]
	void PaintTile(int id, int diceValue, int tileTypeNum, bool islandLayer) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		if (clientBoard != null) {
			GameTile tileToPaint = clientBoard.GameTiles [id];
			tileToPaint.setTileType (tileTypeNum);
			tileToPaint.setDiceValue (diceValue);
			tileToPaint.atIslandLayer = islandLayer;
		}
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
	}

	[PunRPC]
	void PlaceBoardPieces(int boardPieceNum, int tileID) {
		GameBoard clientBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		if (clientBoard != null) {
			GameTile tileToPlace = clientBoard.GameTiles [tileID];

			// Board Piece Num:
			// 0 -> Move Robber
			// 1 -> Move Pirate
			// 2 -> Move Merchant
			switch (boardPieceNum) {
			case 0:
				Robber[] robbers = GameObject.FindObjectsOfType<Robber> ();
				if (robbers != null) {
					for (int i = 0; i < robbers.Length; i++) {
						Destroy (robbers [i].gameObject);
					}
				}
				clientBoard.robber = null;
				clientBoard.MoveRobber (tileID);
				break;
			case 1:
				Pirate[] pirates = GameObject.FindObjectsOfType<Pirate> ();
				if (pirates != null) {
					for (int i = 0; i < pirates.Length; i++) {
						Destroy (pirates [i].gameObject);
					}
				}
				clientBoard.pirate = null;
				clientBoard.MovePirate (tileID);
				break;
			case 2:
				Merchant[] merchants = GameObject.FindObjectsOfType<Merchant> ();
				if (merchants != null) {
					for (int i = 0; i < merchants.Length; i++) {
						Destroy (merchants [i].gameObject);
					}
				}
				clientBoard.merchant = null;
				clientBoard.MoveMerchant (tileID);
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
	void BuildIntersectionUnit(int userNum, int unitType, int intersectionID) {
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		GameBoard clientGameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();

		print (clientCatanManager.players [userNum].playerName + " builds a " + ((UnitType)unitType).ToString() + " on intersection #" + intersectionID);

		GameObject intersectionUnitGO = null;
		IntersectionUnit intersectionUnit = null;

		switch ((UnitType)unitType) {
		case UnitType.Settlement:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.Settlement));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
			break;
		case UnitType.City:
			intersectionUnitGO = Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.City));
			intersectionUnit = intersectionUnitGO.GetComponent<IntersectionUnit> ();
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

		intersectionUnit.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [userNum].playerColor;
		intersectionUnit.gameObject.SetActive (true);

		if (!EventTransferManager.instance.setupPhase) {
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

		GameObject cityGameObject = (GameObject)Instantiate (clientCatanManager.unitManager.GetPrefabOfType (UnitType.City));
		City newCity = cityGameObject.GetComponent<City> ();
		newCity.id = settlementToUpgrade.id;

		clientCatanManager.unitManager.unitsInPlay [settlementToUpgrade.id] = newCity;

		settlementToUpgrade.locationIntersection.occupier = newCity;
		newCity.locationIntersection = settlementToUpgrade.locationIntersection;

		clientCatanManager.players [playerNum].removeOwnedUnit (settlementToUpgrade, typeof(Settlement));
		clientCatanManager.players [playerNum].addOwnedUnit (newCity, typeof(City));
		newCity.owner = clientCatanManager.players [playerNum];

		newCity.transform.position = settlementToUpgrade.transform.position;
		newCity.transform.parent = settlementToUpgrade.transform.parent;
		newCity.transform.localScale = settlementToUpgrade.transform.localScale;

		newCity.GetComponentInChildren<Renderer> ().material.color = clientCatanManager.players [playerNum].playerColor;

		Destroy (settlementToUpgrade.gameObject);

		clientCatanManager.players [playerNum].spendAssets (clientCatanManager.resourceManager.getCostOfUnit (UnitType.City));
		clientCatanManager.boardManager.highlightUnitsWithColor (clientCatanManager.players [playerNum].getOwnedUnitsOfType (UnitType.Settlement), true, clientCatanManager.players [playerNum].playerColor);
		//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";

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
	void BuildEdgeUnit(int userNum, int unitType, int edgeID) {
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

		if (!EventTransferManager.instance.setupPhase) {
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
		EventTransferManager.instance.currentActiveButton = -1;
	}

	[PunRPC]
	void GenerateProgressCards(){
		GameObject clientcardsStackGO = Instantiate (ProgressCardsStackManagerPrefab);
		ProgressCardStackManager clientcards = clientcardsStackGO.GetComponent<ProgressCardStackManager> ();
		//master sets everyones card orders
		if (PhotonNetwork.isMasterClient) {
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
		}
	}
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
}

public enum MoveType {
	BuildSettlement = 0,
	BuildRoad,
	BuildCity,
	UpgradeSettlement,
	BuildShip,
	TradeBank,
	MoveShip
}

