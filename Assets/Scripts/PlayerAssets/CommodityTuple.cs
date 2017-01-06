using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommodityTuple {

	public int numCloths;
	public int numCoins;
	public int numPapers;

	public Dictionary<CommodityType, int> commodityTuple = new Dictionary<CommodityType, int>();

	// Use this for initialization
	// Use this for initialization
	public CommodityTuple() {
		commodityTuple = new Dictionary<CommodityType, int>();

		commodityTuple.Add (CommodityType.Cloth, numCloths);
		commodityTuple.Add (CommodityType.Coin, numCoins);
		commodityTuple.Add (CommodityType.Paper, numPapers);
	}

	public CommodityTuple(int cloths, int coins, int papers) {
		commodityTuple = new Dictionary<CommodityType, int>();

		numCloths = cloths;
		numCoins = coins;
		numPapers = papers;


		commodityTuple.Add (CommodityType.Cloth, numCloths);
		commodityTuple.Add (CommodityType.Coin, numCoins);
		commodityTuple.Add (CommodityType.Paper, numPapers);
	}

	public void addCommodityWithType(CommodityType key, int value) {
		if(commodityTuple.ContainsKey(key)) {
			commodityTuple[key] = value;
		}
		else{
			commodityTuple.Add(key, value);
		}
	}

	public void printCommodityTuple() {
		MonoBehaviour.print ("This CommodityTuple has: ");
		foreach (var pair in commodityTuple) {
			MonoBehaviour.print (pair.Key.ToString () + " = " + pair.Value.ToString ());
		}

	}
}
