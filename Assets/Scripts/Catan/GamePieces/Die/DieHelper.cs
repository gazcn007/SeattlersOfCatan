using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieHelper : MonoBehaviour {
	public int value=-1;
	public int id;
	// Use this for initialization
	// Update is called once per frame
	void Update () {
			value = this.gameObject.GetComponent<FaceDetection> ().value;
	}
}
