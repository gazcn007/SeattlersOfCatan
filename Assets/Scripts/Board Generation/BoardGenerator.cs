using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {

	//Map settings
	public MapShape mapShape = MapShape.Hexagon;
	public int mapWidth = 3;
	public int mapHeight = 3;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Pointy;
	public float hexRadius = 1;

	public List<GameTile> allTiles;
	public List<GameTile> landTiles;

	private TileTypeSettings hexSettings;
	private GameBoard board;

	private BoardDecorator boardDecorator;

	private int numRepaints = 0;

	private void Start() {
		
	}

	void Update() {
		
	}

	public void GenerateBoard() {
		board = GetComponentInChildren<GameBoard> ();
		hexSettings = GetComponent<TileTypeSettings> ();

		//Set grid settings
		board.mapShape = mapShape;
		board.mapWidth = mapWidth;
		board.mapHeight = mapHeight;
		board.hexOrientation = hexOrientation;
		GameBoard.hexRadius = hexRadius;

		//Generate Grid
		board.GenerateTiles();
		board.GenerateIntersections ();
		board.GenerateEdges ();

		//Set grid properties
		//board.setTileTypesAndValues (hexSettings);
		boardDecorator = new BoardDecorator(board, hexSettings);
		allTiles = boardDecorator.allTiles;
		landTiles = boardDecorator.landTiles;
	}

	public void paintBoard() {
		allTiles = GameBoard.getTiles();

		boardDecorator.paintLandTilesBySettings (allTiles, hexSettings);
		landTiles = boardDecorator.findLandTiles (allTiles);
		boardDecorator.setTileIDsBySettings (landTiles, hexSettings);
	}

	public List<GameTile> getOceanTiles() {
		return allTiles.Except (landTiles).ToList ();
	}

}
