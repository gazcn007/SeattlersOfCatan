using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

	/*public static Tuple<ResourceType, CommodityType> getProductionAssetsOfIndex(int index) {
		Tuple<ResourceType, CommodityType> returnTuple = new Tuple<ResourceType, CommodityType> (ResourceType.Null, CommodityType.Null);

		if (index < 0 || index > 7) {
			return new Tuple<ResourceType, CommodityType> (ResourceType.Null, CommodityType.Null);
		} else {
			if (index < 5) {
				return new Tuple<ResourceType, CommodityType> ((ResourceType)index, CommodityType.Null);
			} else {
				return new Tuple<ResourceType, CommodityType> (ResourceType.Null, (CommodityType)index - 5);
			}
		}
	}*/

	public static AssetTuple getAssetOfIndex(int index, int number) {
		if (index < 0 || index > 7) {
			return new AssetTuple ();
		} else {
			if (index < 5) {
				return getAsset ((ResourceType)index, number);
			} else {
				return getAsset ((CommodityType)index - 5, number);
			}
		}
	}

	public static AssetTuple getAsset(ResourceType resourceType, int number) {
		AssetTuple returnTuple = new AssetTuple ();
		returnTuple.resources.resourceTuple [resourceType] = number;

		return returnTuple;
	}

	public static AssetTuple getAsset(CommodityType commodityType, int number) {
		AssetTuple returnTuple = new AssetTuple ();
		returnTuple.commodities.commodityTuple [commodityType] = number;

		return returnTuple;
	}

	public static AssetTuple getRandomAsset(int number) {
		AssetTuple randomAsset = new AssetTuple (0, 0, 0, 0, 0, 0, 0, 0);

		if (Random.Range (0.0f, 1.0f) <= 0.5f) {
			randomAsset.resources.resourceTuple [(ResourceType)Random.Range (0, Enum.GetNames (typeof(ResourceType)).Length)] = number;
		} else {
			randomAsset.commodities.commodityTuple [(CommodityType)Random.Range (0, Enum.GetNames (typeof(CommodityType)).Length)] = number;
		}

		return randomAsset;
	}
}
