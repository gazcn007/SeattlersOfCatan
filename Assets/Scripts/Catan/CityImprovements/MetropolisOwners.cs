using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetropolisOwners : MonoBehaviour {

	public Player[] metropolisOwners;

	// Use this for initialization
	void Start () {
		metropolisOwners = new Player[PhotonNetwork.playerList.Length];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
