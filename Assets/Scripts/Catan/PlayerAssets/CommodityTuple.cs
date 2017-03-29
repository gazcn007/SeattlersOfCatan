using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommodityTuple {

	public int numPapers;
	public int numCoins;
	public int numCloths;

	public Dictionary<CommodityType, int> commodityTuple = new Dictionary<CommodityType, int>();

	// Use this for initialization
	// Use this for initialization
	public CommodityTuple() {
		commodityTuple = new Dictionary<CommodityType, int>();

		commodityTuple.Add (CommodityType.Paper, numPapers);
		commodityTuple.Add (CommodityType.Coin, numCoins);
		commodityTuple.Add (CommodityType.Cloth, numCloths);}

	public CommodityTuple(int papers, int coins, int cloths) {
		commodityTuple = new Dictionary<CommodityType, int>();

		numPapers = papers;
		numCoins = coins;
		numCloths = cloths;

		commodityTuple.Add (CommodityType.Paper, numPapers);
		commodityTuple.Add (CommodityType.Coin, numCoins);
		commodityTuple.Add (CommodityType.Cloth, numCloths);
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
