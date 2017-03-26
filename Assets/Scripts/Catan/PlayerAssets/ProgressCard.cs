using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCard: MonoBehaviour  {

	public ProgressCardType type;
	public ProgressCardColor color;
	public Sprite cardSprite;
	// Use this for initialization
	void Start () {
		cardSprite=Resources.Load<Sprite> ("ProgressCards/"+type.ToString());
	}
	// Update is called once per frame
	void Update () {
		
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