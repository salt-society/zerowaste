using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour {

    private LineRenderer lineRenderer;
    private float distance;
    private float drawnDistance;

    private bool enableDraw = false;
    private bool finished = false;

    private Vector2 origin;
    private Vector2 destination;

    private int endOfLine = 1;

    public float lineThickness = .20f;
    public float drawSpeed = 3f;

    public void DrawLine(Vector2 origin, Vector2 destination, int positions, int startOfLine, int endOfLine)
    {
        this.origin = origin;
        this.destination = destination;
        this.endOfLine = endOfLine;

        enableDraw = false;

        lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.positionCount = positions;
        lineRenderer.SetPosition(startOfLine, origin);

        distance = Vector2.Distance(origin, destination);
    }

    public void EnableDraw(bool enableDraw)
    {
        this.enableDraw = enableDraw;
    }

	void Update () {
        // This is needed so that the line will draw after a node is instantiated
        if (enableDraw && !finished)
        {
            // Condition is to stop drawing when destination is reached
            if (drawnDistance < distance)
            {
                drawnDistance += Time.deltaTime / drawSpeed;
                float somePoint = Mathf.Lerp(0, distance, drawnDistance);

                Vector3 pointA = origin;
                Vector3 pointB = destination;
                Vector3 renderedLine = somePoint * Vector3.Normalize(pointB - pointA) + pointA;

                lineRenderer.SetPosition(endOfLine, renderedLine);
            }
            else
            {
                enableDraw = false;
                finished = true;
            }
        }
	}

}
