using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harbor : MonoBehaviour {

	public int id;

	public ResourceType resourceType = ResourceType.Null;
	public CommodityType commodityType = CommodityType.Null;

	public List<Intersection> locations = new List<Intersection> ();

	public int tradeRatio = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
