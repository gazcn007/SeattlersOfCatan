using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetropolisOwners {

	public Player[] metropolisOwners;

	public MetropolisOwners() {
		metropolisOwners = new Player[PhotonNetwork.playerList.Length];

		for (int i = 0; i < metropolisOwners.Length; i++) {
			metropolisOwners[i] = null;
		}
	}
}
