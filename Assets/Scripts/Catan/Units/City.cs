using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Settlement {

	public CityWall cityWalls;

	void Start() {
		victoryPointsWorth = 2;
	}
}
