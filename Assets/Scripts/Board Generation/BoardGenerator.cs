using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {

	//Map settings
	public MapShape mapShape = MapShape.Hexagon;
	public int mapWidth = 3;
	public int mapHeight = 3;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Pointy;
	public float hexRadius = 1;

	//Generation Options
	public bool addColliders = true;
	public bool drawOutlines = false;

	//public Material[] hexMaterials = new Material[7];
	public Material lineMaterial; //Assigned in inspector
	private TileTypeSettings hexSettings;

	private GameBoard board;
	private int numTiles;

	private void Start() {
		board = GetComponentInChildren<GameBoard> ();
		hexSettings = GetComponent<TileTypeSettings> ();
		hexSettings.setMapWidthHeight (mapWidth, mapHeight);

		//hexSettings.debugPrintDict ();

		//Set grid settings
		board.mapShape = mapShape;
		board.mapWidth = mapWidth;
		board.mapHeight = mapHeight;
		board.hexOrientation = hexOrientation;
		board.hexRadius = hexRadius;
		//board.hexMaterials = hexMaterials;
		//board.setHexSettings(hexSettings);
		board.addColliders = addColliders;
		board.drawOutlines = drawOutlines;
		board.lineMaterial = lineMaterial;

		//Gen Grid
		board.GenerateTiles();
		board.paintTiles (hexSettings);
		board.GenerateIntersections ();
		board.GenerateEdges ();

		//hexSettings.debugPrintDict ();
	}

}
