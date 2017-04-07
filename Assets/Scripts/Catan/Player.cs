using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {

	public string playerName;
	public Color playerColor;

	public int playerNumber;
	public int goldCoins = 2;
	public int victoryPoints;

	// private Dictionary<System.Type, List<Unit>> ownedUnits -> ownedUnits[typeof(settlement)].add(settlement) -> O(1) access to list of specific type of units
	public Dictionary<System.Type, List<Unit>> ownedUnits;
	public AssetTuple assets = new AssetTuple(20, 20, 20, 20, 20, 10, 10, 10, 0, 0, 0);
	public List<ProgressCard> progressCards = new List<ProgressCard> ();
	public CityImprovementTuple cityImprovements;

	public GameTile lastGameTileSelection;
	public Edge lastEdgeSelection;
	public Intersection lastIntersectionSelection;
	public Unit lastUnitSelection;

	public bool collectedThisTurn;

	public Sprite avatar;

	// Use this for initialization
	void Start () {
		//playerColor = Color.black;
		initializeUnitsDictionary();
		cityImprovements = new CityImprovementTuple ();
	}
	
	// Update is called once per frame
	void Update () {
		victoryPoints = getTotalVictoryPoints ();

		//if (victoryPoints >= GameManager.victoryPointsToWinGame) {
		//	winGame ();
		//}
	}

	void initializeUnitsDictionary() {
		ownedUnits = new Dictionary<System.Type, List<Unit>>();

		ownedUnits.Add(typeof(Settlement), new List<Unit>());
		ownedUnits.Add(typeof(City), new List<Unit>());
		ownedUnits.Add(typeof(Metropolis), new List<Unit>());
		ownedUnits.Add(typeof(CityWall), new List<Unit>());
		ownedUnits.Add(typeof(Road), new List<Unit>());
		ownedUnits.Add(typeof(Ship), new List<Unit>());
		ownedUnits.Add(typeof(Knight), new List<Unit>());

		//ownedUnits.Add (typeof(IntersectionUnit), new List<IntersectionUnit> ());
		//ownedUnits.Add (typeof(TradeUnit), new List<TradeUnit> ());

		ownedUnits.Add (typeof(Unit), new List<Unit> ());
	}

	public bool canUpgrade(CityImprovementType improvementType) {
		int numCities = getOwnedUnitsOfType (UnitType.City).Count;
		bool canUpgrade = false;

		if (numCities > 0) {
			if (hasAvailableAssets (cityImprovements.nextImprovementCost (improvementType))) {
				canUpgrade = true;
			}
		}
		return canUpgrade;
	}

	public bool canDrawProgressCard(EventDieFace faceType, int redDieRoll) {
		bool canDraw = false;

		switch (faceType) {
		case EventDieFace.Green:
			canDraw = (redDieRoll <= (cityImprovements.cityImprovements [CityImprovementType.Science] + 1))
				&& (cityImprovements.cityImprovements [CityImprovementType.Science] != 0);
			break;
		case EventDieFace.Blue:
			canDraw = (redDieRoll <= (cityImprovements.cityImprovements [CityImprovementType.Politics] + 1))
				&& (cityImprovements.cityImprovements [CityImprovementType.Politics] != 0);
			break;
		case EventDieFace.Yellow:
			canDraw = (redDieRoll <= (cityImprovements.cityImprovements [CityImprovementType.Trade] + 1))
				&& (cityImprovements.cityImprovements [CityImprovementType.Trade] != 0);
			break;
		default:
			break;
		}

		return canDraw;
	}

	public bool canBuildMetropolis(CityImprovementType metropolisType) {
		return cityImprovements.cityImprovements [metropolisType] >= 4;
	}

	public bool canStealMetropolis(Player other, CityImprovementType metropolisType) {
		return this.cityImprovements.cityImprovements [metropolisType] > other.cityImprovements.cityImprovements [metropolisType];
	}

	public bool unlockedTradingHouse() {
		return cityImprovements.cityImprovements [CityImprovementType.Trade] >= 3;
	}

	public bool unlockedFortress() {
		return cityImprovements.cityImprovements [CityImprovementType.Politics] >= 3;
	}

	public bool unlockedAqueduct() {
		return cityImprovements.cityImprovements [CityImprovementType.Science] >= 3;
	}

	public void addOwnedUnit(Unit unit, System.Type type) {
		bool defaultCase = false;

		switch (type.ToString()) {
		case "Settlement":
			ownedUnits [typeof(Settlement)].Add ((Settlement)unit);
			break;
		case "City":
			ownedUnits [typeof(City)].Add ((City)unit);
			break;
		case "Metropolis":
			ownedUnits [typeof(Metropolis)].Add ((Metropolis)unit);
			break;
		case "CityWall":
			ownedUnits [typeof(CityWall)].Add ((CityWall)unit);
			break;
		case "Road":
			ownedUnits [typeof(Road)].Add ((Road)unit);
			break;
		case "Ship":
			ownedUnits [typeof(Ship)].Add ((Ship)unit);
			break;
		case "Knight":
			ownedUnits [typeof(Knight)].Add ((Knight)unit);
			break;
		default:
			ownedUnits [typeof(Unit)].Add (unit);
			defaultCase = true;
			break;
		}

		if (!defaultCase) {
			ownedUnits [typeof(Unit)].Add (unit);
		}
	}

	public void addOwnedUnit(Unit unit, UnitType unitType) {
		bool defaultCase = false;

		switch (unitType) {
		case UnitType.Settlement:
			ownedUnits [typeof(Settlement)].Add ((Settlement)unit);
			break;
		case UnitType.City:
			ownedUnits [typeof(City)].Add ((City)unit);
			break;
		case UnitType.Metropolis:
			ownedUnits [typeof(Metropolis)].Add ((Metropolis)unit);
			break;
		case UnitType.CityWalls:
			ownedUnits [typeof(CityWall)].Add ((CityWall)unit);
			break;
		case UnitType.Road:
			ownedUnits [typeof(Road)].Add ((Road)unit);
			break;
		case UnitType.Ship:
			ownedUnits [typeof(Ship)].Add ((Ship)unit);
			break;
		case UnitType.Knight:
			ownedUnits [typeof(Knight)].Add ((Knight)unit);
			break;
		default:
			ownedUnits [typeof(Unit)].Add (unit);
			defaultCase = true;
			break;
		}

		if (!defaultCase) {
			ownedUnits [typeof(Unit)].Add (unit);
		}
	}

	public void removeOwnedUnit(Unit unit, System.Type type) {
		bool defaultCase = false;

		switch (type.ToString()) {
		case "Settlement":
			ownedUnits [typeof(Settlement)].Remove ((Settlement)unit);
			break;
		case "City":
			ownedUnits [typeof(City)].Remove ((City)unit);
			break;
		case "Metropolis":
			ownedUnits [typeof(Metropolis)].Remove ((Metropolis)unit);
			break;
		case "CityWall":
			ownedUnits [typeof(CityWall)].Remove ((CityWall)unit);
			break;
		case "Road":
			ownedUnits [typeof(Road)].Remove ((Road)unit);
			break;
		case "Ship":
			ownedUnits [typeof(Ship)].Remove ((Ship)unit);
			break;
		case "Knight":
			ownedUnits [typeof(Knight)].Remove ((Knight)unit);
			break;
		default:
			ownedUnits [typeof(Unit)].Remove (unit);
			defaultCase = true;
			break;
		}

		if (!defaultCase) {
			ownedUnits [typeof(Unit)].Remove (unit);
		}
	}

	public void addOwnedUnit(Settlement unit) {
		ownedUnits [typeof(Settlement)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(City unit) {
		ownedUnits [typeof(City)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Metropolis unit) {
		ownedUnits [typeof(Metropolis)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(CityWall unit) {
		ownedUnits [typeof(CityWall)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Road unit) {
		ownedUnits [typeof(Road)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Ship unit) {
		ownedUnits [typeof(Ship)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Knight unit) {
		ownedUnits [typeof(Knight)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	//public void addOwnedUnit(Unit unit) {
	//	ownedUnits [typeof(Unit)].Add (unit);
	//}

	#region Asset Receive/Give Methods

	public void receiveAssets(AssetTuple assetToAdd) {
		this.receiveResources (assetToAdd.resources);
		this.receiveCommodities (assetToAdd.commodities);
		this.receiveFishTokens (assetToAdd.fishTokens);
	}

	public void spendAssets(AssetTuple assetToSpend) {
		this.spendResources (assetToSpend.resources);
		this.spendCommodities (assetToSpend.commodities);
		this.spendFishTokens (assetToSpend.fishTokens);
	}

	public bool hasAvailableAssets(AssetTuple assetsNeeded) {
		return hasAvailableResources (assetsNeeded.resources) && hasAvailableCommodities (assetsNeeded.commodities)
			&& hasAvailableFishTokens(assetsNeeded.fishTokens);
	}

	public AssetTuple getCurrentAssets() {
		return assets;
	}

	public int getNumAssets() {
		return getNumResources () + getNumCommodities () + getNumFishTokens();
	}

	public int getNumDiscardsNeeded() {
		int numAssets = getNumAssets () - getNumFishTokens();
		int numDiscardsNeeded = 0;

		if (numAssets > 7) {
			numDiscardsNeeded = (numAssets / 2);
		}

		return numDiscardsNeeded;
	}

	public AssetTuple getRandomSufficientAsset(int number) {
		AssetTuple randomAsset = new AssetTuple ();

		if (Random.Range (0.0f, 1.0f) <= 0.5f) {
			int randomResourceIndex;
			do{
				randomResourceIndex = Random.Range (0, Enum.GetNames (typeof(ResourceType)).Length - 1);
				randomAsset = GameAsset.getAssetOfIndex(randomResourceIndex, number);
			} while(!hasAvailableAssets(randomAsset));
		} else {
			int randomCommodityIndex;
			do{
				randomCommodityIndex = Random.Range (0, Enum.GetNames (typeof(CommodityType)).Length - 1) + 5;
				randomAsset = GameAsset.getAssetOfIndex(randomCommodityIndex, number);
			} while(!hasAvailableAssets(randomAsset));
		}

		return randomAsset;
	}

	public bool hasZeroAssets() {
		return hasZeroResources() && hasZeroCommodities();
	}

	#endregion

	#region Resource Receive/Give Methods

	public void receiveResources(ResourceTuple resourceToAdd) {
		List<ResourceType> resourceKeys = new List<ResourceType>(assets.resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resourceToAdd.resourceTuple [resourceKeys [i]] >= 0) {
				print ("Added " + resourceToAdd.resourceTuple [resourceKeys [i]].ToString () + " " + resourceKeys [i].ToString () + " to " + this.playerName);
				assets.resources.resourceTuple [resourceKeys [i]] += resourceToAdd.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public void spendResources(ResourceTuple resourceToSpend) {
		List<ResourceType> resourceKeys = new List<ResourceType>(assets.resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (assets.resources.resourceTuple [resourceKeys [i]] >= resourceToSpend.resourceTuple [resourceKeys [i]]) {
				print ("Subtracted " + resourceToSpend.resourceTuple [resourceKeys [i]].ToString () + " " + resourceKeys [i].ToString () + " from " + this.playerName);
				assets.resources.resourceTuple [resourceKeys [i]] -= resourceToSpend.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public bool hasAvailableResources(ResourceTuple resourcesNeeded) {
		List<ResourceType> resourceKeys = new List<ResourceType>(assets.resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (assets.resources.resourceTuple [resourceKeys [i]] < resourcesNeeded.resourceTuple [resourceKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveResource(ResourceType resource, int amount) {
		if (amount < 0) {
			//Error
		} else {
			assets.resources.resourceTuple [resource] += amount;
		}
	}

	public ResourceTuple getCurrentResources() {
		return assets.resources;
	}

	public int getNumResources() {
		int sum = 0;
		List<ResourceType> resourceKeys = new List<ResourceType>(assets.resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			sum += assets.resources.resourceTuple [resourceKeys [i]];
		}
		return sum;
	}

	public bool hasZeroResources() {
		bool zero = true;

		List<ResourceType> resourceKeys = new List<ResourceType>(assets.resources.resourceTuple.Keys);
		for (int i = 0; i < resourceKeys.Count; i++) {
			if (assets.resources.resourceTuple [resourceKeys [i]] != 0) {
				zero = false;
			}
		}

		return zero;
	}

	#endregion

	#region Commodity Receive/Give Methods

	public void receiveCommodities(CommodityTuple commodityToAdd) {
		List<CommodityType> commodityKeys = new List<CommodityType>(assets.commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (commodityToAdd.commodityTuple [commodityKeys [i]] >= 0) {
				print ("Added " + commodityToAdd.commodityTuple [commodityKeys [i]].ToString () + " " + commodityKeys [i].ToString () + " to " + this.playerName);
				assets.commodities.commodityTuple [commodityKeys [i]] += commodityToAdd.commodityTuple [commodityKeys [i]];
			}
		}
	}

	public void spendCommodities(CommodityTuple commodityToSpend) {
		List<CommodityType> commodityKeys = new List<CommodityType>(assets.commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (assets.commodities.commodityTuple [commodityKeys [i]] >= commodityToSpend.commodityTuple [commodityKeys [i]]) {
				print ("Subtracted " + commodityToSpend.commodityTuple [commodityKeys [i]].ToString () + " " + commodityKeys [i].ToString () + " from " + this.playerName);
				assets.commodities.commodityTuple [commodityKeys [i]] -= commodityToSpend.commodityTuple [commodityKeys [i]];
			}
		}
	}

	public bool hasAvailableCommodities(CommodityTuple commoditiesNeeded) {
		List<CommodityType> commodityKeys = new List<CommodityType>(assets.commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (assets.commodities.commodityTuple [commodityKeys [i]] < commoditiesNeeded.commodityTuple [commodityKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveCommodity(CommodityType commodity, int amount) {
		if (amount < 0) {
			//Error
		} else {
			assets.commodities.commodityTuple [commodity] += amount;
		}
	}

	public CommodityTuple getCurrentCommodities() {
		return assets.commodities;
	}

	public int getNumCommodities() {
		int sum = 0;
		List<CommodityType> commodityKeys = new List<CommodityType>(assets.commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			sum += assets.commodities.commodityTuple [commodityKeys [i]];
		}
		return sum;
	}

	public bool hasZeroCommodities() {
		bool zero = true;

		List<CommodityType> commodityKeys = new List<CommodityType>(assets.commodities.commodityTuple.Keys);
		for (int i = 0; i < commodityKeys.Count; i++) {
			if (assets.commodities.commodityTuple [commodityKeys [i]] != 0) {
				zero = false;
			}
		}

		return zero;
	}

	#endregion

	#region Fish Receive/Give Methods

	public void receiveFishTokens(FishTuple fishTokensToAdd) {
		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(assets.fishTokens.fishTuple.Keys);

		for (int i = 0; i < fishTokenKeys.Count; i++) {
			if (fishTokensToAdd.fishTuple [fishTokenKeys [i]] >= 0) {
				print ("Added " + fishTokensToAdd.fishTuple [fishTokenKeys [i]].ToString () + " " + fishTokenKeys [i].ToString () + " to " + this.playerName);
				assets.fishTokens.fishTuple [fishTokenKeys [i]] += fishTokensToAdd.fishTuple [fishTokenKeys [i]];
			}
		}
	}

	public void spendFishTokens(FishTuple fishTokensToSpend) {
		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(assets.fishTokens.fishTuple.Keys);

		for (int i = 0; i < fishTokenKeys.Count; i++) {
			if (assets.fishTokens.fishTuple [fishTokenKeys [i]] >= fishTokensToSpend.fishTuple [fishTokenKeys [i]]) {
				print ("Subtracted " + fishTokensToSpend.fishTuple [fishTokenKeys [i]].ToString () + " " + fishTokenKeys [i].ToString () + " from " + this.playerName);
				assets.fishTokens.fishTuple [fishTokenKeys [i]] -= fishTokensToSpend.fishTuple [fishTokenKeys [i]];
			}
		}
	}

	public bool hasAvailableFishTokens(FishTuple fishTokensNeeded) {
		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(assets.fishTokens.fishTuple.Keys);

		for (int i = 0; i < fishTokenKeys.Count; i++) {
			if (assets.fishTokens.fishTuple [fishTokenKeys [i]] < fishTokensNeeded.fishTuple [fishTokenKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveFishTokens(FishTokenType resource, int amount) {
		if (amount < 0) {
			//Error
		} else {
			assets.fishTokens.fishTuple[resource] += amount;
		}
	}

	public FishTuple getCurrentFishTokens() {
		return assets.fishTokens;
	}

	public int getNumFishTokens() {
		int sum = 0;
		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(assets.fishTokens.fishTuple.Keys);

		for (int i = 0; i < fishTokenKeys.Count; i++) {
			sum += assets.fishTokens.fishTuple [fishTokenKeys [i]];
		}
		return sum;
	}

	public bool hasZeroFishTokens() {
		bool zero = true;

		List<FishTokenType> fishTokenKeys = new List<FishTokenType>(assets.fishTokens.fishTuple.Keys);
		for (int i = 0; i < fishTokenKeys.Count; i++) {
			if (assets.fishTokens.fishTuple [fishTokenKeys [i]] != 0) {
				zero = false;
			}
		}

		return zero;
	}

	public bool hasOldBoot() {
		return assets.fishTokens.hasOldBoot ();
	}

	#endregion

	public int getMinimumTradeValue(int demandedInt) {
		if (demandedInt < 0 || demandedInt > 7) {
			return 4;
		} else {
			int minimum = 4;
			if (demandedInt < 5) {
				ResourceType typeDemanded = (ResourceType)demandedInt;
				int settlementsMin = findMinTradeValue (ownedUnits [typeof(Settlement)].Cast<IntersectionUnit>().ToList(), typeDemanded);
				int citiesMin = findMinTradeValue (ownedUnits [typeof(City)].Cast<IntersectionUnit>().ToList(), typeDemanded);
				minimum = Math.Min (settlementsMin, citiesMin);
			} else {
				CommodityType typeDemanded = (CommodityType)(demandedInt - 5);
				int settlementsMin = findMinTradeValue (ownedUnits [typeof(Settlement)].Cast<IntersectionUnit>().ToList(), typeDemanded);
				int citiesMin = findMinTradeValue (ownedUnits [typeof(City)].Cast<IntersectionUnit>().ToList(), typeDemanded);
				minimum = Math.Min (settlementsMin, citiesMin);
			}

			return minimum;
		}
	}

	public int findMinTradeValue(List<IntersectionUnit> units, ResourceType resourceType) {
		int min = 4;
		for (int i = 0; i < units.Count; i++) {
			Debug.Log ("Units[" + units.Count + "] intersection = " + units [i].locationIntersection + " with harbor = " + units [i].locationIntersection.harbor);
			if (units [i].locationIntersection.harbor != null) {
				Debug.Log ("harbor found of type: " + units [i].locationIntersection.harbor.resourceType);
				if (units [i].locationIntersection.harbor.resourceType == resourceType ||
					units [i].locationIntersection.harbor.resourceType == ResourceType.Null) {
					if (units [i].locationIntersection.harbor.tradeRatio < min) {
						min = units [i].locationIntersection.harbor.tradeRatio;
					}
				}
			}
		}
		return min;
	}

	public int findMinTradeValue(List<IntersectionUnit> units, CommodityType commodityType) {
		int min = 4;
		for (int i = 0; i < units.Count; i++) {
			Debug.Log ("Units[" + units.Count + "] intersection = " + units [i].locationIntersection + " with harbor = " + units [i].locationIntersection.harbor);
			if (units [i].locationIntersection.harbor != null) {
				Debug.Log ("Commodity:  harbor found of type: " + units [i].locationIntersection.harbor.resourceType);
				if (units [i].locationIntersection.harbor.commodityType == commodityType ||
					units [i].locationIntersection.harbor.resourceType == ResourceType.Null) {
					if (units [i].locationIntersection.harbor.tradeRatio < min) {
						min = units [i].locationIntersection.harbor.tradeRatio;
					}
				}
			}
		}
		return min;
	}

	public void playTurn() {

	}

	private void winGame() {
		// TODO
	}

	private int getTotalVictoryPoints() {
		List<Unit> unitsOwned = ownedUnits [typeof(Unit)];
		int totalVP = 0;
		for(int i = 0; i < unitsOwned.Count; i++) {
			totalVP += unitsOwned [i].victoryPointsWorth;
		}
		return totalVP;
	}

	public List<Unit> getOwnedUnitsOfType(System.Type type) {
		//print ("Getting type: " + type.ToString ());

		switch (type.ToString()) {
		case "Settlement":
			//print ("In Settlement");
			return ownedUnits [typeof(Settlement)];
		case "City":
			//print ("In City");
			return ownedUnits [typeof(City)];
		case "Metropolis":
			//print ("In Metropolis");
			return ownedUnits [typeof(Metropolis)];
		case "CityWall":
			//print ("In CityWall");
			return ownedUnits [typeof(CityWall)];
		case "Road":
			//print ("In Road");
			return ownedUnits [typeof(Road)];
		case "Ship":
			//print ("In Ship");
			return ownedUnits [typeof(Ship)];
		case "Knight":
			//print ("In Knight");
			return ownedUnits [typeof(Knight)];
		default:
			//print ("Default add case");
			return ownedUnits [typeof(Unit)];
		}
	}

	public List<Unit> getOwnedUnitsOfType(UnitType unitType) {
		//print ("Getting type: " + type.ToString ());

		switch (unitType) {
		case UnitType.Settlement:
			//print ("In Settlement");
			return ownedUnits [typeof(Settlement)];
		case UnitType.City:
			//print ("In City");
			return ownedUnits [typeof(City)];
		case UnitType.Metropolis:
			//print ("In Metropolis");
			return ownedUnits [typeof(Metropolis)];
		case UnitType.CityWalls:
			//print ("In CityWall");
			return ownedUnits [typeof(CityWall)];
		case UnitType.Road:
			//print ("In Road");
			return ownedUnits [typeof(Road)];
		case UnitType.Ship:
			//print ("In Ship");
			return ownedUnits [typeof(Ship)];
		case UnitType.Knight:
			//print ("In Knight");
			return ownedUnits [typeof(Knight)];
		default:
			//print ("Default add case");
			return ownedUnits [typeof(Unit)];
		}
	}

	public int[] getOwnedUnitIDsOfType(UnitType unitType) {
		List<Unit> ownedUnits = getOwnedUnitsOfType (unitType);
		int[] ownedUnitIDs = new int[ownedUnits.Count];

		for (int i = 0; i < ownedUnits.Count; i++) {
			ownedUnitIDs [i] = ownedUnits [i].id;
		}

		return ownedUnitIDs;
	}

	public List<Unit> getOwnedUnits() {
		List<Unit> clone = new List<Unit> (ownedUnits[typeof(Unit)]);
		return clone;
	}

	public List<Settlement> getOwnedUnitsOfThisType(Settlement settlement) {
		return ownedUnits [typeof(Settlement)].Cast<Settlement> ().ToList ();
	}

	public List<City> getOwnedUnitsOfThisType(City city) {
		return ownedUnits [typeof(City)].Cast<City> ().ToList ();
	}

	public List<Metropolis> getOwnedUnitsOfThisType(Metropolis unit) {
		return ownedUnits [typeof(Metropolis)].Cast<Metropolis> ().ToList ();
	}

	public List<CityWall> getOwnedUnitsOfThisType(CityWall unit) {
		return ownedUnits [typeof(CityWall)].Cast<CityWall> ().ToList ();
	}

	public List<Road> getOwnedUnitsOfThisType(Road unit) {
		return ownedUnits [typeof(Road)].Cast<Road> ().ToList ();
	}

	public List<Ship> getOwnedUnitsOfThisType(Ship unit) {
		return ownedUnits [typeof(Ship)].Cast<Ship> ().ToList ();
	}

	public List<Knight> getOwnedUnitsOfThisType(Knight unit) {
		return ownedUnits [typeof(Knight)].Cast<Knight> ().ToList ();
	}

	public int getNumUnits() {
		return ownedUnits[typeof(Unit)].Count;
	}

	public IEnumerator makeEdgeSelection(List<Edge> possibleEdges) {//, TradeUnit unitToBuild) {
		bool selectionMade = false;
		Edge selectedEdge;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.gameObject.tag == "Edge") {
					selectedEdge = hitInfo.transform.gameObject.GetComponent<Edge>();

					if (selectedEdge != null && selectedEdge.occupier == null && possibleEdges.Contains(selectedEdge)) {//(selectedEdge.isLandEdge() || selectedEdge.isShoreEdge())) {
						selectionMade = true;
						lastEdgeSelection = selectedEdge;
					}
				} 
			}
		}
	}

	public IEnumerator makeIntersectionSelection(List<Intersection> possibleIntersections) {
		bool selectionMade = false;
		Intersection selectedIntersection;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.gameObject.tag == "Intersection") {
					
					selectedIntersection = hitInfo.transform.gameObject.GetComponent<Intersection>();

					if (selectedIntersection != null && selectedIntersection.occupier == null && possibleIntersections.Contains(selectedIntersection)) {
						selectionMade = true;
						lastIntersectionSelection = selectedIntersection;
					}
				}
			} 
		}
	}

	public IEnumerator makeUnitSelection(List<Unit> possibleUnits) {
		bool selectionMade = false;
		Unit selectedUnit;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.parent.gameObject.tag == "Unit") {
					selectedUnit = hitInfo.transform.gameObject.GetComponentInParent<Unit>();

					if (selectedUnit != null && possibleUnits.Contains(selectedUnit)) {
						selectionMade = true;
						lastUnitSelection = selectedUnit;
					}
				}
			} 
		}
	}

	public IEnumerator makeGameTileSelection(List<GameTile> possibleTiles) {
		bool selectionMade = false;
		GameTile selectedTile;

		while (!selectionMade) {
			Debug.Log ("Waiting for game tile selection");
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.gameObject.tag == "GameTile") {

					selectedTile = hitInfo.transform.gameObject.GetComponent<GameTile> ();

					if (selectedTile != null && selectedTile.occupier == null && possibleTiles.Contains (selectedTile)) {
						selectionMade = true;
						lastGameTileSelection = selectedTile;
					} else {
						Debug.Log ("HIT BAD INTERSECTION..");
					}
				} else {
					Debug.Log ("hit object with tag: " + hitInfo.transform.gameObject.tag);
				}
			} else {
				Debug.Log ("NO HIT..");
			}
		}
	}
}
