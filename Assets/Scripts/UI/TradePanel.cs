using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : MonoBehaviour {

	public Button[] buttonsOnPanel;
	public ToggleGroup toggleGroup;
	private Toggle[] toggles;
	public Dropdown resourceDropdown;
	public Slider slider;
	private Text numText;

	// Use this for initialization
	void Start () {
		buttonsOnPanel = GetComponentsInChildren<Button> ();
		toggleGroup = GetComponentInChildren<ToggleGroup> ();
		toggles = toggleGroup.GetComponentsInChildren<Toggle> ();
		resourceDropdown = GetComponentInChildren<Dropdown> ();
		slider = GetComponentInChildren<Slider> ();
		numText = slider.GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		numText.text = slider.value.ToString ();
	}

	public ResourceType getTradeChoice() {
		return (ResourceType)resourceDropdown.value;
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
}
