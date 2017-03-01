using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymmetricTuple<T1, T2> : Tuple<T1, T2> {

	//private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;
	//private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	public SymmetricTuple(T1 first, T2 second) : base(first, second) {
	}

	/*public override bool Equals(object obj)
	{
		var other = obj as Tuple<T1, T2>;
		if (object.ReferenceEquals (other, null))
			return false;
		else
			return (Item1Comparer.Equals (first, other.first) &&
			Item2Comparer.Equals (second, other.second));
	}*/
}
