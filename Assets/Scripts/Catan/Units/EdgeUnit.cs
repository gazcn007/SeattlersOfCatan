﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EdgeUnit : Unit {

	public Edge locationEdge;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public abstract bool isRoad ();

}
