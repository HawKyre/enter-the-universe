using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;
    private float playerSpeed = 5f;

    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Because there's physics involved
    void FixedUpdate()
    {
        if (GameState.GetInstance()._CurrentUIState == UIState.GAME)
        {
            var deltaMovement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            rb2D.velocity = deltaMovement * playerSpeed;
        }
    }
}
