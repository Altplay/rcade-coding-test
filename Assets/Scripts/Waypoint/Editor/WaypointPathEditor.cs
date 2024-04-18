using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
	private WaypointPath path;
    private Transform selectedPoint;


	private void DrawInfo()
	{
		Handles.BeginGUI();

		GUILayout.BeginArea(new Rect(10, Screen.height - 100, Screen.width, 90));
		GUILayout.Label("New waypoint: Shift + N");
		GUILayout.Label("Duplicate waypoint: Shift + D");
		GUILayout.Label("Delete waypoint: Delete");
		GUILayout.EndArea();

		Handles.EndGUI();
	}


	private void DestroyWaypoint(Transform waypoint)
	{
		Undo.DestroyObjectImmediate(waypoint.gameObject);
		path.AssignPoints();
	}


	private Transform CreateWaypoint(Transform originalPoint)
	{
		GameObject newPoint = null;

		if (originalPoint == null)
		{
			newPoint = new GameObject("Waypoint");
			newPoint.transform.position = path.transform.position;
		}
		else
		{
			newPoint = (GameObject)Instantiate(originalPoint.gameObject);
			newPoint.name = selectedPoint.name;
		}

		Undo.RegisterCreatedObjectUndo(newPoint, "Create");

		newPoint.transform.SetParent(path.transform);

		if (originalPoint != null)
		{
			newPoint.transform.position = originalPoint.position;
		}

		path.AssignPoints();

		return newPoint.transform;
	}


    private void OnSceneGUI()
    {
		DrawInfo();


        path = (WaypointPath)target;

        path.AssignPoints();

        if (path.points != null)
        {
	        for (int i = 0; i < path.points.Count; i++)
	        {
	            var currentPoint = path.points[i];

				bool isSelected = Handles.Button(currentPoint.position, Quaternion.identity, 0.7f, 0.7f, Handles.SphereHandleCap);

	            if (isSelected)
	            {
	                selectedPoint = currentPoint;
	            }

	            if (currentPoint == selectedPoint)
	            {
	                Vector3 movedPos = Handles.PositionHandle(currentPoint.position, Quaternion.identity);

	                if (currentPoint.position != movedPos)
	                {
	                    Undo.RecordObject(currentPoint, "Move");

	                    currentPoint.position = movedPos;
	                }
	            }
	        }
		}
        
        if (Event.current.type == EventType.KeyDown)
        {
			if (selectedPoint != null)
			{      
	            if (Event.current.keyCode == KeyCode.Delete)
	            {
					DestroyWaypoint(selectedPoint);

	                Event.current.Use();
	            }
	            else if (Event.current.shift && Event.current.keyCode == KeyCode.D)
	            {
					var newPoint = CreateWaypoint(selectedPoint);

	                selectedPoint = newPoint.transform;

	                Event.current.Use();
	            }
			}

			if (Event.current.shift && Event.current.keyCode == KeyCode.N)
			{
				var newPoint = CreateWaypoint(null);

				selectedPoint = newPoint.transform;

				Event.current.Use();
			}
        }        
    }
}
