using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class AssetTuple {

	public ResourceTuple resources;
	public CommodityTuple commodities;

	public AssetTuple() {
		resources = new ResourceTuple();
		commodities = new CommodityTuple();
	}

	public AssetTuple(int brick, int grain, int lumber, int ore, int wool, int paper, int coin, int cloth) {
		resources = new ResourceTuple (brick, grain, lumber, ore, wool);
		commodities = new CommodityTuple (paper, coin, cloth);
	}

	public AssetTuple(ResourceTuple resourceTuple, CommodityTuple commodityTuple) {
		this.resources = resourceTuple;
		this.commodities = commodityTuple;
	}

	public int GetValueAtIndex(int index) {
		if (index < 0 || index > 7) {
			return 0;
		} else {
			if (index < 5) {
				return resources.resourceTuple [(ResourceType)index];
			} else {
				return commodities.commodityTuple [(CommodityType)index - 5];
			}
		}
	}

	public void SetValueAtIndex(int index, int value) {
		if (index < 0 || index > 7) {
		} else {
			if (index < 5) {
				this.resources.resourceTuple [(ResourceType)index] = value;
			} else {
				this.commodities.commodityTuple [(CommodityType)index - 5] = value;
			}
		}
	}
	
}
