using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentFinderExtension {

	public static GameObject FindChildByName(GameObject parent, string childName) {
		GameObject foundChild = null;

		foreach (Transform childTransform in parent.transform) {
			if (childTransform.gameObject.name == childName) {
				foundChild = childTransform.gameObject;
				return foundChild;
			}

			if (childTransform.transform.childCount > 0) {
				foundChild = FindChildByName (childTransform.gameObject, childName);

				if (foundChild != null) {
					return foundChild;
				}
			}
		}
		return foundChild;
	}

	public static GameObject[] FindChildrenByName(GameObject parent, string childName) {
		List<GameObject> foundChildrenList = new List<GameObject> ();

		FindChildrenByNameHelper (ref foundChildrenList, parent, childName);

		return foundChildrenList.ToArray ();
	}

	private static void FindChildrenByNameHelper(ref List<GameObject> childrenList, GameObject objectToSearch, string searchName) {
		foreach (Transform childTransform in objectToSearch.transform) {
			if (childTransform.gameObject.name == searchName) {
				childrenList.Add (childTransform.gameObject);
			}

			if (childTransform.transform.childCount > 0) {
				FindChildrenByNameHelper (ref childrenList, childTransform.gameObject, searchName);
			}
		}
	}
}
