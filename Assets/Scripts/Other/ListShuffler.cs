using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Static class used to randomly shuffle a list.
// Random Selector nodes (decorator) of the behavior
// tree make use of this utility class to
// randomly iterate through its children
public static class ListShuffler {

	public static void Shuffle<T>(this IList<T> list) {  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = Random.Range (0, n + 1);
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

}
