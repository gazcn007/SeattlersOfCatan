using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInfoLookup : MonoBehaviour {

	public TileTypeSettings boardSettings;

	public List<int> diceValues = new List<int>();
	public List<int> materialNumbers = new List<int>();

	Dictionary<TileType, Material> materials;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setSettings(TileTypeSettings settings) {
		boardSettings = settings;
		materials = boardSettings.getMaterialsDictionary ();
	}

	private int paintOceanTiles(List<GameTile> tiles) {
		int numOceanTiles = 0;
		for (int i = 0; i < tiles.Count; i++) {
			if (boardSettings.IsOceanTileBySettings (tiles [i])) {
				boardSettings.assignTileTypeToHex (tiles[i], TileType.Ocean);
				//MonoBehaviour.Destroy (tiles[i].transform.FindChild ("Dice Value").gameObject);
				tiles[i].transform.FindChild ("Dice Value").gameObject.SetActive(false);
				numOceanTiles++;
			}
		}

		return numOceanTiles;
	}
}
