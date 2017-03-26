using System.Collections;
using System.Collections.Generic;
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


		board.placeRobberOnTile (desertTile.id);
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

	private void decorateTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		setTileIDsBySettings (tiles, settings);
	}

	public void paintLandTilesBySettings(List<GameTile> tiles, TileTypeSettings settings) {
		Dictionary<TileType, int> availableLandPieces = settings.getAvailableLandPiecesDictionary ();
		Dictionary<TileType, Material> materials = settings.getMaterialsDictionary ();
		List<GameTile> gameTiles = new List<GameTile> (landTiles);
		Debug.Log ("GAMETILES.COUNT = " + gameTiles.Count);
		int randomNum;

		foreach (var pair in availableLandPieces) {
			for (int i = 0; i < pair.Value; i++) {
				randomNum = Random.Range (0, gameTiles.Count);
				Renderer renderer = gameTiles [randomNum].GetComponent<Renderer> ();
				renderer.material = materials [pair.Key];

				gameTiles[randomNum].tileType = pair.Key;
				if (pair.Key == TileType.Desert) {
					//MonoBehaviour.Destroy (gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject);
					gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject.SetActive (false);
					desertTile = gameTiles[randomNum];
				} else {
					gameTiles [randomNum].transform.FindChild ("Dice Value").gameObject.SetActive (true);
				}
				gameTiles.RemoveAt (randomNum);

				if (gameTiles.Count == 0) {
					return;
				}
			}
		}
	}

	private int paintOceanTiles(List<GameTile> tiles, TileTypeSettings settings) {
		int numOceanTiles = 0;
		for (int i = 0; i < tiles.Count; i++) {
			if (settings.IsOceanTileBySettings (tiles [i])) {
				hexSettings.assignTileTypeToHex (tiles[i], TileType.Ocean);
				//MonoBehaviour.Destroy (tiles[i].transform.FindChild ("Dice Value").gameObject);
				tiles[i].transform.FindChild ("Dice Value").gameObject.SetActive(false);
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
			TileType randomType = hexSettings.getRandomTileType ();
			if (randomType == TileType.Desert) {
				MonoBehaviour.Destroy (tile.transform.FindChild ("Dice Value").gameObject);
			}
			hexSettings.assignTileTypeToHex (tile, randomType);
		}
	}

}
