using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradePanelButton : MonoBehaviour {
	public int id;
	public TradePanel instance;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SelectionGetEvent(){
		instance.getselection = id;
		instance.setGetGlow (this);
	}
	public void SelectionGiveEvent(){
		instance.giveselection =id;
		instance.setGiveGlow (this);
	}
	public void SelectionGetGoldEvent(){
		instance.getgoldselection =id;
		instance.setGetGoldGlow (this);
	}
}
