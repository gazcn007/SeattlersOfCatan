using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDecorator {

	private TileTypeSettings hexSettings;
	private GameBoard gameBoard;

	private int mapWidth;
	private int mapHeight;

	private List<GameTile> allTiles;
	private List<GameTile> landTiles;

	// Use this for initialization
	public BoardDecorator(GameBoard board, TileTypeSettings settings) {
		gameBoard = board;
		hexSettings = settings;

		mapWidth = gameBoard.getMapWidth ();
		mapHeight = gameBoard.getMapHeight ();
		hexSettings.setMapWidthHeight (mapWidth, mapHeight);

		allTiles = gameBoard.getTiles ();
		//hexSettings.setSettingAccordingToNumTiles (allTiles.Count, landTiles.Count);
		int numOceanTiles = paintOceanTiles (allTiles, settings);
		landTiles = findLandTiles (allTiles);
		hexSettings.setTileTypeNumbersByTotalNumberOfHexes (landTiles.Count);
		paintLandTilesBySettings (allTiles, hexSettings);

		hexSettings.setDiceProbabilitiesByTotalNumberOfHexes (landTiles.Count);
		setTileIDsBySettings (landTiles, hexSettings);
	}

	private List<GameTile> findLandTiles(List<GameTile> allTiles) {
		List<GameTile> landTiles = new List<GameTile>();

		for(int i = 0; i < allTiles.Count; i++) {
			if (allTiles [i].tileType != TileType.Ocean && allTiles [i].tileType != TileType.Desert) {
				landTiles.Add (allTiles[i]);
			}
		}

		return landTiles;
	}

	private void decorateTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		setTileIDsBySettings (tiles, settings);
	}

	private void paintLandTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		Dictionary<TileType, int> availableLandPieces = settings.getAvailableLandPiecesDictionary ();
		Dictionary<TileType, Material> materials = settings.getMaterialsDictionary ();
		List<GameTile> gameTiles = new List<GameTile> (landTiles);
		int randomNum;

		foreach (var pair in availableLandPieces) {
			for (int i = 0; i < pair.Value; i++) {
				randomNum = Random.Range (0, gameTiles.Count);
				Renderer renderer = gameTiles [randomNum].GetComponent<Renderer> ();
				renderer.material = materials [pair.Key];

				gameTiles[randomNum].tileType = pair.Key;
				if (pair.Key == TileType.Desert) {
					MonoBehaviour.Destroy (gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject);
				}
				gameTiles.RemoveAt (randomNum);
			}
		}
	}

	private int paintOceanTiles(List<GameTile> tiles, TileTypeSettings settings) {
		int numOceanTiles = 0;
		for (int i = 0; i < tiles.Count; i++) {
			if (settings.IsOceanTileBySettings (tiles [i])) {
				hexSettings.assignTileTypeToHex (tiles[i], TileType.Ocean);
				MonoBehaviour.Destroy (tiles[i].transform.FindChild ("Dice Value").gameObject);
				numOceanTiles++;
			}
		}

		return numOceanTiles;
	}

	private void setTileIDsBySettings(List<GameTile> tiles, TileTypeSettings settings) {
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
				}

				gameTiles.RemoveAt (randomNum);
			}
		}
	}

	private void paintTileWithType(GameTile tile) {
		if (hexSettings.IsOceanTileBySettings (tile)) {
			MonoBehaviour.Destroy (tile.transform.FindChild ("Dice Value").gameObject);
			hexSettings.assignTileTypeToHex (tile, TileType.Ocean);
		} else {
			TileType randomType = hexSettings.getRandomTileType ();
			if (randomType == TileType.Desert) {
				MonoBehaviour.Destroy (tile.transform.FindChild ("Dice Value").gameObject);
			}
			hexSettings.assignTileTypeToHex (tile, randomType);
		}
	}

}
