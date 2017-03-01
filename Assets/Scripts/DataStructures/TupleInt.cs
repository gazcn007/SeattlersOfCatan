using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TupleInt : Tuple<int, int> {
	public static TupleInt zero {
		get { return new TupleInt(0, 0); }
	}

	public static TupleInt one {
		get { return new TupleInt(1, 1); }
	}

	public TupleInt(int a, int b) : base(a, b) {
	}
}
