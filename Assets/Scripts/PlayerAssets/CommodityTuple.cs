using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommodityTuple {

	public int numCloths = 0;
	public int numCoins = 0;
	public int numPapers = 0;

	public Dictionary<CommodityType, int> commodityTuple = new Dictionary<CommodityType, int>();

	// Use this for initialization
	public CommodityTuple () {
		commodityTuple.Add (CommodityType.Cloth, numCloths);
		commodityTuple.Add (CommodityType.Coin, numCoins);
		commodityTuple.Add (CommodityType.Paper, numPapers);
	}
}
