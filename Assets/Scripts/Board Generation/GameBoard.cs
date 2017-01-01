using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

	public static GameBoard inst;

	public GameObject tilePrefab;
	public GameObject intersectionPrefab;
	public GameObject edgePrefab;

	//Map settings
	public MapShape mapShape = MapShape.Rectangle;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Flat;
	public static float hexRadius = 1;

	//Internal variables
	//ID variables
	private int tileID;
	private int intersectionID;
	private int edgeID;

	//Dictionaries for each component
	private Dictionary<string, GameTile> tilesDictionary = new Dictionary<string, GameTile>();
	private static Dictionary<int, GameTile> tilesByIdDictionary = new Dictionary<int, GameTile>();

	private Dictionary<string, Intersection> intersectionsDictionary = new Dictionary<string, Intersection> ();
	private static Dictionary<int, Intersection> intersectionsByIdDictionary = new Dictionary<int, Intersection>();

	private Dictionary<TupleInt, Edge> edgesDictionary = new Dictionary<TupleInt, Edge> ();
	private static Dictionary<int, Edge> edgesByIdDictionary = new Dictionary<int, Edge> ();

	private CubeIndex[] directions = 
		new CubeIndex[] {
		new CubeIndex(1, -1, 0), 
		new CubeIndex(1, 0, -1), 
		new CubeIndex(0, 1, -1), 
		new CubeIndex(-1, 1, 0), 
		new CubeIndex(-1, 0, 1), 
		new CubeIndex(0, -1, 1)
	}; 

	private static List<GameTile> tilesList = new List<GameTile>();
	private static List<Intersection> intersectionsList = new List<Intersection>();
	private static List<Edge> edgesList = new List<Edge>();

	#region Getters and Setters
	public Dictionary<string, GameTile> GameTiles {
		get {return tilesDictionary;}
	}

	public Dictionary<string, Intersection> Intersections {
		get {return intersectionsDictionary;}
	}
	#endregion

	#region Public Methods
	public void GenerateTiles() {
		//Generating a new tilesDictionary, clear any remants and initialise values
		ClearGrid();

		//Generate the tilesDictionary shape
		switch(mapShape) {
		case MapShape.Hexagon:
			GenHexShape();
			break;

		case MapShape.Rectangle:
			GenRectShape();
			break;

		case MapShape.Parrallelogram:
			GenParrallShape();
			break;

		case MapShape.Triangle:
			GenTriShape();
			break;

		default:
			break;
		}

		storeTileInformation ();
	}

	public void GenerateIntersections() {
		GameObject parentObject = GameObject.FindGameObjectWithTag ("Intersections");
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
					GameObject instantiation = (GameObject) Instantiate(intersectionPrefab, cornerPosition, Quaternion.identity, this.transform);
					instantiation.name = "Intersection " + intersectionID;
					currentIntersection = instantiation.GetComponent<Intersection> ();
					currentIntersection.transform.parent = parentObject.transform;
					currentIntersection.setID (intersectionID++);

					intersectionsDictionary.Add(cornerPosition.ToString(), currentIntersection);
					intersectionsByIdDictionary.Add (currentIntersection.getID (), currentIntersection);
					intersectionsList.Add (currentIntersection);
				}

				currentIntersection.addTile (gameTile);
				gameTile.addIntersection (currentIntersection);
			}
		}
	}

	public void GenerateEdges() {
		GameObject parentObject = GameObject.FindGameObjectWithTag ("Edges");
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

					GameObject instantiation = (GameObject)Instantiate (edgePrefab, midPoint, Quaternion.Euler (new Vector3 (0.0f, angle, 0.0f)), this.transform);
					instantiation.transform.localScale = new Vector3 (instantiation.transform.localScale.x * hexRadius, instantiation.transform.localScale.y, instantiation.transform.localScale.z);
					instantiation.name = "Edge " + edgeID;

					currentEdge = instantiation.GetComponent<Edge> ();
					currentEdge.transform.parent = parentObject.transform;
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
	}

	public static List<GameTile> getTiles() {
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
			DestroyImmediate (tile.Value.gameObject, false);
		}
		foreach (var tile in tilesByIdDictionary) {
			if(tile.Value != null)
				DestroyImmediate (tile.Value.gameObject, false);
		}
		
		foreach (var intersection in intersectionsDictionary) {
			DestroyImmediate (intersection.Value.gameObject, false);
		}
		foreach (var intersection in intersectionsByIdDictionary) {
			if(intersection.Value != null)
				DestroyImmediate (intersection.Value.gameObject, false);
		}

		foreach (var edge in edgesDictionary) {
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

		tilesList.Clear ();

		tileID = 0;
		intersectionID = 0;
		edgeID = 0;
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
		if(!inst)
			inst = this;

		GenerateTiles();
	}

	private void GenHexShape() {
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

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private void GenRectShape() {
		GameTile tile;
		Vector3 pos = Vector3.zero;

		switch(hexOrientation){
		case HexOrientation.Flat:
			for(int q = 0; q < mapWidth; q++){
				int qOff = q>>1;
				for (int r = -qOff; r < mapHeight - qOff; r++){
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);

					tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"));
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

					tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"));
					tile.index = new CubeIndex(q,r,-q-r);
					tile.id = tileID++;

					tilesDictionary.Add(tile.index.ToString(), tile);
					tilesByIdDictionary.Add (tile.id, tile);
				}
			}
			break;
		}
	}

	private void GenParrallShape() {
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

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private void GenTriShape() {
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

				tile = CreateHexGO(pos,("GameTile[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				tile.id = tileID++;

				tilesDictionary.Add(tile.index.ToString(), tile);
				tilesByIdDictionary.Add (tile.id, tile);
			}
		}
	}

	private GameTile CreateHexGO(Vector3 postion, string name) {
		GameTile tile;
		GameObject instantiation = (GameObject) Instantiate(tilePrefab);
		GameObject parentObject = GameObject.FindGameObjectWithTag ("GameTiles");

		instantiation.name = name;
		tile = instantiation.GetComponent<GameTile> ();

		tile.transform.position = postion;

		if (hexOrientation == HexOrientation.Flat) {
			tile.transform.Rotate (new Vector3 (0.0f, -30.0f, 0.0f));
			tile.transform.FindChild ("Dice Value").transform.Rotate(new Vector3 (0.0f, 30.0f, 0.0f));
		}

		tile.transform.localScale = tile.transform.localScale * hexRadius;
		tile.transform.parent = parentObject.transform;

		return tile;
	}

	private List<GameTile> getCommonTilesOfTwoIntersections(Intersection i1, Intersection i2) {
		List<GameTile> commonTiles = new List<GameTile> ();

		foreach (var tile in i1.adjacentTiles) {
			if(i2.adjacentTiles.Contains(tile)) {
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
