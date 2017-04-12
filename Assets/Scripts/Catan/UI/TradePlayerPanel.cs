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
	public Text waiting;
	//used to upgrade every frame because network doesnt seem to want to do it
	public TradePlayerPanelButton ahack;
	public GameObject notification;
	public Text notificationText;

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
		if (selectionMade == true) {
			ahack.UpdateSelection ();
		}
	}
	public void OpenPanel (List<Player> opponents,AssetTuple playerAssets){
		this.gameObject.SetActive (true);

		//setting texts
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		playertext.text="Player Selection";
		waiting.text="Waiting For Response...";

		//texts start out disabled
		confirmtext.gameObject.SetActive (false);
		canceltext.gameObject.SetActive (false);
		waiting.gameObject.SetActive (false);

		//confirm map to sending
		confirm.onClick.AddListener (sendTrade);

		//counter not possible
		counter.gameObject.SetActive (false);

		//cancel button mapped to close panel
		cancel.onClick.AddListener (Cancel);
		//there is no selection to start with
		selectionGlow.gameObject.SetActive (false);


		for (int i = 0; i<3; i++) {
			if ((i+1)>opponents.Count) {
				optionsPanel [i].gameObject.SetActive (false);
			} else {

				optionsPanel [i].gameObject.SetActive (true);
				optionsPanel [i].enabled = true;
				optionsPanel [i].image.color = opponents [i].playerColor;

				TradePlayerPanelButton current= optionsPanel [i].GetComponentInChildren<TradePlayerPanelButton>();

				//set values for the buttons 
				current.instance = this;
				current.playernumber = opponents [i].playerNumber - 1;
				current.avatar.sprite = opponents [i].avatar;
			}
		}
		//set texts to 0
		for (int i = 0; i < 8; i++) {
			giveAssetNumTexts [i].text = "0";
			getAssetNumTexts [i].text = "0";
		}

		for(int i = 0; i < giveAssetSliders.Length; i++) {
			//make sure all sliders are enabled
			giveAssetSliders [i].enabled = true;
			getAssetSliders [i].enabled = true;

			//give should be set to the max value of what player has, get set to 10
			giveAssetSliders [i].maxValue = 10;
			giveAssetSliders [i].minValue = giveAssetSliders [i].value = 0;

			getAssetSliders [i].maxValue = 10;
			getAssetSliders [i].minValue = giveAssetSliders [i].value = 0;
		}
		//the tuple for player assets
		currentTuple = playerAssets;


		//default selection
		playerSelection = opponents [0].playerNumber - 1;
		//put glow on the selection;
		selectionMade = true;
		ahack=optionsPanel[0].GetComponentInChildren<TradePlayerPanelButton>();
		//selection not made make this panel active
		this.gameObject.SetActive (true);
	}
	public void OpenRespond(Player sender, AssetTuple give, AssetTuple recieve){
		this.gameObject.SetActive (true);
		selectionGlow.gameObject.SetActive (false);

		//setting texts
		confirmtext.text = "Accept Offer";
		canceltext.text = "Reject Offer";
		playertext.text="Offer From";
		waiting.text="Waiting For Response...";

		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();

		//texts start out disabled
		confirmtext.gameObject.SetActive (false);
		canceltext.gameObject.SetActive (false);
		waiting.gameObject.SetActive (false);

		//confirm is now accept trade
		confirm.onClick.AddListener (sendResponse);

		//activate counter button and add listener
		counter.onClick.AddListener (Counter);
		counter.gameObject.SetActive (true);

		//change cancel to reject
		cancel.onClick.AddListener (Reject);

		selectionGlow.gameObject.SetActive (false);

		//fill assets in reverse and disable changing them
		for(int i=0;i<giveAssetSliders.Length;i++){
			giveAssetSliders [i].maxValue = 10;
			getAssetSliders [i].maxValue = 10;
			giveAssetSliders [i].value = recieve.GetValueAtIndex (i);
			getAssetSliders [i].value = give.GetValueAtIndex (i);
			giveAssetSliders [i].enabled = false;
			getAssetSliders [i].enabled = false;
		}

		//only 1 player display is needed
		optionsPanel [0].gameObject.SetActive (true);
		optionsPanel [1].gameObject.SetActive (false);
		optionsPanel [2].gameObject.SetActive (false);

		//1st player panel is enabled but changing it is not possible
		optionsPanel [0].enabled = false;

		//set the color and avatar of the panel
		optionsPanel [0].image.color = sender.playerColor;
		TradePlayerPanelButton current= optionsPanel [0].GetComponentInChildren<TradePlayerPanelButton>();

		//set values for the buttons 
		current.instance = this;
		//no offset this is correct
		current.playernumber = sender.playerNumber-1;
		current.avatar.sprite = sender.avatar;
		selectionMade = true;
		ahack = current;
		//set button glow
		current.UpdateSelection ();
		this.gameObject.SetActive (false);
		this.gameObject.SetActive (true);
	}

	public void sendResponse(){

		//fill 2 new tuples with values from slider for sending
		AssetTuple getAsset = new AssetTuple ();
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
			Debug.Log ("GetAssetSliders[" + i + "] = " + getAssetSliders [i].value);
		}
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
			Debug.Log ("GiveAssetSliders[" + i + "] = " + giveAssetSliders [i].value);
		}
		if (CatanManager.instance.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (giveAsset)) {
			
			Debug.Log ("Calling OnTrades now");

			//incase unity is moody again and tries reverse trades ??????
			/*if(PhotonNetwork.player.ID - 1==CatanManager.instance.currentPlayerTurn+1){
				//trade call for player responding to trade
				EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, false, getAsset);
				EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, true, giveAsset);

				//trade call for sender
				EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, true, getAsset);
				EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, giveAsset);

				//cleanup operations
				EventTransferManager.instance.OnTradeEnd (CatanManager.instance.currentPlayerTurn,true);
				EventTransferManager.instance.OnOperationFailure ();
			}else{*/
			//trade call for player responding to trade
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, true, getAsset);
			EventTransferManager.instance.OnTradeWithBank (PhotonNetwork.player.ID - 1, false, giveAsset);

			//trade call for sender
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, getAsset);
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, true, giveAsset);

			//cleanup operations
			EventTransferManager.instance.OnTradeEnd (CatanManager.instance.currentPlayerTurn,true);
			EventTransferManager.instance.OnOperationFailure ();
		//	}
		}
	}

	public void sendTrade(){

		//user cannot do anything after trade is sent
		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();

		confirmtext.gameObject.SetActive (false);
		canceltext.gameObject.SetActive (false);

		//waiting for response visible
		waiting.gameObject.SetActive (true);

		//convert sliders to tuples
		AssetTuple getAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
		}
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
		}
		//rpc for trade sending, redundant check for assets
		if (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].hasAvailableAssets (giveAsset)) { 
			waiting.text="Waiting For Response...";
			EventTransferManager.instance.OnTradeOffer(CatanManager.instance.currentPlayerTurn, playerSelection, giveAsset, getAsset);
		}
	}	
	public void sendTradeCounter(){

		//user cannot do anything after trade is sent
		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();

		confirmtext.gameObject.SetActive (false);
		canceltext.gameObject.SetActive (false);

		//waiting for response visible
		waiting.gameObject.SetActive (true);

		//convert sliders to tuples
		AssetTuple getAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
		}
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
		}

		//rpc for trade sending, redundant check for assets
		if (CatanManager.instance.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (giveAsset)) { 
			waiting.text="Waiting For Response...";
			EventTransferManager.instance.OnTradeOfferCounter(PhotonNetwork.player.ID - 1, CatanManager.instance.currentPlayerTurn, giveAsset, getAsset);
		}
	}
	public void sendResponseCounter(){

		//fill 2 new tuples with values from slider for sending
		AssetTuple getAsset = new AssetTuple ();
		AssetTuple giveAsset = new AssetTuple ();
		for (int i = 0; i < getAssetSliders.Length; i++) {
			getAsset.SetValueAtIndex (i,(int) getAssetSliders [i].value);
			Debug.Log ("GetAssetSliders[" + i + "] = " + getAssetSliders [i].value);
		}
		for (int i = 0; i < giveAssetSliders.Length; i++) {
			giveAsset.SetValueAtIndex (i,(int) giveAssetSliders [i].value);
			Debug.Log ("GiveAssetSliders[" + i + "] = " + giveAssetSliders [i].value);
		}
		if (CatanManager.instance.players [PhotonNetwork.player.ID - 1].hasAvailableAssets (giveAsset)) {

			Debug.Log ("Calling OnTrade counter now");
			//trade call for player responding to trade
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, true, getAsset);
			EventTransferManager.instance.OnTradeWithBank (CatanManager.instance.currentPlayerTurn, false, giveAsset);

			//trade call for sender
			EventTransferManager.instance.OnTradeWithBank (playerSelection, false, getAsset);
			EventTransferManager.instance.OnTradeWithBank (playerSelection, true, giveAsset);
			//cleanup operations
			EventTransferManager.instance.OnTradeEnd (playerSelection,true);
			EventTransferManager.instance.OnOperationFailure ();

		}

	}
	public void OpenRespondCounter(Player sender, AssetTuple give, AssetTuple recieve){
		playerSelection = sender.playerNumber;
		//open response but rebind confirm button
		OpenRespond (sender, give, recieve);
		counter.gameObject.SetActive (false);
		confirm.onClick.RemoveAllListeners ();
		confirm.onClick.AddListener (sendResponseCounter);
	}

	public void Cancel(){
		for (int i = 0; i < EventTransferManager.instance.playerChecks.Length; i++) {
			EventTransferManager.instance.OnPlayerReady(i, true);
		}
		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();
		counter.onClick.RemoveAllListeners ();
		EventTransferManager.instance.OnOperationFailure ();
		this.gameObject.SetActive (false);
	}

	public void Reject(){
		//some rejection rpc
		EventTransferManager.instance.OnTradeEnd(CatanManager.instance.currentPlayerTurn,false);
		EventTransferManager.instance.OnPlayerReady(PhotonNetwork.player.ID - 1, true);
		EventTransferManager.instance.OnOperationFailure ();

		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();
		counter.onClick.RemoveAllListeners ();
		this.gameObject.SetActive (false);


	}

	public void Counter(){

		confirm.onClick.RemoveAllListeners ();
		cancel.onClick.RemoveAllListeners ();

		//setting texts
		confirmtext.text = "Send Offer";
		canceltext.text = "Cancel";
		playertext.text="Offer To";
		waiting.gameObject.SetActive (false);

		//counter no longer an option
		confirm.onClick.AddListener (sendTradeCounter);
		counter.gameObject.SetActive (false);
		cancel.onClick.AddListener (Reject);
		cancel.onClick.AddListener (sendTradeCounter);

		//enable the sliders
		for(int i=0;i<giveAssetSliders.Length;i++){
			giveAssetSliders [i].enabled = true;
			getAssetSliders [i].enabled = true;
		}

		//counter can only be sent to original player
		optionsPanel [0].enabled = false;
		optionsPanel [1].gameObject.SetActive (false);
		optionsPanel [2].gameObject.SetActive (false);

	}
	//setting player selection glow
	public void setSelectionGlow(TradePlayerPanelButton button){
		ahack = button;
		selectionGlow.gameObject.SetActive (true);
		selectionGlow.gameObject.transform.position = button.gameObject.transform.position;
	}
}