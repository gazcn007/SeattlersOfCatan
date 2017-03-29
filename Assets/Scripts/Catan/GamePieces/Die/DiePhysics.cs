using UnityEngine;
using System.Collections;

public class DiePhysics : MonoBehaviour {
	public float forceAmount = 10.0f;
	public float torqueAmount = 10.0f;
	public ForceMode forceMode;

	void Start(){
	}

	public void init(Vector3 randomVector){
		GetComponent<Rigidbody>().AddForce (randomVector * forceAmount, forceMode);
		GetComponent<Rigidbody>().AddTorque (randomVector * torqueAmount, forceMode);
	}

	//@todo : maybe add another feature to hold space to fire the dice
}
