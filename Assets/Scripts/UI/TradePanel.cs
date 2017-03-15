using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : MonoBehaviour {

	public Button[] buttonsOnPanel;
	public ToggleGroup toggleGroup;
	private Toggle[] toggles;
	public Dropdown spendDropdown;
	public Dropdown receiveDropdown;
	public Slider slider;
	private Text numText;
	private Text errorText;

	// Use this for initialization
	void Start () {
		buttonsOnPanel = GetComponentsInChildren<Button> ();
		toggleGroup = GetComponentInChildren<ToggleGroup> ();
		//toggles = toggleGroup.GetComponentsInChildren<Toggle> ();
		spendDropdown = GetComponentsInChildren<Dropdown> ()[0];
		receiveDropdown = GetComponentsInChildren<Dropdown> ()[1];
		slider = GetComponentInChildren<Slider> ();
		numText = slider.GetComponentInChildren<Text> ();
		GameObject errorTextObject = ComponentFinderExtension.FindChildByName (this.gameObject, "ErrorText");
		errorText = errorTextObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		numText.text = slider.value.ToString ();
	}

	public ResourceType getTradeChoice() {
		return (ResourceType)spendDropdown.value;
	}

	public int getTradeChoiceInt() {
		return spendDropdown.value;
	}

	public ResourceType getReceiveChoice() {
		int activeNumber = -1;
		Toggle activeToggle = ToggleGroupExtension.GetActive(toggleGroup);

		for (int i = 0; i < toggles.Length; i++) {
			if (activeToggle == toggles [i]) {
				activeNumber = i;
			}
		}

		if (activeNumber == -1) {
			return ResourceType.Null;
		} else {
			return (ResourceType)activeNumber;
		}
	}

	public int getReceiveChoiceInt() {
		return receiveDropdown.value;
	}

	public void showNotEnoughError(int choice) {
		//Tuple<ResourceType, CommodityType> outcome = GameAsset.getProductionAssetsOfIndex (choice);
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
}
