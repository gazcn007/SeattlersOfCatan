using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	public Text nametxt;
	public Text papertxt;
	public Text lumbertxt;
	public Text bricktxt;
	public Text wooltxt;
	public Text graintxt;
	public Text oretxt;
	public Text honeytxt;
	public Text goldtxt;

	// Use this for initialization
	void Start () {
		nametxt= GetComponentsInChildren<Text> ()[0];
		papertxt= GetComponentsInChildren<Text> ()[1];
		lumbertxt= GetComponentsInChildren<Text> ()[2];
		bricktxt= GetComponentsInChildren<Text> ()[3];
		wooltxt= GetComponentsInChildren<Text> ()[4];
		graintxt= GetComponentsInChildren<Text> ()[5];
		oretxt= GetComponentsInChildren<Text> ()[6];
		honeytxt= GetComponentsInChildren<Text> ()[7];
		goldtxt= GetComponentsInChildren<Text> ()[8];
		nametxt.text = "Holder";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void setPlayer(string p){

		//name
		nametxt.text = p;

		//resources
		//Debug.Log(player.resources.resourceTuple[0]);
		//bricktxt.text= p.resources.resourceTuple[0].ToString();
		//graintxt.text= p.resources.resourceTuple[1].ToString();
		//lumbertxt.text= p.resources.resourceTuple[2].ToString();
		//oretxt.text= p.resources.resourceTuple[3].ToString();
		//wooltxt.text= p.resources.resourceTuple[4].ToString();

		//commodities


	}
}
