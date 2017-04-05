using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishTuple {

	public int numOneTokens;
	public int numTwoTokens;
	public int numThreeTokens;

	public Dictionary<FishTokenType, int> fishTuple = new Dictionary<FishTokenType, int>();

	public FishTuple() {
		fishTuple = new Dictionary<FishTokenType, int> ();

		fishTuple.Add (FishTokenType.One, numOneTokens);
		fishTuple.Add (FishTokenType.Two, numTwoTokens);
		fishTuple.Add (FishTokenType.Three, numThreeTokens);
	}

	public FishTuple(int ones, int twos, int threes) {
		fishTuple = new Dictionary<FishTokenType, int>();

		numOneTokens = ones;
		numTwoTokens = twos;
		numThreeTokens = threes;

		fishTuple.Add (FishTokenType.One, numOneTokens);
		fishTuple.Add (FishTokenType.Two, numTwoTokens);
		fishTuple.Add (FishTokenType.Three, numThreeTokens);
	}

	public void addFishTokenWithType(FishTokenType key, int value) {
		if(fishTuple.ContainsKey(key)) {
			fishTuple[key] = value;
		}
		else{
			fishTuple.Add(key, value);
		}
	}

	public void printFishTuple() {
		MonoBehaviour.print ("This FishTuple has: ");
		foreach (var pair in fishTuple) {
			MonoBehaviour.print (pair.Key.ToString () + " = " + pair.Value.ToString ());
		}

	}
}

public enum FishTokenType {
	One = 0,
	Two,
	Three
}
