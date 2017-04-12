using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : EdgeUnit {

	public bool builtThisTurn = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void moveShip() {

	}

	public override bool isRoad() {
		return false;
	}

	public bool canMove() {
		if (builtThisTurn) {
			return false;
		}

		foreach (var adjacentTile in this.locationEdge.getAdjacentTiles()) {
			if (adjacentTile.occupier != null && adjacentTile.occupier.GetType () == typeof(Pirate)) {
				return false;
			}
		}

		List<Intersection> linkedIntersectionsList = this.locationEdge.getLinkedIntersections ();

		foreach (var linkIntersection in linkedIntersectionsList) {
			if (linkIntersection.occupier == null) {
				List<Edge> edgesOfLinkIntersection = linkIntersection.getLinkedEdges ();
				int occupierCount = 0;

				foreach (var neighborEdge in edgesOfLinkIntersection) {
					if (neighborEdge.occupier != null && neighborEdge.occupier.owner == this.owner) {
						occupierCount++;
					}
				}

				if (occupierCount == 1) {
					return true;
				}
			}
		}

		return false;
	}

	public int getNumNeighborUnits() {
		List<Intersection> linkedIntersectionsList = this.locationEdge.getLinkedIntersections ();
		List<Edge> neighborEdgesList = this.locationEdge.getNeighborEdges ();

		int numNeighborUnits = 0;

		foreach (var intersection in linkedIntersectionsList) {
			if (intersection.occupier != null && intersection.occupier.owner == this.owner) {
				numNeighborUnits++;
			}
		}

		foreach (var neighborEdge in neighborEdgesList) {
			if (neighborEdge.occupier != null && neighborEdge.occupier.owner == this.owner) {
				numNeighborUnits++;
			}
		}

		return numNeighborUnits;
	}
}
