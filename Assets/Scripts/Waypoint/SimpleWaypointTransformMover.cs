using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWaypointTransformMover : MonoBehaviour 
{
	public WaypointMover waypointMover;
	public float speed = 5;


	protected virtual void Update()
	{
		waypointMover.UpdateMover();

		Transform currentWaypoint = waypointMover.GetCurrentWaypoint();

        if (currentWaypoint == null)
        {
            return;
        }

		Vector3 directionToCurrentWaypoint = currentWaypoint.position - transform.position;

		if (directionToCurrentWaypoint.sqrMagnitude <= Mathf.Pow(waypointMover.pickNextWaypointDistance, 2))
		{
			transform.position = currentWaypoint.position;
		}
		else
		{
			transform.position += directionToCurrentWaypoint.normalized * speed * Time.deltaTime;
		}
	}
}
