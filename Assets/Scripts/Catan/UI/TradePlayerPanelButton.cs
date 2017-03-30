using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePlayerPanelButton : MonoBehaviour {

	public Image avatar;
	public TradePlayerPanel instance;
	public int playernumber;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}
	public void UpdateSelection(){
		instance.playerSelection = playernumber;
		instance.setSelectionGlow (this);
	}
}
