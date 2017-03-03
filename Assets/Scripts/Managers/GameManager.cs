using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[Range(10, 25)]
	public static int victoryPointsToWinGame = 12;

	public List<Player> players;

	private Canvas canvas;
	private Text[] texts;
	private Button[] uiButtons;
	private TradePanel tradePanel;
	private RobberStealPanel robberStealPanel;
	private PlayerHUD playerHUD;
	private OpponentHUD opponent1HUD;
	private OpponentHUD opponent2HUD;
	private OpponentHUD opponent3HUD;

	private GameBoard gameBoard;
	private BoardGenerator boardGenerator;
	private PrefabManager prefabManager;
	private ResourceDistributionManager resourceManager;
	private NumericDie redDie;
	private NumericDie yellowDie;

	private int currentPlayerTurn = 0;
	private int currentActiveButton = -1;
	private bool waitingForPlayer;
	private static bool setupPhase;

	private int unitID = 0;
	private Dictionary<int, Unit> unitsInPlay;

	// Use this for initialization
	void Start () {
		initializeGameManager ();

		addPlayers ();
		StartCoroutine(settlePlayersOnBoard ());
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (players [currentPlayerTurn].playerName + "'s turn.");
		//players[currentPlayerTurn].playTurn();
		updateCanvas ();

		if (!setupPhase && currentActiveButton == -1) {
			if (Input.GetKeyDown (KeyCode.Mouse1)) {
				//repaint board
				boardGenerator.paintBoard();
			}
		}
	}

	#region Class Initializer Methods

	void initializeGameManager() {
		prefabManager = GetComponent<PrefabManager> ();
		boardGenerator = GameObject.FindGameObjectWithTag ("Board Generator").GetComponent<BoardGenerator>();
		canvas = GameObject.FindObjectOfType<Canvas> ();
		resourceManager = GetComponent<ResourceDistributionManager> ();

		initializeUI ();

		boardGenerator.GenerateBoard ();

		gameBoard = boardGenerator.GetComponentInChildren<GameBoard>();

		players = new List<Player>(4);
		unitsInPlay = new Dictionary<int, Unit> ();
	}

	void initializeUI() {
		GameObject[] textObjects = GameObject.FindGameObjectsWithTag ("DebugUIText");
		GameObject currentTurnTextObject = GameObject.FindGameObjectWithTag ("CurrentTurnText");

		tradePanel = canvas.transform.FindChild("TradePanel").gameObject.GetComponent<TradePanel>();
		robberStealPanel = canvas.transform.FindChild("RobberStealPanel").gameObject.GetComponent<RobberStealPanel>();
		playerHUD = canvas.transform.FindChild("PlayerHUD").gameObject.GetComponent<PlayerHUD>();

		opponent1HUD = canvas.transform.FindChild("opponent1HUD").gameObject.GetComponent<OpponentHUD>();
		opponent2HUD = canvas.transform.FindChild("opponent2HUD").gameObject.GetComponent<OpponentHUD>();
		opponent3HUD = canvas.transform.FindChild("opponent3HUD").gameObject.GetComponent<OpponentHUD>();

		texts = new Text[textObjects.Length + 1];
		for (int i = 0; i < textObjects.Length; i++) {
			texts [i] = textObjects [textObjects.Length - 1 - i].GetComponent<Text> ();
		}
		texts [texts.Length - 1] = currentTurnTextObject.GetComponent<Text> ();

		uiButtons = canvas.GetComponentsInChildren<Button> ();
		uiButtons[0].onClick.AddListener (endTurn);
		uiButtons[1].onClick.AddListener (buildSettlementEvent);
		uiButtons[2].onClick.AddListener (buildRoadEvent);
		uiButtons[3].onClick.AddListener (diceRollEvent);
		uiButtons[4].onClick.AddListener (upgradeSettlementEvent);
		uiButtons[5].onClick.AddListener (buildShipEvent);
		uiButtons [6].onClick.AddListener (tradeWithBankEvent);

		tradePanel.buttonsOnPanel[0].onClick.AddListener(tradeDone);
		tradePanel.buttonsOnPanel[1].onClick.AddListener(tradeCancelled);
	}

	#endregion

	#region Game Initializer Methods

	void addPlayers() {
		GameObject player1Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player1 = player1Object.GetComponent<Player> ();
		player1.playerColor = Color.blue;
		player1.playerName = "Nehir";
		player1.playerNumber = 1;
		playerHUD.setPlayer (player1);

		GameObject player2Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player2 = player2Object.GetComponent<Player> ();
		player2.playerColor = Color.red;
		player2.playerName = "Omer";
		player2.playerNumber = 2;
		opponent1HUD.setPlayer(player2);

		GameObject player3Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player3 = player3Object.GetComponent<Player> ();
		player3.playerColor = Color.yellow;
		player3.playerName = "Milosz";
		player3.playerNumber = 3;
		opponent2HUD.setPlayer(player3);

		GameObject player4Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player4 = player4Object.GetComponent<Player> ();
		player4.playerColor = Color.green;
		player4.playerName = "Carl";
		player4.playerNumber = 4;
		opponent3HUD.setPlayer(player4);

		players.Add (player1);
		players.Add (player2);
		players.Add (player3);
		players.Add (player4);
	}

	IEnumerator settlePlayersOnBoard() {
		setupPhase = true;

		for (currentPlayerTurn = 0; currentPlayerTurn < players.Count; currentPlayerTurn++) {
			print (players [currentPlayerTurn].playerName + "'s turn.");
			print (players [currentPlayerTurn].playerName + " must build a settlement on an intersection.");
			yield return StartCoroutine (buildSettlement ());
			print (players [currentPlayerTurn].playerName + " must build a road on an edge.");
			yield return StartCoroutine (buildRoad ());
		}

		for (currentPlayerTurn = players.Count - 1; currentPlayerTurn >= 0; currentPlayerTurn--) {
			print (players [currentPlayerTurn].playerName + "'s turn.");
			print (players [currentPlayerTurn].playerName + " must build a city on an intersection.");
			yield return StartCoroutine (buildCity ());
			//resourceCollectionEvent ();
			diceRollResolveEvent();
			print (players [currentPlayerTurn].playerName + " must build a road on an edge.");
			yield return StartCoroutine (buildRoad ());
		}

		currentPlayerTurn = 0;
		setupPhase = false;

	}

	#endregion

	#region UI

	void updateCanvas() {
		for (int i = 0; i < players.Count; i++) {
			texts [i].text = "Player " + players [i].playerNumber + "\n" + players [i].playerName 
				+ " \tVP= " + players[i].victoryPoints + "\nUnits Count: " + players [i].getNumUnits() + "\n<";

			foreach (var key in players[i].getCurrentResources().resourceTuple) {
				texts [i].text += key.Value + ", ";
			}
			texts [i].text += ">\n<";

			foreach (var key in players[i].getCurrentCommodities().commodityTuple) {
				texts [i].text += key.Value + ", ";
			}
			texts [i].text += ">";
		}

		texts [4].text = "Current Turn: " + players [currentPlayerTurn].playerName;
	}

	#endregion

	#region Game Events & Button Events

	void buildRoadEvent() {
		int buttonId = 1;
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [2].GetComponentInChildren<Text> ().text = "Cancel";
				currentActiveButton = buttonId;

				StartCoroutine (buildRoad ());
			} else {
				if (currentActiveButton == buttonId) {
					StopAllCoroutines ();

					highlightAllEdges (false);
					waitingForPlayer = false;

					currentActiveButton = -1;
					uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
				}
			}
		}
	}

	void buildShipEvent() {
		int buttonId = 2;
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [5].GetComponentInChildren<Text> ().text = "Cancel";
				currentActiveButton = buttonId;

				StartCoroutine (buildShip ());
			} else {
				if (currentActiveButton == buttonId) {
					StopAllCoroutines ();

					highlightAllEdges (false);
					waitingForPlayer = false;

					currentActiveButton = -1;
					uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
				}
			}
		}
	}

	void buildSettlementEvent() {
		int buttonId = 3;
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [1].GetComponentInChildren<Text> ().text = "Cancel";
				currentActiveButton = buttonId;

				StartCoroutine (buildSettlement ());
			} else {
				if (currentActiveButton == buttonId) {
					StopAllCoroutines ();

					highlightAllIntersections (false);
					waitingForPlayer = false;

					currentActiveButton = -1;
					uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
				}
			}
		}
	}

	void upgradeSettlementEvent() {
		int buttonId = 4;
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [4].GetComponentInChildren<Text> ().text = "Cancel";
				currentActiveButton = buttonId;

				StartCoroutine (upgradeSettlement ());
			} else {
				if (currentActiveButton == buttonId) {
					StopAllCoroutines ();

					highlightUnitsWithColor (players [currentPlayerTurn].getOwnedUnitsOfType (typeof(Settlement)), true, players [currentPlayerTurn].playerColor);
					waitingForPlayer = false;

					currentActiveButton = -1;
					uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
				}
			}
		}
	}

	void tradeWithBankEvent() {
		int buttonId = 5;
		if (!setupPhase) {
			if (!waitingForPlayer) {
				currentActiveButton = buttonId;
				StartCoroutine (tradeXForOne (4));
			}
		}
	}

	/*void tradeDone() {
		bool successful = false;
		//int choice = tradePanel.getTradeChoiceInt ();
		ResourceTuple resourcesToGiveToBank = new ResourceTuple ();
		CommodityTuple commoditiesToGiveToBank = new CommodityTuple ();

		Tuple<ResourceType, CommodityType> spending = GameAsset.getProductionAssetsOfIndex (tradePanel.getTradeChoiceInt ());
		Tuple<ResourceType, CommodityType> receiving = GameAsset.getProductionAssetsOfIndex (tradePanel.getReceiveChoiceInt ());

		if (tradePanel.getTradeChoiceInt() < 5) {
			resourcesToGiveToBank.addResourceWithType (spending.first, 4);
		} else {
			commoditiesToGiveToBank.addCommodityWithType(spending.second, 4);
		}

		if (players [currentPlayerTurn].hasAvailableResources (resourcesToGiveToBank) && players [currentPlayerTurn].hasAvailableCommodities (commoditiesToGiveToBank)) {
			players [currentPlayerTurn].spendResources (resourcesToGiveToBank);
			players [currentPlayerTurn].spendCommodities (commoditiesToGiveToBank);

			ResourceTuple resourceToReceive = new ResourceTuple ();
			CommodityTuple commodityToReceive = new CommodityTuple ();

			if (tradePanel.getReceiveChoiceInt () < 5) {
				resourceToReceive.addResourceWithType (receiving.first, 1);
			} else {
				commodityToReceive.addCommodityWithType (receiving.second, 1);
			}

			players [currentPlayerTurn].receiveResources (resourceToReceive);
			players [currentPlayerTurn].receiveCommodities (commodityToReceive);

			//print (players [currentPlayerTurn].playerName + " gives 4 " + tradePanel.getTradeChoice ().ToString () + " to the bank and receives 1 " + tradePanel.getReceiveChoice ());
			print (players [currentPlayerTurn].playerName + " gives 4 " + spending.ToString () + " to the bank and receives 1 " + receiving.ToString());

			currentActiveButton = -1;
			tradePanel.hideErrorText ();
			tradePanel.gameObject.SetActive (false);
			waitingForPlayer = false;
		} else {
			print ("Insufficient resources! Please try again...");
			tradePanel.showNotEnoughError (tradePanel.getTradeChoiceInt ());
		}
	}*/

	void tradeDone() {
		AssetTuple assetsToSpend = GameAsset.getAssetOfIndex (tradePanel.getTradeChoiceInt (), 4);
		AssetTuple assetsToReceive = GameAsset.getAssetOfIndex (tradePanel.getReceiveChoiceInt (), 1);

		if (players [currentPlayerTurn].hasAvailableAssets (assetsToSpend)) { 
			players [currentPlayerTurn].spendAssets (assetsToSpend);
			players [currentPlayerTurn].receiveAssets (assetsToReceive);
			//print (players [currentPlayerTurn].playerName + " gives 4 " + tradePanel.getTradeChoiceInt ().ToString () + " to the bank and receives 1 " + assetsToReceive.ToString());

			currentActiveButton = -1;
			tradePanel.hideErrorText ();
			tradePanel.gameObject.SetActive (false);
			waitingForPlayer = false;
		} else {
			print ("Insufficient resources! Please try again...");
			tradePanel.showNotEnoughError (tradePanel.getTradeChoiceInt ());
		}

	}

	void tradeCancelled() {
		StopAllCoroutines();

		currentActiveButton = -1;
		tradePanel.hideErrorText ();
		tradePanel.gameObject.SetActive (false);
		waitingForPlayer = false;
	}

	void endTurn() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				currentPlayerTurn = (currentPlayerTurn + 1) % players.Count;
				destroyCancelledUnits ();
			}
		}
	}

	void diceRollEvent() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				//StartCoroutine (diceRollPhase ());
				//resourceCollectionEvent ();
				diceRollResolveEvent();
			}
		}
	}

	/*IEnumerator */void diceRollPhase() {
		//yield return null; //StartCoroutine (resourceManager.diceRollEvent ());
		resourceManager.diceRollEvent();
	}

	/*void makePlayerAction(PlayerAction actionType) {
		if (!waitingForPlayer) {
			StartCoroutine (actionType ());
		}
	}*/

	#endregion

	#region Build Methods for Specific Units

	IEnumerator buildSettlement() {
		GameObject intersectionUnitGameObject = (GameObject)Instantiate (prefabManager.settlementPrefab);
		Settlement settlement = intersectionUnitGameObject.GetComponent<Settlement> ();

		settlement.id = unitID++;
		settlement.gameObject.SetActive (false);
		unitsInPlay.Add (settlement.id, settlement);

		yield return StartCoroutine (buildIntersectionUnit (settlement, typeof(Settlement)));
	}

	IEnumerator buildCity() {
		GameObject intersectionUnitGameObject = (GameObject)Instantiate (prefabManager.cityPrefab);
		City city = intersectionUnitGameObject.GetComponent<City> ();

		city.id = unitID++;
		city.gameObject.SetActive (false);
		unitsInPlay.Add (city.id, city);

		yield return StartCoroutine (buildIntersectionUnit (city, typeof(City)));
	}

	IEnumerator buildRoad() {
		GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.roadPrefab);
		Road road = tradeUnitGameObject.GetComponent<Road> ();

		road.id = unitID++;
		road.gameObject.SetActive (false);
		unitsInPlay.Add (road.id, road);

		yield return StartCoroutine (buildTradeUnit (road, typeof(Road)));

	}

	IEnumerator buildShip() {
		GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.shipPrefab);
		Ship ship = tradeUnitGameObject.GetComponent<Ship> ();

		ship.id = unitID++;
		ship.gameObject.SetActive (false);
		unitsInPlay.Add (ship.id, ship);

		yield return StartCoroutine (buildTradeUnit (ship, typeof(Ship)));
	}

	#endregion

	#region Player Input Events

	IEnumerator upgradeSettlement() {
		waitingForPlayer = true;
		List<Settlement> ownedSettlements = players [currentPlayerTurn].getOwnedUnitsOfType (typeof(Settlement)).Cast<Settlement> ().ToList ();
		//List<Unit> ownedSettlements = players [currentPlayerTurn].getOwnedUnitsOfType (typeof(Settlement));
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (typeof(City));

		if (ownedSettlements.Count == 0) {
			print ("No settlements owned!");
			uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			currentActiveButton = -1;
			waitingForPlayer = false;
			yield break;
		}

		if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
			print ("Insufficient Resources to upgrade a settlement to a city!");
			uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			currentActiveButton = -1;
			waitingForPlayer = false;
			yield break;
		}

		highlightUnitsWithColor (ownedSettlements.Cast<Unit> ().ToList (), true, Color.black);

		yield return StartCoroutine (players [currentPlayerTurn].makeUnitSelection (ownedSettlements.Cast<Unit> ().ToList ()));//new Road(unitID++)));

		Settlement settlementToUpgrade = (Settlement)unitsInPlay [players [currentPlayerTurn].lastUnitSelectionId];
		//print ("Selected settlement has id#: " + selection.id + " and is owned by " + selection);
		print("Found settlement with id#: " + settlementToUpgrade.id + ". Residing on intersection id#: " + settlementToUpgrade.locationIntersection.id);

		GameObject cityGameObject = (GameObject)Instantiate (prefabManager.cityPrefab);
		City newCity = cityGameObject.GetComponent<City> ();
		newCity.id = settlementToUpgrade.id;

		unitsInPlay [settlementToUpgrade.id] = newCity;

		settlementToUpgrade.locationIntersection.occupier = newCity;
		newCity.locationIntersection = settlementToUpgrade.locationIntersection;

		players [currentPlayerTurn].removeOwnedUnit (settlementToUpgrade, typeof(Settlement));
		players [currentPlayerTurn].addOwnedUnit (newCity, typeof(City));
		newCity.owner = players [currentPlayerTurn];

		newCity.transform.position = settlementToUpgrade.transform.position;
		newCity.transform.parent = settlementToUpgrade.transform.parent;
		newCity.transform.localScale = settlementToUpgrade.transform.localScale;

		newCity.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;

		Destroy (settlementToUpgrade.gameObject);

		players [currentPlayerTurn].spendResources (costOfUnit);
		highlightUnitsWithColor (ownedSettlements.Cast<Unit>().ToList(), true, players[currentPlayerTurn].playerColor);
		uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";

		currentActiveButton = -1;
		waitingForPlayer = false;
	}

	IEnumerator tradeXForOne(int resourceToGiveForOne) {
		waitingForPlayer = true;

		ResourceTuple currentResources = players [currentPlayerTurn].getCurrentResources ();
		bool canTrade = false;

		foreach (var pair in currentResources.resourceTuple) {
			if (pair.Value >= resourceToGiveForOne) {
				canTrade = true;
			}
		}

		if (!canTrade) {
			print (players [currentPlayerTurn].playerName + " can not trade with bank for " + resourceToGiveForOne + ":1! Insufficient resources!");
			waitingForPlayer = false;
			currentActiveButton = -1;
			yield break;
		}

		tradePanel.gameObject.SetActive (true);

		//TODO:
		// Set which type is possible to be selected, map slider value to text, etc. (must also do this in update)
		// Then in done, can check if it works for player's current number of resources. or can check this here too...

	}

	#endregion

	#region Generic Build Methods

	IEnumerator buildTradeUnit(TradeUnit tradeUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Edge> validEdgesToBuild = getValidEdgesForPlayer (players[currentPlayerTurn], unitType == typeof(Road));
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (unitType);
		System.Type newType = unitType;

		if (costOfUnit == null) {
			print ("costofunit is null, returning.");
			waitingForPlayer = false;
			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
			currentActiveButton = -1;
			Destroy (tradeUnit.gameObject);
			removeUnitFromGame (tradeUnit);
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build this trade unit!");
				waitingForPlayer = false;
				uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
				uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
				currentActiveButton = -1;
				Destroy (tradeUnit.gameObject);
				removeUnitFromGame (tradeUnit);
				yield break;
			}
		}

		if (validEdgesToBuild.Count == 0) {
			print ("No possible location to build this trade unit!");
			Destroy (tradeUnit.gameObject);
			waitingForPlayer = false;
			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
			currentActiveButton = -1;
			removeUnitFromGame (tradeUnit);
			yield break;
		}

		highlightEdgesWithColor (validEdgesToBuild, true, players [currentPlayerTurn].playerColor);
		//highlightEdges (validEdgesToBuild, true);

		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToBuild));//, tradeUnit));//new Road(unitID++)));

		print (players [currentPlayerTurn].playerName + " builds a " + unitType.ToString() + " on edge #" + players [currentPlayerTurn].lastEdgeSelection.id);

		if (setupPhase && !(players [currentPlayerTurn].lastEdgeSelection.isLandEdge() || players [currentPlayerTurn].lastEdgeSelection.isShoreEdge())) {
			GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.shipPrefab);
			Ship replacedShip = tradeUnitGameObject.GetComponent<Ship> ();
			replacedShip.id = tradeUnit.id;
			removeUnitFromGame (tradeUnit);
			tradeUnit = replacedShip;
			unitsInPlay.Add (tradeUnit.id, tradeUnit);
			newType = typeof(Ship);
		}

		players [currentPlayerTurn].lastEdgeSelection.occupier = tradeUnit;
		tradeUnit.locationEdge = players [currentPlayerTurn].lastEdgeSelection;
		players [currentPlayerTurn].addOwnedUnit (tradeUnit, newType);
		tradeUnit.owner = players [currentPlayerTurn];

		tradeUnit.transform.position = players [currentPlayerTurn].lastEdgeSelection.transform.position;
		tradeUnit.transform.rotation = players [currentPlayerTurn].lastEdgeSelection.transform.rotation;
		tradeUnit.transform.localScale = players [currentPlayerTurn].lastEdgeSelection.transform.localScale;
		tradeUnit.transform.parent = players [currentPlayerTurn].lastEdgeSelection.transform;

		tradeUnit.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		tradeUnit.gameObject.SetActive (true);

		if (!setupPhase) {
			players [currentPlayerTurn].spendResources (costOfUnit);

			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
		}
		highlightAllEdges(false);
		currentActiveButton = -1;

		waitingForPlayer = false;
	}

	IEnumerator buildIntersectionUnit(IntersectionUnit intersectionUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Intersection> validIntersectionsToBuild = getValidIntersectionsForPlayer (players[currentPlayerTurn]);
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (unitType);

		if (!setupPhase && costOfUnit == null) {
			print ("costofunit is null, returning.");
			Destroy (intersectionUnit.gameObject);
			waitingForPlayer = false;
			uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
			currentActiveButton = -1;
			removeUnitFromGame (intersectionUnit);
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build this intersection unit!");
				Destroy (intersectionUnit.gameObject);
				waitingForPlayer = false;
				uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
				currentActiveButton = -1;
				removeUnitFromGame (intersectionUnit);
				yield break;
			}
		}

		if (validIntersectionsToBuild.Count == 0) {
			print ("No possible location to build this intersection unit!");
			Destroy (intersectionUnit.gameObject);
			waitingForPlayer = false;
			uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
			currentActiveButton = -1;
			removeUnitFromGame (intersectionUnit);
			yield break;
		}

		highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (validIntersectionsToBuild));//, intersectionUnit));
		print (players [currentPlayerTurn].playerName + " builds a " + unitType.ToString() + " on intersection #" + players [currentPlayerTurn].lastIntersectionSelection.id);

		players [currentPlayerTurn].lastIntersectionSelection.occupier = intersectionUnit;
		intersectionUnit.locationIntersection = players [currentPlayerTurn].lastIntersectionSelection;
		players [currentPlayerTurn].addOwnedUnit(intersectionUnit, unitType);
		intersectionUnit.owner = players [currentPlayerTurn];

		intersectionUnit.transform.position = players [currentPlayerTurn].lastIntersectionSelection.transform.position;
		intersectionUnit.transform.parent = players [currentPlayerTurn].lastIntersectionSelection.transform;
		intersectionUnit.transform.localScale = intersectionUnit.transform.localScale * GameBoard.hexRadius;

		intersectionUnit.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		intersectionUnit.gameObject.SetActive (true);

		//unitsInPlay.Add (intersectionUnit.id, intersectionUnit);

		if (!setupPhase) {
			players [currentPlayerTurn].spendResources (costOfUnit);

			uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
		}

		highlightAllIntersections(false);

		currentActiveButton = -1;
		waitingForPlayer = false;
	}

	#endregion

	private void destroyCancelledUnits() {
		List<int> keys = new List<int>(unitsInPlay.Keys);

		for (int i = 0; i < keys.Count; i++) {
			if (unitsInPlay [keys [i]].owner == null) {
				Destroy (unitsInPlay [keys [i]].gameObject);
				unitsInPlay.Remove (keys [i]);
			}
		}
	}

	private void removeUnitFromGame(Unit unit) {
		Destroy (unitsInPlay [unit.id].gameObject);
		unitsInPlay.Remove (unit.id);
	}

	#region Highlighter Methods

	private void highlightContainersForUnitType(Unit unit) {
		if (typeof(IntersectionUnit).IsAssignableFrom (unit.GetType())) {

		} else if (typeof(TradeUnit).IsAssignableFrom (unit.GetType())) {

		}
	}

	private void highlightUnitsWithColor(List<Unit> units, bool highlight, Color colorToHighlight) {
		for (int i = 0; i < units.Count; i++) {
			Renderer renderer = units [i].gameObject.GetComponentInChildren<Renderer> ();
			renderer.material.color = colorToHighlight;
		}

	}

	private void highlightIntersectionsWithColor(List<Intersection> intersections, bool highlight, Color playerColor) {
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersectionWithColor (highlight, playerColor);
			}
		}
	}

	private void highlightIntersections(List<Intersection> intersections, bool highlight) {
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersection (highlight);
			}
		}
	}

	private void highlightAllIntersections(bool highlight) {
		List<Intersection> intersections = GameBoard.getIntersections ();
		for (int i = 0; i < intersections.Count; i++) {
			intersections [i].highlightIntersection (highlight);
		}
	}

	private void highlightEdgesWithColor(List<Edge> edges, bool highlight, Color playerColor) {
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdgeWithColor (highlight, playerColor);
		}

	}

	private void highlightEdges(List<Edge> edges, bool highlight) {
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdge (highlight);
		}

	}

	private void highlightAllEdges(bool highlight) {
		List<Edge> edges = GameBoard.getEdges ();
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdge (highlight);
		}
	}

	#endregion

	#region Resource Distribution

	void giveResourcesToPlayer(Player player, ResourceTuple resourceObtained) {
		player.receiveResources (resourceObtained);
	}

	void giveCommoditiesToPlayer(Player player, CommodityTuple commodityObtained) {
		player.receiveCommodities (commodityObtained);
	}

	ResourceTuple getResourceForTile(GameTile tile, int numCollected) {
		ResourceTuple resourceCollected = new ResourceTuple ();
		ResourceType typeOfResource = GameAsset.getResourceOfHex (tile.tileType);

		if (typeOfResource != ResourceType.Null) {
			resourceCollected.resourceTuple [typeOfResource] = numCollected;
		}

		return resourceCollected;
	}

	CommodityTuple getCommodityForTile(GameTile tile, int numCollected) {
		CommodityTuple commodityCollected = new CommodityTuple ();
		CommodityType typeOfCommodity = GameAsset.getCommodityOfHex (tile.tileType);

		if (typeOfCommodity != CommodityType.Null) {
			commodityCollected.commodityTuple [typeOfCommodity] = numCollected;
		}

		return commodityCollected;
	}

	List<GameTile> getTilesWithDiceValue(Player player, int valueRolled, bool isCommodity) {
		List<GameTile> eligibleTiles = new List<GameTile> ();

		List<City> ownedCities = player.getOwnedUnitsOfType (typeof(City)).Cast<City> ().ToList ();
		List<Settlement> ownedSettlements = player.getOwnedUnitsOfType (typeof(Settlement)).Cast<Settlement> ().ToList ();

		for (int i = 0; i < ownedCities.Count; i++) {
			Intersection relatedIntersection = ownedCities[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				if ((tile.diceValue == valueRolled || setupPhase) && (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean)) {
					eligibleTiles.Add (tile);
				}
			}
		}

		if (!isCommodity && !setupPhase) {
			for (int i = 0; i < ownedSettlements.Count; i++) {
				Intersection relatedIntersection = ownedSettlements[i].locationIntersection;

				List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
				foreach(GameTile tile in adjacentTiles) {
					if ((tile.diceValue == valueRolled || setupPhase) && (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean)) {
						eligibleTiles.Add (tile);
					}
				}
			}
		}

		return eligibleTiles;
	}

	void diceRollResolveEvent() {
		int diceOutcome = resourceManager.diceRollEvent ();

		if (!setupPhase && diceOutcome == 7) {
			StartCoroutine(moveRobberForCurrentPlayer ());
		} else {
			resourceCollectionEvent (diceOutcome);

			if(!setupPhase)
				commodityCollectionEvent (diceOutcome);
		}
	}

	void resourceCollectionEvent(int diceOutcome) {
		for (int i = 0; i < players.Count; i++) {
			if (setupPhase && i != currentPlayerTurn) {
				continue;
			} else {
				List<GameTile> eligibleTilesForPlayer = getTilesWithDiceValue (players [i], diceOutcome, false);

				for (int j = 0; j < eligibleTilesForPlayer.Count; j++) {
					if (eligibleTilesForPlayer [j].canProduce ()) {
						print (players [i].playerName + " gets " + "1 " + GameAsset.getResourceOfHex (eligibleTilesForPlayer [j].tileType));
						giveResourcesToPlayer (players [i], getResourceForTile (eligibleTilesForPlayer [j], 1));
					}
				}
			}
		}
	}

	void commodityCollectionEvent(int diceOutcome) {
		for (int i = 0; i < players.Count; i++) {
			if (setupPhase && i != currentPlayerTurn) {
				continue;
			} else {
				List<GameTile> eligibleTilesForPlayer = getTilesWithDiceValue (players [i], diceOutcome, true);

				for (int j = 0; j < eligibleTilesForPlayer.Count; j++) {
					if (eligibleTilesForPlayer [j].canProduce ()) {
						print (players [i].playerName + " gets " + "1 " + GameAsset.getCommodityOfHex (eligibleTilesForPlayer [j].tileType));
						//giveResourcesToPlayer (players [i], getResourceForTile (eligibleTilesForPlayer [j], 1));
						giveCommoditiesToPlayer(players[i], getCommodityForTile(eligibleTilesForPlayer[j], 1));
					}
				}
			}
		}
	}

	#endregion

	#region Move Game Pieces for Players

	IEnumerator moveRobberForCurrentPlayer() {
		waitingForPlayer = true;

		yield return StartCoroutine (players [currentPlayerTurn].makeGameTileSelection (boardGenerator.landTiles));
		gameBoard.MoveRobber (players [currentPlayerTurn].lastGameTileSelection);

		List<IntersectionUnit> opponentUnits = new List<IntersectionUnit> ();
		foreach (Intersection intersection in players [currentPlayerTurn].lastGameTileSelection.intersections) {
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
			stealableOpponents [0].spendAssets (randomStolenAsset);
			players [currentPlayerTurn].receiveAssets (randomStolenAsset);
		} else if (stealableOpponents.Count > 1) {
			robberStealPanel.displayPanelForChoices (stealableOpponents);
			bool selectionMade = false;

			while (!selectionMade) {
				yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

				// IF BUTTON WAS PRESSED, selectionMade = true
			}


		}
		// STEAL CARDS FROM OTHERS (RANDOM?) 
		// Something along the lines of forall intersections at selected tile, if occupied && occupier.owner != plyaer[currentturn]
		// then steal 1 random resource (generate random num from 0 to resourceTypes.range - 1 (excluding null)
		// while victimPlayer.resourcetuple[randomNum] == 0 then subtract 1 and add 1 to the player

		waitingForPlayer = false;
	}

	#endregion

	#region Container Validators for Player

	public static List<Edge> getValidEdgesForPlayer(Player player, bool roadBuilt) {
		//print ("roadBuilt == " + roadBuilt.ToString ());
		List<Edge> validEdges = new List<Edge> ();
		List<Unit> ownedUnits = player.getOwnedUnits ();

		if (setupPhase) {
			for (int i = 0; i < ownedUnits.Count; i++) {
				if (typeof(IntersectionUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
					bool validSet = true;
					IntersectionUnit intersectionUnit = (IntersectionUnit)(ownedUnits [i]);
					Intersection relatedIntersection = intersectionUnit.locationIntersection;
					List<Edge> connectedEdges = relatedIntersection.getLinkedEdges ();

					for (int j = 0; j < connectedEdges.Count; j++) {
						if (connectedEdges [j].occupier != null) {
							validSet = false;
						}
					}
					if (validSet) {
						for (int j = 0; j < connectedEdges.Count; j++) {
							if (connectedEdges [j].occupier == null) {
								validEdges.Add (connectedEdges [j]);
							}
						}
					}
				}
			}
		} else {
			for (int i = 0; i < ownedUnits.Count; i++) {
				if (typeof(TradeUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
					TradeUnit tradeUnit = (TradeUnit)(ownedUnits [i]);
					Edge relatedEdge = tradeUnit.locationEdge;

					List<Intersection> connectedIntersections = relatedEdge.getLinkedIntersections ();
					for (int j = 0; j < connectedIntersections.Count; j++) {
						if (connectedIntersections [j].occupier == null || connectedIntersections [j].occupier.owner == player) {
							List<Edge> connectedEdges = connectedIntersections [j].getLinkedEdges ();

							for (int k = 0; k < connectedEdges.Count; k++) {
								if (connectedEdges [k].occupier == null && (roadBuilt == connectedEdges [k].isLandEdge () || connectedEdges [k].isShoreEdge ())) {
									if (relatedEdge.occupier.isRoad () == roadBuilt || (connectedIntersections [j].occupier != null && connectedIntersections [j].occupier.owner == player)) {
										validEdges.Add (connectedEdges [k]);
									}
								}
							}
						}
					}
				}
			}
		}

		return validEdges;
	}

	public static List<Intersection> getValidIntersectionsForPlayer(Player player) {
		List<Intersection> allIntersections = GameBoard.getIntersections (); 
		List<Intersection> validIntersections = new List<Intersection> ();

		if (setupPhase) {
			for (int i = 0; i < allIntersections.Count; i++) {
				List<Intersection> neighborsOfIntersection = allIntersections [i].getNeighborIntersections ();
				bool validIntersection = true;

				for (int j = 0; j < neighborsOfIntersection.Count; j++) {
					if (!allIntersections[i].isSettleable() || neighborsOfIntersection [j].occupier != null) {// && neighborsOfIntersection [j].occupier.owner != player) {
						validIntersection = false;
					}
				}

				if (validIntersection) {
					validIntersections.Add (allIntersections [i]);
				}
			}
		} else {
			allIntersections = new List<Intersection> ();
			List<Unit> ownedUnits = player.getOwnedUnits ();

			for (int i = 0; i < ownedUnits.Count; i++) {
				if (typeof(TradeUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
					TradeUnit tradeUnit = (TradeUnit)(ownedUnits [i]);
					Edge relatedEdge = tradeUnit.locationEdge;

					List<Intersection> neighborIntersections = relatedEdge.getLinkedIntersections ();

					for (int j = 0; j < neighborIntersections.Count; j++) {
						if (neighborIntersections [j].occupier == null && neighborIntersections [j].isSettleable()
							&& neighborIntersections [j].isNotNeighboringIntersectionWithUnit()) {
							validIntersections.Add (neighborIntersections [j]);
						}
					}
				}
			}
		}

		return validIntersections;
	}

	#endregion

	#region Old Methods

	IEnumerator buildTradeUnit2(TradeUnit tradeUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Edge> validEdgesToBuild = getValidEdgesForPlayer (players[currentPlayerTurn], true);
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (unitType);

		if (costOfUnit == null) {
			print ("costofunit is null, returning.");
			waitingForPlayer = false;
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build a road!");
				waitingForPlayer = false;
				yield break;
			}
		}

		if (validEdgesToBuild.Count == 0) {
			print ("No possible location to build a road!");
			Destroy (tradeUnit);
			waitingForPlayer = false;
			yield break;
		}

		highlightEdgesWithColor (validEdgesToBuild, true, players [currentPlayerTurn].playerColor);

		tradeUnit.id = unitID++;
		tradeUnit.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToBuild));//, tradeUnit));//new Road(unitID++)));

		tradeUnit.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		tradeUnit.gameObject.SetActive (true);

		if (!setupPhase) {
			players [currentPlayerTurn].spendResources (costOfUnit);

			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
		}
		highlightAllEdges(false);

		waitingForPlayer = false;
	}

	IEnumerator buildIntersectionUnit2(IntersectionUnit intersectionUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Intersection> validIntersectionsToBuild = getValidIntersectionsForPlayer (players[currentPlayerTurn]);
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (unitType);

		if (costOfUnit == null) {
			print ("Cost of unit is null, returning.");
			waitingForPlayer = false;
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build a road!");
				waitingForPlayer = false;
				yield break;
			}
		}

		if (validIntersectionsToBuild.Count == 0) {
			print ("No possible location to build a settlement!");
			Destroy (intersectionUnit);
			waitingForPlayer = false;
			yield break;
		}

		highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);

		intersectionUnit.id = unitID++;
		intersectionUnit.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (validIntersectionsToBuild));//, intersectionUnit));

		intersectionUnit.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		intersectionUnit.gameObject.SetActive (true);

		if (!setupPhase) {
			players [currentPlayerTurn].spendResources (costOfUnit);

			uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
		}

		highlightAllIntersections(false);

		waitingForPlayer = false;
	}


	#endregion
}
