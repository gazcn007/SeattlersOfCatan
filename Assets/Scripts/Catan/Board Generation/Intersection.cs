using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Intersection : MonoBehaviour {

	public List<int> adjacentTiles;
	public List<int> linkedEdges;
	public List<int> neighborIntersections;
	public int id;
	public IntersectionUnit occupier;
	//public int issettleableint = 0;

	public Harbor harbor = null;

	// Use this for initialization
	public static float AngleOfCorner(int corner, HexOrientation orientation){
		float angle = 210 - 60 * corner;
		if(orientation == HexOrientation.Flat)
			angle += 30;
		return angle;
	}

	public Intersection() {
		adjacentTiles = new List<int>();
		linkedEdges = new List<int> ();
		neighborIntersections = new List<int> ();
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addLinkedEdge(Edge e) {
		linkedEdges.Add (e.id);
	}

	public void addNeighborIntersection(Intersection i) {
		neighborIntersections.Add (i.id);
	}

	public List<Intersection> getNeighborIntersections() {
		List<Intersection> linkedIntersections = new List<Intersection> ();

		foreach (int i in neighborIntersections) {
			linkedIntersections.Add (GameBoard.instance.Intersections [i]);
		}

		return linkedIntersections;
	}

	public List<Edge> getLinkedEdges() {
		List<Edge> linkEdges = new List<Edge> ();

		foreach (int i in linkedEdges) {
			linkEdges.Add (GameBoard.instance.Edges [i]);
		}

		return linkEdges;
	}

	private int landTilesCount() {
		int landTiles = 0;
		for (int i = 0; i < adjacentTiles.Count; i++) {
			if (GameBoard.instance.GameTiles[adjacentTiles [i]].tileType != TileType.Ocean) {
				landTiles++;
			}
		}
		//issettleableint = landTiles;
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
		adjacentTiles.Add (tile.id);
	}

	public List<GameTile> getAdjacentTiles() {
		List<GameTile> adjTiles = new List<GameTile> ();

		foreach (int i in adjacentTiles) {
			adjTiles.Add (GameBoard.instance.GameTiles [i]);
		}

		return adjTiles;
	}

	public void setID(int id) {
		if (id >= 0) {
			this.id = id;
		}
	}

	public int getID() {
		return id;
	}

	public Intersection getShoreNeighbor() {
		Intersection shoreNeighbor = null;

		foreach (Intersection neighbor in getNeighborIntersections()) {
			if (neighbor.isShoreIntersection ()) {
				shoreNeighbor = neighbor;
			}
		}

		return shoreNeighbor;
	}

	public GameTile getCommonTileWith(Intersection other, TileType tileType) {
		if (this.getNeighborIntersections().Contains (other)) {
			List<GameTile> commonTiles = this.getAdjacentTiles().Intersect (other.getAdjacentTiles()).ToList ();

			foreach (GameTile tile in commonTiles) {
				if (tile.tileType == tileType) {
					return tile;
				}
			}
		}
		return null;
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

	public bool isNotNeighboringIntersectionWithUnit() {
		bool notNeighboring = true;

		foreach (Intersection neighbor in getNeighborIntersections()) {
			if (neighbor.occupier != null) {
				notNeighboring = false;
			}
		}

		return notNeighboring;
	}
}
