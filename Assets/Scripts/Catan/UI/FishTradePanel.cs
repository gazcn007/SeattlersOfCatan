using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishTradePanel : MonoBehaviour {

	//attributes for getting the player selection
	public Image selectionGlow;
	public int rewardSelection;
	public List<Button> optionsPanel;
	public int tokensValue;
	public bool selectionMade;


	//slider holders
	public Slider[] FishTokens;
	public Text[] TokensText;

	private AssetTuple currentTuple;
	// Update is called once per frame
	void Update (){

		//update texts
		for (int i = 0; i < FishTokens.Length; i++) {
			TokensText [i].text = FishTokens [i].value.ToString ();
		}
		//update total fish values
		tokensValue=(int)FishTokens [0].value+2*(int)FishTokens [1].value+3*(int)FishTokens [2].value;

		//set active rewards
		if (tokensValue > 2) {
			optionsPanel [0].gameObject.SetActive (true);
		}
		else {
			optionsPanel [0].gameObject.SetActive (false);
		}
		if (tokensValue > 3) {
			optionsPanel [1].gameObject.SetActive (true);
		}
		else {
			optionsPanel [1].gameObject.SetActive (false);
		}
		if (tokensValue > 4) {
			optionsPanel [2].gameObject.SetActive (true);
		}
		else {
			optionsPanel [2].gameObject.SetActive (false);
		}
		if (tokensValue > 5) {
			optionsPanel [3].gameObject.SetActive (true);
		} else {
			optionsPanel [3].gameObject.SetActive (false);
		}
		if (tokensValue > 7) {
			optionsPanel [4].gameObject.SetActive (true);
		}
		else {
			optionsPanel [4].gameObject.SetActive (false);
		}
	}
	public void OpenPanel(AssetTuple playerAssets){

		for(int i = 0; i < FishTokens.Length; i++) {
			//FishTokens [i].maxValue = playerAssets.GetFishTokensAtIndex (i);
			FishTokens [i].minValue = FishTokens [i].value = 0;
		}

		//set texts to 0
		for (int i = 0; i < 8; i++) {
			TokensText [i].text = "0";
		}
		tokensValue = 0;
		//all rewards disabled
		for (int i = 0; i < optionsPanel.Count; i++) {
			optionsPanel [i].GetComponent<FishTradePanelButton> ().instance = this;
			optionsPanel [i].gameObject.SetActive (false);
		}

		this.gameObject.SetActive (true);

	}
	public void confirmSelection(){
		switch(rewardSelection){
		case 0:
			break;
			//move robber
		case 1:
			//steal random resource
			break;
		case 2:
			//take resource of choice
			break;
		case 3:
			//build road
			break;
		case 4:
			//draw progess card
			break;
		}
	}
	public void Cancel(){
		this.gameObject.SetActive (false);
	}
	//setting selection glow
	public void setRewardGlow(FishTradePanelButton button){
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}
}
