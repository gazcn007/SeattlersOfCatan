﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

	public static UnitManager instance;

	public GameObject settlementPrefab;
	public GameObject cityPrefab;
	public GameObject cityWallPrefab;

	public GameObject[] metropolisPrefabs;

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
	public Dictionary<MetropolisType, GameObject> metropolisPrefabsDictionary = new Dictionary<MetropolisType, GameObject> ();

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
		unitPrefabsDictionary.Add (UnitType.CityWalls, cityWallPrefab);

		unitPrefabsDictionary.Add (UnitType.Road, roadPrefab);
		unitPrefabsDictionary.Add (UnitType.Ship, shipPrefab);

		unitPrefabsDictionary.Add (UnitType.Knight, basicKnightPrefab);

		metropolisPrefabsDictionary.Add (MetropolisType.Science, metropolisPrefabs [0]);
		metropolisPrefabsDictionary.Add (MetropolisType.Politics, metropolisPrefabs [1]);
		metropolisPrefabsDictionary.Add (MetropolisType.Trade, metropolisPrefabs [2]);
	}

	public GameObject GetPrefabOfType(UnitType unitType) {
		if (!unitPrefabsDictionary.ContainsKey (unitType)) {
			return null;
		}
		return unitPrefabsDictionary [unitType];
	}

	public GameObject GetMetropolisOfType(MetropolisType metropolisType) {
		if (!metropolisPrefabsDictionary.ContainsKey (metropolisType)) {
			return null;
		}
		return metropolisPrefabsDictionary [metropolisType];
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


	public IEnumerator buildRoad(bool paid) {
		yield return StartCoroutine (CatanManager.instance.buildEdgeUnit (UnitType.Road, paid));
	}

	public IEnumerator buildShip() {
		yield return StartCoroutine (CatanManager.instance.buildEdgeUnit (UnitType.Ship, true));
	}

	public IEnumerator upgradeSettlement() {
		yield return StartCoroutine (CatanManager.instance.upgradeSettlement());
	}

	public IEnumerator moveShip() {
		yield return StartCoroutine (CatanManager.instance.moveShip());
	}

	public IEnumerator upgradeCity(int metropolistType) {
		yield return StartCoroutine (CatanManager.instance.upgradeCity(metropolistType));
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
