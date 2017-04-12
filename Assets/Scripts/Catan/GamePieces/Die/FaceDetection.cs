using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceDetection : MonoBehaviour {

	public List<Vector3> directions;
	public List<int> faceValues;
	public int value=-1;
	protected Vector3 localHitNormalized;
	protected float validMargin = 0.45F;
	public bool ready=false;
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
	protected bool valid(float t, float v)
	{
		if (t > (v - validMargin) && t < (v + validMargin))
			return true;
		else
			return false;
	}

	// calculate the normalized hit vector and should always return true
	protected bool localHit
	{
		get
		{
			// create a Ray from straight above this Die , moving downwards
			Ray ray = new Ray(transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
			RaycastHit hit = new RaycastHit();
			// cast the ray and validate it against this die's collider
			if (GetComponent<Collider>().Raycast(ray, out hit, 3 * transform.localScale.magnitude))
			{
				// we got a hit so we determine the local normalized vector from the die center to the face that was hit.
				// because we are using local space, each die side will have its own local hit vector coordinates that will always be the same.
				localHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
				return true;
			}
			// in theory we should not get at this position!
			return false;
		}
	}
	// calculate this die's value
	void GetValue()
	{
		// value = 0 -> undetermined or invalid
		value = 0;
		float delta = 1;
		// start with side 1 going up.
		int side = 1;
		Vector3 testHitVector;
		// check all sides of this die, the side that has a valid hitVector and smallest x,y,z delta (if more sides are valid) will be the closest and this die's value
		do
		{
			// get testHitVector from current side, HitVector is a overriden method in the dieType specific Die subclass
			// eacht dieType subclass will expose all hitVectors for its sides,
			testHitVector = HitVector(side);
			if (testHitVector != Vector3.zero)
			{
				// this side has a hitVector so validate the x,y and z value against the local normalized hitVector using the margin.
				if (valid(localHitNormalized.x, testHitVector.x) &&
					valid(localHitNormalized.y, testHitVector.y) &&
					valid(localHitNormalized.z, testHitVector.z))
				{
					// this side is valid within the margin, check the x,y, and z delta to see if we can set this side as this die's value
					// if more than one side is within the margin (especially with d10, d12, d20 ) we have to use the closest as the right side
					float nDelta = Mathf.Abs(localHitNormalized.x - testHitVector.x) + Mathf.Abs(localHitNormalized.y - testHitVector.y) + Mathf.Abs(localHitNormalized.z - testHitVector.z);
					if (nDelta < delta)
					{
						value = side;
						ready=true;
						delta = nDelta;
					}
				}
			}
			// increment side
			side++;
			// if we got a Vector.zero as the testHitVector we have checked all sides of this die
		} while (testHitVector != Vector3.zero);
	}
	void Update()
	{
		// determine the value is the die is not rolling
		if (!rolling() && localHit)
			GetValue();
	}
	// true is die is still rolling
	public bool rolling()
	{

			return !(this.GetComponent<Rigidbody>().velocity.sqrMagnitude < .1F && this.GetComponent<Rigidbody>().angularVelocity.sqrMagnitude < .1F);
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
	private Vector3 HitVector(int side)
	{
		switch (side)
		{
		case 1: return new Vector3(0F, 0F, 1F);
		case 2: return new Vector3(0F, -1F, 0F);
		case 3: return new Vector3(-1F, 0F, 0F);
		case 4: return new Vector3(1F, 0F, 0F);
		case 5: return new Vector3(0F, 1F, 0F);
		case 6: return new Vector3(0F, 0F, -1F);
		}
		return Vector3.zero;
	}
}
