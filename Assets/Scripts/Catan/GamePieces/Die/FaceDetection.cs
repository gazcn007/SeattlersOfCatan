using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceDetection : MonoBehaviour {

	public List<Vector3> directions;
	public List<int> faceValues;

	void Awake() {

		// Pre-defined direction and face values
		// This is based on D6's material, @todo number will need to be reassigned if change material
		if (directions.Count == 0) {
			directions.Add(Vector3.up);
			faceValues.Add(5);
			directions.Add(Vector3.down);
			faceValues.Add(2); 
			directions.Add(Vector3.left);
			faceValues.Add(3);
			directions.Add(Vector3.right);
			faceValues.Add(4); 
			directions.Add(Vector3.forward);
			faceValues.Add (1); 
			directions.Add(Vector3.back);
			faceValues.Add(6); 
		}
	}

	public void showNumber() {
		string name = transform.name;
		//prints number that is facing up to the console
		Debug.Log("The "+name+" has value: " + getNumber(Vector3.up, 30f));
	}

	public int getNumber(Vector3 referenceVectorUp, float epsilonDeg) {
		// use reference of object transformation
		Vector3 referenceObjectSpace = transform.InverseTransformDirection(referenceVectorUp);

		// Use delta to find the most possible face that is facing up
		float min = float.MaxValue;
		int mostSimilarDirectionIndex = -1;
		for (int i=0; i < directions.Count; ++i) {
			// compare angles
			float a = Vector3.Angle(referenceObjectSpace, directions[i]);
			if (a <= epsilonDeg && a < min) {
				min = a;
				mostSimilarDirectionIndex = i;
			}
		}

		// return -1 for corner cases
		return (mostSimilarDirectionIndex >= 0) ? faceValues[mostSimilarDirectionIndex] : -1; 
	}
}
