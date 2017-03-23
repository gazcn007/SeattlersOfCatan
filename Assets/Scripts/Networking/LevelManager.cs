using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : Photon.MonoBehaviour {

	public static LevelManager instance = null;

	public GameObject boardPrefab;
	public GameObject settingsPrefab;
	public GameObject catanPlayerPrefab;

	//public Dictionary<int, Player> players;
	public List<Player> players;

	private GameBoard board;
	private TileTypeSettings hexSettings;

	public int currentConnections = 0;

	void Awake() {
		// if the static class instance is null (singleton pattern)
		if (instance == null)
			instance = this;

		// if instance already exists and it's not this:
		else if (instance != this)

			// then destroy this. Enforces singletonPattern
			Destroy(gameObject);

		// Sets this to not be destroyed on scene reload
		DontDestroyOnLoad(gameObject);

		Debug.Log("LevelManager.cs: Manager initialized");

	}	

	public void LoadLevelScene(bool online){

		Debug.Log("SceneManager.cs: Loading scene...");

		if (online) {
			PhotonNetwork.LoadLevel((int)Scenes.MainScene);
		} else {
			SceneManager.LoadScene ((int)Scenes.MainScene);
		}

		GameObject boardGO;
		GameObject settingsGO;

		/*if (online) {
			boardGO = (GameObject)PhotonNetwork.Instantiate ("GameBoard", Vector3.zero, Quaternion.identity, 0) as GameObject;
			board = boardGO.GetComponent<GameBoard> ();

			settingsGO = (GameObject)PhotonNetwork.Instantiate ("TileSettings", Vector3.zero, Quaternion.identity, 0) as GameObject;
			hexSettings = settingsGO.GetComponent<TileTypeSettings> ();
		} else {
			boardGO = (GameObject)Instantiate (boardPrefab) as GameObject;
			board = boardGO.GetComponent<GameBoard> ();

			settingsGO = Instantiate (settingsPrefab) as GameObject;
			hexSettings = settingsGO.GetComponent<TileTypeSettings> ();
		}

		board.online = online;
		boardGO.tag = "Server";*/
		//CreateBoard ();
		//HideBoard ();
		//BoardDecorator decorator = new BoardDecorator (board, hexSettings);
		addPlayers();
		//initiate progress cards

		GameObject ETManagerGO = (GameObject)PhotonNetwork.Instantiate("EventTransferManager", Vector3.zero, Quaternion.identity, 0);
		EventTransferManager ETManager = ETManagerGO.GetComponent<EventTransferManager>();
		//ETManager.board = board;
		//ETManager.settings = hexSettings;

		//DontDestroyOnLoad (boardGO);
		DontDestroyOnLoad (ETManagerGO);

		ETManager.OnReadyToPlay ();
		ETManager.GenerateClientCards ();

		//DontDestroyOnLoad (settingsGO);

		//GetComponent<PhotonView> ().RPC ("SyncTiles", PhotonTargets.All, new object[] {});

		/*GameObject elementManagerGO = Instantiate (elementManagerPrefab) as GameObject;
		elementManagerGO.transform.parent = this.gameObject.transform;

		GameObject player = Instantiate(playerPrefab, new Vector3(5f, 2.5f, 2.5f), Quaternion.identity) as GameObject;
		this.player = player.GetComponent<Player>();

		GameObject cam = Instantiate(Resources.Load("Camera")) as GameObject;
		cam.GetComponent<CameraControls>().setCharacter(player);
		DontDestroyOnLoad(cam);

		Camera.main.enabled = false;
		if (TimeState != TimeStates.Offline){
			GameObject ETManagerGO = (GameObject)PhotonNetwork.Instantiate("EventTransferManager", Vector3.zero, Quaternion.identity, 0);
			EventTransferManager ETManager = ETManagerGO.GetComponent<EventTransferManager>();
			ETManager.player = player.GetComponent<Player>();
			player.GetComponent<Player>().ETmanager = ETManager;
			DontDestroyOnLoad(ETManagerGO);
		}

		DontDestroyOnLoad(player);*/

	}

	void CreateBoard() {
		board.GenerateTiles (board.transform);
		board.GenerateIntersections (board.transform);
		board.GenerateEdges (board.transform);
	}

	void HideBoard() {
		foreach (GameTile tile in board.GameTiles.Values) {
			Debug.Log ("Dice value is: " + tile.diceValue);
			tile.gameObject.SetActive (false);
		}
		foreach (Intersection intersection in board.Intersections.Values) {
			intersection.gameObject.SetActive (false);
		}
		foreach (Edge edge in board.Edges.Values) {
			edge.gameObject.SetActive (false);
		}
	}

	void addPlayers() {
		//players = new Dictionary<int, Player> (4);
		players = new List<Player>(4);
		GameObject player1Object = (GameObject) Instantiate (catanPlayerPrefab);
		Player player1 = player1Object.GetComponent<Player> ();
		player1.playerColor = Color.blue;
		player1.playerName = "Nehir";
		player1.playerNumber = 1;
		player1.avatar = Resources.Load<Sprite> ("avatars/charlie-chaplin");
		player1.gameObject.tag = "Player";
		//playerHUD.setPlayer (player1);

		GameObject player2Object = (GameObject) Instantiate (catanPlayerPrefab);
		Player player2 = player2Object.GetComponent<Player> ();
		player2.playerColor = Color.red;
		player2.playerName = "Angela";
		player2.playerNumber = 2;
		player2.avatar = Resources.Load<Sprite> ("avatars/steve-jobs");
		player2.gameObject.tag = "Player";
		//opponent1HUD.setPlayer(player2);

		GameObject player3Object = (GameObject) Instantiate (catanPlayerPrefab);
		Player player3 = player3Object.GetComponent<Player> ();
		player3.playerColor = Color.yellow;
		player3.playerName = "Milosz";
		player3.playerNumber = 3;
		player3.avatar = Resources.Load<Sprite> ("avatars/barack-obama");
		player3.gameObject.tag = "Player";
		//opponent2HUD.setPlayer(player3);

		GameObject player4Object = (GameObject) Instantiate (catanPlayerPrefab);
		Player player4 = player4Object.GetComponent<Player> ();
		player4.playerColor = Color.green;
		player4.playerName = "Carl";
		player4.playerNumber = 4;
		player4.avatar = Resources.Load<Sprite> ("avatars/che-guevara");
		player4.gameObject.tag = "Player";
		//opponent3HUD.setPlayer(player4);

		//players.Add(player1.playerNumber, player1);
		//players.Add (player2.playerNumber, player2);
		//players.Add (player3.playerNumber, player3);
		//players.Add (player4.playerNumber, player4);

		players.Add(player1);
		players.Add (player2);
		players.Add (player3);
		players.Add (player4);

		DontDestroyOnLoad (player1);
		DontDestroyOnLoad (player2);
		DontDestroyOnLoad (player3);
		DontDestroyOnLoad (player4);

	}

	public int GetNewPlayerIndex() {
		return currentConnections++;
	}
}
