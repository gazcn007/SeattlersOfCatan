using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : Tile {
	
	public int diceValue;

	private TileType tileType;
	private int id;

	private List<Edge> edges;
	private List<Intersection> intersections;

	// Use this for initialization
	void Start () {
		edges = new List<Edge> ();
		intersections = new List<Intersection> ();
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
