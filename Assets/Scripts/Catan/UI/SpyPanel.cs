using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpyPanel : MonoBehaviour {

	public List<SpyPanelButton> buttons;
	public Image glow;
	public bool selectionMade;
	public ProgressCardType selection;
	public Text paneltitle;

	public void openPanel(List<ProgressCardType> cards,string title){
		this.gameObject.SetActive (true);
		//set the selection to nothing
		paneltitle.text=title;
		selection=ProgressCardType.None;
		//hide the glow
		glow.gameObject.SetActive (false);
		//for test = 4
		for (int i = 0; i < 5; i++) {
			if (i < cards.Count) {
				buttons [i].gameObject.SetActive (true);
				buttons [i].card = cards [i];
				buttons[i].display.sprite=Resources.Load<Sprite> ("ProgressCards/"+cards[i].ToString());
				buttons [i].instance = this;
			} else {
				buttons [i].gameObject.SetActive (false);
			}
		}

	}

	public void confirmSelection(){
		if (selection != ProgressCardType.None) {
			selectionMade = true;
		}

	}
	public void setGlow(SpyPanelButton button){
		glow.gameObject.SetActive (true);
		glow.gameObject.transform.position = button.gameObject.transform.position;

	}
	public IEnumerator waitUntilButtonDown() {
		yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

		while (!selectionMade) {
			Debug.Log ("waiting key down");
			yield return new WaitForEndOfFrame ();
		}
		Debug.Log ("Keydown recieved");
	}
}
