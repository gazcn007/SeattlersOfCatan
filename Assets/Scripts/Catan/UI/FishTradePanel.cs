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

	private Vector3 scale;

	private AssetTuple currentTuple;

	void Start() {
		scale = this.transform.localScale;
	}

	// Update is called once per frame
	void Update (){

		//update texts
		for (int i = 0; i < FishTokens.Length; i++) {
			TokensText [i].text = FishTokens [i].value.ToString ();
		}
		//update total fish values
		tokensValue=(int)FishTokens [0].value+2*(int)FishTokens [1].value+3*(int)FishTokens [2].value;

		//set active rewards
		if (tokensValue >= 2) {
			optionsPanel [0].gameObject.SetActive (true);
		}
		else {
			optionsPanel [0].gameObject.SetActive (false);
		}
		if (tokensValue >= 3) {
			optionsPanel [1].gameObject.SetActive (true);
		}
		else {
			optionsPanel [1].gameObject.SetActive (false);
		}
		if (tokensValue >= 4) {
			optionsPanel [2].gameObject.SetActive (true);
		}
		else {
			optionsPanel [2].gameObject.SetActive (false);
		}
		if (tokensValue >= 5) {
			optionsPanel [3].gameObject.SetActive (true);
		} else {
			optionsPanel [3].gameObject.SetActive (false);
		}
		if (tokensValue >= 7) {
			optionsPanel [4].gameObject.SetActive (true);
		}
		else {
			optionsPanel [4].gameObject.SetActive (false);
		}
	}
	public void OpenPanel(AssetTuple playerAssets){
		for(int i = 0; i < FishTokens.Length; i++) {
			FishTokens [i].maxValue = playerAssets.GetValueAtIndex (i + 8);
			FishTokens [i].minValue = FishTokens [i].value = 0;
		}

		//set texts to 0
		for (int i = 0; i < TokensText.Length; i++) {
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
		StartCoroutine (spendFishTokenResults());
	}

	IEnumerator spendFishTokenResults() {
		int numTokensNeeded = 0;
		bool success = false;
		AssetTuple assetsToRemove = new AssetTuple();

		switch(rewardSelection){
		case 0:
			//move robber
			numTokensNeeded = 2;

			Robber robber = GameObject.FindObjectOfType<Robber> ();
			Pirate pirate = GameObject.FindObjectOfType<Pirate> ();
			if (robber != null && pirate != null) {
				
			} else if (robber != null) {
				StartCoroutine (CatanManager.instance.moveGamePieceForCurrentPlayer (0, true, false));

				for (int i = 0; i < FishTokens.Length; i++) {
					Debug.Log ("Set index " + (i + 8) + " of assetToRemove to = " + (int)FishTokens [i].value);
					assetsToRemove.SetValueAtIndex (i + 8, (int)FishTokens [i].value);
				}
				success = true;
			} else if (pirate != null) {

			}
			break;
		case 1:
			//steal random resource
			numTokensNeeded = 3;

			List<Player> stealableOpponents = new List<Player> ();
			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				stealableOpponents.Add (CatanManager.instance.players[i]);
			}
			stealableOpponents.Remove (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn]);

			if (stealableOpponents.Count != 0) {
				yield return StartCoroutine (CatanManager.instance.stealRandomResource (stealableOpponents));
				this.gameObject.SetActive (false);

				for(int i = 0; i < FishTokens.Length; i++) {
					Debug.Log ("Set index " + (i + 8) + " of assetToRemove to = " + (int)FishTokens [i].value);
					assetsToRemove.SetValueAtIndex (i + 8, (int)FishTokens [i].value);
				}
				success = true;
			}
			break;
		case 2:
			//take resource of choice
			numTokensNeeded = 4;

			CatanManager.instance.uiManager.fishResourceSelection ();
			bool selectionMade = false;
			this.transform.localScale = Vector3.zero;

			while (!selectionMade) {
				if (!CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
					yield return StartCoroutine (CatanManager.instance.uiManager.fishresourcepanel.waitUntilButtonDown ());
				}
				if (CatanManager.instance.uiManager.fishresourcepanel.selectionMade) {
					selectionMade = true;
				}
			}

			CatanManager.instance.uiManager.fishresourcepanel.gameObject.SetActive (false);
			this.transform.localScale = scale;
			this.gameObject.SetActive (false);

			AssetTuple assetsToGain = GameAsset.getAssetOfIndex (CatanManager.instance.uiManager.fishresourcepanel.getSelection (), 1);
			EventTransferManager.instance.OnTradeWithBank(CatanManager.instance.currentPlayerTurn, true, assetsToGain);

			for(int i = 0; i < FishTokens.Length; i++) {
				Debug.Log ("Set index " + (i + 8) + " of assetToRemove to = " + (int)FishTokens [i].value);
				assetsToRemove.SetValueAtIndex (i + 8, (int)FishTokens [i].value);
			}
			success = true;
			break;
		case 3:
			//build road
			numTokensNeeded = 5;

			int numRoadsInitial = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (UnitType.Road).Count;
			this.transform.localScale = Vector3.zero;

			yield return StartCoroutine (CatanManager.instance.unitManager.buildRoad (false));
			int numRoadsFinal = CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (UnitType.Road).Count;

			this.transform.localScale = scale;
			this.gameObject.SetActive (false);

			if (numRoadsInitial + 1 != numRoadsFinal) {
				this.gameObject.SetActive (true);
			} else {
				for(int i = 0; i < FishTokens.Length; i++) {
					Debug.Log ("Set index " + (i + 8) + " of assetToRemove to = " + (int)FishTokens [i].value);
					assetsToRemove.SetValueAtIndex (i + 8, (int)FishTokens [i].value);
				}
				success = true;
			}
			break;
		case 4:
			numTokensNeeded = 7;
			//draw progess card
			break;
		}

		if (success) {
			int numTokens = assetsToRemove.fishTokens.numTotalTokens ();
			int indexRemoved = 0;

			while (numTokens >= numTokensNeeded) {
				int nextLargestIndex = assetsToRemove.fishTokens.nextAvailableLargestIndex ();

				if(assetsToRemove.fishTokens.fishTuple.ContainsKey((FishTokenType)nextLargestIndex)) {
					assetsToRemove.fishTokens.fishTuple [(FishTokenType)nextLargestIndex]--;
					indexRemoved = nextLargestIndex;
				}
				else{
					break;
				}

				numTokens = assetsToRemove.fishTokens.numTotalTokens ();
			}

			assetsToRemove.fishTokens.fishTuple [(FishTokenType)indexRemoved]++;

			//CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].spendAssets (assetsToRemove);
			EventTransferManager.instance.OnTradeWithBank(CatanManager.instance.currentPlayerTurn, false, assetsToRemove);
			rewardSelection = -1;
			selectionGlow.gameObject.SetActive (false);
			this.gameObject.SetActive (false);
			StopAllCoroutines ();
			EventTransferManager.instance.OnOperationFailure ();
		}
	}

	public void Cancel(){
		StopAllCoroutines ();
		this.gameObject.SetActive (false);
		EventTransferManager.instance.OnOperationFailure ();
	}
	//setting selection glow
	public void setRewardGlow(FishTradePanelButton button){
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}
}
