using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControllerJumpIO : NetworkBehaviour
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
    public float AnimationSyncInterval = 0.01f;

    public Transform LeftFoot;
    public Transform RightFoot;
    public float GroundCheckDistance = 0.05f;
    public LayerMask GroundLayers;


    #region Client variables

    private Animator animator;
    private int RunningParamID;
    private int FallingParamID;
    private int MidAirParamID;

    private InputInfo lastInput;

    #endregion


    #region Client and Server variables

    private Rigidbody2D rigidBody;

    [SyncVar(hook = nameof(UpdateAnimator))]
    private AnimatorVariables animatorInfo;

    #endregion


    #region Server variables

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

    private float animatorUpdateTime;

    #endregion


    public struct InputInfo
    {
        public int Horizontal;
        public bool JumpPressed;
        public bool JumpHeld;
    }

    public struct AnimatorVariables
    {
        public bool IsRunning;
        public bool IsFalling;
        public bool IsMidAir;
    }

    [ClientCallback]
    private void Update()
    {
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

    [Client]
    private void UpdateAnimator(AnimatorVariables oldState, AnimatorVariables newState)
    {
        if (animator == null)
            return;

        animator.SetBool(RunningParamID, newState.IsRunning);
        animator.SetBool(FallingParamID, newState.IsFalling);
        animator.SetBool(MidAirParamID, newState.IsMidAir);
    }


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = isServer;

        if (isClient)
        {
            animator = GetComponent<Animator>();
            RunningParamID = Animator.StringToHash("IsRunning");
            FallingParamID = Animator.StringToHash("IsFalling");
            MidAirParamID = Animator.StringToHash("IsMidAir");
            UpdateAnimator(new AnimatorVariables(), animatorInfo);
        }
    }


    [ServerCallback]
    private void FixedUpdate()
    {
        CheckGround();

        AirMovement();

        Move();

        if (rigidBody.velocity.y < MaxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, MaxFallSpeed);

        if (Time.fixedTime > animatorUpdateTime)
        {
            SyncAnimatorState();
            animatorUpdateTime = Time.fixedTime + AnimationSyncInterval;
        }
    }

    [Command]
    private void CmdSendInputInfo(InputInfo input)
    {
        horizontalServer = Mathf.Clamp(input.Horizontal, -1, 1);
        jumpHeldServer = input.JumpHeld;
        if (input.JumpPressed)
        {
            jumpPressedServer = true;
            jumpBufferTime = Time.fixedTime + JumpBuffer;
        }
    }

    [Server]
    public void Jump()
    {
        isJumping = true;
        jumpTime = Time.fixedTime + JumpHoldDuration;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, JumpForce);

        jumpPressedServer = false;
    }

    [Server]
    public override void OnStartServer()
    {
        animatorUpdateTime = Time.fixedTime + 0.2f;
    }

    [Server]
    private void CheckGround()
    {
        isGrounded = false;

        RaycastHit2D leftCheck = Physics2D.Raycast(LeftFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);
        RaycastHit2D rightCheck = Physics2D.Raycast(RightFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);

        if (leftCheck || rightCheck)
        {
            isGrounded = true;
            isJumping = false;
            coyoteTime = Time.fixedTime + CoyoteDuration;
            extraJumpCount = ExtraJumps;
        }
    }

    [Server]
    private void AirMovement()
    {
        if (jumpTime <= Time.fixedTime)
            isJumping = false;

        if ((isGrounded || coyoteTime > Time.fixedTime) && (jumpBufferTime > Time.fixedTime || jumpPressedServer && jumpHeldServer) && !isJumping)
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
    private void Move()
    {
        horizontalMove = horizontalServer * RunSpeed;

        if (horizontalMove * playerDir < 0f)
            FlipCharacterDirection();

        Vector2 targetVelocity = new Vector2(horizontalMove * Time.fixedDeltaTime * 50f, rigidBody.velocity.y);
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, MovementSmoothing);
    }

    [Server]
    private void FlipCharacterDirection()
    {
        playerDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    [Server]
    private void SyncAnimatorState()
    {
        animatorInfo = new AnimatorVariables()
        {
            IsRunning = Mathf.Abs(horizontalMove) > 0.01f,
            IsFalling = rigidBody.velocity.y < -0.01f,
            IsMidAir = !isGrounded
        };
    }
}
