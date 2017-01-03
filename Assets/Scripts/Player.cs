using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public string playerName;
	public Color playerColor;

	public int playerNumber;
	public int goldCoins;
	public int victoryPoints;

	// private Dictionary<System.Type, List<Unit>> ownedUnits -> ownedUnits[typeof(settlement)].add(settlement) -> O(1) access to list of specific type of units
	private List<Unit> ownedUnits;
	public ResourceTuple resources;
	public CommodityTuple commodities;

	public Edge lastEdgeSelection;
	public Intersection lastIntersectionSelection;

	// Use this for initialization
	void Start () {
		//playerColor = Color.black;
		ownedUnits = new List<Unit> ();
	}
	
	// Update is called once per frame
	void Update () {
		victoryPoints = getTotalVictoryPoints ();

		if (victoryPoints >= GameManager.victoryPointsToWinGame) {
			winGame ();
		}
	}

	public void receiveResources(ResourceTuple resourceToAdd) {
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resourceToAdd.resourceTuple [resourceKeys [i]] >= 0) {
				resources.resourceTuple [resourceKeys [i]] += resourceToAdd.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public void spendResources(ResourceTuple resourceToSpend) {
		print ("In spendresources");
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			print ("Loop index: " + i.ToString ());
			if (resources.resourceTuple [resourceKeys [i]] >= resourceToSpend.resourceTuple [resourceKeys [i]]) {
				print ("Subtracted " + resourceToSpend.resourceTuple [resourceKeys [i]].ToString () + " " + resourceKeys [i].ToString () + " from " + this.playerName);
				resources.resourceTuple [resourceKeys [i]] -= resourceToSpend.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public bool hasAvailableResources(ResourceTuple resourcesNeeded) {
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resources.resourceTuple [resourceKeys [i]] < resourcesNeeded.resourceTuple [resourceKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveResource(ResourceType resource, int amount) {
		if (amount < 0) {
			//Error
		} else {
			resources.resourceTuple [resource] += amount;
		}
	}

	public ResourceTuple getCurrentResources() {
		return resources;
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

	public void addOwnedUnit(Unit unit) {
		ownedUnits.Add (unit);
	}

	public int getNumUnits() {
		return ownedUnits.Count;
	}

	public IEnumerator makeEdgeSelection(List<Edge> possibleEdges, TradeUnit unitToBuild) {
		bool selectionMade = false;
		//List<Edge> possibleEdges = GameManager.getValidEdgesForPlayer (this);
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
						/*selectedEdge.occupier = unitToBuild;
						unitToBuild.locationEdge = selectedEdge;
						this.ownedUnits.Add (unitToBuild);
						unitToBuild.owner = this;
						selectionMade = true;

						unitToBuild.transform.position = selectedEdge.transform.position;
						unitToBuild.transform.rotation = selectedEdge.transform.rotation;
						unitToBuild.transform.localScale = selectedEdge.transform.localScale;
						unitToBuild.transform.parent = selectedEdge.transform;*/
						selectionMade = true;
						lastEdgeSelection = selectedEdge;

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

					if (selectedIntersection != null && selectedIntersection.occupier == null && possibleIntersections.Contains(selectedIntersection)) {//selectedIntersection.isSettleable()) {
						/*selectedIntersection.occupier = unitToBuild;
						unitToBuild.locationIntersection = selectedIntersection;
						this.ownedUnits.Add (unitToBuild);
						unitToBuild.owner = this;
						selectionMade = true;

						unitToBuild.transform.position = selectedIntersection.transform.position;
						unitToBuild.transform.parent = selectedIntersection.transform;
						unitToBuild.transform.localScale = unitToBuild.transform.localScale * GameBoard.hexRadius;*/
						selectionMade = true;
						lastIntersectionSelection = selectedIntersection;

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
