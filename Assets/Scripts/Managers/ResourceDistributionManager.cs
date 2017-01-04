using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributionManager : MonoBehaviour {

	public GameObject numericDiePrefab;
	private Die redDie;
	private Die yellowDie;
	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		GameObject redDieObj = (GameObject) Instantiate (numericDiePrefab);
		redDie = redDieObj.GetComponent<NumericDie> ();

		GameObject yellowDieObj = (GameObject) Instantiate (numericDiePrefab);
		yellowDie = yellowDieObj.GetComponent<NumericDie> ();

		//resourceCostManager = GetComponent<ResourceCostManager> ();
	}

	public /*IEnumerator*/ int diceRollEvent() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
		int redDieRoll = redDie.roll ();
		int yellowDieRoll = yellowDie.roll ();

		print ("Red die rolled: " + redDieRoll);
		print ("Yellow die rolled: " + yellowDieRoll);

		return redDieRoll + yellowDieRoll;
	}
}
