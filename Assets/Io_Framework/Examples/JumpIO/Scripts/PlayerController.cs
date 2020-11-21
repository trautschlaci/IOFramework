using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float RunSpeed = 4f;
    public float JumpForce = 6f;
    public float JumpHoldForce = 0.6f;
    public float JumpHoldDuration = 0.2f;
    public float CoyoteDuration = 0.1f;
    public float JumpBuffer = 0.1f;
    public float MaxFallSpeed = -12f;
    public int ExtraJumps;
    public float MovementSmoothing = 0.1f;

    public Transform LeftFoot;
    public Transform RightFoot;
    public float GroundCheckDistance = 0.05f;
    public LayerMask GroundLayers;


    private Animator animator;
    private int SpeedParamID;
    private int VerticalVelocityParamID;
    private int MidAirParamID;


    private Rigidbody2D rigidBody;
    private InputInfo lastInput;


    private float horizontalServer;
    private bool jumpPressedServer;
    private bool jumpHeldServer;

    private Vector2 velocity = Vector2.zero;
    private float horizontalMove;
    private int playerDir = 1;
    private bool isGrounded;
    private bool isJumping;
    private float coyoteTime;
    private float jumpBufferTime;
    private float jumpTime;
    private int extraJumpCount;

    public struct InputInfo
    {
        public int Horizontal;
        public bool JumpPressed;
        public bool JumpHeld;
    }

    [Server]
    public void Jump()
    {
        isJumping = true;
        jumpTime = Time.time + JumpHoldDuration;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, JumpForce);

        jumpPressedServer = false;
    }

    void Start()
    {
        GetComponent<Rigidbody2D>().simulated = isServer;

        if (isClient)
        {
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody2D>();
            SpeedParamID = Animator.StringToHash("Speed");
            VerticalVelocityParamID = Animator.StringToHash("VerticalVelocity");
            MidAirParamID = Animator.StringToHash("IsMidAir");
        }
    }

    [ClientCallback]
    void Update()
    {
        animator.SetBool(MidAirParamID, !isGrounded);
        animator.SetFloat(SpeedParamID, Mathf.Abs(horizontalMove));
        animator.SetFloat(VerticalVelocityParamID, rigidBody.velocity.y);


        if (!hasAuthority)
            return;

        var inputInfo = new InputInfo
        {
            Horizontal = (int)Input.GetAxisRaw("Horizontal"),
            JumpPressed = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump")
        };

        if (!lastInput.Equals(inputInfo))
        {
            lastInput = inputInfo;
            CmdSendInputInfo(inputInfo);
        }
    }

    [Command]
    void CmdSendInputInfo(InputInfo input)
    {
        horizontalServer = input.Horizontal;
        jumpHeldServer = input.JumpHeld;
        if (input.JumpPressed)
        {
            jumpPressedServer = true;
            jumpBufferTime = Time.time + JumpBuffer;
        }
    }

    [ServerCallback]
    void FixedUpdate()
    {
        CheckGround();

        AirMovement();

        Move();

        if (rigidBody.velocity.y < MaxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, MaxFallSpeed);
    }

    [Server]
    void CheckGround()
    {
        isGrounded = false;

        RaycastHit2D leftCheck = Physics2D.Raycast(LeftFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);
        RaycastHit2D rightCheck = Physics2D.Raycast(RightFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);

        if (leftCheck || rightCheck)
        {
            isGrounded = true;
            isJumping = false;
            coyoteTime = Time.time + CoyoteDuration;
            extraJumpCount = ExtraJumps;
        }
    }

    [Server]
    void AirMovement()
    {
        if (jumpTime <= Time.time)
            isJumping = false;

        if ((isGrounded || coyoteTime > Time.time) && (jumpBufferTime > Time.time || jumpPressedServer && jumpHeldServer) && !isJumping)
        {
            Jump();
            return;
        }

        if (jumpPressedServer && extraJumpCount > 0 && !isGrounded)
        {
            Jump();
            extraJumpCount--;
            return;
        }

        if (isJumping && jumpHeldServer)
        {
            rigidBody.AddForce(new Vector2(0f, JumpHoldForce), ForceMode2D.Impulse);
        }

        if (isGrounded)
        {
            jumpPressedServer = false;
        }
    }

    [Server]
    void Move()
    {
        horizontalMove = horizontalServer * RunSpeed;

        if (horizontalMove * playerDir < 0f)
            FlipCharacterDirection();

        Vector2 targetVelocity = new Vector2(horizontalMove * Time.deltaTime * 50f, rigidBody.velocity.y);
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, MovementSmoothing);
    }

    [Server]
    void FlipCharacterDirection()
    {
        playerDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}