using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributionManager : MonoBehaviour {


	private Die redDie;
	private Die yellowDie;
	private GameObject redDieObj;
	private GameObject yellowDieObj;
	private PrefabManager prefabManager;
	//Die eventDie;
	//ResourceCostManager resourceCostManager;

	// Use this for initialization
	void Start () {
		prefabManager = GetComponent<PrefabManager> ();
		redDieObj = (GameObject) Instantiate (prefabManager.d6);
		redDie = redDieObj.GetComponent<Die> ();
		redDieObj.GetComponent<Renderer>().material=(Material) Resources.Load("Materials/d6/d6-red");
		redDieObj.gameObject.SetActive (false);

		yellowDieObj = (GameObject) Instantiate (prefabManager.d6);
		yellowDie = yellowDieObj.GetComponent<Die> ();
		yellowDieObj.GetComponent<Renderer>().material=(Material) Resources.Load("Materials/d6/d6-yellow");
		yellowDieObj.gameObject.SetActive (false);
		//resourceCostManager = GetComponent<ResourceCostManager> ();
	}


	public /*IEnumerator*/ int diceRollEvent() {
		redDieObj.gameObject.SetActive (true);
		yellowDieObj.gameObject.SetActive (true);

		//reset position random rotations
		redDieObj.transform.position = new Vector3 (-6,1,-7);
		redDieObj.transform.Rotate (new Vector3 (Random.value * 360, Random.value * 360, Random.value * 360));
		yellowDieObj.transform.position = new Vector3(-6,1,-6);
		yellowDieObj.transform.Rotate (new Vector3 (Random.value * 360, Random.value * 360, Random.value * 360));

		//do some rolling
		int turnTime = 3;
		int time = 0;
		while (time < turnTime) {
			time ++;
			redDieObj.GetComponent<Rigidbody>().AddForce( new  Vector3(2,0,0), ForceMode.Impulse);
			redDieObj.GetComponent<Rigidbody>().AddTorque(new Vector3(2,0,0), ForceMode.Impulse);
			yellowDieObj.GetComponent<Rigidbody>().AddForce( new  Vector3(2,0,0), ForceMode.Impulse);
			yellowDieObj.GetComponent<Rigidbody>().AddTorque(new Vector3(2,0,0), ForceMode.Impulse);
		}
		//timer so dice dont dissapear right away
		time = 0;
		while (time < turnTime) {
			time ++;
		}

		redDie = redDieObj.GetComponent<Die> ();
		yellowDie = yellowDieObj.GetComponent<Die> ();
		//redDieObj.gameObject.SetActive (false);
		//yellowDieObj.gameObject.SetActive (false);
		return redDie.value+yellowDie.value;

	}
}
