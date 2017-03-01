using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

	private List<GameTile> adjacentTiles;
	private List<Intersection> linkedIntersections;
	public int id;
	public TradeUnit occupier;

	// Use this for initialization
	public Edge () {
		adjacentTiles = new List<GameTile>(2);
		linkedIntersections = new List<Intersection> (2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addTile(GameTile tile) {
		adjacentTiles.Add (tile);
	}

	public void addIntersection(Intersection intersection) {
		linkedIntersections.Add (intersection);
	}

	public List<GameTile> getAdjacentTiles() {
		List<GameTile> clone = new List<GameTile> (adjacentTiles);
		return clone;
	}

	public List<Intersection> getLinkedIntersections() {
		List<Intersection> clone = new List<Intersection> (linkedIntersections);
		return clone;
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

		for (int i = 0; i < adjacentTiles.Count; i++) {
			if (adjacentTiles [i].tileType != TileType.Ocean) {
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
