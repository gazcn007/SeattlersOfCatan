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
	public int gold;

	public AssetTuple() {
		resources = new ResourceTuple();
		commodities = new CommodityTuple();
		fishTokens = new FishTuple ();
		this.gold = 0;
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (0, 0, 0);
		this.gold = 0;
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth, int oneTokens, int twoTokens, int threeTokens) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (oneTokens, twoTokens, threeTokens);
		this.gold = 0;
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth, int oneTokens, int twoTokens, int threeTokens, int oldBoot) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (oneTokens, twoTokens, threeTokens, oldBoot);
		this.gold = 0;
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth, int oneTokens, int twoTokens, int threeTokens, int oldBoot, int gold) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
		fishTokens = new FishTuple (oneTokens, twoTokens, threeTokens, oldBoot);
		this.gold = gold;
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = new FishTuple ();
		this.gold = 0;
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple, int gold) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = new FishTuple ();
		this.gold = gold;
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple, FishTuple fishTuple) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = fishTuple;
		this.gold = 0;
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple, FishTuple fishTuple, int gold) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
		this.fishTokens = fishTuple;
		this.gold = gold;
	}

	public int GetValueAtIndex(int index) {
		if (index < 0 || index > 11) {
			return 0;
		} else {
			if (index < 5) {
				return resources.resourceTuple [(ResourceType)index];
			} else if (index < 8) {
				return commodities.commodityTuple [(CommodityType)index - 5];
			} else if (index < 11) {
				return fishTokens.fishTuple [(FishTokenType)index - 8];
			} else {
				return gold;
			}
		}
	}

	public void SetValueAtIndex(int index, int value) {
		if (index < 0 || index > 11) {
		} else {
			if (index < 5) {
				this.resources.resourceTuple [(ResourceType)index] = value;
			} else if (index < 8) {
				this.commodities.commodityTuple [(CommodityType)index - 5] = value;
			} else if (index < 11) {
				this.fishTokens.fishTuple [(FishTokenType)index - 8] = value;
			} else {
				this.gold = value;
			}
		}
	}
	
}
