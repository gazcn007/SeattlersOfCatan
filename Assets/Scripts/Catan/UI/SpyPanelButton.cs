using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpyPanelButton : MonoBehaviour {
	public ProgressCardType card;
	public SpyPanel instance;
	public Image display;


	public void SelectionEvent(){
		instance.selection = card;
		instance.setGlow (this);
	}	
}
