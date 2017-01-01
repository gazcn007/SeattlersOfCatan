using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public string playerName;
	public Color playerColor;

	public int playerNumber;
	public int goldCoins;
	public int victoryPoints;

	private List<Unit> ownedUnits;
	private GameAsset[] resources;

	// Use this for initialization
	void Start () {
		//playerColor = Color.black;
		ownedUnits = new List<Unit> ();
		resources = new GameAsset[5];
	}
	
	// Update is called once per frame
	void Update () {
		victoryPoints = getTotalVictoryPoints ();

		if (victoryPoints >= GameManager.victoryPointsToWinGame) {
			winGame ();
		}
	}

	public void playTurn() {

	}

	private void winGame() {
		// TODO
	}

	private int getTotalVictoryPoints() {
		int totalVP = 0;
		for(int i = 0; i < ownedUnits.Count; i++) {
			totalVP += ownedUnits [i].victoryPointsWorth;
		}
		return totalVP;
	}

	public List<Unit> getOwnedUnits() {
		List<Unit> clone = new List<Unit> (ownedUnits);
		return clone;
	}

	public int getNumUnits() {
		return ownedUnits.Count;
	}

	public IEnumerator makeEdgeSelection(TradeUnit unitToBuild) {
		bool selectionMade = false;
		List<Edge> possibleEdges = GameManager.getValidEdgesForPlayer (this);
		Edge selectedEdge;

		while (!selectionMade) {
			//print ("Waiting for mouse down");
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) 
			{
				//Debug.Log("Hit " + hitInfo.transform.gameObject.name);
				if (hitInfo.transform.gameObject.tag == "Edge")
				{
					//Debug.Log ("It's working!");
					//Debug.Log ("Hit edge: " + hitInfo.transform.gameObject.name.ToString ());

					selectedEdge = hitInfo.transform.gameObject.GetComponent<Edge>();

					if (selectedEdge != null && selectedEdge.occupier == null && possibleEdges.Contains(selectedEdge)) {//(selectedEdge.isLandEdge() || selectedEdge.isShoreEdge())) {
						selectedEdge.occupier = unitToBuild;
						unitToBuild.locationEdge = selectedEdge;
						this.ownedUnits.Add (unitToBuild);
						unitToBuild.owner = this;
						selectionMade = true;

						unitToBuild.transform.position = selectedEdge.transform.position;
						unitToBuild.transform.rotation = selectedEdge.transform.rotation;
						unitToBuild.transform.localScale = selectedEdge.transform.localScale;
						unitToBuild.transform.parent = selectedEdge.transform;

						print (this.playerName + " builds a road on edge #" + selectedEdge.id);
					}
				} else {
					//Debug.Log ("nopz");
				}
			} else {
				//Debug.Log("No hit");
			}
		}
	}

	public IEnumerator makeIntersectionSelection(IntersectionUnit unitToBuild) {
		bool selectionMade = false;
		Intersection selectedIntersection;
		List<Intersection> possibleIntersections = GameManager.getValidIntersectionsForPlayer (this);

		while (!selectionMade) {
			//print ("Waiting for mouse down");
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) 
			{
				//Debug.Log("Hit " + hitInfo.transform.gameObject.name);
				if (hitInfo.transform.gameObject.tag == "Intersection")
				{
					//Debug.Log ("It's working!");
					//Debug.Log ("Hit intersection: " + hitInfo.transform.gameObject.name.ToString ());

					selectedIntersection = hitInfo.transform.gameObject.GetComponent<Intersection>();

					if (selectedIntersection.occupier == null && possibleIntersections.Contains(selectedIntersection)) {//selectedIntersection.isSettleable()) {
						selectedIntersection.occupier = unitToBuild;
						unitToBuild.locationIntersection = selectedIntersection;
						this.ownedUnits.Add (unitToBuild);
						unitToBuild.owner = this;
						selectionMade = true;

						unitToBuild.transform.position = selectedIntersection.transform.position;
						unitToBuild.transform.parent = selectedIntersection.transform;
						unitToBuild.transform.localScale = unitToBuild.transform.localScale * GameBoard.hexRadius;

						print (this.playerName + " builds a settlement on intersection #" + selectedIntersection.id);
					}
				} else {
					//Debug.Log ("nopz");
				}
			} else {
				//Debug.Log("No hit");
			}
		}
	}
}
