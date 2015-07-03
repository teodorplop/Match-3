using UnityEngine;
using System.Collections;
using System;

public class RandomGenerator {
	public static CandyType NextCandy() {
		return (CandyType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CandyType)).Length);
	}
}