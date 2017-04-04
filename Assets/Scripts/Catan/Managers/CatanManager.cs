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
		List<Intersection> validIntersectionsToBuildList = boardManager.getValidIntersectionsForPlayer (players[currentPlayerTurn]);
		int[] validIntersectionsToBuild = boardManager.getValidIntersectionIDsForPlayer (players[currentPlayerTurn]);
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
		EventTransferManager.instance.OnBuildUnitForUser (unitType, currentPlayerTurn, players [currentPlayerTurn].lastIntersectionSelection.id);
	}

	public IEnumerator buildIntersectionUnit(IntersectionUnit intersectionUnit, System.Type unitType) {
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
	}

	public IEnumerator buildEdgeUnit(UnitType unitType) {
		waitingForPlayer = true;
		List<Edge> validEdgesToBuildList = boardManager.getValidEdgesForPlayer (players[currentPlayerTurn], unitType == UnitType.Road);
		int[] validEdgesToBuild = boardManager.getValidEdgeIDsForPlayer (players[currentPlayerTurn], unitType == UnitType.Road);
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
		if (EventTransferManager.instance.setupPhase && !(players [currentPlayerTurn].lastEdgeSelection.isLandEdge () || players [currentPlayerTurn].lastEdgeSelection.isShoreEdge ())) {
			unitTypeToBuild = UnitType.Ship;
		}

		EventTransferManager.instance.OnBuildUnitForUser(unitTypeToBuild, currentPlayerTurn, players [currentPlayerTurn].lastEdgeSelection.id);
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
		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedSettlements.Cast<Unit> ().ToList ()));//new Road(unitID++)));
		EventTransferManager.instance.OnBuildUnitForUser(UnitType.City, currentPlayerTurn, players[currentPlayerTurn].lastUnitSelection.id);
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

	public IEnumerator discardResourcesForPlayers() {
		EventTransferManager.instance.waitingForPlayer = true;
		int numDiscards = players [PhotonNetwork.player.ID - 1].getNumDiscardsNeeded ();

		if (numDiscards > 0) {
			uiManager.discardPanel.displayPanelForAssets (players [PhotonNetwork.player.ID - 1].getCurrentAssets (), numDiscards);
			yield return StartCoroutine (uiManager.discardPanel.waitUntilButtonDown ());

			//players [currentPlayerTurn].spendAssets (uiManager.discardPanel.discardTuple);
			EventTransferManager.instance.OnTradeWithBank(players [PhotonNetwork.player.ID - 1].playerNumber - 1, false, uiManager.discardPanel.discardTuple);
			uiManager.discardPanel.gameObject.SetActive (false);
		}

		EventTransferManager.instance.waitingForPlayer = false;
		//EventTransferManager.instance.playerChecks [PhotonNetwork.player.ID - 1] = true;
		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
	}

	public IEnumerator moveRobberForCurrentPlayer() {
		EventTransferManager.instance.waitingForPlayer = true;

		yield return StartCoroutine (players [currentPlayerTurn].makeGameTileSelection (boardManager.getLandTiles(true)));
		EventTransferManager.instance.OnMoveGamePiece (0, players [currentPlayerTurn].lastGameTileSelection.id);

		List<IntersectionUnit> opponentUnits = new List<IntersectionUnit> ();
		foreach (Intersection intersection in players [currentPlayerTurn].lastGameTileSelection.getIntersections()) {
			if (intersection.occupier != null && intersection.occupier.owner != players [currentPlayerTurn]) {
				opponentUnits.Add (intersection.occupier);
			}
		}

		List<Player> stealableOpponents = new List<Player> ();
		foreach (IntersectionUnit opponentUnit in opponentUnits) {
			if (!stealableOpponents.Contains (opponentUnit.owner) && !opponentUnit.owner.hasZeroAssets()) {
				stealableOpponents.Add (opponentUnit.owner);
			}
		}

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

		EventTransferManager.instance.waitingForPlayer = false;
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

		List<Player> opponents = new List<Player> (players);
		opponents.Remove(players[currentPlayerTurn]);
		uiManager.tradePlayerPanel.OpenPanel (opponents, players [currentPlayerTurn].assets);
	}

	void handleBuildFailure(string errorMessage, Button[] cancelledButton) {
		Debug.Log (errorMessage);
		waitingForPlayer = false;
		currentActiveButton = -1;
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


}
