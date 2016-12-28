using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

	public static GameBoard inst;

	//Map settings
	public MapShape mapShape = MapShape.Rectangle;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Flat;
	public float hexRadius = 1;
	public Material hexMaterial;

	//Generation Options
	public bool addColliders = true;
	public bool drawOutlines = true;
	public Material lineMaterial;

	//Internal variables
	private Dictionary<string, GameTile> grid = new Dictionary<string, GameTile>();
	private Mesh hexMesh = null;
	private CubeIndex[] directions = 
		new CubeIndex[] {
		new CubeIndex(1, -1, 0), 
		new CubeIndex(1, 0, -1), 
		new CubeIndex(0, 1, -1), 
		new CubeIndex(-1, 1, 0), 
		new CubeIndex(-1, 0, 1), 
		new CubeIndex(0, -1, 1)
	}; 

	#region Getters and Setters
	public Dictionary<string, GameTile> GameTiles {
		get {return grid;}
	}
	#endregion

	#region Public Methods
	public void GenerateGrid() {
		//Generating a new grid, clear any remants and initialise values
		ClearGrid();
		GetMesh();

		//Generate the grid shape
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
	}

	public void ClearGrid() {
		//Debug.Log ("Clearing grid...");
		foreach(var tile in grid)
			DestroyImmediate(tile.Value.gameObject, false);

		grid.Clear();
	}

	public GameTile GameTileAt(CubeIndex index){
		if(grid.ContainsKey(index.ToString()))
			return grid[index.ToString()];
		return null;
	}

	public GameTile GameTileAt(int x, int y, int z){
		return GameTileAt(new CubeIndex(x,y,z));
	}

	public GameTile GameTileAt(int x, int z){
		return GameTileAt(new CubeIndex(x,z));
	}

	public List<GameTile> Neighbours(GameTile tile) {
		List<GameTile> ret = new List<GameTile>();
		CubeIndex o;

		for(int i = 0; i < 6; i++) {
			o = tile.index + directions[i];
			if(grid.ContainsKey(o.ToString()))
				ret.Add(grid[o.ToString()]);
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
		//Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range
		List<GameTile> ret = new List<GameTile>();
		CubeIndex o;

		for(int dx = -range; dx <= range; dx++){
			for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++){
				o = new CubeIndex(dx, dy, -dx-dy) + center.index;
				if(grid.ContainsKey(o.ToString()))
					ret.Add(grid[o.ToString()]);
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

		GenerateGrid();
	}

	private void GetMesh() {
		hexMesh = null;
		GameTile.GetHexMesh(hexRadius, hexOrientation, ref hexMesh);
	}

	private void GenHexShape() {
		//Debug.Log ("Generating hexagonal shaped grid...");

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
				grid.Add(tile.index.ToString(), tile);
			}
		}
	}

	private void GenRectShape() {
		//Debug.Log ("Generating rectangular shaped grid...");

		GameTile tile;
		Vector3 pos = Vector3.zero;

		switch(hexOrientation){
		case HexOrientation.Flat:
			for(int q = 0; q < mapWidth; q++){
				int qOff = q>>1;
				for (int r = -qOff; r < mapHeight - qOff; r++){
					pos.x = hexRadius * 3.0f/2.0f * q;
					pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q/2.0f);

					tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
					tile.index = new CubeIndex(q,r,-q-r);
					grid.Add(tile.index.ToString(), tile);
				}
			}
			break;

		case HexOrientation.Pointy:
			for(int r = 0; r < mapHeight; r++){
				int rOff = r>>1;
				for (int q = -rOff; q < mapWidth - rOff; q++){
					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
					pos.z = hexRadius * 3.0f/2.0f * r;

					tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
					tile.index = new CubeIndex(q,r,-q-r);
					grid.Add(tile.index.ToString(), tile);
				}
			}
			break;
		}
	}

	private void GenParrallShape() {
		//Debug.Log ("Generating parrellelogram shaped grid...");

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

				tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				grid.Add(tile.index.ToString(), tile);
			}
		}
	}

	private void GenTriShape() {
		//Debug.Log ("Generating triangular shaped grid...");

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

				tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				grid.Add(tile.index.ToString(), tile);
			}
		}
	}

	private GameTile CreateHexGO(Vector3 postion, string name) {
		GameObject go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(GameTile));

		if(addColliders)
			go.AddComponent<MeshCollider>();

		if(drawOutlines)
			go.AddComponent<LineRenderer>();

		go.transform.position = postion;
		go.transform.parent = this.transform;

		GameTile tile = go.GetComponent<GameTile>();
		MeshFilter fil = go.GetComponent<MeshFilter>();
		MeshRenderer ren = go.GetComponent<MeshRenderer>();

		fil.sharedMesh = hexMesh;

		ren.material = (hexMaterial)? hexMaterial : UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

		if(addColliders){
			MeshCollider col = go.GetComponent<MeshCollider>();
			col.sharedMesh = hexMesh;
		}

		if(drawOutlines) {
			LineRenderer lines = go.GetComponent<LineRenderer>();
			lines.useLightProbes = false;
			lines.receiveShadows = false;

			lines.SetWidth(0.1f, 0.1f);
			lines.SetColors(Color.black, Color.black);
			lines.material = lineMaterial;

			lines.SetVertexCount(7);

			for(int vert = 0; vert <= 6; vert++)
				lines.SetPosition(vert, GameTile.Corner(tile.transform.position, hexRadius, vert, hexOrientation));
		}

		return tile;
	}
	#endregion
}

public enum TileType {
	Brick = 0,
	Grain,
	Lumber,
	Ore,
	Wool,
	Gold,
	Desert,
	Ocean
}
