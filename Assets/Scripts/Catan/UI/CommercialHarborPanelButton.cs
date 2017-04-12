using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommercialHarborPanelButton : MonoBehaviour {
	public int playernum;
	public int id;
	public CommercialHarborPanel instance;


	public void SelectionEvent(){
		
		switch (playernum) {
		case 1:
			instance.selection1 = id;
			instance.selection1flag = true;
			instance.setGlow (this,playernum);
			break;
		case 2:
			instance.selection2 = id;
			instance.selection2flag = true;
			instance.setGlow (this,playernum);
			break;
		case 3:
			instance.selection3 = id;
			instance.selection3flag = true;
			instance.setGlow (this,playernum);
			break;
		}
	}
}
