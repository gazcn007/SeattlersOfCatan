using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishTradePanelButton : MonoBehaviour {

	public int id;
	public FishTradePanel instance;
	public Text text;

	public void RewardSelectionEvent(){
		instance.rewardSelection = id;
		instance.setRewardGlow (this);
	}
	public void OnMouseEnter(){
		text.gameObject.SetActive (true);
	}
	public void OnMouseExit(){
		text.gameObject.SetActive (false);
	}
}
