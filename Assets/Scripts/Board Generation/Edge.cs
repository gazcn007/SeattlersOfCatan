using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

	public List<GameObject> adjacentHexes;
	// Use this for initialization
	void Start () {
		adjacentHexes = new List<GameObject>(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
