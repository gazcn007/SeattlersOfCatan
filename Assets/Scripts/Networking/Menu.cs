using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public void LoadOffline() {
		Debug.Log("Menu.cs: <offline button clicked>");

		// Load the main scene
		gm.InitLevel(false);	
	}
}