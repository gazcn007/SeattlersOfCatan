using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate IEnumerator PlayerAction();

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject playerPrefab;
	public GameObject settlementPrefab;
	public GameObject roadPrefab;

	[Range(10, 25)]
	public static int victoryPointsToWinGame = 12;

	public List<Player> players = new List<Player>(4);

	private Canvas canvas;
	private Text[] texts;
	private Button[] uiButtons;

	private GameBoard gameBoard;
	private BoardGenerator boardGenerator;

	private int currentPlayerTurn = 0;
	private bool waitingForPlayer;
	private static bool setupPhase;

	private int unitID = 0;

	// Use this for initialization
	void Start () {
		addPlayers ();
		boardGenerator = GameObject.FindGameObjectWithTag ("Board Generator").GetComponent<BoardGenerator>();
		boardGenerator.GenerateBoard ();
		canvas = GameObject.FindObjectOfType<Canvas> ();
		GameObject[] textObjects = GameObject.FindGameObjectsWithTag ("DebugUIText");
		texts = new Text[textObjects.Length];
		for (int i = 0; i < textObjects.Length; i++) {
			texts [i] = textObjects [textObjects.Length - 1 - i].GetComponent<Text> ();
		}

		uiButtons = canvas.GetComponentsInChildren<Button> ();
		uiButtons[0].onClick.AddListener (endTurn);
		uiButtons[1].onClick.AddListener (buildSettlementEvent);
		uiButtons[2].onClick.AddListener (buildRoadEvent);

		gameBoard = boardGenerator.GetComponentInChildren<GameBoard>();

		setupPhase = true;
		StartCoroutine(settlePlayersOnBoard ());
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (players [currentPlayerTurn].playerName + "'s turn.");
		//players[currentPlayerTurn].playTurn();
		updateCanvas ();

	}

	IEnumerator settlePlayersOnBoard() {
		for (currentPlayerTurn = 0; currentPlayerTurn < players.Count; currentPlayerTurn++) {
			print (players [currentPlayerTurn].playerName + "'s turn.");
			print (players [currentPlayerTurn].playerName + " must build a settlement on an intersection.");
			yield return StartCoroutine (buildSettlement ());
			print (players [currentPlayerTurn].playerName + " must build a road on an edge.");
			yield return StartCoroutine (buildRoad ());
		}

		for (currentPlayerTurn = players.Count - 1; currentPlayerTurn >= 0; currentPlayerTurn--) {
			print (players [currentPlayerTurn].playerName + "'s turn.");
			print (players [currentPlayerTurn].playerName + " must build a settlement on an intersection.");
			yield return StartCoroutine (buildSettlement ());
			print (players [currentPlayerTurn].playerName + " must build a road on an edge.");
			yield return StartCoroutine (buildRoad ());
		}

		currentPlayerTurn = 0;
		setupPhase = false;

	}

	void addPlayers() {
		GameObject player1Object = (GameObject) Instantiate (playerPrefab);
		Player player1 = player1Object.GetComponent<Player> ();
		player1.playerColor = Color.blue;
		player1.playerName = "Nehir";
		player1.playerNumber = 1;

		GameObject player2Object = (GameObject) Instantiate (playerPrefab);
		Player player2 = player2Object.GetComponent<Player> ();
		player2.playerColor = Color.red;
		player2.playerName = "Omer";
		player2.playerNumber = 2;

		GameObject player3Object = (GameObject) Instantiate (playerPrefab);
		Player player3 = player3Object.GetComponent<Player> ();
		player3.playerColor = Color.yellow;
		player3.playerName = "Milosz";
		player3.playerNumber = 3;

		GameObject player4Object = (GameObject) Instantiate (playerPrefab);
		Player player4 = player4Object.GetComponent<Player> ();
		player4.playerColor = Color.green;
		player4.playerName = "Carl";
		player4.playerNumber = 4;

		players.Add (player1);
		players.Add (player2);
		players.Add (player3);
		players.Add (player4);
	}

	void updateCanvas() {
		for (int i = 0; i < players.Count; i++) {
			texts [i].text = "Player " + players [i].playerNumber + "\n" + players [i].playerName + " VP= " + players[i].victoryPoints + "\nUnits Count: " + players [i].getNumUnits();
		}
	}

	void endTurn() {
		if (!waitingForPlayer) {
			currentPlayerTurn = (currentPlayerTurn + 1) % players.Count;
		}
	}

	void makePlayerAction(PlayerAction actionType) {
		if (!waitingForPlayer) {
			StartCoroutine (actionType ());
		}
	}

	void buildRoadEvent() {
		if (!waitingForPlayer) {
			StartCoroutine (buildRoad ());
		}
	}

	IEnumerator buildRoad() {
		waitingForPlayer = true;
		//if (!setupPhase) {
		//	print("Insufficient Resources to build a road!");
		//	players [currentPlayerTurn].checkResources (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue)
		//	waitingForPlayer = false;
		//	yield break;
		//}

		if (!highlightEdgesWithColor (true, players [currentPlayerTurn].playerColor)) {
			print ("No possible location to build a road!");
			waitingForPlayer = false;
			yield break;
		}

		GameObject roadGameObject = (GameObject)Instantiate (roadPrefab);
		Road road = roadGameObject.GetComponent<Road> ();
		road.id = unitID++;
		road.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeEdgeSelection (road));//new Road(unitID++)));

		road.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		road.gameObject.SetActive (true);

		//if (!setupPhase) {
		//	players [currentPlayerTurn].decreaseResources (Road.ResourceValue);
		//}
		highlightAllEdges(false);

		waitingForPlayer = false;
	}

	void buildSettlementEvent() {
		if (!waitingForPlayer) {
			StartCoroutine (buildSettlement ());
		}
	}

	IEnumerator buildSettlement() {
		waitingForPlayer = true;

		//if (!setupPhase) {
		//	print("Insufficient Resources to build a road!");
		//	players [currentPlayerTurn].checkResources (Road.ResourceValue);//ResourceCost.getResourceValueOf(Road.ResourceValue);
		//	waitingForPlayer = false;
		//	yield break;
		//}

		if (!highlightIntersectionsWithColor (true, players [currentPlayerTurn].playerColor)) {
			print ("No possible location to build a settlement!");
			waitingForPlayer = false;
			yield break;
		}

		GameObject settlementGameObject = (GameObject)Instantiate (settlementPrefab);
		Settlement settlement = settlementGameObject.GetComponent<Settlement> ();
		settlement.id = unitID++;
		settlement.gameObject.SetActive (false);

		yield return StartCoroutine (players [currentPlayerTurn].makeIntersectionSelection (settlement));

		settlement.GetComponentInChildren<Renderer> ().material.color = players[currentPlayerTurn].playerColor;
		settlement.gameObject.SetActive (true);

		//if (!setupPhase) {
		//	players [currentPlayerTurn].decreaseResources (Road.ResourceValue);
		//}
		highlightAllIntersections(false);

		waitingForPlayer = false;
	}

	private void highlightEdges(bool highlight) {
		List<Edge> edges = GameBoard.getEdges ();
		for (int i = 0; i < edges.Count; i++) {
			if (edges [i].occupier == null && (edges [i].isLandEdge() || edges [i].isShoreEdge())) {
				edges [i].highlightEdge (highlight);
			}
		}
	}

	private void highlightAllEdges(bool highlight) {
		List<Edge> edges = GameBoard.getEdges ();
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdge (highlight);
		}
	}

	private void highlightAllIntersections(bool highlight) {
		List<Intersection> intersections = GameBoard.getIntersections ();
		for (int i = 0; i < intersections.Count; i++) {
			intersections [i].highlightIntersection (highlight);
		}
	}

	private bool highlightEdgesWithColor(bool highlight, Color playerColor) {
		List<Edge> edges = getValidEdgesForPlayer (players [currentPlayerTurn]);
		bool listNotEmpty = false;

		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdgeWithColor (highlight, playerColor);
		}

		if (edges.Count > 0) {
			listNotEmpty = true;
		}

		return listNotEmpty;
	}

	public static List<Edge> getValidEdgesForPlayer(Player player) {
		List<Edge> validEdges = new List<Edge> ();
		List<Unit> ownedUnits = player.getOwnedUnits ();

		for (int i = 0; i < ownedUnits.Count; i++) {
			if(typeof(IntersectionUnit).IsAssignableFrom(ownedUnits[i].GetType())) {
				IntersectionUnit intersectionUnit = (IntersectionUnit)(ownedUnits [i]);
				Intersection relatedIntersection = intersectionUnit.locationIntersection;
				List<Edge> connectedEdges = relatedIntersection.getLinkedEdges ();

				for (int j = 0; j < connectedEdges.Count; j++) {
					if (connectedEdges [i].occupier == null) {
						validEdges.Add (connectedEdges [j]);
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
					if (neighborsOfIntersection [j].occupier != null && neighborsOfIntersection [j].occupier.owner != player) {
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

	private bool highlightIntersectionsWithColor(bool highlight, Color playerColor) {
		List<Intersection> intersections = getValidIntersectionsForPlayer (players [currentPlayerTurn]);
		bool listNotEmpty = false;
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersectionWithColor (highlight, playerColor);
			}
		}

		if (intersections.Count > 0) {
			listNotEmpty = true;
		}

		return listNotEmpty;
	}

	private void highlightIntersections(bool highlight) {
		List<Intersection> intersections = GameBoard.getIntersections ();
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersection (highlight);
			}
		}
	}
}
