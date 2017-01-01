using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCard : GameAsset {

	public ProgressCardType type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum ProgressCardType {
	Brick = 0,
	Grain,
	Lumber,
	Ore,
	Wool
}
