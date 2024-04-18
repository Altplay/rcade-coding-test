using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FloatRange
{
	public float min, max;


	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}


	private float GetRandomValue()
	{
		return Random.Range(min, max);
	}


	public static implicit operator float(FloatRange d) 
	{
		return d.GetRandomValue();
	}
}
