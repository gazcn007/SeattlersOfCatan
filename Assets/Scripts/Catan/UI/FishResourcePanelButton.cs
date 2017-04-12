using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishResourcePanelButton : MonoBehaviour {
	public int id;
	public FishResourcePanel instance;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void SelectionGetEvent(){
		instance.selection = id;
		instance.valchange = true;
		instance.setGlow (this);
	}

}
