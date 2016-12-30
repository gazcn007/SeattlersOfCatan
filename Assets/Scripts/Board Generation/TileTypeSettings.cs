using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeSettings : MonoBehaviour {
	//public int numberOfBrickHexes = 	3;
	//public int numberOfGrainHexes = 	4;
	//public int numberOfLumberHexes = 	4;
	//public int numberOfOreHexes = 		3;
	//public int numberOfWoolHexes =		4;
	//public int numberOfDesertHexes = 	1;

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

	[Range(0,3)]
	public int oceanLayers = 1;

	public Material brickMaterial;
	public Material grainMaterial;
	public Material lumberMaterial;
	public Material oreMaterial;
	public Material woolMaterial;
	public Material oceanMaterial;
	public Material desertMaterial;

	private Dictionary<TileType, int> availableLandPiecesDictionary = new Dictionary<TileType, int>();
	private Dictionary<TileType, Material> materialsDictionary = new Dictionary<TileType, Material>();
	private List<TileType> landTileTypesList = new List<TileType>();

	private int mapWidth = 3;
	private int mapHeight = 3;

	void Awake() {
		StoreMaterialsInDictionary();
		StoreLandTileTypesInList ();
	}

	private bool IsOceanTile(GameTile tile) {
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

		//if (oceanLayers == 0) {
		//	return coordsSum == maxMapDimension * 2;
		//}

		return false;
	}

	private bool IsDesertTile(GameTile tile) {
		if (tile.index.x == 0 && tile.index.y == 0 && tile.index.z == 0) {
			return true;
		} else {
			return false;
		}
	}

	public void paintTile(GameTile tile) {
		if (IsOceanTile (tile)) {
			assignTileTypeToHex (tile, TileType.Ocean);
		} else {
			TileType randomType = getRandomTileType ();
			assignTileTypeToHex (tile, randomType);
		}
	}

	private void assignTileTypeToHex(GameTile tile, TileType tileType) {
		Renderer hexRenderer = tile.GetComponent<Renderer> ();
		hexRenderer.material = materialsDictionary [tileType];

		tile.tileType = tileType;
	}

	private TileType getRandomTileType() {
		int randomNumber;
		randomNumber = Random.Range (0, landTileTypesList.Count);

		do {
			randomNumber = Random.Range (0, landTileTypesList.Count);
		} while(availableLandPiecesDictionary [(TileType)randomNumber] == 0);

		availableLandPiecesDictionary [(TileType)randomNumber]--;
		return (TileType)randomNumber;
	}

	public void setMapWidthHeight(int width, int height) {
		mapWidth = width;
		mapHeight = height;
	}

	public void setTilesForNumberOnBoard(int numTiles) {
		int numberOfBrickHexes = (Mathf.FloorToInt (numTiles * brickTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * brickTilesPercentage / 100) : 1;
		int numberOfGrainHexes = (Mathf.FloorToInt (numTiles * grainTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * grainTilesPercentage / 100) : 1;
		int numberOfLumberHexes = (Mathf.FloorToInt (numTiles * lumberTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * lumberTilesPercentage / 100) : 1;
		int numberOfOreHexes = (Mathf.FloorToInt (numTiles * oreTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * oreTilesPercentage / 100) : 1;
		int numberOfWoolHexes = (Mathf.FloorToInt (numTiles * woolTilesPercentage / 100) > 0f)? Mathf.FloorToInt (numTiles * woolTilesPercentage / 100) : 1;
		int numberOfDesertHexes = desertTiles;

		int landTiles = numberOfBrickHexes + numberOfGrainHexes + numberOfLumberHexes + numberOfOreHexes + numberOfWoolHexes + numberOfDesertHexes;

		if (oceanLayers == 0) {
			while (landTiles < numTiles) {
				numberOfBrickHexes++;
				numberOfGrainHexes++;
				numberOfLumberHexes++;
				numberOfOreHexes++;
				numberOfWoolHexes++;

				landTiles += 5;
			}
		}

		//int oceanTiles = numTiles - landTiles;

		availableLandPiecesDictionary.Add (TileType.Brick, numberOfBrickHexes);
		availableLandPiecesDictionary.Add (TileType.Grain, numberOfGrainHexes);
		availableLandPiecesDictionary.Add (TileType.Lumber, numberOfLumberHexes);
		availableLandPiecesDictionary.Add (TileType.Ore, numberOfOreHexes);
		availableLandPiecesDictionary.Add (TileType.Wool, numberOfWoolHexes);
		availableLandPiecesDictionary.Add (TileType.Desert, numberOfDesertHexes);
	}

	private void StoreMaterialsInDictionary() {
		materialsDictionary.Add (TileType.Brick, brickMaterial);
		materialsDictionary.Add (TileType.Grain, grainMaterial);
		materialsDictionary.Add (TileType.Lumber, lumberMaterial);
		materialsDictionary.Add (TileType.Ore, oreMaterial);
		materialsDictionary.Add (TileType.Wool, woolMaterial);
		materialsDictionary.Add (TileType.Desert, desertMaterial);
		materialsDictionary.Add (TileType.Ocean, oceanMaterial);
	}

	private void StoreLandTileTypesInList() {
		//for (int i = 0; i < 5; i++) {
		//	landTileTypesList.Add ((TileType)i);
		//}
		landTileTypesList.Add (TileType.Brick);
		landTileTypesList.Add (TileType.Grain);
		landTileTypesList.Add (TileType.Lumber);
		landTileTypesList.Add (TileType.Ore);
		landTileTypesList.Add (TileType.Wool);
		landTileTypesList.Add (TileType.Desert);
	}
}
