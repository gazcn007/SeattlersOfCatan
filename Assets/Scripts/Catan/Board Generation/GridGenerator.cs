using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {
	
	public Material hexMaterial; //Assigned in inspector
	public Material lineMaterial; //Assigned in inspector

	private Grid board;

	private void Start() {
		board = GetComponentInChildren<Grid> ();
		//Set grid settings
		board.mapShape = MapShape.Hexagon;
		board.mapWidth = 3;
		board.mapHeight = 3;
		board.hexOrientation = HexOrientation.Pointy;
		board.hexRadius = 1;
		board.hexMaterial = hexMaterial;
		board.addColliders = true;
		board.drawOutlines = false;
		board.lineMaterial = lineMaterial;

		//Gen Grid
		board.GenerateGrid();
	}
}
