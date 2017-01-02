using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAsset : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static ResourceType getResourceOfHex(TileType tileType) {
		switch (tileType) {
		case TileType.Hills:
			return ResourceType.Brick;
		case TileType.Fields:
			return ResourceType.Grain;
		case TileType.Forests:
			return ResourceType.Lumber;
		case TileType.Mountains:
			return ResourceType.Ore;
		case TileType.Pastures:
			return ResourceType.Wool;
		default:
			return ResourceType.Null;
		}
	}
}
