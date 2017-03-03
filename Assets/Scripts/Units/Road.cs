using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : TradeUnit {

	private bool assigned;
	private Vector3 distance;
	private Vector2 position;

	// Use this for initialization
	void Start() {
		
	}

	void OnMouseDown() {
		//distance = Camera.main.WorldToScreenPoint (this.transform.position);
	}

	public override bool isRoad() {
		return true;
	}
}
