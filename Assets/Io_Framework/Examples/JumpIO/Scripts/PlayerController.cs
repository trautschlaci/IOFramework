using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 10f;
    public float JumpForce = 2f;
    public float JumpHoldForce = 0.5f;
    public float JumpHoldDuration = 0.2f;
    public float CoyoteDuration = 0.1f;

    public Transform LeftFoot;
    public Transform RightFoot;
    public float GroundCheckDistance = 0.05f;
    public LayerMask GroundLayers;

    private Rigidbody2D rigidBody;
    private float horizontalMove;
    private int playerDir = 1;

    private bool isGrounded;
    private bool isJumping;
    private bool jumpPressed;
    private bool jumpHeld;
    private float coyoteTime;
    private float jumpTime;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        horizontalMove = horizontal * RunSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        jumpHeld = Input.GetButton("Jump");
    }

    void FixedUpdate()
    {
        CheckGround();

        Jump();

        Move();
    }

    void FlipCharacterDirection()
    {
        playerDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void CheckGround()
    {
        isGrounded = false;

        RaycastHit2D leftCheck = Physics2D.Raycast(LeftFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);
        RaycastHit2D rightCheck = Physics2D.Raycast(RightFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);

        if (leftCheck || rightCheck)
        {
            isGrounded = true;
            coyoteTime = Time.time + CoyoteDuration;
        }
    }

    void Jump()
    {
        if (jumpPressed && !isJumping && (isGrounded || coyoteTime > Time.time))
        {
            isJumping = true;
            jumpTime = Time.time + JumpHoldDuration;
            rigidBody.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
            jumpPressed = false;
        }
        else if (isJumping)
        {
            if (jumpHeld)
                rigidBody.AddForce(new Vector2(0f, JumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime <= Time.time)
                isJumping = false;
        }
    }

    void Move()
    {
        if (horizontalMove * playerDir < 0f)
            FlipCharacterDirection();

        rigidBody.velocity = new Vector2(horizontalMove, rigidBody.velocity.y);
    }
}
