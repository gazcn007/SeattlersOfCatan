using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCardHolder : MonoBehaviour {
	public GameObject progressCardPrefab;	
	public Image DisplayCardref;
	public List<ProgressCard> progressCardList;
	public UIManager UIinstance;

	//ui method
	public void togglePanel(){
		if (this.isActiveAndEnabled == true) {
			this.gameObject.SetActive (false);
		} else {
			this.gameObject.SetActive (true);
		}
	}
	//adds new cards to the ui
	public IEnumerator SpawnCard(ProgressCardColor color,ProgressCardType type){

		//spawn gameobject
		GameObject card= Instantiate (progressCardPrefab);
		ProgressCard newcard = card.GetComponent<ProgressCard> ();
		Image cardImage = card.GetComponent<Image> ();
		Debug.Log ("card type: " + type.ToString ());
		//set values
		newcard.type=type;
		newcard.color = color;
		newcard.cardSprite=Resources.Load<Sprite> ("ProgressCards/"+type.ToString());
		newcard.DisplayCard = DisplayCardref;
		newcard.UIinstance = UIinstance;
		cardImage.sprite=newcard.cardSprite;
		card.name = type.ToString ();
		if (type==ProgressCardType.Printer || type==ProgressCardType.Constitution) {
			GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ().playCard (type);
		} else {
			EventTransferManager.instance.addCardToHand (PhotonNetwork.player.ID - 1, type);
		}
		card.transform.parent=this.transform;
		card.gameObject.SetActive (true);
		//add to list
		progressCardList.Add(newcard);

		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		if(clientCatanManager.players[PhotonNetwork.player.ID-1].progressCards.Count>4){
			bool selectionMade = false;
			//get the selection
			clientCatanManager.uiManager.spyPanel.openPanel(clientCatanManager.players[PhotonNetwork.player.ID-1].progressCards,"You have too many cards, select 1 to discard");
			while (!selectionMade) {
				if (!clientCatanManager.uiManager.spyPanel.selectionMade) {
					yield return StartCoroutine (CatanManager.instance.uiManager.spyPanel.waitUntilButtonDown ());
				}else{
					selectionMade = true;
				}
				Debug.Log ("test: " + selectionMade.ToString());
			}

			ProgressCardType selectedcard = clientCatanManager.uiManager.spyPanel.selection;
			clientCatanManager.uiManager.spyPanel.selectionMade=false;
			clientCatanManager.uiManager.spyPanel.gameObject.SetActive (false);

			int temp = (int)selectedcard;
			ProgressCardColor selectedcolor;
			if (temp <= 10) {
				selectedcolor = ProgressCardColor.Green;
			}else if(temp<=16){
				selectedcolor = ProgressCardColor.Yellow;
			}
			else{
				selectedcolor=ProgressCardColor.Blue;
			}
			selectionMade = false;
			clientCatanManager.uiManager.spyPanel.selectionMade=false;
			clientCatanManager.uiManager.spyPanel.gameObject.SetActive (false);

	

			GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ().returnCardToStack(selectedcolor,selectedcard);
		}
	}
	public void spawnLoad(){
		CatanManager clientCatanManager = GameObject.FindGameObjectWithTag ("CatanManager").GetComponent<CatanManager> ();
		Player currentplayer=clientCatanManager.players[PhotonNetwork.player.ID];

		//spawn the 2 odd ones if needed
		if (currentplayer.playedConstitution) {
			
			GameObject card= Instantiate (progressCardPrefab);
			ProgressCard newcard = card.GetComponent<ProgressCard> ();
			Image cardImage = card.GetComponent<Image> ();
			//set values
			newcard.type=ProgressCardType.Constitution;
			newcard.color =ProgressCardColor.Blue;
			newcard.cardSprite=Resources.Load<Sprite> ("ProgressCards/"+ProgressCardType.Constitution.ToString());
			newcard.DisplayCard = DisplayCardref;
			newcard.UIinstance = UIinstance;
			cardImage.sprite=newcard.cardSprite;
			card.name = ProgressCardType.Constitution.ToString ();
			card.transform.parent=this.transform;
			card.gameObject.SetActive (true);

		}
		if (currentplayer.playedPrinter) {
			GameObject card= Instantiate (progressCardPrefab);
			ProgressCard newcard = card.GetComponent<ProgressCard> ();
			Image cardImage = card.GetComponent<Image> ();
			//set values
			newcard.type=ProgressCardType.Printer;
			newcard.color =ProgressCardColor.Green;
			newcard.cardSprite=Resources.Load<Sprite> ("ProgressCards/"+ProgressCardType.Printer.ToString());
			newcard.DisplayCard = DisplayCardref;
			newcard.UIinstance = UIinstance;
			cardImage.sprite=newcard.cardSprite;
			card.name = ProgressCardType.Printer.ToString ();
			card.transform.parent=this.transform;
			card.gameObject.SetActive (true);
		}
		for (int i = 0; i < currentplayer.progressCards.Count; i++) {
			ProgressCardType curr=currentplayer.progressCards[i];
			GameObject card= Instantiate (progressCardPrefab);
			ProgressCard newcard = card.GetComponent<ProgressCard> ();
			Image cardImage = card.GetComponent<Image> ();
			//set values
			newcard.type=curr;
			newcard.color =ProgressCardColor.Green;
			newcard.cardSprite=Resources.Load<Sprite> ("ProgressCards/"+curr.ToString());
			newcard.DisplayCard = DisplayCardref;
			newcard.UIinstance = UIinstance;
			cardImage.sprite=newcard.cardSprite;
			card.name = curr.ToString ();
			card.transform.parent=this.transform;
			card.gameObject.SetActive (true);
		}
	}
}
