using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassJoystick : MonoBehaviour {

    [Header("Maps Transforms")]
    public Transform mare, terre, atmos;

    [Header("Map Controller")]
    public MapController mapController;

    [Header("Particle Systems")]
    public ParticleSystem[] mareParticles;
    public ParticleSystem[] terreParticles;
    public ParticleSystem[] atmosParticles;

    [Space]
    public float moveSpeed = 5.0f;

    [Space]
    [Header("Joystick Parts")]
    public Transform joystickBorder;
    public Transform joystickButton;
    public Transform[] joystickDirs;

    private bool startMove = false;
    private bool canMove = false;
    private Vector2 pointA;
    private Vector2 pointB;

	void Update () {
        /*if (Input.GetMouseButtonDown(0))
        {
            pointA = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                Camera.main.transform.position.z));

            joystickBorder.transform.position = pointA;
            joystickButton.transform.position = pointA;
            joystickBorder.GetComponent<SpriteRenderer>().enabled = true;
            joystickButton.GetComponent<SpriteRenderer>().enabled = true;

            foreach (Transform direction in joystickDirs)
                direction.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (Input.GetMouseButton(0))
        {
            startMove = true;
            pointB = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                Camera.main.transform.position.z));
        }
        else
        {
            startMove = false;
        }*/

        if (!canMove)
            return;

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pointA = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                Camera.main.transform.position.z));
            }

            else if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {

            }

            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                startMove = true;

                joystickBorder.transform.position = pointA;
                joystickButton.transform.position = pointA;
                joystickBorder.GetComponent<SpriteRenderer>().enabled = true;
                joystickButton.GetComponent<SpriteRenderer>().enabled = true;

                foreach (Transform direction in joystickDirs)
                    direction.GetComponent<SpriteRenderer>().enabled = true;

                pointB = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    Camera.main.transform.position.z));
            }

            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                startMove = false;
            }
            else
            {
                startMove = false;
            }
        }
        else
        {
            startMove = false;
        }
        
	}

    void FixedUpdate()
    {
        if (startMove)
        {
            Vector2 offset = pointB - pointA;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
            moveMaps(direction * -1);

            joystickButton.transform.position = new Vector2(pointA.x + direction.x, pointA.y + direction.y);

            foreach (ParticleSystem particle in mareParticles)
                particle.transform.position = mare.position;

            foreach (ParticleSystem particle in terreParticles)
                particle.transform.position = terre.position;

            foreach (ParticleSystem particle in atmosParticles)
                particle.transform.position = atmos.position;

        }
        else
        {
            joystickBorder.GetComponent<SpriteRenderer>().enabled = false;
            joystickButton.GetComponent<SpriteRenderer>().enabled = false;

            foreach (Transform direction in joystickDirs)
                direction.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void moveMaps(Vector2 direction)
    {
        mare.Translate(direction * moveSpeed * Time.deltaTime);
        terre.Translate(direction * moveSpeed * Time.deltaTime);
        atmos.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public bool IsMoving()
    {
        return startMove;
    }

    public void CanMoveMapAround(bool canMove)
    {
        this.canMove = canMove;
    }
}
