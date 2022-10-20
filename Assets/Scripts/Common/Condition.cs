using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition
{
	public enum Predicate
	{
		EQUAL,
		LESS,
		LESS_EQUAL,
		GREATER,
		GREATER_EQUAL
	}

	public abstract bool IsTrue();
}
