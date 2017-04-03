using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour {
	public Button buildbutton;
	public Button[] buttonsOnPanel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//used for ui movement element
	public void OnMouseEnter(){
		this.gameObject.SetActive (true);
	}
	public void OnMouseExit(){
		for (int i = 0; i < buttonsOnPanel.Length; i++) {
			buttonsOnPanel [i].GetComponent<GenericButton> ().onMouseExit ();
		}
		this.gameObject.SetActive (false);
	}
}
