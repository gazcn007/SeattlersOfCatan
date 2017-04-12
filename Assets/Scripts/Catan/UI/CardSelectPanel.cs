﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectPanel : MonoBehaviour {

	public void drawBlue(){
		EventTransferManager.instance.DrawCard ((int)ProgressCardColor.Blue, PhotonNetwork.player.ID - 1);
		this.gameObject.SetActive (false);
	}
	public void drawGreen(){
		EventTransferManager.instance.DrawCard ((int)ProgressCardColor.Green, PhotonNetwork.player.ID - 1);
		this.gameObject.SetActive (false);
	}
	public void drawYellow(){
		EventTransferManager.instance.DrawCard ((int)ProgressCardColor.Yellow, PhotonNetwork.player.ID - 1);
		this.gameObject.SetActive (false);
	}
}
