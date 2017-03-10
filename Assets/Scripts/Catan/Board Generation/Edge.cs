using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

	public List<int> adjacentTiles;
	public List<int> linkedIntersections;
	public int id;
	public EdgeUnit occupier;

	// Use this for initialization
	public Edge () {
		adjacentTiles = new List<int>(2);
		linkedIntersections = new List<int> (2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addTile(GameTile tile) {
		adjacentTiles.Add (tile.id);
	}

	public void addIntersection(Intersection intersection) {
		linkedIntersections.Add (intersection.id);
	}

	public List<GameTile> getAdjacentTiles() {
		List<GameTile> adjTiles = new List<GameTile> ();

		foreach (int i in adjacentTiles) {
			adjTiles.Add (GameBoard.instance.GameTiles [i]);
		}

		return adjTiles;
	}

	public List<Intersection> getLinkedIntersections() {
		List<Intersection> linkIntersections = new List<Intersection> (linkedIntersections.Count);

		foreach(int i in linkedIntersections) {
			linkIntersections.Add (GameBoard.instance.Intersections [i]);
		}

		return linkIntersections;
	}

	public void setID(int id) {
		if (id >= 0) {
			this.id = id;
		}
	}

	public int getID() {
		return id;
	}

	public bool isSeaEdge() {
		if (landTilesCount () < 1) {
			return true;
		} else {
			return false;
		}
	}

	public bool isShoreEdge() {
		if (landTilesCount () == 1) {
			return true;
		} else {
			return false;
		}
	}

	public bool isLandEdge() {
		if (landTilesCount () > 1) {
			return true;
		} else {
			return false;
		}
	}

	private int landTilesCount() {
		int landTiles = 0;

		for (int i = 0; i < getAdjacentTiles().Count; i++) {
			if (getAdjacentTiles() [i].tileType != TileType.Ocean) {
				landTiles++;
			}
		}
		return landTiles;
	}

	public void highlightEdge(bool highlight) {
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		//if (occupier == null) {
			renderer.enabled = highlight;
		//}
		if (occupier != null && occupier.owner != null) {
			renderer.material.color = occupier.owner.playerColor;
		}
	}

	public void highlightEdgeWithColor(bool highlight, Color color) {
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		if (occupier == null) {
			renderer.enabled = highlight;
			renderer.material.color = color;
		}
	}
}
