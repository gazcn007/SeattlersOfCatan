using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : IntersectionUnit {

	public bool activated;
	public KnightRank rank;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum KnightRank {
	Basic = 0,
	Strong,
	Mighty
}
