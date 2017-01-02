using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceTuple {

	public int numBricks;
	public int numGrains;
	public int numLumbers;
	public int numOres;
	public int numWools;

	public Dictionary<ResourceType, int> resourceTuple = new Dictionary<ResourceType, int> ();

	// Use this for initialization
	public ResourceTuple () {
		resourceTuple.Add (ResourceType.Brick, numBricks);
		resourceTuple.Add (ResourceType.Grain, numGrains);
		resourceTuple.Add (ResourceType.Lumber, numLumbers);
		resourceTuple.Add (ResourceType.Ore, numOres);
		resourceTuple.Add (ResourceType.Wool, numWools);
	}
}
