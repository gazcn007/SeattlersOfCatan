using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : Tile {
	
	public int diceValue = -1;
	public TileType tileType;
	public int id;

	public List<Edge> edges  = new List<Edge> ();
	public List<Intersection> intersections = new List<Intersection> ();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEdge(Edge edge) {
		edges.Add(edge);
	}

	public void addIntersection(Intersection intersection) {
		intersections.Add (intersection);
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
