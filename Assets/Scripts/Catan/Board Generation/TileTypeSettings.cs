using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeSettings : MonoBehaviour {
	[Range(5.0f, 15.0f)]
	public float brickTilesPercentage = 15.0f;

	[Range(5.0f, 20.0f)]
	public float grainTilesPercentage = 20.0f;

	[Range(5.0f, 20.0f)]
	public float lumberTilesPercentage = 20.0f;

	[Range(5.0f, 15.0f)]
	public float oreTilesPercentage = 15.0f;

	[Range(5.0f, 20.0f)]
	public float woolTilesPercentage = 20.0f;

	[Range(1, 3)]
	public int desertTiles = 1;

	[Range(0,20)]
	public int oceanLayers = 1;

	[Range(0, 7)]
	public int numIslands = 3;

	public Material brickMaterial;
	public Material grainMaterial;
	public Material lumberMaterial;
	public Material oreMaterial;
	public Material woolMaterial;
	public Material oceanMaterial;
	public Material desertMaterial;
	public Material goldMaterial;

	private Dictionary<TileType, int> availableLandPiecesDictionary = new Dictionary<TileType, int>();
	private Dictionary<TileType, Material> materialsDictionary = new Dictionary<TileType, Material>();
	private List<TileType> landTileTypesList = new List<TileType>();

	private Dictionary<int, int> diceProbabilities = new Dictionary<int, int>();

	private int mapWidth = 3;
	private int mapHeight = 3;

	void Awake() {
		StoreMaterialsInDictionary();
		StoreLandTileTypesInList ();
	}

	public bool IsOceanTileBySettings(GameTile tile) {
		CubeIndex index = tile.index;
		int maxMapDimension = Mathf.Max (mapWidth, mapHeight);
		int coordsSum = Mathf.Abs (index.x) + Mathf.Abs (index.y) + Mathf.Abs (index.z);

		if (maxMapDimension <= oceanLayers) {
			oceanLayers = maxMapDimension - 1;
		}

		for (int i = 0; i < oceanLayers; i++) {
			if (coordsSum == (maxMapDimension - i) * 2) {
				return true;
			}
		}

		return false;
	}

	public bool IsDesertTileBySettings(GameTile tile) {
		if (tile.index.x == 0 && tile.index.y == 0 && tile.index.z == 0) {
			return true;
		} else {
			return false;
		}
	}

	public void assignTileTypeToHex(GameTile tile, TileType tileType) {
		Renderer hexRenderer = tile.GetComponent<Renderer> ();
		hexRenderer.material = materialsDictionary [tileType];

		tile.tileType = tileType;
	}

	public TileType getRandomTileType(bool canBeLake) {
		int randomNumber;
		randomNumber = Random.Range (0, landTileTypesList.Count);

		if (!canBeLake) {
			do {
				randomNumber = Random.Range (0, landTileTypesList.Count);
			} while(availableLandPiecesDictionary [(TileType)randomNumber] == 0 || (TileType)randomNumber == TileType.Desert);
		} else {
			do {
				randomNumber = Random.Range (0, landTileTypesList.Count);
			} while(availableLandPiecesDictionary [(TileType)randomNumber] == 0);
		}


		availableLandPiecesDictionary [(TileType)randomNumber]--;
		return (TileType)randomNumber;
	}

	public Dictionary<int, int> getDiceProbabilities() {
		return diceProbabilities;
	}

	public Dictionary<TileType, int> getAvailableLandPiecesDictionary() {
		return availableLandPiecesDictionary;
	}

	public Dictionary<TileType, Material> getMaterialsDictionary() {
		return materialsDictionary;
	}

	public void setMapWidthHeight(int width, int height) {
		mapWidth = width;
		mapHeight = height;
	}

	public void setSettingAccordingToNumTiles(int allTiles, int landTiles) {
		setTileTypeNumbersByTotalNumberOfHexes (allTiles);
		setDiceProbabilitiesByTotalNumberOfHexes (landTiles);
	}

	#region Private Methods

	public void setTileTypeNumbersByTotalNumberOfHexes(int numTiles) {
		int numberOfBrickHexes = (Mathf.CeilToInt (numTiles * brickTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * brickTilesPercentage / 100) : 1;
		int numberOfGrainHexes = (Mathf.CeilToInt (numTiles * grainTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * grainTilesPercentage / 100) : 1;
		int numberOfLumberHexes = (Mathf.CeilToInt (numTiles * lumberTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * lumberTilesPercentage / 100) : 1;
		int numberOfOreHexes = (Mathf.CeilToInt (numTiles * oreTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * oreTilesPercentage / 100) : 1;
		int numberOfWoolHexes = (Mathf.CeilToInt (numTiles * woolTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * woolTilesPercentage / 100) : 1;
		int numberOfDesertHexes = desertTiles;

		int landTiles = numberOfBrickHexes + numberOfGrainHexes + numberOfLumberHexes + numberOfOreHexes + numberOfWoolHexes + numberOfDesertHexes;

		while (landTiles < numTiles) {
			numberOfBrickHexes++;
			numberOfGrainHexes++;
			numberOfLumberHexes++;
			numberOfOreHexes++;
			numberOfWoolHexes++;

			landTiles += 5;
		}

		//int oceanTiles = numTiles - landTiles;

		availableLandPiecesDictionary.Add (TileType.Hills, numberOfBrickHexes);
		availableLandPiecesDictionary.Add (TileType.Fields, numberOfGrainHexes);
		availableLandPiecesDictionary.Add (TileType.Forests, numberOfLumberHexes);
		availableLandPiecesDictionary.Add (TileType.Mountains, numberOfOreHexes);
		availableLandPiecesDictionary.Add (TileType.Pastures, numberOfWoolHexes);
		availableLandPiecesDictionary.Add (TileType.Desert, numberOfDesertHexes);
	}

	public void setDiceProbabilitiesByTotalNumberOfHexes(int landTiles) {
		int twoTwelve = (Mathf.CeilToInt (landTiles * 1f / 18f) > 0f)? Mathf.FloorToInt (landTiles * 1f / 18f) : 1;
		int rest = (Mathf.CeilToInt (landTiles * 2f / 18f) > 0f)? Mathf.FloorToInt (landTiles * 2f / 18f) : 1;
			
		while (twoTwelve * 2 + rest * 8 < landTiles - desertTiles) {
			twoTwelve++;
			rest++;
		}

		diceProbabilities.Add (2, twoTwelve);
		for (int i = 3; i < 12; i++) {
			if (i == 7) {
				continue;
			}
			diceProbabilities.Add (i, rest);
		}
		diceProbabilities.Add (12, twoTwelve);

		//print ("twoTwelve = " + twoTwelve);
		//print ("rest = " + rest);
	}

	private void StoreMaterialsInDictionary() {
		materialsDictionary.Add (TileType.Hills, brickMaterial);
		materialsDictionary.Add (TileType.Fields, grainMaterial);
		materialsDictionary.Add (TileType.Forests, lumberMaterial);
		materialsDictionary.Add (TileType.Mountains, oreMaterial);
		materialsDictionary.Add (TileType.Pastures, woolMaterial);
		materialsDictionary.Add (TileType.Desert, desertMaterial);
		materialsDictionary.Add (TileType.Ocean, oceanMaterial);
		materialsDictionary.Add (TileType.Gold, goldMaterial);
	}

	private void StoreLandTileTypesInList() {
		landTileTypesList.Add (TileType.Hills);
		landTileTypesList.Add (TileType.Fields);
		landTileTypesList.Add (TileType.Forests);
		landTileTypesList.Add (TileType.Mountains);
		landTileTypesList.Add (TileType.Pastures);
		landTileTypesList.Add (TileType.Desert);
		landTileTypesList.Add (TileType.Gold);
	}

	#endregion
}
