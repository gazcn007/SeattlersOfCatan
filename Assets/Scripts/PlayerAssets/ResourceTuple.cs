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

	public Dictionary<ResourceType, int> resourceTuple;

	// Use this for initialization
	public ResourceTuple() {
		resourceTuple = new Dictionary<ResourceType, int>();

		resourceTuple.Add (ResourceType.Brick, numBricks);
		resourceTuple.Add (ResourceType.Grain, numGrains);
		resourceTuple.Add (ResourceType.Lumber, numLumbers);
		resourceTuple.Add (ResourceType.Ore, numOres);
		resourceTuple.Add (ResourceType.Wool, numWools);
	}

	public ResourceTuple(int bricks, int grains, int lumbers, int ores, int wools) {
		resourceTuple = new Dictionary<ResourceType, int>();

		numBricks = bricks;
		numGrains = grains;
		numLumbers = lumbers;
		numOres = ores;
		numWools = wools;


		resourceTuple.Add (ResourceType.Brick, numBricks);
		resourceTuple.Add (ResourceType.Grain, numGrains);
		resourceTuple.Add (ResourceType.Lumber, numLumbers);
		resourceTuple.Add (ResourceType.Ore, numOres);
		resourceTuple.Add (ResourceType.Wool, numWools);
	}

	public void addResourceWithType(ResourceType key, int value) {
		if(resourceTuple.ContainsKey(key)) {
			resourceTuple[key] = value;
		}
		else{
			resourceTuple.Add(key, value);
		}
	}

	public void printResourceTuple() {
		MonoBehaviour.print ("This ResourceTuple has: ");
		foreach (var pair in resourceTuple) {
			MonoBehaviour.print (pair.Key.ToString () + " = " + pair.Value.ToString ());
		}
		//MonoBehaviour.print("Num bricks: " + numBricks);
		//MonoBehaviour.print("Num grains: " + numGrains);
		//MonoBehaviour.print("Num lumbers: " + numLumbers);
		//MonoBehaviour.print("Num ores: " + numOres);
		//MonoBehaviour.print("Num wools: " + numWools);

	}
}