using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTile : MonoBehaviour {

	public int id;
	public GameTile locationTile;
	public int diceValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setDiceValue(int diceValue) {
		this.transform.FindChild ("Dice Value").gameObject.SetActive (true);
		TextMesh randomDiceValue = this.transform.FindChild ("Dice Value").gameObject.GetComponentInChildren<TextMesh>();
		string diceValueString = diceValue.ToString () + "\n";

		for (int k = Mathf.Abs (diceValue - 7); k < 6; k++) {
			diceValueString = diceValueString + ".";
		}

		randomDiceValue.text = diceValueString;
		randomDiceValue.color = Color.black;

		this.diceValue = diceValue;
		//Debug.Log ("I set " + this.name + "'s dice vallue to " + this.diceValue);
	}
}
