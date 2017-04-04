using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePlayerPanel : MonoBehaviour {


	//attributes for getting the player selection
	public Image selectionGlow;
	public int playerSelection;
	public List<Button> optionsPanel;
	public bool selectionMade;
	public Button cancel;
	public Button counter;
	public Text confirmtext;
	public Text canceltext;


	//slider holders
	public Slider[] giveAssetSliders;
	public Text[] giveAssetNumTexts;
	public Slider[] getAssetSliders;
	public Text[] getAssetNumTexts;
	private AssetTuple currentTuple;

	void Update(){
		
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAssetNumTexts [i].text = giveAssetSliders [i].value.ToString ();
			getAssetNumTexts [i].text = getAssetSliders [i].value.ToString ();
		}
	}

	public void OpenPanel (List<Player> opponents,AssetTuple playerAssets){
		this.gameObject.SetActive (true);
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		counter.gameObject.SetActive (false);
		cancel.onClick.AddListener (Cancel);
		selectionGlow.gameObject.SetActive (false);

		for (int i = 0; i<3; i++) {
			if ((i+1)>opponents.Count) {
				optionsPanel [i].gameObject.SetActive (false);
			} else {
				optionsPanel [i].gameObject.SetActive (true);
				optionsPanel [i].image.color = opponents [i].playerColor;
				TradePlayerPanelButton current= optionsPanel [i].GetComponentInChildren<TradePlayerPanelButton>();

				//set values for the buttons 
				current.instance = this;
				current.playernumber = i;
				current.avatar.sprite = opponents [i].avatar;

				//set button
				optionsPanel[i].onClick.AddListener (current.UpdateSelection);
			}
		}
		//set texts to 0
		for (int i = 0; i < 8; i++) {
			giveAssetNumTexts [i].text = "0";
			getAssetNumTexts [i].text = "0";
		}

		for(int i = 0; i < giveAssetSliders.Length; i++) {
			giveAssetSliders [i].maxValue = playerAssets.GetValueAtIndex (i);
			giveAssetSliders [i].minValue = giveAssetSliders [i].value = 0;
		}

		currentTuple = playerAssets;

		selectionMade = false;
		this.gameObject.SetActive (true);
		selectionMade = false;
		this.gameObject.SetActive (true);

	}
	public void OpenCounter(){
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		counter.gameObject.SetActive (false);
		cancel.onClick.AddListener (Cancel);
		selectionGlow.gameObject.SetActive (false);

	}

	public void OpenRespond(){
		confirmtext.text = "Accept Offer";
		canceltext.text = "Reject Offer";
		counter.gameObject.SetActive (true);
		cancel.onClick.AddListener (Reject);
		selectionGlow.gameObject.SetActive (false);

	}
	public void sendTrade(){

	}
	public void Cancel(){
		this.gameObject.SetActive (false);
	}
	public void Reject(){
		this.gameObject.SetActive (false);
	}
	public void Counter(){

	}
	//setting player selection glow
	public void setSelectionGlow(TradePlayerPanelButton button){
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}

}
