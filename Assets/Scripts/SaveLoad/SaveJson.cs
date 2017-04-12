using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;
using System.Linq;
using Persistence;

public static class SaveJson {


	static FileStream filestream;

	public static void saveJson (string fileToWrite)
	{	
		// write to current filenameIndex file
		pe_SavefileNames pe_savefileNames = new pe_SavefileNames();
		string[] savefileNames = LoadJson.loadSavefileNames().pe_savefilenames;
		string[] newSavefileNames;
		if (savefileNames.Contains (fileToWrite)) {
			newSavefileNames = savefileNames;
		} else {
			newSavefileNames = new string[savefileNames.Length + 1];
			savefileNames.CopyTo (newSavefileNames, 0);
			newSavefileNames [savefileNames.Length] = fileToWrite;
		}
		pe_savefileNames.pe_savefilenames = newSavefileNames;
		writeToJson (JsonMapper.ToJson(pe_savefileNames), "savefileIndex.json");

		pe_GameState gameState = new pe_GameState();
		gameState.eventTransferManager = saveEventTransferManager ();
		gameState.players = savePlayers();
		gameState.gameBoard = saveGameBoard ();
		gameState.units = saveUnits ();
		gameState.progressCardStack = saveProgressCardStack ();
		string jsonresult = JsonMapper.ToJson(gameState);
		Debug.Log (jsonresult);
		writeToJson (jsonresult, fileToWrite+".json");

	}

	public static pe_ETM saveEventTransferManager(){
		pe_ETM pe_etm = new pe_ETM ();
		EventTransferManager eventTransferManager = GameObject.FindGameObjectWithTag ("EventTransferManager").GetComponent<EventTransferManager> ();
		pe_etm.currentPlayerTurn = eventTransferManager.currentPlayerTurn;
		pe_etm.currentActiveButton = eventTransferManager.currentActiveButton;
		pe_etm.setupPhase = eventTransferManager.setupPhase;
		pe_etm.waitingForPlayer = eventTransferManager.waitingForPlayer;
		pe_etm.diceRolledThisTurn = eventTransferManager.diceRolledThisTurn;
		pe_etm.shipMovedThisTurn = eventTransferManager.shipMovedThisTurn;
		pe_etm.waitingforcards = eventTransferManager.waitingforcards;
		pe_etm.waitingForPlayers = eventTransferManager.waitingForPlayers;
		pe_etm.playerChecks = eventTransferManager.playerChecks;
		return pe_etm;
	}

	public static pe_Players savePlayers(){
		// create a object
		pe_Players p_players = new pe_Players();
		p_players.total = GameObject.Find ("NetworkManager(Clone)").GetComponent<NetworkManager> ().numPlayersSet+1;
		p_players.playerArray = new pe_Player[4];
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		Player[] playerGOs = clientCatanManager.players.ToArray();
		for (int i = 0; i < 4; i++) {
			p_players.playerArray[i] = new Persistence.pe_Player();
			Player player = playerGOs [i].GetComponent<Player> ();
			p_players.playerArray[i].playerName = player.playerName;
			p_players.playerArray[i].playerNumber = player.playerNumber;
			p_players.playerArray[i].victoryPoints = player.victoryPoints;
			p_players.playerArray [i].assets = new int[12];
			p_players.playerArray [i].assets [0] = player.assets.GetValueAtIndex(0);
			p_players.playerArray [i].assets [1] = player.assets.GetValueAtIndex(1);
			p_players.playerArray [i].assets [2] = player.assets.GetValueAtIndex(2);
			p_players.playerArray [i].assets [3] = player.assets.GetValueAtIndex(3);
			p_players.playerArray [i].assets [4] = player.assets.GetValueAtIndex(4);
			p_players.playerArray [i].assets [5] = player.assets.GetValueAtIndex(5);
			p_players.playerArray [i].assets [6] = player.assets.GetValueAtIndex(6);
			p_players.playerArray [i].assets [7] = player.assets.GetValueAtIndex(7);
			p_players.playerArray [i].assets [8] = player.assets.GetValueAtIndex(8);
			p_players.playerArray [i].assets [9] = player.assets.GetValueAtIndex(9);
			p_players.playerArray [i].assets [10] = player.assets.GetValueAtIndex(10);
			p_players.playerArray [i].assets [11] = player.assets.GetValueAtIndex(11);
			//p_players.playerArray[0].cityImprovements = playerGOs [0].GetComponent<Player> ().CityImprovementTuple;
			p_players.playerArray [i].avatar = player.avatar.name;
			p_players.playerArray [i].progressCards= player.progressCards.Select(x=>(int)x).ToArray();
		}
		return p_players;
	}

	public static pe_GameBoard saveGameBoard(){
		pe_GameBoard pe_gameBoard = new pe_GameBoard ();
		GameBoard gameBoard = GameObject.FindGameObjectWithTag ("Board").GetComponent<GameBoard> ();
		pe_gameBoard.mapShape = (int)gameBoard.mapShape;
		pe_gameBoard.mapWidth = gameBoard.mapWidth;
		pe_gameBoard.mapHeight = gameBoard.mapHeight;
		pe_gameBoard.hexOrientation = (int)gameBoard.hexOrientation;
		pe_gameBoard.hexRadius = gameBoard.hexRadius;

		// ++++++++++ create Game Tiles array ++++++++++++++
		pe_gameBoard.pe_gameTiles = new pe_GameTile[gameBoard.GameTiles.Count];
		foreach(KeyValuePair<int, GameTile> entry in gameBoard.GameTiles){
			pe_gameBoard.pe_gameTiles [entry.Key] = new pe_GameTile ();
			pe_gameBoard.pe_gameTiles [entry.Key].index = new int[3];
			pe_gameBoard.pe_gameTiles [entry.Key].index [0] = entry.Value.index.x;
			pe_gameBoard.pe_gameTiles [entry.Key].index [1] = entry.Value.index.y;
			pe_gameBoard.pe_gameTiles [entry.Key].index [2] = entry.Value.index.z;
			pe_gameBoard.pe_gameTiles [entry.Key].diceValue = entry.Value.diceValue;
			pe_gameBoard.pe_gameTiles [entry.Key].tileType = (int)entry.Value.tileType; // get enum's mapping int
			pe_gameBoard.pe_gameTiles [entry.Key].id = entry.Value.id;
			pe_gameBoard.pe_gameTiles [entry.Key].atIslandLayer = entry.Value.atIslandLayer;
			pe_gameBoard.pe_gameTiles [entry.Key].hasHarbor = entry.Value.hasHarbor;
			pe_gameBoard.pe_gameTiles [entry.Key].edges = entry.Value.edges.ToArray();
			pe_gameBoard.pe_gameTiles [entry.Key].intersections = entry.Value.intersections.ToArray();
			if (entry.Value.fishTile == null) {
				pe_gameBoard.pe_gameTiles [entry.Key].fishTile = null;
			} else {
				pe_gameBoard.pe_gameTiles [entry.Key].fishTile = new pe_FishTile ();
				pe_gameBoard.pe_gameTiles [entry.Key].fishTile.id = entry.Value.fishTile.id;
				pe_gameBoard.pe_gameTiles [entry.Key].fishTile.locationTileId= entry.Value.fishTile.locationTile.id;
				pe_gameBoard.pe_gameTiles [entry.Key].fishTile.diceValue= entry.Value.fishTile.diceValue;
			}
			if (entry.Value.occupier == null) {
				pe_gameBoard.pe_gameTiles [entry.Key].occupier = null;
			} else {
				pe_gameBoard.pe_gameTiles [entry.Key].occupier = new pe_GamePiece ();
				pe_gameBoard.pe_gameTiles [entry.Key].occupier.isActive = entry.Value.occupier.isActive;
				pe_gameBoard.pe_gameTiles [entry.Key].occupier.location = entry.Value.occupier.location;
				pe_gameBoard.pe_gameTiles [entry.Key].occupier.occupyingTileId = entry.Value.occupier.occupyingTile.id;
			}
		}

		// ++++++++++ create Intersections array ++++++++++++++
		pe_gameBoard.pe_intersections = new pe_Intersection[gameBoard.Intersections.Count];
		foreach (KeyValuePair<int, Intersection> entry in gameBoard.Intersections) {
			pe_gameBoard.pe_intersections [entry.Key] = new pe_Intersection ();
			pe_gameBoard.pe_intersections [entry.Key].adjacentTiles = entry.Value.adjacentTiles.ToArray();
			pe_gameBoard.pe_intersections [entry.Key].linkedEdges = entry.Value.linkedEdges.ToArray ();
			pe_gameBoard.pe_intersections [entry.Key].neighborIntersections = entry.Value.neighborIntersections.ToArray ();
			pe_gameBoard.pe_intersections [entry.Key].id = entry.Value.id;
		}

		// ++++++++++ create Edges array ++++++++++++++
		pe_gameBoard.pe_edges = new pe_Edge[gameBoard.Edges.Count];
		foreach (KeyValuePair<int, Edge> entry in gameBoard.Edges) {
			pe_gameBoard.pe_edges [entry.Key] = new pe_Edge ();
			pe_gameBoard.pe_edges [entry.Key].adjacentTiles = entry.Value.adjacentTiles.ToArray();
			pe_gameBoard.pe_edges [entry.Key].linkedIntersections = entry.Value.linkedIntersections.ToArray ();
			pe_gameBoard.pe_edges [entry.Key].id = entry.Value.id;
		}

		// ++++++++++ create FileTile array +++++++++++++
		pe_gameBoard.pe_fishTiles = new pe_FishTile[gameBoard.FishTiles.Count];
		foreach (KeyValuePair<int, FishTile> entry in gameBoard.FishTiles) {
			pe_gameBoard.pe_fishTiles [entry.Key] = new pe_FishTile ();
			pe_gameBoard.pe_fishTiles [entry.Key].id = entry.Value.id;
			pe_gameBoard.pe_fishTiles [entry.Key].locationTileId = entry.Value.locationTile.id;
			pe_gameBoard.pe_fishTiles [entry.Key].diceValue = (int)entry.Value.diceValue;
		}

		// ++++++++++ create Harbor array +++++++++++++ Nehir Hard Coded
		//		pe_gameBoard.pe_harbors = new pe_Harbor[gameBoard.Harbors.Count];
		//		foreach (KeyValuePair<int, Harbor> entry in gameBoard.Harbors) {
		//			pe_gameBoard.pe_harbors [entry.Key] = new pe_Harbor ();
		//			pe_gameBoard.pe_harbors [entry.Key].id = entry.Value.id;
		//			pe_gameBoard.pe_harbors [entry.Key].resourceType = (int)entry.Value.resourceType;
		//			pe_gameBoard.pe_harbors [entry.Key].commodityType = (int)entry.Value.commodityType;
		//			pe_gameBoard.pe_harbors [entry.Key].locations = entry.Value.locations.Select(x=>x.id).ToArray(); //Intersection Id essentially
		//			pe_gameBoard.pe_harbors [entry.Key].tradeRatio = entry.Value.tradeRatio;
		//			Debug.Log("Harbor KEY :" + entry.Key);
		//		}

		return pe_gameBoard;
	}

	public static pe_Units saveUnits(){
		UnitManager unitManager = GameObject.FindGameObjectWithTag ("UnitManager").GetComponent<UnitManager> ();

		pe_Units pe_units = new pe_Units ();
		pe_units.unitsInPlay = new pe_Unit[unitManager.unitsInPlay.Count];
		for (int i = 0; i < unitManager.unitsInPlay.Count; i++) {
			pe_units.unitsInPlay [i] = new pe_Unit ();
			pe_units.unitsInPlay [i].id = unitManager.unitsInPlay [i].id;
			pe_units.unitsInPlay [i].name = unitManager.unitsInPlay [i].name;
			pe_units.unitsInPlay [i].ownerPlayerNumber = unitManager.unitsInPlay [i].owner.playerNumber-1;
			pe_units.unitsInPlay [i].tag = unitManager.unitsInPlay [i].tag;
			pe_units.unitsInPlay [i].type = unitManager.unitsInPlay [i].GetType ().Name;
			if (unitManager.unitsInPlay [i].GetType ().BaseType.Name == "EdgeUnit") {
				pe_units.unitsInPlay [i].locationId = ((EdgeUnit)unitManager.unitsInPlay [i]).locationEdge.id;
			} else {
				pe_units.unitsInPlay [i].locationId = ((IntersectionUnit)unitManager.unitsInPlay [i]).locationIntersection.id;
			} 
			pe_units.unitsInPlay [i].victoryPointsWorth = unitManager.unitsInPlay [i].victoryPointsWorth;
		}
		return pe_units;
	}

	public static pe_ProgressCardStack saveProgressCardStack(){
		ProgressCardStackManager progressCardStackManager = GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ();

		pe_ProgressCardStack pe_progressCardStack = new pe_ProgressCardStack ();
		pe_progressCardStack.yellowCards = (int[]) progressCardStackManager.yellowCardsQueue.Select(x=>(int)x).ToArray ();
		pe_progressCardStack.blueCards = (int[]) progressCardStackManager.blueCardsQueue.Select(x=>(int)x).ToArray ();
		pe_progressCardStack.greenCards = (int[]) progressCardStackManager.greenCardsQueue.Select(x=>(int)x).ToArray ();

		return pe_progressCardStack;
	}

	public static void writeToJson(string jsonString, string filePath){
		if (!File.Exists (filePath)) {
			filestream = File.Create (filePath);
			filestream.Close ();
			filestream.Dispose ();
		}

		using (StreamWriter writer = new StreamWriter (filePath, false, Encoding.UTF8)) {
			writer.WriteLine (jsonString);
		}
	}
}
