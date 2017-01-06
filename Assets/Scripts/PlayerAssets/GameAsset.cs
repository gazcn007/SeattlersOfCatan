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

	public static CommodityType getCommodityOfHex(TileType tileType) {
		switch (tileType) {
		case TileType.Forests:
			return CommodityType.Paper;
		case TileType.Mountains:
			return CommodityType.Coin;
		case TileType.Pastures:
			return CommodityType.Cloth;
		default:
			return CommodityType.Null;
		}
	}

	public static Tuple<ResourceType, CommodityType> getProductionAssetsOfIndex(int number) {
		Tuple<ResourceType, CommodityType> returnTuple = new Tuple<ResourceType, CommodityType> (ResourceType.Null, CommodityType.Null);

		if (number < 0 || number > 7) {
			return new Tuple<ResourceType, CommodityType> (ResourceType.Null, CommodityType.Null);
		} else {
			if (number < 5) {
				return new Tuple<ResourceType, CommodityType> ((ResourceType)number, CommodityType.Null);
			} else {
				return new Tuple<ResourceType, CommodityType> (ResourceType.Null, (CommodityType)number - 5);
			}
		}
	}
}
