using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardDecorator {

	public GameTile desertTile;

	private TileTypeSettings hexSettings;
	private GameBoard gameBoard;

	private int mapWidth;
	private int mapHeight;

	public List<GameTile> allTiles;
	public List<GameTile> landTiles;

	// Use this for initialization
	public BoardDecorator(GameBoard board, TileTypeSettings settings) {
		gameBoard = board;
		hexSettings = settings;

		mapWidth = gameBoard.getMapWidth ();
		mapHeight = gameBoard.getMapHeight ();
		hexSettings.setMapWidthHeight (mapWidth, mapHeight);

		allTiles = board.getTiles();
		//hexSettings.setSettingAccordingToNumTiles (allTiles.Count, landTiles.Count);
		paintOceanTiles (allTiles, settings);
		landTiles = findLandTiles (allTiles);
		hexSettings.setTileTypeNumbersByTotalNumberOfHexes (landTiles.Count);
		paintLandTilesBySettings (allTiles, hexSettings);

		hexSettings.setDiceProbabilitiesByTotalNumberOfHexes (landTiles.Count);
		setTileIDsBySettings (landTiles, hexSettings);

		board.placeRobberOnTile (desertTile.id + 1);

		generateIslands ();

		//List<GameTile> pirateTiles = getTilesForPirate ();
		//board.placePirateOnTile (pirateTiles [Random.Range(0, pirateTiles.Count)].id);
	}

	public List<GameTile> findLandTiles(List<GameTile> allTiles) {
		List<GameTile> landTiles = new List<GameTile>();

		for(int i = 0; i < allTiles.Count; i++) {
			if (allTiles [i].tileType != TileType.Ocean && allTiles [i].tileType != TileType.Desert) {
				landTiles.Add (allTiles[i]);
			}
		}

		return landTiles;
	}

	public List<GameTile> getTilesForPirate() {
		//List<GameTile> pirateTiles = 
		int maxMapDimension = Mathf.Max (mapWidth, mapHeight);
		int difference = maxMapDimension - hexSettings.oceanLayers;

		List<GameTile> possiblePirateTiles = gameBoard.GameTiles.Values.Where (tile => 
			(Mathf.Abs (tile.index.x) + Mathf.Abs (tile.index.y) + Mathf.Abs (tile.index.z)) == ((difference + 2) * 2)).ToList();
		return possiblePirateTiles;
	}

	private void decorateTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		setTileIDsBySettings (tiles, settings);
	}

	public void paintLandTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		Dictionary<TileType, int> availableLandPieces = settings.getAvailableLandPiecesDictionary ();
		Dictionary<TileType, Material> materials = settings.getMaterialsDictionary ();
		List<GameTile> gameTiles = new List<GameTile> (landTiles);
		Debug.Log ("GAMETILES.COUNT = " + gameTiles.Count);
		int randomNum;
		int maxMapDimension = Mathf.Max (mapWidth, mapHeight);
		int difference = maxMapDimension - hexSettings.oceanLayers;

		bool canBeLake;
		do {
			randomNum = Random.Range (0, gameTiles.Count);
			canBeLake = (Mathf.Abs (gameTiles[randomNum].index.x) + Mathf.Abs (gameTiles[randomNum].index.y) + Mathf.Abs (gameTiles[randomNum].index.z)) < (difference * 2);
		} while(!canBeLake);

		Renderer renderer = gameTiles [randomNum].GetComponent<Renderer> ();
		renderer.material = materials [TileType.Desert];

		gameTiles[randomNum].tileType = TileType.Desert;
		gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject.SetActive (false);
		gameTiles [randomNum].transform.FindChild ("Dice Values").gameObject.SetActive (true);
		desertTile = gameTiles[randomNum];

		gameTiles.RemoveAt (randomNum);
		availableLandPieces [TileType.Desert]--;

		foreach (var pair in availableLandPieces) {
			for (int i = 0; i < pair.Value; i++) {
				randomNum = Random.Range (0, gameTiles.Count);
				renderer = gameTiles [randomNum].GetComponent<Renderer> ();
				renderer.material = materials [pair.Key];

				gameTiles[randomNum].tileType = pair.Key;
				gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject.SetActive (true);
				gameTiles.RemoveAt (randomNum);

				if (gameTiles.Count == 0) {
					return;
				}
			}
		}
	}

	private void generateIslands() {
		int maxMapDimension = Mathf.Max (mapWidth, mapHeight);
		int difference = maxMapDimension - hexSettings.oceanLayers;

		if (difference < 2) {
			return;
		}

		int randomLayer = Random.Range (difference + 2, difference + 4);
		 List<GameTile> possibleIslandTiles = gameBoard.GameTiles.Values.Where (tile => 
			(Mathf.Abs (tile.index.x) + Mathf.Abs (tile.index.y) + Mathf.Abs (tile.index.z)) >= ((difference + 3) * 2) &&
			(Mathf.Abs (tile.index.x) + Mathf.Abs (tile.index.y) + Mathf.Abs (tile.index.z)) <= ((difference + 4) * 2)).ToList();
		//List<GameTile> possibleIslandTiles = gameBoard.GameTiles.Values.Where (tile =>
		//	(tile.index.x + tile.index.y + tile.index.z) == (randomLayer + 1 * 2)).ToList();

		int islandsLeft = hexSettings.numIslands;

		while (islandsLeft > 0) {
			int randomIndex;
			do {
				randomIndex = Random.Range (0, possibleIslandTiles.Count);
			} while(!possibleIslandTiles [randomIndex].isPossibleIslandTile ());

			List<GameTile> islandTiles = new List<GameTile> ();
			islandTiles.Add (possibleIslandTiles [randomIndex]);
			int islandTilesIndex = 0;

			if (possibleIslandTiles [randomIndex].tileType == TileType.Ocean) {
				possibleIslandTiles [randomIndex].setTileType(Random.Range(0, 5));

				int randDiceValue;
				if (Random.Range (0.0f, 1.0f) < 0.5f) {
					randDiceValue = Random.Range (2, 7);
				} else {
					randDiceValue = Random.Range (8, 13);
				}
				possibleIslandTiles [randomIndex].setDiceValue(randDiceValue);
			}

			int islandSize = Random.Range (3, 7);

			GameTile currentTile = possibleIslandTiles [randomIndex];

			while (islandSize > 0) {
				int randomIndexChild;
				List<GameTile> neighborTiles = new List<GameTile>();
				bool goBackToParent = true;

				while (islandTilesIndex >= 0 && goBackToParent) {
					foreach (var neighbor in currentTile.getNeighborTiles ()) {
						if (possibleIslandTiles.Contains (neighbor) && neighbor.tileType == TileType.Ocean
							&& !islandTiles.Contains(neighbor) && neighbor.isPossibleIslandTile()) {
							goBackToParent = false;
						}
					}

					if(goBackToParent) {
						islandTilesIndex--;

						if (islandTilesIndex < 0) {
							break;
						}
						currentTile = islandTiles [islandTilesIndex];
						neighborTiles = currentTile.getNeighborTiles ();
						//neighborTiles = possibleIslandTiles [randomIndex].getNeighborTiles ();
					}
					else{
						currentTile = islandTiles [islandTilesIndex];
						neighborTiles = currentTile.getNeighborTiles ();
					}
				}

				if (islandTilesIndex < 0) {
					foreach (var failedIslandTile in islandTiles) {
						failedIslandTile.setTileType ((int)TileType.Ocean);
					}
					islandSize = 0;
					continue;
				}

				do {
					randomIndexChild = Random.Range (0, neighborTiles.Count);
				} while(!neighborTiles [randomIndexChild].isPossibleIslandTile () || 
					!possibleIslandTiles.Contains(neighborTiles [randomIndexChild]) ||
					neighborTiles[randomIndexChild].tileType != TileType.Ocean ||
					islandTiles.Contains(neighborTiles[randomIndexChild]));

				currentTile = neighborTiles [randomIndexChild];
				currentTile.setTileType(Random.Range(0, 5));
				islandTiles.Add (currentTile);
				islandTilesIndex = islandTiles.Count - 1;

				int randDiceValue;
				if (Random.Range (0.0f, 1.0f) < 0.5f) {
					randDiceValue = Random.Range (2, 7);
				} else {
					randDiceValue = Random.Range (8, 13);
				}
				currentTile.setDiceValue(randDiceValue);

				islandSize--;
			}
			islandsLeft--;
		}

		/*foreach (GameTile islandTile in islandTiles) {
			int islandSize = Random.Range (3, 7);
			GameTile currentTile = islandTile;

			while (islandSize > 0) {
				int randomIndex;
				List<GameTile> neighborTiles = currentTile.getNeighborTiles ();

				do {
					randomIndex = Random.Range (0, neighborTiles.Count);
				} while(!neighborTiles [randomIndex].isPossibleIslandTile () || 
					!possibleIslandTiles.Contains(neighborTiles [randomIndex]));

				currentTile = neighborTiles [randomIndex];
				currentTile.setTileType(Random.Range(0, 5));

				int randDiceValue;
				if (Random.Range (0.0f, 1.0f) < 0.5f) {
					randDiceValue = Random.Range (0, 7);
				} else {
					randDiceValue = Random.Range (8, 13);
				}
				currentTile.setDiceValue(randDiceValue);

				islandSize--;
			}
		}*/

	}

	private int paintOceanTiles(List<GameTile> tiles, TileTypeSettings settings) {
		int numOceanTiles = 0;
		for (int i = 0; i < tiles.Count; i++) {
			if (settings.IsOceanTileBySettings (tiles [i])) {
				hexSettings.assignTileTypeToHex (tiles[i], TileType.Ocean);
				tiles[i].transform.FindChild ("Dice Value").gameObject.SetActive(false);
				tiles [i].atIslandLayer = true;
				numOceanTiles++;
			}
		}

		return numOceanTiles;
	}

	public void setTileIDsBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		Dictionary<int, int> diceProbabilities = settings.getDiceProbabilities ();
		List<GameTile> gameTiles = new List<GameTile> (tiles);
		int randomNum;

		foreach (var pair in diceProbabilities) {
			for(int i = 0; i < pair.Value; i++) {
				randomNum = Random.Range (0, gameTiles.Count);

				while (gameTiles [randomNum].tileType == TileType.Desert) {
					gameTiles.RemoveAt (randomNum);
					randomNum = Random.Range (0, gameTiles.Count);
				}

				gameTiles [randomNum].diceValue = pair.Key;

				TextMesh randomDiceValue = gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject.GetComponentInChildren<TextMesh>();
				string diceValueString = pair.Key.ToString () + "\n";

				for (int k = Mathf.Abs (pair.Key - 7); k < 6; k++) {
					diceValueString = diceValueString + ".";
				}

				randomDiceValue.text = diceValueString;

				if (pair.Key == 6 || pair.Key == 8) {
					randomDiceValue.color = Color.red;
				} else {
					randomDiceValue.color = Color.black;
				}

				gameTiles.RemoveAt (randomNum);

				if (gameTiles.Count == 0) {
					return;
				}
			}
		}
	}

	private void paintTileWithType(GameTile tile) {
		if (hexSettings.IsOceanTileBySettings (tile)) {
			MonoBehaviour.Destroy (tile.transform.FindChild ("Dice Value").gameObject);
			hexSettings.assignTileTypeToHex (tile, TileType.Ocean);
		} else {
			int maxMapDimension = Mathf.Max (mapWidth, mapHeight);
			int difference = maxMapDimension - hexSettings.oceanLayers;
			
			bool canBeLake = (Mathf.Abs (tile.index.x) + Mathf.Abs (tile.index.y) + Mathf.Abs (tile.index.z)) < (difference * 2);
			Debug.Log ("Tile index = [" + Mathf.Abs (tile.index.x) + ", " + Mathf.Abs (tile.index.y) + ", " + Mathf.Abs (tile.index.z) + "], difference * 2 = " + (difference * 2) + "canBeLake = " + canBeLake);
			TileType randomType = hexSettings.getRandomTileType (canBeLake);
			if (randomType == TileType.Desert) {
				MonoBehaviour.Destroy (tile.transform.FindChild ("Dice Value").gameObject);
			}
			hexSettings.assignTileTypeToHex (tile, randomType);
		}
	}

}
