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

    private Animator _animator;
    private int _runningParamId;
    private int _fallingParamId;
    private int _midAirParamId;

    private InputInfo _lastInput;

    #endregion


    #region Client and Server variables

    private Rigidbody2D _rigidBody;

    [SyncVar]
    private AnimatorVariables _animatorInfo;

    #endregion


    #region Server variables

    private float _horizontalServer;
    private bool _jumpPressedServer;
    private bool _jumpHeldServer;

    private Vector2 _velocity = Vector2.zero;
    private float _horizontalMove;
    private int _playerDir = 1;
    private bool _isGrounded;
    private bool _isJumping;
    private float _coyoteTime;
    private float _jumpBufferTime;
    private float _jumpTime;
    private int _extraJumpCount;

    private float _animatorUpdateTime;

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
        UpdateAnimator();


        if (!hasAuthority)
            return;

        UpdateInput();
    }

    [Client]
    private void UpdateAnimator()
    {
        _animator.SetBool(_runningParamId, _animatorInfo.IsRunning);
        _animator.SetBool(_fallingParamId, _animatorInfo.IsFalling);
        _animator.SetBool(_midAirParamId, _animatorInfo.IsMidAir);
    }

    [Client]
    private void UpdateInput()
    {
        var inputInfo = new InputInfo
        {
            Horizontal = (int)Input.GetAxisRaw("Horizontal"),
            JumpPressed = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump")
        };

        if (_lastInput.Equals(inputInfo)) return;

        _lastInput = inputInfo;
        CmdSendInputInfo(inputInfo);
    }


    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.simulated = isServer;


        if (!isClient) return;

        _animator = GetComponent<Animator>();
        _runningParamId = Animator.StringToHash("IsRunning");
        _fallingParamId = Animator.StringToHash("IsFalling");
        _midAirParamId = Animator.StringToHash("IsMidAir");
    }


    [ServerCallback]
    private void FixedUpdate()
    {
        CheckGround();

        AirMovement();

        Move();

        if (_rigidBody.velocity.y < MaxFallSpeed)
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, MaxFallSpeed);

        if (Time.fixedTime > _animatorUpdateTime)
        {
            SyncAnimatorState();
            _animatorUpdateTime = Time.fixedTime + AnimationSyncInterval;
        }
    }

    [Command]
    private void CmdSendInputInfo(InputInfo input)
    {
        _horizontalServer = Mathf.Clamp(input.Horizontal, -1, 1);
        _jumpHeldServer = input.JumpHeld;
        if (input.JumpPressed)
        {
            _jumpPressedServer = true;
            _jumpBufferTime = Time.fixedTime + JumpBuffer;
        }
    }

    [Server]
    public void Jump()
    {
        _isJumping = true;
        _jumpTime = Time.fixedTime + JumpHoldDuration;
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, JumpForce);

        _jumpPressedServer = false;
    }

    [Server]
    public override void OnStartServer()
    {
        _animatorUpdateTime = Time.fixedTime + 0.2f;
    }

    [Server]
    private void CheckGround()
    {
        _isGrounded = false;

        RaycastHit2D leftCheck = Physics2D.Raycast(LeftFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);
        RaycastHit2D rightCheck = Physics2D.Raycast(RightFoot.position, Vector2.down, GroundCheckDistance, GroundLayers);

        if (leftCheck || rightCheck)
        {
            _isGrounded = true;
            _isJumping = false;
            _coyoteTime = Time.fixedTime + CoyoteDuration;
            _extraJumpCount = ExtraJumps;
        }
    }

    [Server]
    private void AirMovement()
    {
        if (_jumpTime <= Time.fixedTime)
            _isJumping = false;

        if ((_isGrounded || _coyoteTime > Time.fixedTime) && (_jumpBufferTime > Time.fixedTime || _jumpPressedServer && _jumpHeldServer) && !_isJumping)
        {
            Jump();
            return;
        }

        if (_jumpPressedServer && _extraJumpCount > 0 && !_isGrounded)
        {
            Jump();
            _extraJumpCount--;
            return;
        }

        if (_isJumping && _jumpHeldServer)
        {
            _rigidBody.AddForce(new Vector2(0f, JumpHoldForce), ForceMode2D.Impulse);
        }

        if (_isGrounded)
        {
            _jumpPressedServer = false;
        }
    }

    [Server]
    private void Move()
    {
        _horizontalMove = _horizontalServer * RunSpeed;

        if (_horizontalMove * _playerDir < 0f)
            FlipCharacterDirection();

        Vector2 targetVelocity = new Vector2(_horizontalMove * Time.fixedDeltaTime * 50f, _rigidBody.velocity.y);
        _rigidBody.velocity = Vector2.SmoothDamp(_rigidBody.velocity, targetVelocity, ref _velocity, MovementSmoothing);
    }

    [Server]
    private void FlipCharacterDirection()
    {
        _playerDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    [Server]
    private void SyncAnimatorState()
    {
        _animatorInfo = new AnimatorVariables
        {
            IsRunning = Mathf.Abs(_horizontalMove) > 0.01f,
            IsFalling = _rigidBody.velocity.y < -0.01f,
            IsMidAir = !_isGrounded
        };
    }
}
