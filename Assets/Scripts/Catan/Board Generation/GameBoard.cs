using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

	public static GameBoard instance;

	public bool online;

	public GameObject tilePrefab;
	public GameObject intersectionPrefab;
	public GameObject edgePrefab;

	public GameObject robberPrefab;
	public GameObject piratePrefab;
	public GameObject merchantPrefab;

	public GameObject[] harborPrefabs;

	public GameObject fishGroundTilePrefab;

	public Robber robber;
	public Pirate pirate;
	public Merchant merchant;

	//Map settings
	public MapShape mapShape = MapShape.Rectangle;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Flat;
	public float hexRadius = 1;

	//Internal variables
	//ID variables
	private int tileID;
	private int intersectionID;
	private int edgeID;
	private int harborID;
	private int fishTileID;

	//Dictionaries for each component
	private Dictionary<string, GameTile> tilesDictionary = new Dictionary<string, GameTile>();
	private static Dictionary<int, GameTile> tilesByIdDictionary = new Dictionary<int, GameTile>();

	private Dictionary<string, Intersection> intersectionsDictionary = new Dictionary<string, Intersection> ();
	private static Dictionary<int, Intersection> intersectionsByIdDictionary = new Dictionary<int, Intersection>();

	private Dictionary<TupleInt, Edge> edgesDictionary = new Dictionary<TupleInt, Edge> ();
	private static Dictionary<int, Edge> edgesByIdDictionary = new Dictionary<int, Edge> ();

	private static Dictionary<int, Harbor> harborsByIdDictionary = new Dictionary<int, Harbor> ();

	private static Dictionary<int, FishTile> fishTilesByIdDictionary = new Dictionary<int, FishTile> ();

	private CubeIndex[] directions = 
		new CubeIndex[] {
		new CubeIndex(1, -1, 0), 
		new CubeIndex(1, 0, -1), 
		new CubeIndex(0, 1, -1), 
		new CubeIndex(-1, 1, 0), 
		new CubeIndex(-1, 0, 1), 
		new CubeIndex(0, -1, 1)
	}; 

	private List<GameTile> tilesList = new List<GameTile>();
	private static List<Intersection> intersectionsList = new List<Intersection>();
	private static List<Edge> edgesList = new List<Edge>();
	private static List<Harbor> harbors = new List<Harbor> ();

	private List<FishTile> fishTiles = new List<FishTile>();

	#region Getters and Setters
	public Dictionary<int, GameTile> GameTiles {
		get {return tilesByIdDictionary;}
	}

	public Dictionary<int, Intersection> Intersections {
		get {return intersectionsByIdDictionary;}
	}

	public Dictionary<int, Edge> Edges {
		get {return edgesByIdDictionary;}
	}

	public Dictionary<int, Harbor> Harbors {
		get {return harborsByIdDictionary;}
	}

	public Dictionary<int, FishTile> FishTiles {
		get {return fishTilesByIdDictionary;}
	}
	#endregion

	#region Public Methods
	public void GenerateTiles(Transform parent) {
		//Generating a new tilesDictionary, clear any remants and initialise values
		ClearGrid();

		//Generate the tilesDictionary shape
		switch(mapShape) {
		case MapShape.Hexagon:
			GenHexShape(parent);
			break;

		case MapShape.Rectangle:
			GenRectShape(parent);
			break;

		case MapShape.Parrallelogram:
			GenParrallShape(parent);
			break;

		case MapShape.Triangle:
			GenTriShape(parent);
			break;

		default:
			break;
		}

		storeTileInformation ();
	}

	public void GenerateIntersections(Transform parent) {
		//GameObject parentObject = GameObject.FindGameObjectWithTag ("Intersections");
		Vector3 cornerPosition;
		Intersection currentIntersection;
		GameTile gameTile;

		foreach(var tile in tilesDictionary) {
			gameTile = tile.Value;

			for(int vert = 0; vert < 6; vert++) {
				cornerPosition = GameTile.Corner(gameTile.transform.position, hexRadius, vert, hexOrientation);

				if (intersectionsDictionary.ContainsKey (cornerPosition.ToString())) {
					currentIntersection = intersectionsDictionary [cornerPosition.ToString()];
				} else {
					GameObject instantiation;
					if(online) {
						instantiation = (GameObject)PhotonNetwork.Instantiate("Intersection", cornerPosition, Quaternion.identity, 0);
					}
					else{
						instantiation = (GameObject)Instantiate(intersectionPrefab, cornerPosition, Quaternion.identity, this.transform);
					}
						
					instantiation.name = "Intersection " + intersectionID;
					currentIntersection = instantiation.GetComponent<Intersection> ();
					currentIntersection.transform.parent = parent.transform;
					currentIntersection.setID (intersectionID++);

					intersectionsDictionary.Add(cornerPosition.ToString(), currentIntersection);
					intersectionsByIdDictionary.Add (currentIntersection.getID (), currentIntersection);
					intersectionsList.Add (currentIntersection);
				}

				currentIntersection.addTile (gameTile);
				gameTile.addIntersection (currentIntersection);
			}
		}

		//storeIntersectionInformation();
	}

	public void GenerateEdges(Transform parent) {
		//GameObject parentObject = GameObject.FindGameObjectWithTag ("Edges");
		Edge currentEdge;
		Intersection i1, i2;

		foreach (var tile in tilesDictionary) {
			List<Intersection> intersectionsOfTile = IntersectionsOfTile (tile.Value);

			for (int i = 1; i <= intersectionsOfTile.Count; i++) {
				i1 = IntersectionOfTileAtCorner (tile.Value, i % intersectionsOfTile.Count);
				i2 = IntersectionOfTileAtCorner (tile.Value, (i-1) % intersectionsOfTile.Count);

				TupleInt tuple = new TupleInt (i1.id, i2.id);
				TupleInt reverseTuple = new TupleInt (i2.id, i1.id);

				if (!edgesDictionary.ContainsKey (tuple) && !edgesDictionary.ContainsKey (reverseTuple)) {
					Vector3 midPoint = (i1.transform.position + i2.transform.position) / 2;
					float angle = Intersection.AngleOfCorner ((i - 1) % intersectionsOfTile.Count, hexOrientation);

					GameObject instantiation;
					if (online) {
						instantiation = (GameObject)PhotonNetwork.Instantiate ("Edge", midPoint, Quaternion.Euler (new Vector3 (0.0f, angle, 0.0f)), 0);
					}
					else{
						instantiation = (GameObject)Instantiate (edgePrefab, midPoint, Quaternion.Euler (new Vector3 (0.0f, angle, 0.0f)), this.transform);
					}

					instantiation.transform.localScale = new Vector3 (instantiation.transform.localScale.x * hexRadius, instantiation.transform.localScale.y, instantiation.transform.localScale.z);
					instantiation.name = "Edge " + edgeID;

					currentEdge = instantiation.GetComponent<Edge> ();
					currentEdge.transform.parent = parent.transform;
					currentEdge.setID (edgeID++);

					edgesDictionary.Add (tuple, currentEdge);
					edgesByIdDictionary.Add (currentEdge.getID (), currentEdge);
					edgesList.Add (currentEdge);

					List<GameTile> neighbors = getCommonTilesOfTwoIntersections (i1, i2);
					foreach (GameTile gameTile in neighbors) {
						currentEdge.addTile (gameTile);
						gameTile.addEdge (currentEdge);
					}

					currentEdge.addIntersection (i1);
					currentEdge.addIntersection (i2);
					i1.addLinkedEdge (currentEdge);
					i2.addLinkedEdge (currentEdge);

					i1.addNeighborIntersection (i2);
					i2.addNeighborIntersection (i1);
				}
			}
		}

		//storeEdgeInformation ();
	}

	public void GenerateHarbors(Transform parent) {
		List<Tuple<Intersection, Intersection>> harborIntersectionPairsList = new List<Tuple<Intersection, Intersection>> ();
		List<Intersection> shoreIntersections = new List<Intersection>();

		foreach (Intersection intersection in Intersections.Values) {
			if (intersection.isShoreIntersection () && intersection.isMainIslandIntersection()) {
				shoreIntersections.Add (intersection);
			}
		}

		int numHarbors = 14;
		int segmentLength = shoreIntersections.Count / numHarbors;
		int randNum = Random.Range (0, 3);

		for (int i = 0; i < shoreIntersections.Count; i++) {
			if (i % segmentLength == 0 && shoreIntersections[i].harbor == null) {
				List<Intersection> neighbors = shoreIntersections [i].getNeighborIntersections();
				Intersection neighbor = null;

				foreach (Intersection neighborIntersection in neighbors) {
					if (neighborIntersection.isShoreIntersection ()) {
						neighbor = neighborIntersection;
					}
				}

				if (harborID == 1 || harborID == 5 || harborID == 6 || harborID == 8 || harborID == 9 || harborID == 12) {
					harborID++;
					continue;
				}
 
				int randomNum = Random.Range (0, harborPrefabs.Length);
				int harborIndex = harborID % harborPrefabs.Length;

				if (harborID == 10) {
					harborIndex = 5;
				}
				GameObject harborGO = Instantiate(harborPrefabs[harborIndex], parent);
				//GameObject harborGO = Instantiate(harborPrefabs[randomNum], parent);
				Harbor harbor = harborGO.GetComponent<Harbor> ();
				harbor.id = harborID++;

				shoreIntersections [i].harbor = harbor;
				neighbor.harbor = harbor;

				harborGO.transform.position = shoreIntersections [i].getCommonTileWith (neighbor, TileType.Ocean).transform.position
				+ 0.01f * Vector3.up;
				harborGO.transform.localScale *= 1.25f;
				harborGO.name = "Harbor " + harbor.id;

				harbor.locations.Add (shoreIntersections [i]);
				harbor.locations.Add (neighbor);

				shoreIntersections [i].getCommonTileWith (neighbor, TileType.Ocean).hasHarbor = true;

				SpriteRenderer[] arrows = harbor.GetComponentsInChildren<SpriteRenderer> ();

				/*arrows [1].transform.LookAt (shoreIntersections [i].transform.position + Vector3.up * arrows[1].transform.position.y);
				arrows[1].transform.Translate(Vector3.up * 0.3f);
				arrows [1].transform.Translate (Vector3.forward * 0.2f, Space.Self);
				arrows[1].transform.Rotate(new Vector3(90f, 2700f, 0.0f));
				arrows [2].transform.LookAt (neighbor.transform.position + Vector3.up * arrows[2].transform.position.y);
				arrows [2].transform.Translate (Vector3.forward * 0.2f, Space.Self);
				arrows[2].transform.Translate(Vector3.up * 0.3f);
				arrows[2].transform.Rotate(new Vector3(90f, 270f, 0.0f));*/

				int cornerNum1 = shoreIntersections [i].getCommonTileWith (neighbor, TileType.Ocean).getCornerNumberOfIntersection (shoreIntersections [i]);
				arrows [1].transform.rotation = Quaternion.Euler(new Vector3 (90.0f, 0.0f, 30.0f + 60.0f * cornerNum1));
				arrows[1].transform.Translate(Vector3.right * (float)(5 * hexRadius / 8), Space.Self);

				int cornerNum2 = shoreIntersections [i].getCommonTileWith (neighbor, TileType.Ocean).getCornerNumberOfIntersection (neighbor);
				arrows [2].transform.rotation = Quaternion.Euler(new Vector3 (90.0f, 0.0f, 30.0f + 60.0f * cornerNum2));
				arrows[2].transform.Translate(Vector3.right * (float)(5 * hexRadius / 8), Space.Self);

				harbors.Add (harbor);
				harborsByIdDictionary.Add (harborID - 1, harbor);
			}
		}
	}

	public void GenerateFishGroundTiles() {
		List<GameTile> allTiles = GameTiles.Values.ToList ();
		List<GameTile> fishTiles = new List<GameTile> ();

		foreach (var tile in allTiles) {
			int shoreIntersectionCount = 0;
			int harborCount = 0;

			List<Intersection> tileIntersections = tile.getIntersections ();
			foreach (var intersection in tileIntersections) {
				if (intersection.isShoreIntersection () && intersection.isMainIslandIntersection()) {
					shoreIntersectionCount++;
				}
				//if (intersection.harbor != null) {
				//	harborCount++;
				//}
			}

			if (tile.tileType == TileType.Ocean && shoreIntersectionCount == 3 && !tile.hasHarbor) {//&& harborCount == 1) {
				fishTiles.Add (tile);
			}
		}

		foreach (var tile in fishTiles) {
			List<Intersection> tileIntersections = tile.getIntersections ();

			List<int> shoreIntersectionCornerNumbers = new List<int> ();
			for (int i = 0; i < tileIntersections.Count; i++) {
				if (tileIntersections [i].isShoreIntersection ()) {
					shoreIntersectionCornerNumbers.Add(tile.getCornerNumberOfIntersection (tileIntersections [i]));
				}
			}

			int corner1 = shoreIntersectionCornerNumbers [0];
			int corner2 = shoreIntersectionCornerNumbers [1];
			int corner3 = shoreIntersectionCornerNumbers [2];

			int middleCorner = -1;

			if (corner1 > corner2) {
				if (corner2 > corner3) {
					middleCorner = corner2;
				} else if (corner1 > corner3) {
					middleCorner = corner3;
				} else {
					middleCorner = corner1;
				}
			} else {
				if (corner1 > corner3) {
					middleCorner = corner1;
				} else if (corner2 > corner3) {
					middleCorner = corner3;
				} else {
					middleCorner = corner2;
				}
			}

			if (shoreIntersectionCornerNumbers.Contains (0) && shoreIntersectionCornerNumbers.Contains (5)) {
				if (shoreIntersectionCornerNumbers.Contains (4)) {
					middleCorner = 5;
				} else {
					middleCorner = 0;
				}
			}

			GameObject fishTileGO = Instantiate (fishGroundTilePrefab);
			fishTileGO.transform.position = tile.transform.position + Vector3.up * 0.015f;
			fishTileGO.transform.rotation = Quaternion.Euler(new Vector3 (90.0f, 0.0f, 30.0f + 60.0f * middleCorner));
			fishTileGO.transform.Translate (Vector3.right * 0.38f, Space.Self);
			Transform diceValue = fishTileGO.transform.FindChild ("Dice Value");
			diceValue.transform.rotation = Quaternion.Euler (new Vector3 ((-1.0f * fishTileGO.transform.rotation.z)-90.0f, 0.0f, 0.0f));
			diceValue.transform.Rotate (new Vector3 (90.0f, 0.0f, 0.0f), Space.Self);
			fishTileGO.transform.parent = tile.transform;

			FishTile fishTile = fishTileGO.GetComponent<FishTile> ();
			fishTile.locationTile = tile;
			fishTile.id = fishTileID++;

			fishTileGO.name = "Fish Tile " + fishTile.id;

			int randDiceValue;
			if (Random.Range (0.0f, 1.0f) < 0.5f) {
				randDiceValue = Random.Range (2, 7);
			} else {
				randDiceValue = Random.Range (8, 13);
			}
			fishTile.setDiceValue(randDiceValue);

			tile.fishTile = fishTile;
			tile.diceValue = fishTile.diceValue;

			fishTilesByIdDictionary.Add (fishTile.id, fishTile);
		}

	}

	#region GamePiece Place and Move Methods

	public void placeRobberOnTile(int tileID) {
		GameTile tile = GameTiles [tileID];

		GameObject robberGO = Instantiate (robberPrefab, tile.transform);
		robberGO.transform.position = tile.transform.position;

		robber = robberGO.GetComponent<Robber> ();
		robber.isActive = true;
		robber.occupyingTile = tile;

		tile.occupier = robber;
	}

	public void placePirateOnTile(int tileID) {
		GameTile tile = GameTiles [tileID];

		GameObject pirateGO = Instantiate (piratePrefab, tile.transform);
		pirateGO.transform.position = tile.transform.position;

		pirate = pirateGO.GetComponent<Pirate> ();
		pirate.isActive = true;
		pirate.occupyingTile = tile;

		tile.occupier = pirate;
	}

	public void placeMerchantOnTile(int tileID) {
		GameTile tile = GameTiles [tileID];

		GameObject merchantGO = Instantiate (merchantPrefab, tile.transform);
		merchantGO.transform.position = tile.transform.position;

		merchant = merchantGO.GetComponent<Merchant> ();
		merchant.isActive = true;
		merchant.occupyingTile = tile;

		tile.occupier = merchant;
	}

	public void MoveRobber(int newTileID) {
		GameTile newTile = GameTiles [newTileID];

		if (newTile.tileType != TileType.Ocean) {
			if (robber == null) {
				placeRobberOnTile(newTileID);
			} else {
				if (robber.occupyingTile.tileType != TileType.Desert) {
					robber.occupyingTile.transform.FindChild ("Dice Value").gameObject.SetActive (true);
				}
				robber.occupyingTile.occupier = null;
				robber.occupyingTile = newTile;
				robber.transform.position = newTile.transform.position;
				newTile.occupier = robber;
				newTile.transform.FindChild ("Dice Value").gameObject.SetActive (false);
			}
		}
	}

	public void MovePirate(int newTileID) {
		GameTile newTile = GameTiles [newTileID];

		if (newTile.tileType == TileType.Ocean) {
			if (pirate == null) {
				placePirateOnTile(newTileID);
			} else {
				pirate.occupyingTile.occupier = null;
				pirate.occupyingTile = newTile;
				pirate.transform.position = newTile.transform.position;
				newTile.occupier = pirate;
			}
		}
	}

	public void MoveMerchant(int newTileID) {
		GameTile newTile = GameTiles [newTileID];

		if (newTile.tileType != TileType.Ocean) {
			if (merchant == null) {
				placeMerchantOnTile (newTileID);
			} else {
				if (merchant.occupyingTile.tileType != TileType.Desert) {
					merchant.occupyingTile.transform.FindChild ("Dice Value").gameObject.SetActive (true);
				}
				merchant.occupyingTile.occupier = null;
				merchant.occupyingTile = newTile;
				merchant.transform.position = newTile.transform.position;
				newTile.occupier = merchant;
				newTile.transform.FindChild ("Dice Value").gameObject.SetActive (false);
			}
		}
	}

	#endregion

	public List<GameTile> getTiles() {
		List<GameTile> clone = new List<GameTile> (tilesList);
		return clone;
	}

	public static List<Intersection> getIntersections() {
		List<Intersection> clone = new List<Intersection> (intersectionsList);
		return clone;
	}

	public static List<Edge> getEdges() {
		List<Edge> clone = new List<Edge> (edgesList);
		return clone;
	}

	public static Intersection getIntersectionWithID(int id) {
		return intersectionsByIdDictionary [id];
	}

	public static Edge getEdgeWithID(int id) {
		return edgesByIdDictionary [id];
	}

	public int getNumTiles() {
		return tilesList.Count;
	}

	public int getNumIntersections() {
		return intersectionsList.Count;
	}

	public int getNumEdges() {
		return edgesList.Count;
	}

	public int getMapWidth() {
		return mapWidth;
	}

	public int getMapHeight() {
		return mapHeight;
	}

	public void ClearGrid() {
		//Debug.Log ("Clearing tilesDictionary...");
		foreach (var tile in tilesDictionary) {
			if(tile.Value != null)
			DestroyImmediate (tile.Value.gameObject, false);
		}
		foreach (var tile in tilesByIdDictionary) {
			if(tile.Value != null)
				DestroyImmediate (tile.Value.gameObject, false);
		}
		
		foreach (var intersection in intersectionsDictionary) {
			if(intersection.Value != null)
			DestroyImmediate (intersection.Value.gameObject, false);
		}
		foreach (var intersection in intersectionsByIdDictionary) {
			if(intersection.Value != null)
				DestroyImmediate (intersection.Value.gameObject, false);
		}

		foreach (var edge in edgesDictionary) {
			if(edge.Value != null)
			DestroyImmediate (edge.Value.gameObject, false);
		}
		foreach (var edge in edgesByIdDictionary) {
			if(edge.Value != null)
				DestroyImmediate (edge.Value.gameObject, false);
		}

		tilesDictionary.Clear();
		tilesByIdDictionary.Clear ();

		intersectionsDictionary.Clear ();
		intersectionsByIdDictionary.Clear ();

		edgesDictionary.Clear ();
		edgesByIdDictionary.Clear ();

		harborsByIdDictionary.Clear ();

		fishTilesByIdDictionary.Clear ();

		tilesList.Clear ();
		fishTiles.Clear ();

		tileID = 0;
		intersectionID = 0;
		edgeID = 0;
		harborID = 0;
		fishTileID = 0;
	}

	public void ClearHarbors() {
		harborID = 0;
		harborsByIdDictionary.Clear ();
	}

	public void ClearFishTiles() {
		fishTileID = 0;
		fishTilesByIdDictionary.Clear ();
	}

	public GameTile GameTileAt(CubeIndex index){
		if(tilesDictionary.ContainsKey(index.ToString()))
			return tilesDictionary[index.ToString()];
		return null;
	}

	public GameTile GameTileAt(int x, int y, int z){
		return GameTileAt(new CubeIndex(x,y,z));
	}

	public GameTile GameTileAt(int x, int z){
		return GameTileAt(new CubeIndex(x,z));
	}

	public int IdOfGameTileAt(CubeIndex index){
		if(tilesDictionary.ContainsKey(index.ToString()))
			return tilesDictionary[index.ToString()].id;
		return -1;
	}

	public int IdOfGameTileAt(int x, int y, int z){
		return GameTileAt(new CubeIndex(x,y,z)).id;
	}

	public int IdOfGameTileAt(int x, int z){
		return GameTileAt(new CubeIndex(x,z)).id;
	}

	public Intersection IntersectionOfTileAtCorner (GameTile tile, int cornerNumber) {
		Vector3 cornerPosition = GameTile.Corner(tile.transform.position, hexRadius, cornerNumber, hexOrientation);
		if (intersectionsDictionary.ContainsKey (cornerPosition.ToString())) {
			return intersectionsDictionary [cornerPosition.ToString()];
		}
		return null;
	}

	public List<Intersection> IntersectionsOfTile(GameTile tile) {
		List<Intersection> intersections = new List<Intersection> ();
		Vector3 cornerPosition;

		for (int vert = 0; vert < 6; vert++) {
			cornerPosition = GameTile.Corner(tile.transform.position, hexRadius, vert, hexOrientation);
			if (intersectionsDictionary.ContainsKey (cornerPosition.ToString())) {
				intersections.Add (intersectionsDictionary [cornerPosition.ToString()]);
			}
		}

		return intersections;
	}

	public bool isNeighbors(GameTile tile1, GameTile tile2) {
		List<GameTile> neighbors = Neighbours (tile1);

		if (neighbors.Contains (tile2)) {
			return true;
		} else {
			return false;
		}
	}

	public List<GameTile> Neighbours(GameTile tile) {
		List<GameTile> ret = new List<GameTile>();
		CubeIndex o;

		for(int i = 0; i < 6; i++) {
			o = tile.index + directions[i];
			if(tilesDictionary.ContainsKey(o.ToString()))
				ret.Add(tilesDictionary[o.ToString()]);
		}
		return ret;
	}

	public List<GameTile> Neighbours(CubeIndex index){
		return Neighbours(GameTileAt(index));
	}

	public List<GameTile> Neighbours(int x, int y, int z){
		return Neighbours(GameTileAt(x,y,z));
	}

	public List<GameTile> Neighbours(int x, int z){
		return Neighbours(GameTileAt(x,z));
	}

	public List<GameTile> GameTilesInRange(GameTile center, int range){
		//Return tiles range steps from center, http://www.redblobgames.com/tilesDictionarys/hexagons/#range
		List<GameTile> ret = new List<GameTile>();
		CubeIndex o;

		for(int dx = -range; dx <= range; dx++){
			for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++){
				o = new CubeIndex(dx, dy, -dx-dy) + center.index;
				if(tilesDictionary.ContainsKey(o.ToString()))
					ret.Add(tilesDictionary[o.ToString()]);
			}
		}
		return ret;
	}

	public List<GameTile> GameTilesInRange(CubeIndex index, int range){
		return GameTilesInRange(GameTileAt(index), range);
	}

	public List<GameTile> GameTilesInRange(int x, int y, int z, int range){
		return GameTilesInRange(GameTileAt(x,y,z), range);
	}

	public List<GameTile> GameTilesInRange(int x, int z, int range){
		return GameTilesInRange(GameTileAt(x,z), range);
	}

	public int Distance(CubeIndex a, CubeIndex b){
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
	}

	public int Distance(GameTile a, GameTile b){
		return Distance(a.index, b.index);
	}
	#endregion

	#region Private Methods
	private void Awake() {
		if(!instance)
			instance = this;

		//GenerateTiles();
	}

	private void GenHexShape(Transform parent) {
		GameTile tile;
		Vector3 pos = Vector3.zero;

		int mapSize = Mathf.Max(mapWidth, mapHeight);

		for (int q = -mapSize; q <= mapSize; q++){
			int r1 = Mathf.Max(-mapSize, -q-mapSize);
			int r2 = Mathf.Min(mapSize, -q+mapSize);
			for(int r = r1; r <= r2; r++){
				switch(hexOrientation){
				case HexOrientation.Flat:
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);
					break;

				case HexOrientation.Pointy:
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
					pos.z = hexRadius * 3.0f/2.0f * r;
					break;
				}

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"), parent);
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private void GenRectShape(Transform parent) {
		GameTile tile;
		Vector3 pos = Vector3.zero;

		switch(hexOrientation){
		case HexOrientation.Flat:
			for(int q = 0; q < mapWidth; q++){
				int qOff = q>>1;
				for (int r = -qOff; r < mapHeight - qOff; r++){
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);

					tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"), parent);
					tile.index = new CubeIndex(q,r,-q-r);
					tile.id = tileID++;

					tilesDictionary.Add(tile.index.ToString(), tile);
					tilesByIdDictionary.Add (tile.id, tile);
				}
			}
			break;

		case HexOrientation.Pointy:
			for(int r = 0; r < mapHeight; r++){
				int rOff = r>>1;
				for (int q = -rOff; q < mapWidth - rOff; q++){
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
					pos.z = hexRadius * 3.0f/2.0f * r;

					tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"), parent);
					tile.index = new CubeIndex(q,r,-q-r);
					tile.id = tileID++;

					tilesDictionary.Add(tile.index.ToString(), tile);
					tilesByIdDictionary.Add (tile.id, tile);
				}
			}
			break;
		}
	}

	private void GenParrallShape(Transform parent) {
		GameTile tile;
		Vector3 pos = Vector3.zero;

		for (int q = 0; q <= mapWidth; q++){
			for(int r = 0; r <= mapHeight; r++){
				switch(hexOrientation){
				case HexOrientation.Flat:
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);
					break;

				case HexOrientation.Pointy:
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
					pos.z = hexRadius * 3.0f/2.0f * r;
					break;
				}

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"), parent);
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private void GenTriShape(Transform parent) {
		GameTile tile;
		Vector3 pos = Vector3.zero;

		int mapSize = Mathf.Max(mapWidth, mapHeight);

		for (int q = 0; q <= mapSize; q++){
			for(int r = 0; r <= mapSize - q; r++){
				switch(hexOrientation){
				case HexOrientation.Flat:
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);
					break;

				case HexOrientation.Pointy:
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
					pos.z = hexRadius * 3.0f/2.0f * r;
					break;
				}

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"), parent);
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private GameTile CreateHexGO(Vector3 postion, string name, Transform parent) {
		GameTile tile;
		GameObject instantiation;
		if (online) {
			instantiation = (GameObject)PhotonNetwork.Instantiate ("GameTile", Vector3.zero, Quaternion.identity, 0);
		} else {
			instantiation = (GameObject) Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, this.transform);
		}

		//GameObject parentObject = GameObject.FindGameObjectWithTag ("GameTiles");

		instantiation.name = name;
		tile = instantiation.GetComponent<GameTile> ();

		tile.transform.position = postion;

		if (hexOrientation == HexOrientation.Flat) {
			tile.transform.Rotate (new Vector3 (0.0f, -30.0f, 0.0f));
			tile.transform.FindChild ("Dice Value").transform.Rotate(new Vector3 (0.0f, 30.0f, 0.0f));
		}

		tile.transform.localScale = tile.transform.localScale * hexRadius;
		tile.transform.parent = parent.transform;

		return tile;
	}

	private List<GameTile> getCommonTilesOfTwoIntersections(Intersection i1, Intersection i2) {
		List<GameTile> commonTiles = new List<GameTile> ();

		foreach (GameTile tile in i1.getAdjacentTiles()) {
			if(i2.getAdjacentTiles().Contains(tile)) {
				commonTiles.Add(tile);
			}
		}
		return commonTiles;
	}

	private void storeTileInformation() {
		foreach (var tile in tilesDictionary) {
			tilesList.Add (tile.Value);
		}
	}

	private void storeIntersectionInformation() {
		foreach (var intersection in intersectionsDictionary) {
			intersectionsList.Add (intersection.Value);
		}
	}

	private void storeEdgeInformation() {
		foreach (var edge in edgesDictionary) {
			edgesList.Add (edge.Value);
		}
	}

	#endregion
}
