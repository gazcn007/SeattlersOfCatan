using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobberStealPanelButton : MonoBehaviour {

	public Image avatar;
	public RobberStealPanel instance;
	public int playernumber;

	// Use this for initialization
	void Start () {
		avatar= GetComponentsInChildren<Image> () [0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void UpdateSelection(){
		instance.selection = playernumber;
		instance.setSelectionGlow (this);
	}
}
