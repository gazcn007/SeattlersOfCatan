using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributionManager : MonoBehaviour {

	private Object d6;
	private RollingDie redDie;
	private RollingDie yellowDie;
	private GameObject redSpawn;
	private GameObject yellowSpawn;

	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		//find spawnpoints
		d6=Resources.Load("Prefabs/d6");
		redSpawn = GameObject.Find("redDieSpawn");
		yellowSpawn = GameObject.Find("yellowDieSpawn");

		GameObject redDieObj = (GameObject) Instantiate (d6);

		GameObject yellowDieObj = (GameObject) Instantiate (d6);


		//instantiate both dies
		redDie=new RollingDie(redDieObj,"redDie", "d6-red", redSpawn.transform.position, Force(redSpawn));
		yellowDie= new RollingDie (yellowDieObj,"yellowDie", "d6-yellow", yellowSpawn.transform.position, Force(yellowSpawn));


		//resourceCostManager = GetComponent<ResourceCostManager> ();
	}

	public /*IEnumerator*/ int diceRollEvent() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		//print ("Red die rolled: " + redDieRoll);
		//print ("Yellow die rolled: " + yellowDieRoll);
		redDie.ReRoll();
		yellowDie.ReRoll ();
		return redDie.value + redDie.value;
	}

	//random force for dice roll
	private Vector3 Force(GameObject position)
	{
		Vector3 rollTarget = Vector3.zero + new Vector3(2 + 7 * Random.value, .5F + 4 * Random.value, -2 - 3 * Random.value);
		return Vector3.Lerp(position.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
	}

}
