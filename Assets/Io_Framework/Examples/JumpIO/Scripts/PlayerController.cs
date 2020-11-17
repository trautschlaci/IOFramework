using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 4f;
    public float JumpForce = 6f;
    public float JumpHoldForce = 1f;
    public float JumpHoldDuration = 0.1f;
    public float CoyoteDuration = 0.1f;
    public float JumpBuffer = 0.1f;
    public float MaxFallSpeed = -12f;

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
    private float jumpBufferTime;
    private float jumpTime;
    private float horizontal;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
            jumpBufferTime = Time.time + JumpBuffer;
        }

        jumpHeld = Input.GetButton("Jump");
    }

    void FixedUpdate()
    {
        CheckGround();

        Jump();

        Move();

        if (rigidBody.velocity.y < MaxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, MaxFallSpeed);
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
        if ((jumpBufferTime > Time.time || jumpPressed && jumpHeld) && !isJumping && (isGrounded || coyoteTime > Time.time))
        {
            isJumping = true;
            jumpTime = Time.time + JumpHoldDuration;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, JumpForce);

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
        horizontalMove = horizontal * RunSpeed;

        if (horizontalMove * playerDir < 0f)
            FlipCharacterDirection();

        rigidBody.velocity = new Vector2(horizontalMove * Time.deltaTime * 50f, rigidBody.velocity.y);
    }

    void FlipCharacterDirection()
    {
        playerDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
