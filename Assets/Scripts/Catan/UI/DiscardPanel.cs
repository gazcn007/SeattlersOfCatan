using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPanel : MonoBehaviour {

	public Text leftText;
	public Slider[] assetSliders;
	public Text[] assetNumTexts;
	public Button submitButton;

	public AssetTuple discardTuple;

	private AssetTuple currentTuple;
	private int neededDiscards;
	private int leftDiscards;
	private bool selectionMade = false;

	// Use this for initialization
	void Start () {
		submitButton.onClick.AddListener (submitSelection);
	}
	
	// Update is called once per frame
	void Update () {
		if (!selectionMade) {
			leftText.text = "Remaining : " + leftDiscards.ToString ();

			int sum = 0;
			for (int i = 0; i < assetSliders.Length; i++) {
				//assetSliders [i].maxValue = currentTuple.GetValueAtIndex (i);
				assetNumTexts [i].text = assetSliders [i].value.ToString ();
				sum += (int)assetSliders [i].value;
			}
			leftDiscards = neededDiscards - sum;
		}
	}

	public void displayPanelForAssets(AssetTuple playerAssets, int delta, bool isPositive) {
		neededDiscards = delta;
		leftDiscards = neededDiscards;

		foreach (Text text in assetNumTexts) {
			text.text = "0";
		}

		for(int i = 0; i < assetSliders.Length; i++) {
			if (isPositive) {
				assetSliders [i].maxValue = delta;
			} else {
				assetSliders [i].maxValue = playerAssets.GetValueAtIndex (i);
			}

			assetSliders [i].minValue = assetSliders [i].value = 0;
		}
			
		currentTuple = playerAssets;
		selectionMade = false;
		this.gameObject.SetActive (true);
	}

	public void submitSelection() {
		if (leftDiscards == 0) {
			selectionMade = true;

			AssetTuple assetsToDiscard = new AssetTuple (0, 0, 0, 0, 0, 0, 0, 0);
			for (int i = 0; i < assetSliders.Length; i++) {
				assetsToDiscard.SetValueAtIndex (i, (int)assetSliders [i].value);
			}

			discardTuple = assetsToDiscard;
		} else {
			Debug.Log ("Need to select the correct amount!");
		}
	}

	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		while (!selectionMade) {
			yield return new WaitForEndOfFrame ();
		}
	}

}
