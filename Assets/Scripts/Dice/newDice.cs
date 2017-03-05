using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newDice : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*	public GameObject gameObject;
		private int tempdie;				// associated gameObject
		public Die die;							// associated Die (value calculation) script
		public string name = "";				// dieType
		public string mat;						// die material (asString)
		public Vector3 spawnPoint;			// die spawnPoiunt
		public Vector3 force;					// die initial force impuls

		// rolling attribute specifies if this die is still rolling
		public bool rolling
		{
			get
			{
				return die.rolling;
			}
		}

		public int value
		{
			get
			{
				int val = die.value;
				return val;

			}
		}

		// constructor
		public RollingDie(GameObject gameObject, string name, string mat, Vector3 spawnPoint)
		{
			Vector3 rollTarget = Vector3.zero + new Vector3(2, 1, 3);

			this.gameObject = gameObject;
			this.name = name;
			this.mat = mat;
			this.spawnPoint = spawnPoint;
			this.force = Vector3.Lerp(gameObject.transform.position, rollTarget, 1).normalized * (2);;
			//this.value = Dice.Value;
			// get Die script of current gameObject
			die = (Die)gameObject.GetComponent(typeof(Die));
		}

		// ReRoll this specific die
		public void ReRoll()
		{
			if (name != "")
			{
				GameObject.Destroy(gameObject);
				tempdie = Dice.Roll (name, mat, spawnPoint, force);
			}
		}
*/
}
