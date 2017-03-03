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
	
}
