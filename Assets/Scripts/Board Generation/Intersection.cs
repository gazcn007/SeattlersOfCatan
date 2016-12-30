using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour {

	public HashSet<GameTile> adjacentTiles;
	public int id;

	// Use this for initialization

	public static float AngleOfCorner(int corner, HexOrientation orientation){
		float angle = 210 - 60 * corner;
		if(orientation == HexOrientation.Flat)
			angle += 30;
		return angle;
	}

	public Intersection() {
		adjacentTiles = new HashSet<GameTile>();
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
