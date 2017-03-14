using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

	public static UnitManager instance;

	public GameObject settlementPrefab;
	public GameObject cityPrefab;
	public GameObject metropolisPrefab;
	public GameObject cityWallPrefab;

	public GameObject roadPrefab;
	public GameObject shipPrefab;

	public GameObject robberPrefab;
	public GameObject piratePrefab;

	public GameObject basicKnightPrefab;
	public GameObject strongKnightPrefab;
	public GameObject mightyKnightPrefab;

	public int unitID = 0;
	public Dictionary<int, Unit> unitsInPlay = new Dictionary<int, Unit> ();

	public Dictionary<UnitType, GameObject> unitPrefabsDictionary = new Dictionary<UnitType, GameObject>();

	public Dictionary<int, Unit> Units {
		get {return unitsInPlay;}
	}

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
	}

	void Start() {
		PopulatePrefabsDictionary ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void PopulatePrefabsDictionary() {
		unitPrefabsDictionary.Add (UnitType.Settlement, settlementPrefab);
		unitPrefabsDictionary.Add (UnitType.City, cityPrefab);
		unitPrefabsDictionary.Add (UnitType.Metropolis, metropolisPrefab);
		unitPrefabsDictionary.Add (UnitType.CityWalls, cityWallPrefab);

		unitPrefabsDictionary.Add (UnitType.Road, roadPrefab);
		unitPrefabsDictionary.Add (UnitType.Ship, shipPrefab);

		unitPrefabsDictionary.Add (UnitType.Knight, basicKnightPrefab);
	}

	public GameObject GetPrefabOfType(UnitType unitType) {
		if (!unitPrefabsDictionary.ContainsKey (unitType)) {
			return null;
		}
		return unitPrefabsDictionary [unitType];
	}

	public void removeUnitFromGame(Unit unit) {
		if (unitsInPlay.ContainsKey (unit.id)) {
			Destroy (unitsInPlay [unit.id].gameObject);
			unitsInPlay.Remove (unit.id);
		} else {
			Destroy (unit.gameObject);
		}
	}

	public IEnumerator buildSettlement() {
		yield return StartCoroutine (CatanManager.instance.buildIntersectionUnit (UnitType.Settlement));
	}

	public IEnumerator buildCity() {
		yield return StartCoroutine (CatanManager.instance.buildIntersectionUnit (UnitType.City));
	}


	public IEnumerator buildRoad() {
		yield return StartCoroutine (CatanManager.instance.buildEdgeUnit (UnitType.Road));
	}

	public IEnumerator buildShip() {
		yield return StartCoroutine (CatanManager.instance.buildEdgeUnit (UnitType.Ship));
	}

	public IEnumerator upgradeSettlement() {
		yield return StartCoroutine (CatanManager.instance.upgradeSettlement());
	}

}

public enum UnitType {
	Settlement = 0,
	City,
	Road,
	Ship,
	Metropolis,
	CityWalls,
	Knight
}
