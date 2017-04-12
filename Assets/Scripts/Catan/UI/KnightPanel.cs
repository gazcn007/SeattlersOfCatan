using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightPanel : MonoBehaviour {
	public Button Knightbutton;
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

	public void MoveKnight() {
		int buttonId = 13;
		Debug.Log ("Move knight event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientMoveKnightForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					BoardManager.instance.highlightKnightsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Knight)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					BoardManager.instance.highlightAllIntersections (false);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void BuildKnight() {
		int buttonId = 10;
		Debug.Log ("Knight build event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientBuildKnightForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					BoardManager.instance.highlightAllIntersections (false);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void ActivateKnight() {
		int buttonId = 11;
		Debug.Log ("Activate knight event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientActivateKnightForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					BoardManager.instance.highlightKnightsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Knight)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void DisplaceKnight() {
		int buttonId = 14;
		Debug.Log ("Displace opponent's knight event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientDisplaceKnightForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
						BoardManager.instance.highlightKnightsWithColor (CatanManager.instance.players [i].getOwnedUnitsOfType (typeof(Knight)), true, CatanManager.instance.players [i].playerColor);
					}
					BoardManager.instance.highlightAllIntersections (false);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void UpgradeKnight() {
		int buttonId = 12;
		Debug.Log ("Upgrade knight event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientPromoteKnightForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					BoardManager.instance.highlightKnightsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Knight)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

	public void ChaseRobber() {
		int buttonId = 15;
		Debug.Log ("Chase away robber event()");
		if (CatanManager.instance.currentPlayerTurn == PhotonNetwork.player.ID - 1 && !EventTransferManager.instance.setupPhase) {
			if (!EventTransferManager.instance.waitingForPlayer && EventTransferManager.instance.diceRolledThisTurn) {
				EventTransferManager.instance.currentActiveButton = buttonId;

				StartCoroutine (EventTransferManager.instance.ClientChaseRobberForAll(PhotonNetwork.player.ID - 1));
			} else {
				if (EventTransferManager.instance.currentActiveButton == buttonId) {
					StopAllCoroutines ();
					CatanManager.instance.operationCancelled = true;

					BoardManager.instance.highlightKnightsWithColor (CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].getOwnedUnitsOfType (typeof(Knight)), true, CatanManager.instance.players [CatanManager.instance.currentPlayerTurn].playerColor);
					EventTransferManager.instance.OnOperationFailure ();
				}
			}
		}
	}

}