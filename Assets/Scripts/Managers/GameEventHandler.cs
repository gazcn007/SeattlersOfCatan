using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventHandler {

	public static IEnumerator WaitForKeyDown(KeyCode keycode) {
		while (!Input.GetKeyDown (keycode)) {
			yield return null;
		}
	}
}
