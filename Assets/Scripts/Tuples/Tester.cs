using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SymmetricTupleInt one = new SymmetricTupleInt (5, 7);
		SymmetricTupleInt two = new SymmetricTupleInt (5, 7);
		SymmetricTupleInt three = new SymmetricTupleInt (7, 5);

		SymmetricTupleInt four = new SymmetricTupleInt (7, 7);
		SymmetricTupleInt five = new SymmetricTupleInt (5, 5);

		print ("One == Two: " + (one == two).ToString());
		print ("Two == Three: " + (two.Equals(three)).ToString());
		print ("One == Three: " + (one == three).ToString());

		print ("Two == Four: " + (two == four).ToString());
		print ("Two == Five: " + (two == five).ToString());

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
