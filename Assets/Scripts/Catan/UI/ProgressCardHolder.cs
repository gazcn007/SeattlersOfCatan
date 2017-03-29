using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCardHolder : MonoBehaviour {
	public GameObject progressCardPrefab;	
	public Image DisplayCardref;
	public List<ProgressCard> progressCardList;


	// Use this for initialization
	void Start () {


	}
	public void SpawnCard(ProgressCardColor color,ProgressCardType type){

		//spawn gameobject
		GameObject card= Instantiate (progressCardPrefab);
		ProgressCard newcard = card.GetComponent<ProgressCard> ();
		Image cardImage = card.GetComponent<Image> ();

		//set values
		newcard.type=type;
		newcard.color = color;
		newcard.cardSprite=Resources.Load<Sprite> ("ProgressCards/"+type.ToString());
		newcard.DisplayCard = DisplayCardref;
		cardImage.sprite=newcard.cardSprite;
		card.name = type.ToString ();
		//setactive
		card.transform.parent=this.transform;
		card.gameObject.SetActive (true);


		//add to list
		progressCardList.Add(newcard);
	}
}
