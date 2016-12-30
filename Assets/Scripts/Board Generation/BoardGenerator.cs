﻿using System.Collections;
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

	private TileTypeSettings hexSettings;
	private GameBoard board;

	private BoardDecorator boardDecorator;

	private void Start() {
		board = GetComponentInChildren<GameBoard> ();
		hexSettings = GetComponent<TileTypeSettings> ();
		//hexSettings.setMapWidthHeight (mapWidth, mapHeight);

		//Set grid settings
		board.mapShape = mapShape;
		board.mapWidth = mapWidth;
		board.mapHeight = mapHeight;
		board.hexOrientation = hexOrientation;
		board.hexRadius = hexRadius;

		//Generate Grid
		board.GenerateTiles();
		board.GenerateIntersections ();
		board.GenerateEdges ();

		//Set grid properties
		//board.setTileTypesAndValues (hexSettings);
		boardDecorator = new BoardDecorator(board, hexSettings);

	}

}
