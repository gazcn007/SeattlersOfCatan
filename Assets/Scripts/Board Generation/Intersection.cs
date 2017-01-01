using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour {

	public HashSet<GameTile> adjacentTiles;
	public List<Edge> linkedEdges;
	public List<Intersection> neighborIntersections;
	public int id;
	public IntersectionUnit occupier;

	// Use this for initialization

	public static float AngleOfCorner(int corner, HexOrientation orientation){
		float angle = 210 - 60 * corner;
		if(orientation == HexOrientation.Flat)
			angle += 30;
		return angle;
	}

	public Intersection() {
		adjacentTiles = new HashSet<GameTile>();
		linkedEdges = new List<Edge> ();
		neighborIntersections = new List<Intersection> ();
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addLinkedEdge(Edge e) {
		linkedEdges.Add (e);
	}

	public void addNeighborIntersection(Intersection i) {
		neighborIntersections.Add (i);
	}

	public List<Intersection> getNeighborIntersections() {
		List<Intersection> clone = new List<Intersection> (neighborIntersections);
		return clone;
	}

	public List<Edge> getLinkedEdges() {
		List<Edge> clone = new List<Edge> (linkedEdges);
		return clone;
	}

	private int landTilesCount() {
		int landTiles = 0;
		foreach (var tile in adjacentTiles) {
			if (tile.tileType != TileType.Ocean) {
				landTiles++;
			}
		}

		return landTiles;
	}

	public bool isSettleable() {
		if (landTilesCount () > 0) {
			return true;
		} else {
			return false;
		}
	}

	public bool isSeaIntersection() {
		return !isSettleable ();
	}

	public bool isShoreIntersection() {
		int landTiles = landTilesCount ();

		if (landTiles > 0 && landTiles < adjacentTiles.Count) {
			return true;
		} else {
			return false;
		}
	}

	public void addTile(GameTile tile) {
		adjacentTiles.Add (tile);
	}

	public HashSet<GameTile> getAdjacentTiles() {
		return adjacentTiles;
	}

	public void setID(int id) {
		if (id >= 0) {
			this.id = id;
		}
	}

	public int getID() {
		return id;
	}

	public void highlightIntersection(bool highlight) {
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		//if (occupier == null) {
			renderer.enabled = highlight;
		//}
		if (occupier != null && occupier.owner != null) {
			renderer.material.color = occupier.owner.playerColor;
		}
	}

	public void highlightIntersectionWithColor(bool highlight, Color color) {
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		if (occupier == null) {
			renderer.enabled = highlight;
			renderer.material.color = color;
		}
	}
}
