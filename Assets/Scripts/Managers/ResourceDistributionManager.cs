using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributionManager : MonoBehaviour {

	private Object d6;
	private RollingDie redDie;
	private RollingDie yellowDie;


	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		//find spawnpoints


		//instantiate both dies
		redDie=new RollingDie(GameObject.Find("DieSpawn"),"redDie", "d6-red", GameObject.Find("DieSpawn").transform.position);
		yellowDie= new RollingDie (GameObject.Find("DieSpawn"),"yellowDie", "d6-yellow", GameObject.Find("DieSpawn").transform.position);

		//resourceCostManager = GetComponent<ResourceCostManager> ();
	}

	public /*IEnumerator*/ int diceRollEvent() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
		redDie.ReRoll();
		yellowDie.ReRoll ();
		int redval = redDie.value;
		int yellowval = yellowDie.value;
		print ("test: " + redval +", " + yellowval);
		return redDie.value + redDie.value;
	}

}
