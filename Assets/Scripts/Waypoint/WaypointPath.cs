using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    public bool closed = true;

    public List<Transform> points { get; set; }
    public List<WaypointTag> waypointTags { get; set; }


    private void Awake()
    {
        AssignPoints();
    }


    [ContextMenu("Assign Points")]
    public void AssignPoints()
    {
        points = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            points.Add(transform.GetChild(i));
        }


        waypointTags = new List<WaypointTag>();

        for (int i = 0; i < points.Count; i++)
        {
            waypointTags.Add(points[i].GetComponent<WaypointTag>());
        }
    }


	private void OnDrawGizmos()
	{
		Color color = Color.white;
		color.a = 0.7f;

		Gizmos.color = color;

		DrawPath(false);
	}


    private void OnDrawGizmosSelected()
    {
        DrawPath();
    }


	public void DrawPath(bool wireframe = true)
    {
        AssignPoints();

        for (int i = 0; i < points.Count; i++)
        {
			if (wireframe)
			{
            	Gizmos.DrawWireSphere(points[i].transform.position, 0.5f);
			}
			else
			{
				Gizmos.DrawSphere(points[i].transform.position, 0.5f);
			}

			if (!closed && i == points.Count - 1)
            {
                break;
            }

            var currentPoint = points[i];
            var nextPoint = (i == points.Count - 1) ? points[0] : points[i + 1];

            Gizmos.DrawLine(currentPoint.transform.position, nextPoint.transform.position);
        }
    }
}
