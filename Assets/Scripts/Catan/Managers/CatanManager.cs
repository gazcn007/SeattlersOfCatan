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
	public bool boardLoaded;

	public bool[] readys;

	void Awake() {
		if (instance == null)
			instance = this;
	}

	// Use this for initialization
	void Start () {
		InitializeManagers ();

		players = LevelManager.instance.players;
		readys = new bool[PhotonNetwork.playerList.Length];

		setupPhase = true;
		waitingForPlayer = false;
		boardLoaded = false;

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

		boardLoaded = true;
	}

	public bool CheckIfEveryoneReady() {
		bool allReady = true;

		for (int i = 0; i < readys.Length; i++) {
			if (readys [i] == false) {
				allReady = false;
			}
		}

		return allReady;
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
		EventTransferManager.instance.OnBuildUnitForUser(UnitType.City, currentPlayerTurn, players[currentPlayerTurn].lastUnitSelectionId);
	}

	public void tradeWithBankAttempt(int resourceToGiveForOne) {
		bool canTrade = resourceManager.canTrade (players [currentPlayerTurn], resourceToGiveForOne);

		if (!canTrade) {
			print (players [currentPlayerTurn].playerName + " can not trade with bank for " + resourceToGiveForOne + ":1! Insufficient resources!");

			EventTransferManager.instance.OnOperationFailure ();
			return;
		}

		uiManager.tradePanel.gameObject.SetActive (true);
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
