using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymmetricTupleInt : TupleInt {

	public SymmetricTupleInt(int a, int b) : base(a, b) {
	}

	public override bool Equals(object obj)
	{
		var other = obj as TupleInt;
		if (object.ReferenceEquals (other, null))
			return false;
		else
			return (first.Equals(other.first) && second.Equals(other.second)) ||
				(first.Equals(other.second) && second.Equals(other.first));
	}
}
