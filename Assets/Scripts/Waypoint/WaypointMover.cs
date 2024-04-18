using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class WaypointMover : MonoBehaviour
{
	public enum LoopType
	{
		Yoyo,
		Restart
	}

	public event Action IdleEnteredEvent;
	public event Action IdleExitedEvent;

	public event Action ReachedEndEvent;


	public WaypointPath path;
	public bool ignoreWaypointPosiitionY = false;
	public float pickNextWaypointDistance = 0.2f;

	public bool hasIdle = true;
	[ShowIf(nameof(hasIdle))] public float idleDuration = 0;
	[ShowIf(nameof(hasIdle))] public bool randomIdleDuration = false;
	[ShowIf(nameof(randomIdleDuration))] public FloatRange idleDurationRange;

	public bool loop = true;
	public int directionSign = 1;
    public bool randomDirectionSign = false;

    private float currentIdleDuration;
	private float elapsedIdleTime;

    public int currentWaypointIndex { get; private set; }
	public bool isIdle { get; private set; }
	public bool reachedEnd { get; private set; }


	private void Start()
	{
        if (path == null)
        {
            return;
        }

		SetNearestWaypointAsCurrent();

        SetCurrentIdleDuration();
	}


	public Transform GetNearestWaypoint()
	{
		Transform nearestWaypoint = null;
		float nearestSqrDistance = Mathf.Infinity;

		for (int i = 0; i < path.points.Count; i++)
		{
			float sqrDistance = (transform.position - path.points[i].position).sqrMagnitude;

			if (sqrDistance < nearestSqrDistance)
			{
				nearestSqrDistance = sqrDistance;
				nearestWaypoint = path.points[i];
			}
		}

		return nearestWaypoint;
	}


	public void SetCurrentWaypoint(Transform waypoint)
	{
		SetCurrentWaypoint(path.points.IndexOf(waypoint));
	}


	public void SetCurrentWaypoint(int index)
	{
		if (index < 0 || index >= path.points.Count)
		{
			return;
		}

		currentWaypointIndex = index;
		elapsedIdleTime = 0;

        SetCurrentIdleDuration();

        RefreshDirectionSign();
	}


    private void SetCurrentIdleDuration()
    {
		if (!hasIdle)
			return;

        if (randomIdleDuration)
        {
            currentIdleDuration = (float)idleDurationRange;
        }
        else
        {
            currentIdleDuration = idleDuration;
        }

        // Include the stayDuration of the current waypoint
        if (currentWaypointIndex >= 0 && currentWaypointIndex < path.points.Count)
        {
            var currentWaypointTag = path.waypointTags[currentWaypointIndex];

            if (currentWaypointTag != null)
            {
                currentIdleDuration += currentWaypointTag.stayDuration;
            }
        }
    }


    private void RefreshDirectionSign()
    {
        if (randomDirectionSign)
        {
            directionSign = UnityEngine.Random.value > 0.5f ? 1 : -1;
        }
    }


    public void SetNearestWaypointAsCurrent()
	{
		SetCurrentWaypoint(GetNearestWaypoint());
	}


	public Transform GetCurrentWaypoint()
	{
        if (path == null)
        {
            return null;
        }

		if (currentWaypointIndex < 0 || currentWaypointIndex >= path.points.Count)
		{
			return null;
		}

		return path.points[currentWaypointIndex];
	}
		

	public void UpdateMover()
	{
        if (path == null || path.points == null || path.points.Count <= 0)
        {
            return;
        }

		var waypoints = path.points;

		Transform currentWaypoint = null;
		Vector3 directionToCurrentWaypoint = new Vector3();
		float sqrDistanceToCurrentWaypoint = 0;


		// Pick next waypoint
		while (true)
		{
			isIdle = false;

			currentWaypoint = waypoints[currentWaypointIndex];

			directionToCurrentWaypoint = currentWaypoint.position - transform.position;

			if (ignoreWaypointPosiitionY)
			{
				directionToCurrentWaypoint.y = 0;
			}

			sqrDistanceToCurrentWaypoint = directionToCurrentWaypoint.sqrMagnitude;

			bool reachedCurrentWaypoint = sqrDistanceToCurrentWaypoint <= pickNextWaypointDistance * pickNextWaypointDistance;

			if (!reachedCurrentWaypoint)
			{
				break;
			}

			directionToCurrentWaypoint = Vector3.zero;

			IdleEnteredEvent?.Invoke();

			break;

			//IdleUpdate();

			//PickNextWaypoint();
		}
	}


	public void ChangePath(WaypointPath path)
	{
		if (this.path == path)
		{
			return;
		}

		this.path = path;

		SetNearestWaypointAsCurrent();
	}

	public void IdleUpdate()
    {
		if (currentIdleDuration > 0)
		{
			isIdle = true;

			elapsedIdleTime += Time.deltaTime;

			// Idling...
			if (elapsedIdleTime < currentIdleDuration)
			{
				return;
			}

			elapsedIdleTime = 0;

			// Refresh direction sign
			RefreshDirectionSign();
			PickNextWaypoint();

			IdleExitedEvent?.Invoke();

			isIdle = false;
		}
	}

	public void PickNextWaypoint()
    {
		var waypoints = path.points;
		currentWaypointIndex += directionSign;

		if (!loop)
		{
			if (currentWaypointIndex >= waypoints.Count || currentWaypointIndex < 0)
			{
				if (!reachedEnd)
				{
					reachedEnd = true;

					OnReachedEnd();
				}
			}
			else
			{
				reachedEnd = false;
			}

			currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Count - 1);

			// Reset idle duration when currentWaypointIndex changed
			SetCurrentIdleDuration();

			//if (reachedEnd)
			//{
			//	break;
			//}
		}
		else
		{
			LoopType loopType = LoopType.Yoyo;

			if (path.closed)
			{
				loopType = LoopType.Restart;
			}
			else
			{
				loopType = LoopType.Yoyo;
			}

			if (currentWaypointIndex < 0)
			{
				if (loopType == LoopType.Yoyo)
				{
					currentWaypointIndex = 1;
					directionSign *= -1;
				}
				else if (loopType == LoopType.Restart)
				{
					currentWaypointIndex = waypoints.Count - 1;
				}
			}
			else if (currentWaypointIndex >= waypoints.Count)
			{
				if (loopType == LoopType.Yoyo)
				{
					currentWaypointIndex = waypoints.Count - 2;
					directionSign *= -1;
				}
				else if (loopType == LoopType.Restart)
				{
					currentWaypointIndex = 0;
				}
			}

			// Reset idle duration when currentWaypointIndex changed
			SetCurrentIdleDuration();
		}
	}


	private void OnReachedEnd()
	{
		if (ReachedEndEvent != null)
		{
			ReachedEndEvent();
		}
	}

    
    // Methods for event call
    public void SetIdleDuration(float d)
    {
        randomIdleDuration = false;
        idleDuration = d;
    }

    public void SetRandomIdleDurationMin(float d)
    {
        randomIdleDuration = true;
        idleDurationRange.min = d;
    }

    public void SetRandomIdleDurationMax(float d)
    {
        randomIdleDuration = true;
        idleDurationRange.max = d;
    }

    public void SetRandomIdleDuration(bool b)
    {
        randomIdleDuration = b;
    }

    public void SetLoop(bool b)
    {
        loop = b;
    }

    public void SetDirectionSign(int i)
    {
        directionSign = i;
    }

    public void FlipDirectionSign()
    {
        directionSign *= -1;
    }
}