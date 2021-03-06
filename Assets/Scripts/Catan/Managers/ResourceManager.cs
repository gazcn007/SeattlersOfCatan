﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

	public GameObject numericDiePrefab;
	private Die redDie;
	private Die yellowDie;

	public ResourceTuple settlementCost;
	public ResourceTuple cityCost;
	public ResourceTuple metropolisCost;
	public ResourceTuple cityWallsCost;

	public ResourceTuple roadCost;
	public ResourceTuple shipCost;

	public ResourceTuple knightCost;

	public ResourceTuple knightActivationCost;

	private Dictionary<System.Type, ResourceTuple> costsChart = new Dictionary<System.Type, ResourceTuple>();
	private Dictionary<UnitType, AssetTuple> resourceCostsChart = new Dictionary<UnitType, AssetTuple>();
	private Dictionary<MoveType, AssetTuple> knightActionCostsChart = new Dictionary<MoveType, AssetTuple>();

	private FishTuple fishTokensAvailable;

	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		//GameObject redDieObj = (GameObject) Instantiate (numericDiePrefab);
		//redDie = redDieObj.GetComponent<NumericDie> ();

		//GameObject yellowDieObj = (GameObject) Instantiate (numericDiePrefab);
		//yellowDie = yellowDieObj.GetComponent<NumericDie> ();

		PopulateLookupDictionaries ();
	}

	public void PopulateLookupDictionaries() {
		PopulateCostsDictionary ();
		PopulateResoureCostsDictionary ();
		PopulateFishTokensDictionary ();
		PopulateKnightActionCostsDictionary ();
	}

	public void PopulateCostsDictionary() {
		costsChart.Add (typeof(Settlement), new ResourceTuple(settlementCost.numBricks, settlementCost.numGrains, settlementCost.numLumbers, settlementCost.numOres, settlementCost.numWools));
		costsChart.Add (typeof(City), new ResourceTuple(cityCost.numBricks, cityCost.numGrains, cityCost.numLumbers, cityCost.numOres, cityCost.numWools));
		//costsChart.Add (metropolis.GetType ().ToString (), metropolisCost);
		costsChart.Add (typeof(CityWall), new ResourceTuple(cityWallsCost.numBricks, cityWallsCost.numGrains, cityWallsCost.numLumbers, cityWallsCost.numOres, cityWallsCost.numWools));

		costsChart.Add (typeof(Road), new ResourceTuple(roadCost.numBricks, roadCost.numGrains, roadCost.numLumbers, roadCost.numOres, roadCost.numWools));
		costsChart.Add (typeof(Ship), new ResourceTuple(shipCost.numBricks, shipCost.numGrains, shipCost.numLumbers, shipCost.numOres, shipCost.numWools));

		costsChart.Add (typeof(Knight), new ResourceTuple(knightCost.numBricks, knightCost.numGrains, knightCost.numLumbers, knightCost.numOres, knightCost.numWools));
	}

	public void PopulateResoureCostsDictionary() {
		resourceCostsChart.Add (UnitType.Settlement, new AssetTuple(settlementCost.numBricks, settlementCost.numGrains, settlementCost.numLumbers, settlementCost.numOres, settlementCost.numWools, 0, 0, 0));
		resourceCostsChart.Add (UnitType.City, new AssetTuple(cityCost.numBricks, cityCost.numGrains, cityCost.numLumbers, cityCost.numOres, cityCost.numWools, 0, 0, 0));
		resourceCostsChart.Add (UnitType.Metropolis, new AssetTuple(metropolisCost.numBricks, metropolisCost.numGrains, metropolisCost.numLumbers, metropolisCost.numOres, metropolisCost.numWools, 0, 0, 0));
		resourceCostsChart.Add (UnitType.CityWalls, new AssetTuple(cityWallsCost.numBricks, cityWallsCost.numGrains, cityWallsCost.numLumbers, cityWallsCost.numOres, cityWallsCost.numWools, 0, 0, 0));

		resourceCostsChart.Add (UnitType.Road, new AssetTuple(roadCost.numBricks, roadCost.numGrains, roadCost.numLumbers, roadCost.numOres, roadCost.numWools, 0, 0, 0));
		resourceCostsChart.Add (UnitType.Ship, new AssetTuple(shipCost.numBricks, shipCost.numGrains, shipCost.numLumbers, shipCost.numOres, shipCost.numWools, 0, 0, 0));

		resourceCostsChart.Add (UnitType.Knight, new AssetTuple(knightCost.numBricks, knightCost.numGrains, knightCost.numLumbers, knightCost.numOres, knightCost.numWools, 0, 0, 0));
	}

	public void PopulateKnightActionCostsDictionary() {
		knightActionCostsChart.Add(MoveType.ActivateKnight, new AssetTuple(knightActivationCost.numBricks, knightActivationCost.numGrains, knightActivationCost.numLumbers, knightActivationCost.numOres, knightActivationCost.numWools, 0, 0, 0));
	}

	public void PopulateFishTokensDictionary() {
		//fishTokensAvailable.Add (FishTokenType.One, 11);
		//fishTokensAvailable.Add (FishTokenType.Two, 10);
		//fishTokensAvailable.Add (FishTokenType.Three, 8);

		fishTokensAvailable = new FishTuple (11, 10, 8, 1);
	}

	public ResourceTuple getCostOfUnit(System.Type unitType) {
		if (!costsChart.ContainsKey (unitType)) {
			return null;
		}
		return costsChart [unitType];
	}

	public AssetTuple getCostOfUnit(UnitType unitType) {
		if (!resourceCostsChart.ContainsKey (unitType)) {
			return null;
		}
		return resourceCostsChart [unitType];
	}

	public AssetTuple getCostOf(MoveType moveType) {
		if (!knightActionCostsChart.ContainsKey (moveType)) {
			return new AssetTuple();
		}
		return knightActionCostsChart [moveType];
	}

	public /*IEnumerator*/ int diceRollEvent() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
		//int redDieRoll = redDie.roll ();
		//int yellowDieRoll = yellowDie.roll ();
		int redDieRoll = dieRoll();
		int yellowDieRoll = dieRoll();

		print ("Red die rolled: " + redDieRoll);
		print ("Yellow die rolled: " + yellowDieRoll);

		return redDieRoll + yellowDieRoll;
	}

	public int dieRoll() {
		return Random.Range (0, 7);
	}

	public bool canTrade(Player player, int resourceToGiveForOne) {
		AssetTuple currentAssets = player.getCurrentAssets ();
		bool canTrade = false;

		foreach (var pair in currentAssets.resources.resourceTuple) {
			if (pair.Value >= resourceToGiveForOne) {
				canTrade = true;
			}
		}
		return canTrade;
	}

	public ResourceTuple getResourceForTile(GameTile tile, int numCollected) {
		ResourceTuple resourceCollected = new ResourceTuple ();
		ResourceType typeOfResource = GameAsset.getResourceOfHex (tile.tileType);

		if (typeOfResource != ResourceType.Null) {
			resourceCollected.resourceTuple [typeOfResource] = numCollected;
		}

		return resourceCollected;
	}

	public CommodityTuple getCommodityForTile(GameTile tile, int numCollected) {
		CommodityTuple commodityCollected = new CommodityTuple ();
		CommodityType typeOfCommodity = GameAsset.getCommodityOfHex (tile.tileType);

		if (typeOfCommodity != CommodityType.Null) {
			commodityCollected.commodityTuple [typeOfCommodity] = numCollected;
		}

		return commodityCollected;
	}

	public void receiveFishTokens(FishTuple fishTokensUsed) {
		foreach (var pair in fishTokensUsed.fishTuple) {
			fishTokensAvailable.fishTuple [pair.Key] += pair.Value;
		}
	}

	public void giveFishTokens(FishTuple fishTokensGiven) {
		foreach (var pair in fishTokensGiven.fishTuple) {
			fishTokensAvailable.fishTuple [pair.Key] -= pair.Value;
		}
	}

	public FishTuple getFishTokenForTile(GameTile tile, int numCollected) {
		FishTuple fishTokensCollected = new FishTuple ();
		if (tile.tileType != TileType.Desert && tile.tileType != TileType.Ocean) {
			Debug.Log ("Tile is not fish, returning!");
			return fishTokensCollected;
		}

		if (numFishTokensLeft () < numCollected) {
			Debug.Log ("numFishTokensLeft () < numCollected");
			foreach (var pair in fishTokensAvailable.fishTuple) {
				Debug.Log ("Key = " + pair.Key + ", Value = " + pair.Value);
			}
			return fishTokensCollected;
		}

		int collectionLeft = numCollected;
		List<FishTokenType> tokensAvailable = fishTokensAvailable.listForm ();
		while (collectionLeft > 0) {
			int randomNum = Random.Range(0, tokensAvailable.Count);
			FishTokenType drawnToken = tokensAvailable [randomNum];

			fishTokensCollected.addFishTokenWithType (drawnToken, 1);
			collectionLeft--;

			if (drawnToken == FishTokenType.OldBoot) {
				Debug.Log (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerName + " draws the Old Boot!");
			}
		}

		return fishTokensCollected;
	}

	public int numFishTokensLeft() {
		int sum = 0;
		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(fishTokensAvailable.fishTuple.Keys);

		for (int i = 0; i < fishTokenKeys.Count; i++) {
			sum += fishTokensAvailable.fishTuple [fishTokenKeys [i]];
		}
		return sum;
	}

	public AssetTuple getAssetTupleForTile(GameTile tile, int numCollected) {
		ResourceTuple resources = getResourceForTile (tile, numCollected);
		CommodityTuple commodities = getCommodityForTile (tile, numCollected);

		return new AssetTuple (resources, commodities);
	}
}
