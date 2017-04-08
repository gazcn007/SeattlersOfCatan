using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CatanManager : MonoBehaviour {

	public static CatanManager instance;

	public GameObject uiManagerPrefab;
	public GameObject resourceManagerPrefab;
	public GameObject unitManagerPrefab;
	public GameObject boardManagerPrefab;
	public UIManager uiManager;
	public ResourceManager resourceManager;
	public UnitManager unitManager;
	public BoardManager boardManager;

	public List<Player> players;
	public MetropolisOwners metropolisOwners;

	public int currentPlayerTurn;
	public int currentActiveButton;
	public bool setupPhase;
	public bool waitingForPlayer;

	void Awake() {
		if (instance == null)
			instance = this;
	}

	// Use this for initialization
	void Start () {
		InitializeManagers ();

		players = LevelManager.instance.players;
		metropolisOwners = new MetropolisOwners ();

		setupPhase = true;
		waitingForPlayer = false;

		StartCoroutine(settlePlayersOnBoard ());
		//StartCoroutine(waitForBoardSync());
		//EventTransferManager.instance.SignalReadyToEveryone(PhotonNetwork.player.ID);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Initializer Methods

	void InitializeManagers() {
		GameObject resourceManagerGO = Instantiate (resourceManagerPrefab, this.transform);
		GameObject unitManagerGO = Instantiate (unitManagerPrefab, this.transform);
		GameObject boardManagerGO = Instantiate (boardManagerPrefab, this.transform);


		resourceManager = GetComponentInChildren<ResourceManager> ();
		unitManager = GetComponentInChildren<UnitManager> ();
		boardManager = GetComponentInChildren<BoardManager> ();

		InitializeUI ();
	}

	void InitializeUI() {
		GameObject uiManagerGO = Instantiate (uiManagerPrefab);
		uiManager = uiManagerGO.GetComponent<UIManager> ();
		uiManager.offset = PhotonNetwork.player.ID - 1;
	}

	#endregion

	#region Setup Method

	IEnumerator settlePlayersOnBoard() {
		setupPhase = true;

		yield return StartCoroutine (waitForBoardSync ());
	}

	IEnumerator waitForBoardSync() {
		GameObject boardGO;
		GameBoard board = null;

		do {
			boardGO = GameObject.FindGameObjectWithTag ("Board");

			if(boardGO != null)
				board = boardGO.GetComponent<GameBoard> ();
			yield return new WaitForSeconds(1.0f);
		} while(board == null);

	}

	#endregion

	public IEnumerator buildIntersectionUnit(UnitType unitType) {
		Debug.Log ("Current turn is: " + players[currentPlayerTurn].playerName + " and my photon id is: " + PhotonNetwork.player.ID + " note -1 ");

		waitingForPlayer = true;
		List<Intersection> validIntersectionsToBuildList = boardManager.getValidIntersectionsForPlayer (players [currentPlayerTurn], unitType == UnitType.Knight);
		int[] validIntersectionsToBuild = boardManager.getValidIntersectionIDsForPlayer (players[currentPlayerTurn], unitType == UnitType.Knight);

		AssetTuple costOfUnit = resourceManager.getCostOfUnit (unitType);

		if (EventTransferManager.instance.setupPhase && costOfUnit == null) {
			handleBuildFailure ("costofunit is null, returning.", uiManager.uiButtons);
			yield break;
		}

		if (!EventTransferManager.instance.setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableAssets (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				handleBuildFailure ("Insufficient Resources to build this intersection unit!", uiManager.uiButtons);
				yield break;
			}
		}

		if (validIntersectionsToBuild.Length == 0) {
			handleBuildFailure ("No possible location to build this intersection unit!", uiManager.uiButtons);
			yield break;
		}

		//boardManager.highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);
		EventTransferManager.instance.OnHighlightForUser(0, currentPlayerTurn, true, validIntersectionsToBuild);
		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (validIntersectionsToBuildList));
		EventTransferManager.instance.OnBuildUnitForUser (unitType, currentPlayerTurn, players [currentPlayerTurn].lastIntersectionSelection.id, true, -1);
	}

	/*public IEnumerator buildIntersectionUnit(IntersectionUnit intersectionUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Intersection> validIntersectionsToBuildList = boardManager.getValidIntersectionsForPlayer (players[currentPlayerTurn]);
		int[] validIntersectionsToBuild = boardManager.getValidIntersectionIDsForPlayer (players[currentPlayerTurn]);
		ResourceTuple costOfUnit = resourceManager.getCostOfUnit (unitType);

		if (EventTransferManager.instance.setupPhase && costOfUnit == null) {
			handleBuildFailure ("costofunit is null, returning.", intersectionUnit, uiManager.uiButtons);
			yield break;
		}

		if (!EventTransferManager.instance.setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				handleBuildFailure ("Insufficient Resources to build this intersection unit!", intersectionUnit, uiManager.uiButtons);
				yield break;
			}
		}

		if (validIntersectionsToBuild.Length == 0) {
			handleBuildFailure ("No possible location to build this intersection unit!", intersectionUnit, uiManager.uiButtons);
			yield break;
		}

		//boardManager.highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);
		EventTransferManager.instance.OnHighlightForUser(0, currentPlayerTurn, true, validIntersectionsToBuild);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (validIntersectionsToBuildList));//, intersectionUnit));
		print (players [currentPlayerTurn].playerName + " builds a " + unitType.ToString() + " on intersection #" + players [currentPlayerTurn].lastIntersectionSelection.id);

		intersectionUnit.id = unitManager.unitID++;
		unitManager.unitsInPlay.Add (intersectionUnit.id, intersectionUnit);

		players [currentPlayerTurn].lastIntersectionSelection.occupier = intersectionUnit;
		intersectionUnit.locationIntersection = players [currentPlayerTurn].lastIntersectionSelection;
		players [currentPlayerTurn].addOwnedUnit(intersectionUnit, unitType);
		intersectionUnit.owner = players [currentPlayerTurn];

		intersectionUnit.transform.position = players [currentPlayerTurn].lastIntersectionSelection.transform.position;
		intersectionUnit.transform.parent = players [currentPlayerTurn].lastIntersectionSelection.transform;
		intersectionUnit.transform.localScale = intersectionUnit.transform.localScale * GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ().hexRadius;

		intersectionUnit.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		intersectionUnit.gameObject.SetActive (true);

		//unitsInPlay.Add (intersectionUnit.id, intersectionUnit);

		if (!EventTransferManager.instance.setupPhase) {
			players [currentPlayerTurn].spendResources (costOfUnit);

			//uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
		}

		boardManager.highlightAllIntersections(false);

		currentActiveButton = -1;
		waitingForPlayer = false;
	}*/

	public IEnumerator buildEdgeUnit(UnitType unitType, bool paid) {
		waitingForPlayer = true;
		List<Edge> validEdgesToBuildList = boardManager.getValidEdgesForPlayer (players[currentPlayerTurn], unitType == UnitType.Road);
		int[] validEdgesToBuild = boardManager.getValidEdgeIDsForPlayer (players[currentPlayerTurn], unitType == UnitType.Road);

		bool canBeShip = EventTransferManager.instance.setupPhase || players [currentPlayerTurn].playedRoadBuilding;

		if (canBeShip) {
			int[] validShipEdgesToBuild = boardManager.getValidEdgeIDsForPlayer (players[currentPlayerTurn], unitType == UnitType.Ship);
			List<Edge> validShipEdgesToBuildList =  boardManager.getValidEdgesForPlayer (players[currentPlayerTurn], unitType == UnitType.Ship);

			validEdgesToBuild = validEdgesToBuild.Union (validShipEdgesToBuild).ToArray();
			validEdgesToBuildList = validEdgesToBuildList.Union (validShipEdgesToBuildList).ToList ();
		}

		AssetTuple costOfUnit = resourceManager.getCostOfUnit (unitType);

		if (costOfUnit == null) {
			handleBuildFailure ("costofunit is null, returning.", uiManager.uiButtons);
			yield break;
		}

		if (!EventTransferManager.instance.setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableAssets (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				handleBuildFailure ("Insufficient Resources to build this edge unit!", uiManager.uiButtons);
				yield break;
			}
		}

		if (validEdgesToBuild.Length == 0) {
			handleBuildFailure ("No possible location to build this edge unit!", uiManager.uiButtons);
			yield break;
		}

		EventTransferManager.instance.OnHighlightForUser(1, currentPlayerTurn, true, validEdgesToBuild);
		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToBuildList));//, edgeUnit));//new Road(unitID++)));

		UnitType unitTypeToBuild = unitType;
		if (canBeShip && !(players [currentPlayerTurn].lastEdgeSelection.isLandEdge () || players [currentPlayerTurn].lastEdgeSelection.isShoreEdge ())) {
			unitTypeToBuild = UnitType.Ship;
		}

		EventTransferManager.instance.OnBuildUnitForUser(unitTypeToBuild, currentPlayerTurn, players [currentPlayerTurn].lastEdgeSelection.id, paid, -1);
	}

	public IEnumerator upgradeSettlement() {
		waitingForPlayer = true;
		List<Settlement> ownedSettlements = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Settlement).Cast<Settlement> ().ToList ();
		int[] ownedSettlementIDs = players [currentPlayerTurn].getOwnedUnitIDsOfType (UnitType.Settlement);
		AssetTuple costOfUnit = resourceManager.getCostOfUnit (UnitType.City);

		if (ownedSettlements.Count == 0) {
			handleBuildFailure("No settlements owned!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		if (!players [currentPlayerTurn].hasAvailableAssets (costOfUnit)) { 
			handleBuildFailure("Insufficient Resources to upgrade a settlement to a city!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		boardManager.highlightUnitsWithColor (ownedSettlements.Cast<Unit> ().ToList (), true, Color.black);
		//EventTransferManager.instance.OnHighlightForUser(2, currentPlayerTurn, true, ownedSettlementIDs);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedSettlements.Cast<Unit> ().ToList ()));
		EventTransferManager.instance.OnBuildUnitForUser(UnitType.City, currentPlayerTurn, players[currentPlayerTurn].lastUnitSelection.id, true, -1);
	}

	public IEnumerator upgradeCity(int metropolisType) {
		waitingForPlayer = true;
		List<City> ownedCities = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.City).Cast<City> ().ToList ();
		int[] ownedCityIDs = players [currentPlayerTurn].getOwnedUnitIDsOfType (UnitType.City);

		if (metropolisOwners.metropolisOwners [metropolisType] != null) {
			if (!players [currentPlayerTurn].canStealMetropolis (metropolisOwners.metropolisOwners [metropolisType], (CityImprovementType)metropolisType)) {
				handleBuildFailure("Can not steal metropolis of type " + (MetropolisType)metropolisType + " from " + metropolisOwners.metropolisOwners [metropolisType].playerName + "!", uiManager.uiButtons);
				//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
				yield break;
			}
		}

		// AssetTuple costOfUnit = resourceManager.getCostOfUnit (UnitType.City);
		if (!players [currentPlayerTurn].canBuildMetropolis ((CityImprovementType)metropolisType)) { 
			handleBuildFailure("City improvement level not eligible to build a metropolis!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		if (ownedCities.Count == 0) {
			handleBuildFailure("No cities owned!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		boardManager.highlightUnitsWithColor (ownedCities.Cast<Unit> ().ToList (), true, Color.black);
		//EventTransferManager.instance.OnHighlightForUser(2, currentPlayerTurn, true, ownedSettlementIDs);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedCities.Cast<Unit> ().ToList ()));
		EventTransferManager.instance.OnBuildUnitForUser(UnitType.Metropolis, currentPlayerTurn, players[currentPlayerTurn].lastUnitSelection.id, true, metropolisType);
	}


	public IEnumerator moveShip() {
		if (EventTransferManager.instance.shipMovedThisTurn) {
			handleBuildFailure ("Can not move ships twice per turn!", uiManager.uiButtons);
			yield break;
		}
		List<Ship> ownedShips = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Ship).Cast<Ship> ().ToList ();
		List<Ship> moveableShips = ownedShips.Where (ship => ship.canMove ()).Cast<Ship> ().ToList ();

		boardManager.highlightUnitsWithColor (moveableShips.Cast<Unit> ().ToList (), true, Color.black);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (moveableShips.Cast<Unit> ().ToList ()));
		boardManager.highlightUnitsWithColor (players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Ship), true, players [currentPlayerTurn].playerColor);

		List<Edge> validEdgesToMoveShip = boardManager.getValidEdgesForPlayerShipMove (players [currentPlayerTurn], players [currentPlayerTurn].lastUnitSelection as Ship);
		int[] validEdgeIDsToMoveShip = boardManager.getValidEdgeIDsForPlayerShipMove (players [currentPlayerTurn], players [currentPlayerTurn].lastUnitSelection as Ship);
		EventTransferManager.instance.OnHighlightForUser(1, currentPlayerTurn, true, validEdgeIDsToMoveShip);
		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToMoveShip));
		boardManager.highlightAllEdges(false);

		EventTransferManager.instance.OnMoveShipForUser (currentPlayerTurn, players [currentPlayerTurn].lastUnitSelection.id, players [currentPlayerTurn].lastEdgeSelection.id);
	}

	public IEnumerator activateKnight(bool paid) {
		waitingForPlayer = true;
		List<Knight> ownedKnights = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => !knight.isActive).ToList ();
		AssetTuple costOfActivation = resourceManager.getCostOf (MoveType.ActivateKnight);

		if (ownedKnights.Count == 0) {
			handleBuildFailure("No knights owned!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		if (paid && !players [currentPlayerTurn].hasAvailableAssets (costOfActivation)) { 
			handleBuildFailure("Insufficient Resources to activate a knight!", uiManager.uiButtons);
			//uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			yield break;
		}

		boardManager.highlightKnightsWithColor (ownedKnights.Cast<Unit> ().ToList (), true, Color.black);
		//EventTransferManager.instance.OnHighlightForUser(2, currentPlayerTurn, true, ownedSettlementIDs);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedKnights.Cast<Unit> ().ToList ()));
		EventTransferManager.instance.OnKnightActionForUser (MoveType.ActivateKnight, currentPlayerTurn, players [currentPlayerTurn].lastUnitSelection.id, -1, true, paid);
	}

	public IEnumerator promoteKnight(bool paid) {
		waitingForPlayer = true;
		List<Knight> ownedKnights = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => knight.rank != KnightRank.Mighty).ToList ();
		AssetTuple costOfActivation = resourceManager.getCostOfUnit (UnitType.Knight);

		if (ownedKnights.Count == 0) {
			handleBuildFailure("No promotable knights owned!", uiManager.uiButtons);
			yield break;
		}

		if (paid && !players [currentPlayerTurn].hasAvailableAssets (costOfActivation)) { 
			handleBuildFailure("Insufficient Resources to promote a knight!", uiManager.uiButtons);
			yield break;
		}

		List<Knight> promotableKnights = new List<Knight> ();
		for (int i = 0; i < ownedKnights.Count; i++) {
			if (players [currentPlayerTurn].unlockedFortress () || ownedKnights [i].rank != KnightRank.Strong) {
				promotableKnights.Add (ownedKnights [i]);
			}
		}

		if (promotableKnights.Count == 0) {
			handleBuildFailure("Cannot promote strong knights, politics city improvement level 3 not unlocked!", uiManager.uiButtons);
			yield break;
		}

		boardManager.highlightKnightsWithColor (promotableKnights.Cast<Unit> ().ToList (), true, Color.black);
		//EventTransferManager.instance.OnHighlightForUser(2, currentPlayerTurn, true, ownedSettlementIDs);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (promotableKnights.Cast<Unit> ().ToList ()));
		EventTransferManager.instance.OnKnightActionForUser (MoveType.PromoteKnight, currentPlayerTurn, players [currentPlayerTurn].lastUnitSelection.id, -1, true, paid);
	}

	public IEnumerator moveKnight(int knightID, bool forced) {
		waitingForPlayer = true;
		int knightToMoveID;
		int ownerID;

		List<Knight> ownedKnights = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => knight.isActive && !knight.actionPerformedThisTurn).ToList ();

		if (!forced) {
			if (ownedKnights.Count == 0) {
				handleBuildFailure ("No eligible knights to perform action owned!", uiManager.uiButtons);
				yield break;
			}

			boardManager.highlightKnightsWithColor (ownedKnights.Cast<Unit> ().ToList (), true, Color.black);
			yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedKnights.Cast<Unit> ().ToList ()));

			knightToMoveID = players [currentPlayerTurn].lastUnitSelection.id;
		} else {
			knightToMoveID = knightID;
		}


		Knight selectedKnight = unitManager.unitsInPlay [knightToMoveID] as Knight;
		ownerID = selectedKnight.owner.playerNumber - 1;
		List<Intersection> possibleMovementLocations = boardManager.getMoveableIntersectionsForKnight (selectedKnight);
		boardManager.highlightKnightsWithColor (players [ownerID].getOwnedUnitsOfType (UnitType.Knight), true, players [ownerID].playerColor);

		if (possibleMovementLocations.Count == 0) {
			if (forced) {
				EventTransferManager.instance.OnDestroyUnit(UnitType.Knight, knightToMoveID);
				EventTransferManager.instance.OnPlayerReady (ownerID, true);
				handleBuildFailure("No possible location to move for this knight! Knight is destroyed", uiManager.uiButtons);
			} else {
				handleBuildFailure("No possible location to move for this knight!", uiManager.uiButtons);
			}
			yield break;
		}

		selectedKnight.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.black;
		boardManager.highlightIntersectionsWithColor (possibleMovementLocations, true, players [ownerID].playerColor);

		yield return StartCoroutine (players [ownerID].makeIntersectionSelection (possibleMovementLocations));

		EventTransferManager.instance.OnKnightActionForUser (MoveType.MoveKnight, ownerID, selectedKnight.id, players [ownerID].lastIntersectionSelection.id, true, false);

		if (forced) {
			EventTransferManager.instance.OnPlayerReady (ownerID, true);
		}
	}

	public IEnumerator displaceKnight() {
		waitingForPlayer = true;
		List<Knight> ownedKnights = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => knight.isActive && !knight.actionPerformedThisTurn).ToList ();

		if (ownedKnights.Count == 0) {
			handleBuildFailure("No eligible knights to perform action owned!", uiManager.uiButtons);
			yield break;
		}

		boardManager.highlightKnightsWithColor (ownedKnights.Cast<Unit> ().ToList (), true, Color.black);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedKnights.Cast<Unit> ().ToList ()));

		Knight selectedKnight = unitManager.unitsInPlay [players [currentPlayerTurn].lastUnitSelection.id] as Knight;
		List<Knight> displaceableKnights = boardManager.getDisplaceableKnightsFor (selectedKnight);
		boardManager.highlightKnightsWithColor (players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight), true, players [currentPlayerTurn].playerColor);

		if (displaceableKnights.Count == 0) {
			handleBuildFailure("No possible opponent knight to displace!", uiManager.uiButtons);
			yield break;
		}

		selectedKnight.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.black;
		boardManager.highlightKnightsWithColor (displaceableKnights.Cast<Unit> ().ToList (), true, Color.black);

		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (displaceableKnights.Cast<Unit> ().ToList ()));

		EventTransferManager.instance.OnKnightActionForUser (MoveType.DisplaceKnight, currentPlayerTurn, selectedKnight.id, players [currentPlayerTurn].lastUnitSelection.id, true, false);
	}

	public IEnumerator chaseRobber() {
		waitingForPlayer = true;
		List<Knight> ownedKnights = players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight).Cast<Knight> ().Where (knight => knight.isActive && !knight.actionPerformedThisTurn).ToList ();
		List<Knight> eligibleKnights = new List<Knight> ();
		List<Intersection> eligibleIntersections = boardManager.getRobberPirateIntersections ();

		foreach (var knight in ownedKnights) {
			if (eligibleIntersections.Contains (knight.locationIntersection)) {
				eligibleKnights.Add (knight);
			}
		}
		
		if (eligibleKnights.Count == 0) {
			handleBuildFailure("No eligible knights to perform action owned!", uiManager.uiButtons);
			yield break;
		}

		if (eligibleIntersections.Count == 0) {
			handleBuildFailure("No game piece on board to chase away!", uiManager.uiButtons);
			yield break;
		}

		boardManager.highlightKnightsWithColor (eligibleKnights.Cast<Unit> ().ToList (), true, Color.black);
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (eligibleKnights.Cast<Unit> ().ToList ()));

		Knight selectedKnight = unitManager.unitsInPlay [players [currentPlayerTurn].lastUnitSelection.id] as Knight;

		boardManager.highlightKnightsWithColor (players [currentPlayerTurn].getOwnedUnitsOfType (UnitType.Knight), true, players [currentPlayerTurn].playerColor);
		selectedKnight.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.black;

		EventTransferManager.instance.OnKnightActionForUser (MoveType.ChaseRobber, currentPlayerTurn, selectedKnight.id, 0, true, false);
	}

	public IEnumerator receiveNResourceSelection(int playerNum, int numResourcesGained) {
		EventTransferManager.instance.waitingForPlayer = true;

		uiManager.fishResourceSelection ();
		bool selectionMade = false;
		uiManager.fishresourcepanel.gameObject.SetActive (true);

		while (!selectionMade) {
			if (!uiManager.fishresourcepanel.selectionMade) {
				yield return StartCoroutine (uiManager.fishresourcepanel.waitUntilButtonDown ());
			}
			if (uiManager.fishresourcepanel.selectionMade) {
				selectionMade = true;
			}
		}

		uiManager.fishresourcepanel.gameObject.SetActive (false);
		uiManager.fishresourcepanel.selectionMade = false;

		AssetTuple assetsToGain = GameAsset.getAssetOfIndex (CatanManager.instance.uiManager.fishresourcepanel.getSelection (), numResourcesGained);
		EventTransferManager.instance.OnTradeWithBank(playerNum, true, assetsToGain);

		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
	}

	public IEnumerator selectResourcesForPlayers(int numDelta, bool isPositiveDelta) {
		EventTransferManager.instance.waitingForPlayer = true;

		if (numDelta > 0) {
			uiManager.discardPanel.displayPanelForAssets (players [PhotonNetwork.player.ID - 1].getCurrentAssets (), numDelta, isPositiveDelta);
			yield return StartCoroutine (uiManager.discardPanel.waitUntilButtonDown ());

			//players [currentPlayerTurn].spendAssets (uiManager.discardPanel.discardTuple);
			EventTransferManager.instance.OnTradeWithBank(players [PhotonNetwork.player.ID - 1].playerNumber - 1, isPositiveDelta, uiManager.discardPanel.discardTuple);
			uiManager.discardPanel.gameObject.SetActive (false);
		}

		EventTransferManager.instance.waitingForPlayer = false;
		//EventTransferManager.instance.playerChecks [PhotonNetwork.player.ID - 1] = true;
		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
	}

	public IEnumerator moveGamePieceForCurrentPlayer(int gamePieceNum, bool remove, bool steal) {
		GamePiece gamePieceToMove;
		List<GameTile> eligibleTiles;

		if (gamePieceNum == 0) {
			gamePieceToMove = GameObject.FindObjectOfType<Robber> () as GamePiece;
			eligibleTiles = boardManager.getLandTiles (true);
		} else {
			gamePieceToMove = GameObject.FindObjectOfType<Pirate> () as GamePiece;
			eligibleTiles = boardManager.getOceanTiles (true);
		}
		if (remove) {
			if (gamePieceToMove != null) {
				EventTransferManager.instance.OnMoveGamePiece (gamePieceNum, gamePieceToMove.occupyingTile.id, remove);
			}
		} else {
			EventTransferManager.instance.waitingForPlayer = true;

			yield return StartCoroutine (players [currentPlayerTurn].makeGameTileSelection (eligibleTiles));
			EventTransferManager.instance.OnMoveGamePiece (gamePieceNum, players [currentPlayerTurn].lastGameTileSelection.id, remove);

			if (steal) {
				List<IntersectionUnit> opponentUnits = new List<IntersectionUnit> ();
				foreach (Intersection intersection in players [currentPlayerTurn].lastGameTileSelection.getIntersections()) {
					if (intersection.occupier != null && intersection.occupier.owner != players [currentPlayerTurn]) {
						opponentUnits.Add (intersection.occupier);
					}
				}

				List<Player> stealableOpponents = new List<Player> ();
				foreach (IntersectionUnit opponentUnit in opponentUnits) {
					if (!stealableOpponents.Contains (opponentUnit.owner) && !opponentUnit.owner.hasZeroAssets ()) {
						stealableOpponents.Add (opponentUnit.owner);
					}
				}

				if (stealableOpponents.Count == 1) { 
					// If you steal 2 things if you have a city, then the argument here would be 2 etc.
					AssetTuple randomStolenAsset = stealableOpponents [0].getRandomSufficientAsset (1);

					EventTransferManager.instance.OnTradeWithBank (stealableOpponents [0].playerNumber - 1, false, randomStolenAsset);
					EventTransferManager.instance.OnTradeWithBank (players [currentPlayerTurn].playerNumber - 1, true, randomStolenAsset);

				} else if (stealableOpponents.Count > 1) {
					if (players [PhotonNetwork.player.ID - 1].playedBishop) {
						for (int i = 0; i < stealableOpponents.Count; i++) {
							AssetTuple randomStolenAsset = stealableOpponents [uiManager.robberStealPanel.getSelection ()].getRandomSufficientAsset (1);
							EventTransferManager.instance.OnTradeWithBank (stealableOpponents [i].playerNumber - 1, false, randomStolenAsset);
							EventTransferManager.instance.OnTradeWithBank (players [currentPlayerTurn].playerNumber - 1, true, randomStolenAsset);

						}
					} else {
						uiManager.robberStealPanel.displayPanelForChoices (stealableOpponents);
						bool selectionMade = false;

						while (!selectionMade) {
							if (!uiManager.robberStealPanel.selectionMade) {
								yield return StartCoroutine (uiManager.robberStealPanel.waitUntilButtonDown ());
							}

							if (uiManager.robberStealPanel.selectionMade) {
								selectionMade = true;
							}
						}

						AssetTuple randomStolenAsset = stealableOpponents [uiManager.robberStealPanel.getSelection ()].getRandomSufficientAsset (1);
						EventTransferManager.instance.OnTradeWithBank (stealableOpponents [uiManager.robberStealPanel.getSelection ()].playerNumber - 1, false, randomStolenAsset);
						EventTransferManager.instance.OnTradeWithBank (players [currentPlayerTurn].playerNumber - 1, true, randomStolenAsset);

						uiManager.robberStealPanel.gameObject.SetActive (false);
					}
				}
			}

			EventTransferManager.instance.waitingForPlayer = false;
		}
	}

	public IEnumerator stealRandomResource(List<Player> stealableOpponents) {
		if (stealableOpponents.Count == 1) { 
			// If you steal 2 things if you have a city, then the argument here would be 2 etc.
			AssetTuple randomStolenAsset = stealableOpponents [0].getRandomSufficientAsset (1);

			EventTransferManager.instance.OnTradeWithBank(stealableOpponents [0].playerNumber - 1, false, randomStolenAsset);
			EventTransferManager.instance.OnTradeWithBank (players [currentPlayerTurn].playerNumber - 1, true, randomStolenAsset);

		} else if (stealableOpponents.Count > 1) {
			uiManager.robberStealPanel.displayPanelForChoices (stealableOpponents);
			bool selectionMade = false;

			while (!selectionMade) {
				if (!uiManager.robberStealPanel.selectionMade) {
					yield return StartCoroutine (uiManager.robberStealPanel.waitUntilButtonDown());
				}

				if (uiManager.robberStealPanel.selectionMade) {
					selectionMade = true;
				}
			}

			AssetTuple randomStolenAsset = stealableOpponents [uiManager.robberStealPanel.getSelection()].getRandomSufficientAsset (1);
			EventTransferManager.instance.OnTradeWithBank(stealableOpponents [uiManager.robberStealPanel.getSelection()].playerNumber - 1, false, randomStolenAsset);
			EventTransferManager.instance.OnTradeWithBank (players [currentPlayerTurn].playerNumber - 1, true, randomStolenAsset);

			uiManager.robberStealPanel.gameObject.SetActive (false);
		}
	}

	public IEnumerator stealRessourcesWedding(int sendto) {
		EventTransferManager.instance.waitingForPlayer = true;

		uiManager.discardPanel.displayPanelForAssets (players [PhotonNetwork.player.ID - 1].getCurrentAssets (), 2, false);
		yield return StartCoroutine (uiManager.discardPanel.waitUntilButtonDown ());

		EventTransferManager.instance.OnTradeWithBank(players [PhotonNetwork.player.ID - 1].playerNumber - 1, false, uiManager.discardPanel.discardTuple);
		EventTransferManager.instance.OnTradeWithBank(sendto, true, uiManager.discardPanel.discardTuple);
		uiManager.discardPanel.gameObject.SetActive (false);

		EventTransferManager.instance.waitingForPlayer = false;
		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
	}
	
	public void tradeWithBankAttempt(int resourceToGiveForOne) {
		/*bool canTrade = resourceManager.canTrade (players [currentPlayerTurn], resourceToGiveForOne);

		if (!canTrade) {
			print (players [currentPlayerTurn].playerName + " can not trade with bank for " + resourceToGiveForOne + ":1! Insufficient resources!");

			EventTransferManager.instance.OnOperationFailure ();
			return;
		}*/

		uiManager.tradePanel.gameObject.SetActive (true);
	}

	public void tradeWithPlayerAttempt(int resourceToGiveForOne) {
		bool canTrade = resourceManager.canTrade (players [currentPlayerTurn], resourceToGiveForOne);

		if (!canTrade) {
			print (players [currentPlayerTurn].playerName + " can not trade with any player due to insufficient resources!");

			EventTransferManager.instance.OnOperationFailure ();
			return;
		}

		List<Player> opponents = new List<Player> ();

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
			opponents.Add (players [i]);
		}
		opponents.Remove (players [currentPlayerTurn]);
		uiManager.tradePlayerPanel.OpenPanel (opponents, players [currentPlayerTurn].assets);
	}

	void handleBuildFailure(string message, Button[] cancelledButton) {
		Debug.Log (message);
		waitingForPlayer = false;
		currentActiveButton = -1;

		uiManager.notificationpanel.gameObject.SetActive(true);
		uiManager.notificationtext.text = message;
		// SEND MESSAGE TO CANCELLED BUTTON TO CANCEL ITS HIGHLIGHT
		EventTransferManager.instance.OnOperationFailure ();
		//Destroy (failedUnit.gameObject);
	}

	void handleBuildFailure(string errorMessage, Unit failedUnit, Button[] cancelledButton) {
		Debug.Log (errorMessage);
		waitingForPlayer = false;
		currentActiveButton = -1;
		// SEND MESSAGE TO CANCELLED BUTTON TO CANCEL ITS HIGHLIGHT
		unitManager.removeUnitFromGame (failedUnit);
		EventTransferManager.instance.OnOperationFailure ();
		//Destroy (failedUnit.gameObject);
	}

	public void endTurn() {
		//if (!setupPhase) {
			//if (!waitingForPlayer) {
				currentPlayerTurn = (currentPlayerTurn + 1) % PhotonNetwork.playerList.Length;
				//destroyCancelledUnits ();
			//}
		//}
	}

	public void SetCurrentTurn(int currentTurn) {
		currentPlayerTurn = currentTurn % PhotonNetwork.playerList.Length;
	}

	public void CancelCommand() {
		StopAllCoroutines ();
	}

}
