using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// League of Legends style camera that moves when cursor is
// set to the edge of the screen
public class CameraRoamer : MonoBehaviour {

	public float speed = 1.0f;
	public int bounds = 5;

	public Vector2 minBounds;
	public Vector2 maxBounds;

	public bool roamEnabled = true;

	// Use this for initialization
	void Start () {
		minBounds = new Vector2 (-15.0f, -20.0f);
		maxBounds = new Vector2 (15.0f, 5.0f);
	}

	// Update is called once per frame
	void Update () {
		// If enabled, drag the camera towards the cursor at set speed if the cursor is near
		// a specific corner of the screen
		if (roamEnabled) {
			Vector3 newPosition = transform.position;
			if (Input.GetKey(KeyCode.W)) {
				newPosition += new Vector3(0.0f, 0.0f, speed * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.A)) {
				newPosition += new Vector3(-1.0f * speed * Time.deltaTime, 0.0f, 0.0f);
			}

			if (Input.GetKey(KeyCode.S)) {
				newPosition += new Vector3(0.0f, 0.0f, -1.0f * speed * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.D)) {
				newPosition += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
			}

			newPosition.x = Mathf.Clamp (newPosition.x, minBounds.x, maxBounds.x);
			newPosition.z = Mathf.Clamp (newPosition.z, minBounds.y, maxBounds.y);

			transform.position = newPosition;
		}

		// Escape stops the camera from moving
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!roamEnabled) {
				roamEnabled = true;
			} else {
				roamEnabled = false;
			}
		}

	}


}
