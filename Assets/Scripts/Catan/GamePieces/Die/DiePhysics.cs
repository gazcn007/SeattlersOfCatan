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
//	//Update is called once per frame
//	void Update(){
//		if (Input.GetKeyDown (KeyCode.Space)) {
//			GetComponent<Rigidbody>().AddForce (Random.onUnitSphere * forceAmount, forceMode);
//			GetComponent<Rigidbody>().AddTorque (Random.onUnitSphere * torqueAmount, forceMode);
//		}
//	}
}
