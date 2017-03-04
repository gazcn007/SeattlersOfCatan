using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributionManager : MonoBehaviour {

	private Object d6;
	public RollingDie redDie;
	public RollingDie yellowDie;


	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		//find spawnpoints


		//instantiate both dies
		redDie=new RollingDie(GameObject.Find("redDieSpawn"),"redDie", "d6-red", GameObject.Find("redDieSpawn").transform.position);
		yellowDie= new RollingDie (GameObject.Find("yellowDieSpawn"),"yellowDie", "d6-yellow", GameObject.Find("yellowDieSpawn").transform.position);

		//resourceCostManager = GetComponent<ResourceCostManager> ();
	}

	public /*IEnumerator*/ int diceRollEvent() {
		//yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));
		redDie.ReRoll();
		yellowDie.ReRoll ();
		return redDie.value + redDie.value;
	}

}
