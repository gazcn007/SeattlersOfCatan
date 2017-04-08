using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public static BoardManager instance;

	//public GameBoard gameBoard;

	void Awake() {
		if (instance == null)
			instance = this;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Container Validators for Player

	public List<GameTile> getTilesWithDiceValue(int playerID, int valueRolled, bool isCommodity) {
		List<GameTile> eligibleTiles = new List<GameTile> ();

		List<Metropolis> ownedMetropolis = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(Metropolis)).Cast<Metropolis> ().ToList ();
		List<City> ownedCities = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(City)).Cast<City> ().ToList ();
		List<Settlement> ownedSettlements = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(Settlement)).Cast<Settlement> ().ToList ();

		for (int i = 0; i < ownedMetropolis.Count; i++) {
			Intersection relatedIntersection = ownedMetropolis[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				//if ((tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) && (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean)) {
				if (tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) {
					eligibleTiles.Add (tile);

					Debug.Log ("Added " + tile.name);
				}
			}
		}

		for (int i = 0; i < ownedCities.Count; i++) {
			Intersection relatedIntersection = ownedCities[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				//if ((tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) && (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean)) {
				if (tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) {
					eligibleTiles.Add (tile);

					Debug.Log ("Added " + tile.name);
				}
			}
		}

		if (!isCommodity && !EventTransferManager.instance.setupPhase) {
			for (int i = 0; i < ownedSettlements.Count; i++) {
				Intersection relatedIntersection = ownedSettlements[i].locationIntersection;

				List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
				foreach(GameTile tile in adjacentTiles) {
					//if ((tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) && (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean)) {
					if (tile.diceValue == valueRolled || EventTransferManager.instance.setupPhase) {
						eligibleTiles.Add (tile);

						Debug.Log ("Added " + tile.name);
					}
				}
			}
		}

		return eligibleTiles;
	}

	public int[] getTileIDsWithDiceValue(int playerID, int valueRolled, bool isCommodity) {
		List<GameTile> eligibleTiles = getTilesWithDiceValue(playerID, valueRolled, isCommodity);
		int[] eligibleTileIDs = new int[eligibleTiles.Count];

		for(int i = 0; i < eligibleTiles.Count; i++) {
			eligibleTileIDs[i] = eligibleTiles[i].id;
		}

		return eligibleTileIDs;
	}

	public List<GameTile> getAdjacentTilesOfType(int playerID, TileType tileType) {
		List<GameTile> eligibleTiles = new List<GameTile> ();

		List<Metropolis> ownedMetropolis = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(Metropolis)).Cast<Metropolis> ().ToList ();
		List<City> ownedCities = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(City)).Cast<City> ().ToList ();
		List<Settlement> ownedSettlements = CatanManager.instance.players[playerID].getOwnedUnitsOfType (typeof(Settlement)).Cast<Settlement> ().ToList ();

		for (int i = 0; i < ownedMetropolis.Count; i++) {
			Intersection relatedIntersection = ownedMetropolis[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				if (tile.tileType == tileType && !eligibleTiles.Contains(tile)) {
					eligibleTiles.Add (tile);
				}
			}
		}

		for (int i = 0; i < ownedCities.Count; i++) {
			Intersection relatedIntersection = ownedCities[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				if (tile.tileType == tileType && !eligibleTiles.Contains(tile)) {
					eligibleTiles.Add (tile);
				}
			}
		}

		for (int i = 0; i < ownedSettlements.Count; i++) {
			Intersection relatedIntersection = ownedSettlements[i].locationIntersection;

			List<GameTile> adjacentTiles = relatedIntersection.getAdjacentTiles ();
			foreach(GameTile tile in adjacentTiles) {
				if (tile.tileType == tileType && !eligibleTiles.Contains(tile)) {
					eligibleTiles.Add (tile);
				}
			}
		}

		return eligibleTiles;
	}

	public int[] getAdjacentTileIDsOfType(int playerID, TileType tileType) {
		List<GameTile> eligibleTiles = getAdjacentTilesOfType(playerID, tileType);
		int[] eligibleTileIDs = new int[eligibleTiles.Count];

		for(int i = 0; i < eligibleTiles.Count; i++) {
			eligibleTileIDs[i] = eligibleTiles[i].id;
		}

		return eligibleTileIDs;
	}

	public GameTile getLakeTile() {
		List<GameTile> allTiles = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard>().GameTiles.Values.ToList ();

		GameTile lakeTile = null;

		foreach (GameTile tile in allTiles) {
			if (tile.tileType == TileType.Desert) {
				lakeTile = tile;
			}
		}

		return lakeTile;
	}

	public int getLakeTileID() {
		GameTile lakeTile = getLakeTile ();
		return lakeTile.id;
	}

	public List<Edge> getValidEdgesForPlayer(Player player, bool roadBuilt) {
		//print ("roadBuilt == " + roadBuilt.ToString ());
		List<Edge> validEdges = new List<Edge> ();
		List<Unit> ownedUnits = player.getOwnedUnits ();

		if (EventTransferManager.instance.setupPhase || player.playedRoadBuilding) {
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
				if (typeof(EdgeUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
					EdgeUnit tradeUnit = (EdgeUnit)(ownedUnits [i]);
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

	public int[] getValidEdgeIDsForPlayer(Player player, bool roadBuilt) {
		List<Edge> validEdges = getValidEdgesForPlayer (player, roadBuilt);
		int[] validEdgeIDs = new int[validEdges.Count];

		for (int i = 0; i < validEdges.Count; i++) {
			validEdgeIDs [i] = validEdges [i].id;
		}

		return validEdgeIDs;
	}

	public List<Edge> getValidEdgesForPlayerShipMove(Player player, Ship shipToMove) {
		List<Edge> validEdges = new List<Edge> ();
		List<Ship> ownedShips = player.getOwnedUnitsOfType (UnitType.Ship).Where(ship => ship != shipToMove).Cast<Ship>().ToList();

		for (int i = 0; i < ownedShips.Count; i++) {
			Ship currentShip = ownedShips [i];
			Edge locationEdge = currentShip.locationEdge;

			List<Intersection> connectedIntersections = locationEdge.getLinkedIntersections ();
			for (int j = 0; j < connectedIntersections.Count; j++) {
				if (connectedIntersections [j].occupier == null || connectedIntersections [j].occupier.owner == player) {
					List<Edge> connectedEdges = connectedIntersections [j].getLinkedEdges ();

					for (int k = 0; k < connectedEdges.Count; k++) {
						if (connectedEdges [k].occupier == null && !connectedEdges [k].isLandEdge()) {
							//if (relatedEdge.occupier.isRoad () == roadBuilt || (connectedIntersections [j].occupier != null && connectedIntersections [j].occupier.owner == player)) {
								validEdges.Add (connectedEdges [k]);
							//}
						}
					}
				}
			}
		}

		if (validEdges.Contains (shipToMove.locationEdge)) {
			validEdges.Remove (shipToMove.locationEdge);
		}
		for (int i = 0; i < validEdges.Count; i++) {
			Debug.Log ("Valid edge to move ship: validEdge[ " + i + "] = " + validEdges [i].name);
		}

		return validEdges;
	}

	public int[] getValidEdgeIDsForPlayerShipMove(Player player, Ship shipToMove) {
		List<Edge> validEdges = getValidEdgesForPlayerShipMove(player, shipToMove);
		int[] validEdgeIDs = new int[validEdges.Count];

		for (int i = 0; i < validEdges.Count; i++) {
			validEdgeIDs [i] = validEdges [i].id;
		}

		return validEdgeIDs;
	}


	public List<Intersection> getValidIntersectionsForPlayer(Player player) {
		List<Intersection> allIntersections = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard>().Intersections.Values.ToList ();
		List<Intersection> validIntersections = new List<Intersection> ();

		if (EventTransferManager.instance.setupPhase) {
			for (int i = 0; i < allIntersections.Count; i++) {
				List<Intersection> neighborsOfIntersection = allIntersections [i].getNeighborIntersections ();
				bool validIntersection = true;

				for (int j = 0; j < neighborsOfIntersection.Count; j++) {
					if (!allIntersections[i].isSettleable() || neighborsOfIntersection [j].occupier != null || !allIntersections[i].isMainIslandIntersection()) {// && neighborsOfIntersection [j].occupier.owner != player) {
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
				if (typeof(EdgeUnit).IsAssignableFrom (ownedUnits [i].GetType ())) {
					EdgeUnit tradeUnit = (EdgeUnit)(ownedUnits [i]);
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

	public int[] getValidIntersectionIDsForPlayer(Player player) {
		List<Intersection> validIntersections = getValidIntersectionsForPlayer (player);
		int[] validIntersectionIDs = new int[validIntersections.Count];

		for (int i = 0; i < validIntersections.Count; i++) {
			validIntersectionIDs [i] = validIntersections [i].id;
		}

		return validIntersectionIDs;
	}


	public List<GameTile> getLandTiles(bool occupierCheck) {
		List<GameTile> allTiles = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard>().GameTiles.Values.ToList ();
		List<GameTile> landTiles = new List<GameTile>();

		foreach (GameTile tile in allTiles) {
			if (occupierCheck) {
				if (tile.tileType != TileType.Ocean && tile.occupier == null) {
					landTiles.Add (tile);
				}
			} else {
				if (tile.tileType != TileType.Ocean) {
					landTiles.Add (tile);
				}
			}
		}
		return landTiles;
	}

	public List<GameTile> getOceanTiles(bool occupierCheck) {
		List<GameTile> allTiles = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard>().GameTiles.Values.ToList ();
		List<GameTile> oceanTiles = new List<GameTile>();

		foreach (GameTile tile in allTiles) {
			if (occupierCheck) {
				if (tile.tileType == TileType.Ocean && tile.occupier == null) {
					oceanTiles.Add (tile);
				}
			} else {
				if (tile.tileType == TileType.Ocean) {
					oceanTiles.Add (tile);
				}
			}
		}
		return oceanTiles;
	}

	public List<GameTile> getMainLandTiles(bool occupierCheck) {
		List<GameTile> allTiles = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard>().GameTiles.Values.ToList ();
		List<GameTile> landTiles = new List<GameTile>();

		foreach (GameTile tile in allTiles) {
			if (occupierCheck) {
				if (tile.tileType != TileType.Ocean && tile.occupier == null && !tile.atIslandLayer) {
					landTiles.Add (tile);
				}
			} else {
				if (tile.tileType != TileType.Ocean && !tile.atIslandLayer) {
					landTiles.Add (tile);
				}
			}
		}

		return landTiles;
	}

	public GameBoard getBoard() {
		return GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
	}

	#endregion

	#region Highlighter Methods

	#region Unit Highligters

	public void highlightContainersForUnitType(Unit unit) {
		if (typeof(IntersectionUnit).IsAssignableFrom (unit.GetType())) {

		} else if (typeof(EdgeUnit).IsAssignableFrom (unit.GetType())) {

		}
	}

	public void highlightUnitsWithColor(List<Unit> units, bool highlight, Color colorToHighlight) {
		for (int i = 0; i < units.Count; i++) {
			Renderer renderer = units [i].gameObject.GetComponentInChildren<Renderer> ();
			renderer.material.color = colorToHighlight;
		}
	}

	public void highlightUnitsWithPlayerColor(int[] unitIDs, bool highlight, int playerID) {
		for (int i = 0; i < unitIDs.Length; i++) {
			Unit unit = GameObject.FindGameObjectWithTag ("UnitManager").GetComponent<UnitManager> ().Units [unitIDs [i]];
			Renderer renderer = unit.gameObject.GetComponentInChildren<Renderer> ();
			renderer.material.color = CatanManager.instance.players[playerID].playerColor;
		}
	}

	#endregion

	#region Intersection Highlighters

	public void highlightIntersectionsWithColor(List<Intersection> intersections, bool highlight, Color playerColor) {
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersectionWithColor (highlight, playerColor);
			}
		}
	}

	public void highlightIntersections(List<Intersection> intersections, bool highlight) {
		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i].occupier == null && intersections[i].isSettleable()) {
				intersections [i].highlightIntersection (highlight);
			}
		}
	}

	public void highlightIntersectionsWithPlayerColor(int[] intersectionIDs, bool highlight, int playerID) {
		for (int i = 0; i < intersectionIDs.Length; i++) {
			GameObject[] boards = GameObject.FindGameObjectsWithTag ("Board");

			for (int k = 0; k < boards.Length; k++) {
				if (boards [k].GetComponent<GameBoard> ().Intersections.Count != 0) {
					//Intersection intersection = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ().Intersections [intersectionIDs[i]];
					Intersection intersection = boards [k].GetComponent<GameBoard> ().Intersections[intersectionIDs[i]];
					if (intersection.occupier == null && intersection.isSettleable()) {
						intersection.highlightIntersectionWithColor (highlight, CatanManager.instance.players[playerID].playerColor);
					}
				}

			}

		}
	}

	public void highlightAllIntersections(bool highlight) {
		GameBoard board = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		List<Intersection> intersections = board.Intersections.Values.ToList ();
		for (int i = 0; i < intersections.Count; i++) {
			intersections [i].highlightIntersection (highlight);
		}
	}

	#endregion

	#region Edge Highlighters

	public void highlightEdgesWithColor(List<Edge> edges, bool highlight, Color playerColor) {
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdgeWithColor (highlight, playerColor);
		}

	}

	public void highlightEdgesWithPlayerColor(int[] edgeIDs, bool highlight, int playerID) {
		for (int i = 0; i < edgeIDs.Length; i++) {
			Edge edge = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ().Edges [edgeIDs[i]];
			edge.highlightEdgeWithColor (highlight, CatanManager.instance.players[playerID].playerColor);
		}

	}

	public void highlightEdges(List<Edge> edges, bool highlight) {
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdge (highlight);
		}

	}

	public void highlightAllEdges(bool highlight) {
		GameBoard board = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		List<Edge> edges = board.Edges.Values.ToList();
		for (int i = 0; i < edges.Count; i++) {
			edges [i].highlightEdge (highlight);
		}
	}

	#endregion

	#endregion
}
