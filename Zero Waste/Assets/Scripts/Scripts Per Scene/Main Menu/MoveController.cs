using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveController : MonoBehaviour {

    public float walkSpeed;

    private float screenWidth;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerBody;

    private Vector2 playerVelocity;

    public Button button;

    void Start()
    {
        // Get sprite of player
        playerSprite = GetComponent<SpriteRenderer>();

        // Get rigidbody
        playerBody = GetComponent<Rigidbody2D>();

        // Get with of screen
        screenWidth = Screen.width;
    }


    void Update()
    {
        /*if (Input.touchCount > 0)
        {
            // Get first touch
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > screenWidth / 2)
            {
                MoveCharacter(1.0f);
            }

            if (touch.position.x < screenWidth / 2)
            {
                MoveCharacter(-1.0f);
            }
        }*/

        int i = 0;
        while (i < Input.touchCount)
        {
            // Get first touch
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > screenWidth / 2)
            {
                MoveCharacter(1.0f);
            }

            if (touch.position.x < screenWidth / 2)
            {
                MoveCharacter(-1.0f);
            }
        }
    }

    void FixedUpdate()
    {
        #if UNITY_EDITOR
        MoveCharacter(Input.GetAxisRaw("Horizontal"));
        #endif
    }

    public void MoveCharacter(float horizontalInput)
    {
        // playerSprite.flipX = !playerSprite.flipX;
        playerBody.AddForce(new Vector2(horizontalInput * walkSpeed * Time.deltaTime, 0));
        /*Vector2 moveInput = new Vector2(transform.position.x - 1, transform.position.y);
        playerVelocity = moveInput.normalized * walkSpeed;
        transform.position = playerVelocity * Time.fixedDeltaTime;
        Debug.Log("Left: " + playerVelocity);*/
    }

    public void MoveRight()
    {
        playerSprite.flipX = true;
        Vector2 moveInput = new Vector2(transform.position.x + 1, transform.position.y);
        playerVelocity = moveInput.normalized * walkSpeed;
        transform.position = playerVelocity * Time.fixedDeltaTime;
        Debug.Log("Right: " + playerVelocity);
    }

}
