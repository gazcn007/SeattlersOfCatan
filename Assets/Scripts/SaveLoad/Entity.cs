﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Persistence
{
	public class pe_SavefileNames{
		public string[] pe_savefilenames; 	
	}

	public class pe_GameState{
		public pe_ETM eventTransferManager;
		public pe_Players players;
		public pe_GameBoard gameBoard;
		public pe_Units units;
		public pe_ProgressCardStack progressCardStack;
	}

	public class pe_ETM{
		public int currentPlayerTurn;
		public int currentActiveButton;
		public bool setupPhase;
		public bool waitingForPlayer;
		public bool diceRolledThisTurn;
		public bool shipMovedThisTurn;
		public bool waitingforcards;
		public bool waitingForPlayers;
		public bool[] playerChecks;	
	}

	public class pe_Players{
		public int total;
		public pe_Player[] playerArray;
	}

	public class pe_Player{
		public string playerName;
		public string playerColor;
		public int playerNumber;
		public int goldCoins;
		public int victoryPoints;
		//		public Dictionary<System.Type, List<Unit>> ownedUnits; -> will be injected by units on the way they were generating
		public int[] assets;
		public CityImprovementTuple cityImprovements;
		public bool collectedThisTurn;// didn't use in class
		public string avatar;
		public int[] progressCards;
	}

	public class pe_GameBoard{

		//Map settings
		public int mapShape; //= MapShape.Rectangle;
		public int mapWidth;
		public int mapHeight;

		//Hex Settings
		public int hexOrientation;// = HexOrientation.Flat;
		public float hexRadius = 1;

		// ---- Game Tiles ----
		public pe_GameTile[] pe_gameTiles;

		// ---- Intersections ----
		public pe_Intersection[] pe_intersections;

		// ---- Edges ----
		public pe_Edge[] pe_edges;

		// ---- FishTiles ----
		public pe_FishTile[] pe_fishTiles;

		// ---- Harbors ----
		//		public pe_Harbor[] pe_harbors;
	}

	// +++++++++++++++ Game Tiles ++++++++++++++++
	public class pe_GameTile{
		// index[0] -> X, index[1] -> Y, index[2] -> Z
		public int[] index;
		// dice value
		public int diceValue;
		// tile type -> enum
		public int tileType;
		// Id
		public int id;
		// At Island Layer
		public bool atIslandLayer;
		// Has Harbor
		public bool hasHarbor;
		// Edges is an array with Neighbor Edge Ids, the information Size can be regenerated by edges.length;
		public int[]edges;
		// Intersection is an array with Neighbor Intersections Ids, the information Size can be regenerated by edges.length;
		public int[]intersections;

		public pe_FishTile fishTile;
		public pe_GamePiece occupier;
	}

	public class pe_FishTile {
		public int id;
		public int locationTileId; // the id to Tile where this FishTile is on
		public int diceValue;
	}

	//	public class pe_Harbor {
	//		public int id;
	//		public int resourceType;
	//		public int commodityType;
	//		public int[] locations;
	//		public int tradeRatio;
	//	}
	//
	public class pe_GamePiece{
		public bool isActive;
		public Vector3 location;
		public int occupyingTileId; // the id to Tile which this GamePiece occupies
	}

	// +++++++++++++ Intersections ++++++++++++++++
	public class pe_Intersection{
		// ids to adjacentTiles
		public int[] adjacentTiles;
		// ids to linkedEdges
		public int[] linkedEdges;
		// ids to neighborIntersections
		public int[] neighborIntersections;
		// id
		public int id;

	}

	//+++++++++++++++++++ Edges +++++++++++++++++++
	public class pe_Edge{
		public int[] adjacentTiles;
		public int[] linkedIntersections;
		public int id;
	}

	//+++++++++++++++++++ Units+++++++++++++++++++
	public class pe_Units{
		public pe_Unit[] unitsInPlay;
	}

	public class pe_Unit{
		//		public bool enable;
		public int id;
		public string name;
		public int ownerPlayerNumber;//owner.playerName
		public string tag;
		public string type;
		public int locationId; //Depends on the tag, say 3, when tag is Edge, means on Edge with id 3
		public int victoryPointsWorth;
	}

	//+++++++++++++++++++ Progress Card Manager State +++++++++++++++++++
	public class pe_ProgressCardStack{
		public int[] yellowCards;
		public int[] blueCards;
		public int[] greenCards;
	}

	public class PE_Robber{
	}
	public class PE_Pirate{

	}
	public class PE_Merchant{

	}
	public enum MapShape {
		Rectangle,
		Hexagon,
		Parrallelogram,
		Triangle
	}
	public enum HexOrientation {
		Pointy,
		Flat
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

}
