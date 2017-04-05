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
	public Button confirm;
	public Button cancel;
	public Button counter;
	public Text confirmtext;
	public Text canceltext;
	public Text playertext;

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
		//setting texts
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		playertext.text="Player Selection";

		//confirm map to sending
		confirm.onClick.AddListener (sendTrade);
		//counter not possible
		counter.gameObject.SetActive (false);
		//cancel button mapped to close panel
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
				//missing offset
				current.playernumber = opponents [i].playerNumber - 1;
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
			giveAssetSliders [i].enabled = true;
			getAssetSliders [i].enabled = true;
			giveAssetSliders [i].maxValue = playerAssets.GetValueAtIndex (i);
			giveAssetSliders [i].minValue = giveAssetSliders [i].value = 0;
			getAssetSliders [i].maxValue = 10;
			getAssetSliders [i].minValue = giveAssetSliders [i].value = 0;
		}

		currentTuple = playerAssets;

		playerSelection = opponents [0].playerNumber - 1;
		selectionMade = false;
		this.gameObject.SetActive (true);
		selectionMade = false;
		this.gameObject.SetActive (true);
	}

	public void OpenRespond(Player sender, AssetTuple give, AssetTuple recieve){
		this.gameObject.SetActive (true);

		for(int i=0;i<giveAssetSliders.Length;i++){
			giveAssetSliders [i].value = give.GetValueAtIndex (i);
			getAssetSliders [i].value = recieve.GetValueAtIndex (i);
			giveAssetSliders [i].enabled = false;
			getAssetSliders [i].enabled = false;
		}
		//only 1 is enabled
		optionsPanel [0].gameObject.SetActive (true);
		optionsPanel [1].gameObject.SetActive (false);
		optionsPanel [2].gameObject.SetActive (false);
		optionsPanel [0].enabled = false;

		optionsPanel [0].image.color = sender.playerColor;
		TradePlayerPanelButton current= optionsPanel [0].GetComponentInChildren<TradePlayerPanelButton>();

		//set values for the buttons 
		current.instance = this;
		current.playernumber = sender.playerNumber;
		current.avatar.sprite = sender.avatar;

		//set button
		optionsPanel[0].onClick.AddListener (current.UpdateSelection);
		current.UpdateSelection ();

		confirmtext.text = "Accept Offer";
		canceltext.text = "Reject Offer";
		playertext.text="Offer From";

		confirm.onClick.AddListener (sendResponse);
		counter.onClick.AddListener (Counter);
		counter.gameObject.SetActive (true);
		cancel.onClick.AddListener (Reject);
		selectionGlow.gameObject.SetActive (false);

		this.gameObject.SetActive (true);
	}

	public void sendResponse(){
		AssetTuple getAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
			Debug.Log ("GetAssetSliders[" + i + "] = " + getAssetSliders [i].value);
		}
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
			Debug.Log ("GiveAssetSliders[" + i + "] = " + giveAssetSliders [i].value);
		}

		if (CatanManager.instance.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (giveAsset)) {
			Debug.Log ("Calling OnTrades now");
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, false, getAsset);
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, true, giveAsset);

			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, true, getAsset);
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, giveAsset);

			EventTransferManager.instance.OnTradeRespose (false);
			EventTransferManager.instance.OnOperationFailure ();
		}
	}

	public void sendTrade(){
		//convert sliders to tuples
		AssetTuple getAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
		}
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
		}

		//rpc for trade sending
		if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].hasAvailableAssets (giveAsset)) { 
			EventTransferManager.instance.OnTradeOffer (CatanManager.instance.currentPlayerTurn, playerSelection, giveAsset, getAsset);
		}
	}

	public void Cancel(){
		for (int i = 0; i < EventTransferManager.instance.playerChecks.Length; i++) {
			EventTransferManager.instance.OnPlayerReady(i, true);
		}
		EventTransferManager.instance.OnOperationFailure ();
		this.gameObject.SetActive (false);
	}

	public void Reject(){
		//some rejection rpc
		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
		EventTransferManager.instance.OnOperationFailure ();
		this.gameObject.SetActive (false);
	}

	public void Counter(){
		for(int i=0;i<giveAssetSliders.Length;i++){
			giveAssetSliders [i].enabled = true;
			getAssetSliders [i].enabled = true;
		}
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		playertext.text="Player Selection";

		counter.gameObject.SetActive (false);
		cancel.onClick.AddListener (Cancel);
		selectionGlow.gameObject.SetActive (false);
	}

	//setting player selection glow
	public void setSelectionGlow(TradePlayerPanelButton button){
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}

}