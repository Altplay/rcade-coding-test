using UnityEngine;
using System;
using System.Collections;

public class CollisionMessageRelay : MonoBehaviour
{
	public event Action<CollisionMessageRelay, Collider> TriggerEnterEvent;
	public event Action<CollisionMessageRelay, Collider> TriggerExitEvent;

	public event Action<CollisionMessageRelay, Collision> CollisionEnterEvent;
	public event Action<CollisionMessageRelay, Collision> CollisionExitEvent;


	private void OnTriggerEnter(Collider otherCollider)
	{
		if (TriggerEnterEvent != null)
		{
			TriggerEnterEvent(this, otherCollider);
		}
	}

	private void OnTriggerExit(Collider otherCollider)
	{
		if (TriggerExitEvent != null)
		{
			TriggerExitEvent(this, otherCollider);
		}
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (CollisionEnterEvent != null)
		{
			CollisionEnterEvent(this, collision);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (CollisionExitEvent != null)
		{
			CollisionExitEvent(this, collision);
		}
	}
}
