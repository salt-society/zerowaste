using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAndPan : MonoBehaviour {

    public Camera targetCamera;

    // private float beforeCameraSize;
    // private float afterCameraSize;

    public float minZoom = 20f;
    public float maxZoom = 40f;
    public float zoomSpeed = .01f;


	void Update () {
        if (Input.touchCount == 2)
        {
            // beforeCameraSize = targetCamera.orthographicSize;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float diffMagnitude = currentMagnitude - prevMagnitude;

            Zoom(diffMagnitude * zoomSpeed);
        }
	}

    void Zoom(float increment)
    {
        targetCamera.orthographicSize = Mathf.Clamp(targetCamera.orthographicSize - increment, minZoom, maxZoom);
        // afterCameraSize = targetCamera.orthographicSize;
    }
    
}
