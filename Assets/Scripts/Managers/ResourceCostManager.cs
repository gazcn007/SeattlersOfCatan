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
		/*settlementCost.printResourceTuple ();
		cityCost.printResourceTuple ();
		cityWallsCost.printResourceTuple ();
		roadCost.printResourceTuple ();
		shipCost.printResourceTuple ();
		knightCost.printResourceTuple ();
		*/
		/*settlementCost = new ResourceTuple();
		cityCost = new ResourceTuple();
		cityWallsCost = new ResourceTuple();

		roadCost = new ResourceTuple();
		shipCost = new ResourceTuple();

		knightCost = new ResourceTuple();*/

		costsChart.Add (typeof(Settlement), new ResourceTuple(settlementCost.numBricks, settlementCost.numGrains, settlementCost.numLumbers, settlementCost.numOres, settlementCost.numWools));
		costsChart.Add (typeof(City), new ResourceTuple(cityCost.numBricks, cityCost.numGrains, cityCost.numLumbers, cityCost.numOres, cityCost.numWools));
		//costsChart.Add (metropolis.GetType ().ToString (), metropolisCost);
		costsChart.Add (typeof(CityWall), new ResourceTuple(cityWallsCost.numBricks, cityWallsCost.numGrains, cityWallsCost.numLumbers, cityWallsCost.numOres, cityWallsCost.numWools));

		costsChart.Add (typeof(Road), new ResourceTuple(roadCost.numBricks, roadCost.numGrains, roadCost.numLumbers, roadCost.numOres, roadCost.numWools));
		costsChart.Add (typeof(Ship), new ResourceTuple(shipCost.numBricks, shipCost.numGrains, shipCost.numLumbers, shipCost.numOres, shipCost.numWools));

		costsChart.Add (typeof(Knight), new ResourceTuple(knightCost.numBricks, knightCost.numGrains, knightCost.numLumbers, knightCost.numOres, knightCost.numWools));
	}

	public void populateResourceCostManager() {
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

	void Update() {
		//settlementCost.printResourceTuple ();
		//cityCost.printResourceTuple ();
		//cityWallsCost.printResourceTuple ();
		//roadCost.printResourceTuple ();
		//shipCost.printResourceTuple ();
		//knightCost.printResourceTuple ();
	}
}
