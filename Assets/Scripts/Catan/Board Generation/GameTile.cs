using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : Tile {
	
	public int diceValue = -1;
	public TileType tileType;
	public int id;
	public bool atIslandLayer = false;

	public bool hasHarbor = false;

	public List<int> edges  = new List<int> ();
	public List<int> intersections = new List<int> ();

	public GameObject HighlightTile;
	public FishTile fishTile;
	public GamePiece occupier;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEdge(Edge edge) {
		edges.Add(edge.id);
	}

	public void addIntersection(Intersection intersection) {
		intersections.Add (intersection.id);
	}

	public List<Edge> getEdges() {
		List<Edge> linkEdges = new List<Edge> ();

		foreach (int i in edges) {
			linkEdges.Add (GameBoard.instance.Edges [i]);
		}

		return linkEdges;
	}

	public List<Intersection> getIntersections() {
		List<Intersection> linkIntersections = new List<Intersection> (intersections.Count);

		foreach(int i in intersections) {
			linkIntersections.Add (GameBoard.instance.Intersections [i]);
		}

		return linkIntersections;
	}

	public int getCornerNumberOfIntersection(Intersection intersection) {
		int cornerNum = -1;

		for (int i = 0; i < intersections.Count; i++) {
			if (intersections [i] == intersection.id) {
				cornerNum = i;
			}
		}

		return cornerNum;
	}

	public List<GameTile> getNeighborTiles() {
		List<GameTile> neighborTiles = new List<GameTile> ();
		List<Intersection> intersectionsList = new List<Intersection> ();

		GameObject[] boards = GameObject.FindGameObjectsWithTag ("Board");

		for (int k = 0; k < boards.Length; k++) {
			if (boards [k].GetComponent<GameBoard> ().Intersections.Count != 0) {
				foreach (int intersectionID in intersections) {
					intersectionsList.Add(boards [k].GetComponent<GameBoard> ().Intersections[intersectionID]);
				}

				List<GameTile> potentialNeighbors = new List<GameTile>();

				foreach (Intersection corner in intersectionsList) {
					foreach (int neighborID in corner.adjacentTiles) {
						potentialNeighbors.Add(boards [k].GetComponent<GameBoard> ().GameTiles[neighborID]);
					}
				}

				foreach (GameTile neighborTile in potentialNeighbors) {
					if (neighborTile != this && !neighborTiles.Contains(neighborTile)) {
						neighborTiles.Add (neighborTile);
					}
				}
			}
		}


		return neighborTiles;
	}

	public void setTileType(int tileType) {
		this.tileType = (TileType)tileType;
		Material newMaterial = GameObject.FindGameObjectWithTag ("TileSettings").GetComponent<TileTypeSettings> ().
			getMaterialsDictionary () [(TileType)tileType];
		this.GetComponent<Renderer> ().material = newMaterial;

		if (this.tileType == TileType.Desert || this.tileType == TileType.Ocean) {
			this.transform.FindChild ("Dice Value").gameObject.SetActive (false);
		} 
	}

	public void setDiceValue(int diceValue) {
		if (this.tileType == TileType.Desert || this.tileType == TileType.Ocean) {
			this.transform.FindChild ("Dice Value").gameObject.SetActive (false);
		} else {
			this.transform.FindChild ("Dice Value").gameObject.SetActive (true);
			TextMesh randomDiceValue = this.transform.FindChild ("Dice Value").gameObject.GetComponentInChildren<TextMesh>();
			string diceValueString = diceValue.ToString () + "\n";

			for (int k = Mathf.Abs (diceValue - 7); k < 6; k++) {
				diceValueString = diceValueString + ".";
			}

			randomDiceValue.text = diceValueString;

			if (diceValue == 6 || diceValue == 8) {
				randomDiceValue.color = Color.red;
			} else {
				randomDiceValue.color = Color.black;
			}

			this.diceValue = diceValue;
			//Debug.Log ("I set " + this.name + "'s dice vallue to " + this.diceValue);
		}
	}

	public bool canProduce() {
		if (occupier == null || !typeof(Robber).IsAssignableFrom (occupier.GetType ())) {
			return true;
		} else {
			Debug.Log (this.name + " can not produce resource/commodities because it is blocked by robber");
			return false;
		}
	}

	public bool isPossibleIslandTile() {
		if (this.tileType != TileType.Ocean) {
			return false;
		}

		foreach (GameTile neighborTile in getNeighborTiles()) {
			if (!neighborTile.atIslandLayer) {
				return false;
			}
		}

		return true;
	}
}

public enum TileType {
	Hills = 0,
	Fields,
	Forests,
	Mountains,
	Pastures,
	Desert,
	Ocean
}
