using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

	private List<GameTile> adjacentTiles;
	public int id;

	// Use this for initialization
	public Edge () {
		adjacentTiles = new List<GameTile>(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addTile(GameTile tile) {
		adjacentTiles.Add (tile);
	}

	public List<GameTile> getAdjacentTiles() {
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

}
