using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class AssetTuple {

	public ResourceTuple resources;
	public CommodityTuple commodities;
	public FishTuple fishTokens;

	public AssetTuple() {
		resources = new ResourceTuple();
		commodities = new CommodityTuple();
		fishTokens = new FishTuple ();
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (0, 0, 0);
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth, int oneTokens, int twoTokens, int threeTokens) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (oneTokens, twoTokens, threeTokens);
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = new FishTuple ();
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple, FishTuple fishTuple) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = fishTuple;
	}

	public int GetValueAtIndex(int index) {
		if (index < 0 || index > 10) {
			return 0;
		} else {
			if (index < 5) {
				return resources.resourceTuple [(ResourceType)index];
			} else if (index < 8) {
				return commodities.commodityTuple [(CommodityType)index - 5];
			} else {
				return fishTokens.fishTuple [(FishTokenType)index - 8];
			}
		}
	}

	public void SetValueAtIndex(int index, int value) {
		if (index < 0 || index > 10) {
		} else {
			if (index < 5) {
				this.resources.resourceTuple [(ResourceType)index] = value;
			} else if (index < 8) {
				this.commodities.commodityTuple [(CommodityType)index - 5] = value;
			} else {
				this.fishTokens.fishTuple [(FishTokenType)index - 8] = value;
			}
		}
	}
	
}
