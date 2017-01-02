using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCostManager : MonoBehaviour {
	public ResourceTuple settlementCost;
	public ResourceTuple cityCost;
	//public ResourceTuple metropolisCost;
	public ResourceTuple cityWallsCost;

	public ResourceTuple roadCost;
	public ResourceTuple shipCost;

	public ResourceTuple knightCost;
	public ResourceTuple knightActivationCost;

	private static Dictionary<System.Type, ResourceTuple> costsChart = new Dictionary<System.Type, ResourceTuple>();

	// Use this for initialization
	void Awake () {
		costsChart.Add (typeof(Settlement), settlementCost);
		costsChart.Add (typeof(City), cityCost);
		//costsChart.Add (metropolis.GetType ().ToString (), metropolisCost);
		costsChart.Add (typeof(CityWall), cityWallsCost);

		costsChart.Add (typeof(Road), roadCost);
		costsChart.Add (typeof(Ship), shipCost);

		costsChart.Add (typeof(Knight), knightCost);
	}
	
	public static ResourceTuple getCostOfUnit(System.Type unitType) {
		if (!costsChart.ContainsKey (unitType)) {
			return null;
		}
		return costsChart [unitType];
	}
}
