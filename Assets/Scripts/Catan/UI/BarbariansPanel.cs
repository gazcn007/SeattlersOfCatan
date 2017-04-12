using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarbariansPanel : MonoBehaviour {
	public List<Image> ships;
	int barbarianDistance;


	// Update is called once per frame
	void Update () {
		//nehir set barbarians lvl here

		barbarianDistance = EventTransferManager.instance.barbariansDistance;
		for (int i = 0; i < ships.Count; i++) {
			ships [i].gameObject.SetActive (false);
		}
		ships [barbarianDistance].gameObject.SetActive (true);
	}
}
