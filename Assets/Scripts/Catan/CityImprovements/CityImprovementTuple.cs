using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityImprovementTuple {

	public Dictionary<CityImprovementType, int> cityImprovements = new Dictionary<CityImprovementType, int>();
	public Dictionary<CityImprovementType, CommodityType> upgradeCommodityChart = new Dictionary<CityImprovementType, CommodityType>();

	public CityImprovementTuple() {
		cityImprovements.Add (CityImprovementType.Science, 0);
		cityImprovements.Add (CityImprovementType.Politics, 0);
		cityImprovements.Add (CityImprovementType.Trade, 0);

		FillUpgradeChart ();
	}

	public void FillUpgradeChart() {
		upgradeCommodityChart.Add (CityImprovementType.Science, CommodityType.Paper);
		upgradeCommodityChart.Add (CityImprovementType.Politics, CommodityType.Coin);
		upgradeCommodityChart.Add (CityImprovementType.Trade, CommodityType.Cloth);
	}

	public void ImproveCityOfType(CityImprovementType improvementType) {
		if (cityImprovements [improvementType] < 5) {
			cityImprovements [improvementType]++;
		}
	}

	public AssetTuple nextImprovementCost(CityImprovementType improvementType) {
		CommodityTuple commodityCost = new CommodityTuple ();

		if (cityImprovements [improvementType] < 5) {
			commodityCost.addCommodityWithType (upgradeCommodityChart [improvementType], cityImprovements [improvementType] + 1);
		}

		return new AssetTuple (new ResourceTuple(), commodityCost);
	}
}

public enum CityImprovementType {
	Science = 0,
	Politics,
	Trade
}

public enum EventDieFace {
	Green = 0,
	Blue,
	Yellow,
	Black
}
