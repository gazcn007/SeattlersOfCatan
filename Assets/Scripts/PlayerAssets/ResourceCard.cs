using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCard : GameAsset {

	public ResourceType type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public enum ResourceType {
	Brick = 0,
	Grain,
	Lumber,
	Ore,
	Wool,
	Null
}

