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
		board.hexRadius = hexRadius;

		//Generate Grid
		board.GenerateTiles(this.transform);
		board.GenerateIntersections (this.transform);
		board.GenerateEdges (this.transform);

		//Set grid properties
		//board.setTileTypesAndValues (hexSettings);
		boardDecorator = new BoardDecorator(board, hexSettings);
		allTiles = boardDecorator.allTiles;
		landTiles = boardDecorator.landTiles;

		board.GenerateHarbors ();
	}

	public void paintBoard() {
		allTiles = board.getTiles();

		boardDecorator.paintLandTilesBySettings (allTiles, hexSettings);
		landTiles = boardDecorator.findLandTiles (allTiles);
		boardDecorator.setTileIDsBySettings (landTiles, hexSettings);
	}

	public List<GameTile> getOceanTiles() {
		return allTiles.Except (landTiles).ToList ();
	}

}
