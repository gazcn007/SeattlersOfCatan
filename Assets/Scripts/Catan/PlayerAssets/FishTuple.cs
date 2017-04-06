using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishTuple {

	public int numOneTokens;
	public int numTwoTokens;
	public int numThreeTokens;

	public int oldBoot;

	public Dictionary<FishTokenType, int> fishTuple = new Dictionary<FishTokenType, int>();

	public FishTuple() {
		fishTuple = new Dictionary<FishTokenType, int> ();

		fishTuple.Add (FishTokenType.One, numOneTokens);
		fishTuple.Add (FishTokenType.Two, numTwoTokens);
		fishTuple.Add (FishTokenType.Three, numThreeTokens);
		fishTuple.Add (FishTokenType.OldBoot, oldBoot);
	}

	public FishTuple(int ones, int twos, int threes) {
		fishTuple = new Dictionary<FishTokenType, int>();

		numOneTokens = ones;
		numTwoTokens = twos;
		numThreeTokens = threes;
		oldBoot = 0;

		fishTuple.Add (FishTokenType.One, numOneTokens);
		fishTuple.Add (FishTokenType.Two, numTwoTokens);
		fishTuple.Add (FishTokenType.Three, numThreeTokens);
		fishTuple.Add (FishTokenType.OldBoot, oldBoot);
	}

	public FishTuple(int ones, int twos, int threes, int oldBoot) {
		fishTuple = new Dictionary<FishTokenType, int>();

		numOneTokens = ones;
		numTwoTokens = twos;
		numThreeTokens = threes;
		this.oldBoot = oldBoot;

		fishTuple.Add (FishTokenType.One, numOneTokens);
		fishTuple.Add (FishTokenType.Two, numTwoTokens);
		fishTuple.Add (FishTokenType.Three, numThreeTokens);
		fishTuple.Add (FishTokenType.OldBoot, oldBoot);
	}

	public void addFishTokenWithType(FishTokenType key, int value) {
		Debug.Log ("Adding " + value + " of " + key.ToString () + " to this fish tuple");
		if(fishTuple.ContainsKey(key)) {
			fishTuple[key] = value;
		}
		else{
			fishTuple.Add(key, value);
		}
	}

	public void OldBootReceive(bool received) {
		if (received) {
			fishTuple [FishTokenType.OldBoot] = 1;
		} else {
			fishTuple [FishTokenType.OldBoot] = 0;
		}
	}

	public bool hasOldBoot() {
		return fishTuple [FishTokenType.OldBoot] == 1;
	}

	public void printFishTuple() {
		MonoBehaviour.print ("This FishTuple has: ");
		foreach (var pair in fishTuple) {
			MonoBehaviour.print (pair.Key.ToString () + " = " + pair.Value.ToString ());
		}
	}

	public int numTotalTokens() {
		int sum = 0;

		for (int i = 0; i < fishTuple.Values.Count - 1; i++) {
			sum += (i + 1) * fishTuple [(FishTokenType)i];
		}
		return sum;
	}

	public int numTokens() {
		int sum = 0;

		for (int i = 0; i < fishTuple.Values.Count - 1; i++) {
			sum += fishTuple [(FishTokenType)i];
		}
		return sum;
	}

	public int nextAvailableLargestIndex() {
		if (fishTuple [FishTokenType.Three] != 0) {
			return (int)FishTokenType.Three;
		} else if (fishTuple [FishTokenType.Two] != 0) {
			return (int)FishTokenType.Two;
		} else if (fishTuple [FishTokenType.One] != 0) {
			return (int)FishTokenType.One;
		}
		return 0;
	}

	public int nextAvailableSmallestIndex() {
		if (fishTuple [FishTokenType.One] != 0) {
			return (int)FishTokenType.One;
		} else if (fishTuple [FishTokenType.Two] != 0) {
			return (int)FishTokenType.Two;
		} else if (fishTuple [FishTokenType.Three] != 0) {
			return (int)FishTokenType.Three;
		}
		return 2;
	}

	public List<FishTokenType> listForm() {
		List<FishTokenType> fishTokens = new List<FishTokenType> ();

		foreach (var pair in fishTuple) {
			int numTokensOfType = pair.Value;
			for (int i = 0; i < numTokensOfType; i++) {
				fishTokens.Add (pair.Key);
			}
		}
		ListShuffler.Shuffle (fishTokens);
		return fishTokens;
	}
}

public enum FishTokenType {
	One = 0,
	Two,
	Three,
	OldBoot
}
