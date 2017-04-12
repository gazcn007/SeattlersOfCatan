using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistence;
public class Menu : MonoBehaviour {


	// Main canvas object
	public Canvas MainCanvas;

	// GameManager GetInstanceGameManager
	private GameManager gm = GameManager.instance;

	public void LoadOnline(int numPlayers) {
		Debug.Log("Menu.cs: <online button clicked>");

		// Initialize network connection
		gm.InitNetwork(numPlayers);

		// Load loading scene
		SceneManager.LoadScene((int) Scenes.Loading);	
	}
	public void LoadSavedGame(){
		Debug.Log ("Menu.cs: <load button clicked>");
		string selection=GameObject.FindGameObjectWithTag ("FileSelection").GetComponent<SaveFileSelect> ().getSaveSelection();
		pe_GameState gameState = LoadJson.loadGameState(selection);
		gm.InitNetwork (gameState.players.total);
		gm.LoadGameMode = true;
		gm.pe_playerArray = gameState.players.playerArray;
		gm.pe_gameBoard = gameState.gameBoard;
		gm.pe_units = gameState.units.unitsInPlay;
		gm.pe_progressCardStack = gameState.progressCardStack;
		gm.pe_eventTransferManager = gameState.eventTransferManager;
		SceneManager.LoadScene((int) Scenes.Loading);	
	}

	public void LoadOffline() {
		Debug.Log("Menu.cs: <offline button clicked>");

		// Load the main scene
		gm.InitLevel(false);	
	}
}