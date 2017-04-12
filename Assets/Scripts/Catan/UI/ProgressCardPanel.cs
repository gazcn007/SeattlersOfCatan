using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressCardPanel : MonoBehaviour {

	//values panel holds
	public ProgressCardType cardType;
	public ProgressCardColor cardColor;
	public Image cardToDisplay;
	public Button confirm;
	public Button cancel;
	public Text titleText;
	public UIManager uiManager;

	public ProgressCardHolder cardHolder;


	//this displays a new card and creates the card
	public void newCard(ProgressCardColor color,ProgressCardType type){
		this.gameObject.SetActive (true);
		confirm.onClick.RemoveAllListeners ();
		cardType = type;
		cardColor = color;
		//first display card on panel
		cardToDisplay.sprite=Resources.Load<Sprite> ("ProgressCards/"+cardType.ToString());
		titleText.text = "New Progress Card";
		//spawn this new card in the ui
		StartCoroutine(cardHolder.SpawnCard(color,type));

		//on click for confirm is decativation of this panel
		confirm.onClick.AddListener (Cancel);
		//no cancel button
		cancel.gameObject.SetActive (false);

	}
	public void SubmitCard(ProgressCardColor color,ProgressCardType type){
		this.gameObject.SetActive (true);
		confirm.onClick.RemoveAllListeners ();
		cardType = type;
		cardColor = color;

		//first display card on panel
		cardToDisplay.sprite=Resources.Load<Sprite> ("ProgressCards/"+cardType.ToString());

		titleText.text = "Do You Want To Play This Card?";
		//assign play card option to panel and cancel as disable panel
		confirm.onClick.AddListener (PlayCard);
		cancel.onClick.AddListener (Cancel);
		cancel.gameObject.SetActive (true);


	}

	public void Cancel(){
		this.gameObject.SetActive (false);
	}


	public void PlayCard (){

		GameObject.FindGameObjectWithTag ("ProgressCardsStackManager").GetComponent<ProgressCardStackManager> ().playCard (cardType);
	

		this.gameObject.SetActive (false);
	}

}
