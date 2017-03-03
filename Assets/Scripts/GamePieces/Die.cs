using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Die : MonoBehaviour {

	public int minimumRoll = 1;
	public int maximumRoll = 6;

	public int roll() {
		int randomNum = Random.Range (minimumRoll, maximumRoll + 1);
		return randomNum;
	}
}
