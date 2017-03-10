using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommodityCard : GameAsset {

	public CommodityType type;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

	}
}

[System.Serializable]
public enum CommodityType {
	Paper = 0,
	Coin,
	Cloth,
	Null
}
