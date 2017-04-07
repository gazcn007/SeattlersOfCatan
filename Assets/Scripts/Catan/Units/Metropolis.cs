using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metropolis : City {

	public MetropolisType metropolisType;

	void Start() {
		victoryPointsWorth = 4;

	}
}

public enum MetropolisType {
	Science = 0,
	Politics,
	Trade
}
