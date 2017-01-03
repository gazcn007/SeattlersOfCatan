using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[Range(10, 25)]
	public static int victoryPointsToWinGame = 12;

	public List<Player> players = new List<Player>(4);

	private Canvas canvas;
	private Text[] texts;
	private Button[] uiButtons;

	private GameBoard gameBoard;
	private BoardGenerator boardGenerator;
	private PrefabManager prefabManager;
	private ResourceDistributionManager resourceManager;
	private NumericDie redDie;
	private NumericDie yellowDie;

	private int currentPlayerTurn = 0;
	private bool waitingForPlayer;
	private static bool setupPhase;
	private bool cancelled;

	private int unitID = 0;

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
	}

	void initializeUI() {
		GameObject[] textObjects = GameObject.FindGameObjectsWithTag ("DebugUIText");

		texts = new Text[textObjects.Length];
		for (int i = 0; i < textObjects.Length; i++) {
			texts [i] = textObjects [textObjects.Length - 1 - i].GetComponent<Text> ();
		}

		uiButtons = canvas.GetComponentsInChildren<Button> ();
		uiButtons[0].onClick.AddListener (endTurn);
		uiButtons[1].onClick.AddListener (buildSettlementEvent);
		uiButtons[2].onClick.AddListener (buildRoadEvent);
		uiButtons[3].onClick.AddListener (diceRollEvent);
		uiButtons[4].onClick.AddListener (upgradeSettlementEvent);
		uiButtons[5].onClick.AddListener (buildShipEvent);
	}

	#endregion

	#region Game Initializer Methods

	void addPlayers() {
		GameObject player1Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player1 = player1Object.GetComponent<Player> ();
		player1.playerColor = Color.blue;
		player1.playerName = "Nehir";
		player1.playerNumber = 1;

		GameObject player2Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player2 = player2Object.GetComponent<Player> ();
		player2.playerColor = Color.red;
		player2.playerName = "Omer";
		player2.playerNumber = 2;

		GameObject player3Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player3 = player3Object.GetComponent<Player> ();
		player3.playerColor = Color.yellow;
		player3.playerName = "Milosz";
		player3.playerNumber = 3;

		GameObject player4Object = (GameObject) Instantiate (prefabManager.playerPrefab);
		Player player4 = player4Object.GetComponent<Player> ();
		player4.playerColor = Color.green;
		player4.playerName = "Carl";
		player4.playerNumber = 4;

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
			resourceCollectionEvent ();
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
				+ " VP= " + players[i].victoryPoints + "\nUnits Count: " + players [i].getNumUnits() + "\n<";
			foreach (var key in players[i].getCurrentResources().resourceTuple) {
				texts [i].text += key.Value + ", ";
			}
			texts [i].text += ">";
		}
	}

	#endregion

	#region Game Events & Button Events

	void buildRoadEvent() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [2].GetComponentInChildren<Text> ().text = "Cancel";

				cancelled = false;

				StartCoroutine (buildRoad ());
			} else {
				StopAllCoroutines();

				highlightAllEdges(false);
				waitingForPlayer = false;
				cancelled = true;

				uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			}
		}
	}

	void buildShipEvent() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				uiButtons [5].GetComponentInChildren<Text> ().text = "Cancel";

				cancelled = false;

				StartCoroutine (buildShip ());
			} else {
				StopAllCoroutines();

				highlightAllEdges(false);
				waitingForPlayer = false;
				cancelled = true;

				uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
			}
		}
	}

	void buildSettlementEvent() {
		if (!setupPhase) {
			//GameObject intersectionUnitGameObject = (GameObject)Instantiate (prefabManager.settlementPrefab);
			if (!waitingForPlayer) {
				uiButtons [1].GetComponentInChildren<Text> ().text = "Cancel";

				cancelled = false;

				StartCoroutine (buildSettlement ());//intersectionUnitGameObject));
			} else {
				StopAllCoroutines();

				highlightAllIntersections(false);
				waitingForPlayer = false;
				//Destroy (intersectionUnitGameObject);

				uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
			}
		}
	}

	void upgradeSettlementEvent() {
		if (!setupPhase) {
			//GameObject intersectionUnitGameObject = (GameObject)Instantiate (prefabManager.settlementPrefab);
			if (!waitingForPlayer) {
				uiButtons [4].GetComponentInChildren<Text> ().text = "Cancel";

				cancelled = false;

				StartCoroutine (buildSettlement ());//intersectionUnitGameObject));
			} else {
				StopAllCoroutines ();

				highlightAllIntersections (false);
				waitingForPlayer = false;
				//Destroy (intersectionUnitGameObject);

				uiButtons [4].GetComponentInChildren<Text> ().text = "Upgrade Settlement";
			}
		}
	}

	void endTurn() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				currentPlayerTurn = (currentPlayerTurn + 1) % players.Count;
			}
		}
	}

	void diceRollEvent() {
		if (!setupPhase) {
			if (!waitingForPlayer) {
				//StartCoroutine (diceRollPhase ());
				resourceCollectionEvent ();
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
		yield return StartCoroutine (buildIntersectionUnit (settlement, typeof(Settlement)));
	}

	IEnumerator buildCity() {
		GameObject intersectionUnitGameObject = (GameObject)Instantiate (prefabManager.cityPrefab);
		City city = intersectionUnitGameObject.GetComponent<City> ();
		yield return StartCoroutine (buildIntersectionUnit (city, typeof(City)));
	}

	IEnumerator buildRoad() {
		GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.roadPrefab);
		Road road = tradeUnitGameObject.GetComponent<Road> ();
		yield return StartCoroutine (buildTradeUnit (road, typeof(Road)));

	}

	IEnumerator buildShip() {
		GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.shipPrefab);
		Ship ship = tradeUnitGameObject.GetComponent<Ship> ();
		yield return StartCoroutine (buildTradeUnit (ship, typeof(Ship)));
	}

	#endregion

	#region Generic Build Methods

	IEnumerator buildTradeUnit(TradeUnit tradeUnit, System.Type unitType) {
		waitingForPlayer = true;
		List<Edge> validEdgesToBuild = getValidEdgesForPlayer (players[currentPlayerTurn], unitType == typeof(Road));
		ResourceTuple costOfUnit = ResourceCostManager.getCostOfUnit (unitType);

		if (costOfUnit == null) {
			print ("costofunit is null, returning.");
			waitingForPlayer = false;
			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
			Destroy (tradeUnit.gameObject);
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build a road!");
				waitingForPlayer = false;
				uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
				uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
				Destroy (tradeUnit.gameObject);
				yield break;
			}
		}

		if (validEdgesToBuild.Count == 0) {
			print ("No possible location to build a road!");
			Destroy (tradeUnit.gameObject);
			waitingForPlayer = false;
			uiButtons [2].GetComponentInChildren<Text> ().text = "Build Road";
			uiButtons [5].GetComponentInChildren<Text> ().text = "Build Ship";
			yield break;
		}

		highlightEdgesWithColor (validEdgesToBuild, true, players [currentPlayerTurn].playerColor);

		tradeUnit.id = unitID++;
		tradeUnit.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToBuild, tradeUnit));//new Road(unitID++)));

		if (setupPhase && !(players [currentPlayerTurn].lastEdgeSelection.isLandEdge() || players [currentPlayerTurn].lastEdgeSelection.isShoreEdge())) {
			GameObject tradeUnitGameObject = (GameObject)Instantiate (prefabManager.shipPrefab);
			Ship replacedShip = tradeUnitGameObject.GetComponent<Ship> ();
			replacedShip.id = tradeUnit.id;
			Destroy (tradeUnit.gameObject);
			tradeUnit = replacedShip;
		}

		players [currentPlayerTurn].lastEdgeSelection.occupier = tradeUnit;
		tradeUnit.locationEdge = players [currentPlayerTurn].lastEdgeSelection;
		players [currentPlayerTurn].addOwnedUnit (tradeUnit);
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
			yield break;
		}

		if (!setupPhase) {
			if (!players [currentPlayerTurn].hasAvailableResources (costOfUnit)) { // (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
				print ("Insufficient Resources to build a road!");
				Destroy (intersectionUnit.gameObject);
				waitingForPlayer = false;
				uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
				yield break;
			}
		}

		if (validIntersectionsToBuild.Count == 0) {
			print ("No possible location to build a settlement!");
			Destroy (intersectionUnit.gameObject);
			waitingForPlayer = false;
			uiButtons [1].GetComponentInChildren<Text> ().text = "Build Settlement";
			yield break;
		}

		highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);

		intersectionUnit.id = unitID++;
		intersectionUnit.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (intersectionUnit));

		players [currentPlayerTurn].lastIntersectionSelection.occupier = intersectionUnit;
		intersectionUnit.locationIntersection = players [currentPlayerTurn].lastIntersectionSelection;
		players [currentPlayerTurn].addOwnedUnit(intersectionUnit);
		intersectionUnit.owner = players [currentPlayerTurn];

		intersectionUnit.transform.position = players [currentPlayerTurn].lastIntersectionSelection.transform.position;
		intersectionUnit.transform.parent = players [currentPlayerTurn].lastIntersectionSelection.transform;
		intersectionUnit.transform.localScale = intersectionUnit.transform.localScale * GameBoard.hexRadius;

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

	#region Highlighter Methods

	private void highlightContainersForUnitType(Unit unit) {
		if (typeof(IntersectionUnit).IsAssignableFrom (unit.GetType())) {

		} else if (typeof(TradeUnit).IsAssignableFrom (unit.GetType())) {

		}
	}

	private void highlightIntersectionsWithColor(List<Intersection> intersections, bool highlight, Color playerColor) {
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersectionWithColor (highlight, playerColor);
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

	ResourceTuple getResourceForTile(GameTile tile, int numCollected) {
		ResourceTuple resourceCollected = new ResourceTuple ();
		ResourceType typeOfResource = GameAsset.getResourceOfHex (tile.tileType);

		if (typeOfResource != ResourceType.Null) {
			resourceCollected.resourceTuple [typeOfResource] = numCollected;
		}

		return resourceCollected;
	}

	List<GameTile> getTilesWithDiceValue(Player player, int valueRolled) {
		List<GameTile> eligibleTiles = new List<GameTile> ();
		List<Unit> ownedUnits = player.getOwnedUnits ();

		// Faster Alternative: Dictionary<int, List<GameTile>> and each key is dice value, so O(1) access
		// and assign each owner that many resources????

		for (int i = 0; i < ownedUnits.Count; i++) {
			if (typeof(IntersectionUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
				if (setupPhase && !typeof(City).IsAssignableFrom (ownedUnits [i].GetType ())) {
					continue;
				}
				IntersectionUnit intersectionUnit = (IntersectionUnit) ownedUnits [i];
				Intersection relatedIntersection = intersectionUnit.locationIntersection;

				HashSet<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
				foreach(GameTile tile in adjacentTiles) {
					if (tile.diceValue == valueRolled || setupPhase) {
						eligibleTiles.Add (tile);
					}
				}
			}
		}

		return eligibleTiles;
	}

	void resourceCollectionEvent() {
		int diceOutcome = resourceManager.diceRollEvent ();

		for (int i = 0; i < players.Count; i++) {
			if (setupPhase && i != currentPlayerTurn) {
				continue;
			} else {
				List<GameTile> eligibleTilesForPlayer = getTilesWithDiceValue (players [i], diceOutcome);

				for (int j = 0; j < eligibleTilesForPlayer.Count; j++) {
					print (players [i].playerName + " gets " + "1 " + GameAsset.getResourceOfHex (eligibleTilesForPlayer [j].tileType));
					giveResourcesToPlayer (players [i], getResourceForTile (eligibleTilesForPlayer [j], 1));
				}
			}
		}
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
						List<Edge> connectedEdges = connectedIntersections [j].getLinkedEdges ();

						for (int k = 0; k < connectedEdges.Count; k++) {
							if (connectedEdges [k].occupier == null && (roadBuilt == connectedEdges [k].isLandEdge() || connectedEdges [k].isShoreEdge())) {
								validEdges.Add (connectedEdges [k]);
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
					if (neighborsOfIntersection [j].occupier != null) {// && neighborsOfIntersection [j].occupier.owner != player) {
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
						if (neighborIntersections [j].occupier == null) {
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

		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (validEdgesToBuild, tradeUnit));//new Road(unitID++)));

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

		if (validIntersectionsToBuild.Count == 0) {
			print ("No possible location to build a settlement!");
			Destroy (intersectionUnit);
			waitingForPlayer = false;
			yield break;
		}

		highlightIntersectionsWithColor (validIntersectionsToBuild, true, players [currentPlayerTurn].playerColor);

		intersectionUnit.id = unitID++;
		intersectionUnit.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (intersectionUnit));

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
