using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : MonoBehaviour {

	public Button[] buttonsOnPanel;
	public Button[] givePanel;
	public Button[] getPanel;
	public Image getglow;
	public Image giveglow;

	public int giveselection;
	public int getselection;
	private Text errorText;

	// Use this for initialization
	void Start () {
		buttonsOnPanel = GetComponentsInChildren<Button> ();
		GameObject errorTextObject = ComponentFinderExtension.FindChildByName (this.gameObject, "ErrorText");

		//assign buttons for both panels to toggle changes only givepanel same length can do in 1 for loop
		givePanel = this.transform.FindChild("GivePanel").gameObject.GetComponentsInChildren<Button> ();
		getPanel = this.transform.FindChild("GetPanel").gameObject.GetComponentsInChildren<Button> ();

		for (int i = 0; i < givePanel.Length; i++) {

			//get instance
			TradePanelButton giveinstance = givePanel[i].GetComponentInChildren<TradePanelButton>();
			TradePanelButton getinstance = getPanel [i].GetComponentInChildren<TradePanelButton> ();

			//assign ids and instances
			giveinstance.id=i;
			getinstance.id=i;
			giveinstance.instance=this;
			getinstance.instance=this;

			//assign listeners
			getPanel[i].onClick.AddListener (getinstance.SelectionGetEvent);
			givePanel[i].onClick.AddListener (giveinstance.SelectionGiveEvent);
		}
			
		errorText = errorTextObject.GetComponent<Text> ();
		errorText.gameObject.SetActive (false);
		giveglow.gameObject.SetActive (false);
		getglow.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getTradeChoiceInt() {
		return giveselection;
	}
		
	public int getReceiveChoiceInt() {
		return getselection;
	}

	public void showNotEnoughError(int choice) {
		errorText.gameObject.SetActive (true);

		if (choice < 5) {
			errorText.text = "Error! Not Enough " + ((ResourceType)choice).ToString () + "s!";
		} else {
			errorText.text = "Error! Not Enough " + ((CommodityType)choice - 5).ToString () + "s!";
		}

	}

	public void hideErrorText() {
		errorText.gameObject.SetActive (false);
	}
	public void setGiveGlow(TradePanelButton button){
		giveglow.gameObject.SetActive (true);
		giveglow.gameObject.transform.position = button.gameObject.transform.position;
	}
	public void setGetGlow(TradePanelButton button){
		getglow.gameObject.SetActive (true);
		getglow.gameObject.transform.position = button.gameObject.transform.position;
	}
}
