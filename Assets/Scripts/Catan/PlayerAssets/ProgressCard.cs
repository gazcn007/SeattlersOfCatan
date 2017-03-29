using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCard: MonoBehaviour  {

	public ProgressCardType type;
	public ProgressCardColor color;
	public Sprite cardSprite;
	public Image DisplayCard;

	//mouseover effect functions
	public void OnMouseEnter(){
		if (DisplayCard.isActiveAndEnabled == false) {
			DisplayCard.gameObject.SetActive (true);
		}

		float height = DisplayCard.GetComponent<RectTransform> ().rect.height;
		Vector3 position = new Vector3 (this.transform.position.x, this.transform.position.y+200, 0);
		DisplayCard.transform.position = position;
		DisplayCard.sprite = cardSprite;
	}
	public void OnMouseExit(){
		if (DisplayCard.isActiveAndEnabled == true) {
			DisplayCard.gameObject.SetActive (false);
		}
	}
}
public enum ProgressCardColor{
	Green = 0,
	Yellow,
	Blue
}
public enum ProgressCardType {
	Alchemist = 0,
	Crane,
	Mining,
	Irrigation,
	Printer,
	Inventor,
	Engineer,
	Medicine,
	Smith,
	RoadBuilding,
	Merchant,
	CommercialHarbor,
	MerchantFleet,
	MasterMerchant,
	TradeMonopoly,
	ResourceMonopoly,
	Bishop,
	Diplomat,
	Warlord,
	Wedding,
	Intrigue,
	Saboteur,
	Spy,
	Deserter,
	Constitution
}