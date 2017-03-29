using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceDetection : MonoBehaviour {

	public List<Vector3> directions;
	public List<int> sideValues;

	void Awake() {
		if (directions.Count == 0) {
			// Pre-defined direction and reference values
			directions.Add(Vector3.up);
			sideValues.Add(5); // up
			directions.Add(Vector3.down);
			sideValues.Add(2); // down

			directions.Add(Vector3.left);
			sideValues.Add(3); // left
			directions.Add(Vector3.right);
			sideValues.Add(4); // right

			directions.Add(Vector3.forward);
			sideValues.Add (1); // fw
			directions.Add(Vector3.back);
			sideValues.Add(6); // back
		}

		if (directions.Count != sideValues.Count) {
			Debug.LogError("Not consistent list sizes");
		}
	}

	public void showNumber() {
		string name = transform.name;
		Debug.Log("The "+name+" has value: " + GetNumber(Vector3.up, 30f));
	}

	// Gets the number of the side pointing in the same direction as the reference vector,
	// allowing epsilon degrees error.
	public int GetNumber(Vector3 referenceVectorUp, float epsilonDeg = 5f) {
		// here I would assert lookup is not empty, epsilon is positive and larger than smallest possible float etc
		// Transform reference up to object space
		Vector3 referenceObjectSpace = transform.InverseTransformDirection(referenceVectorUp);

		// Find smallest difference to object space direction
		float min = float.MaxValue;
		int mostSimilarDirectionIndex = -1;
		for (int i=0; i < directions.Count; ++i) {
			float a = Vector3.Angle(referenceObjectSpace, directions[i]);
			if (a <= epsilonDeg && a < min) {
				min = a;
				mostSimilarDirectionIndex = i;
			}
		}

		// -1 as error code for not within bounds
		return (mostSimilarDirectionIndex >= 0) ? sideValues[mostSimilarDirectionIndex] : -1; 
	}
}
