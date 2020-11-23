using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControllerAgar : NetworkBehaviour
{
    public float Speed = 3.0f;
    public float JumpSpeed = 10.0f;
    public float MovementSmoothing = 0.1f;
    public float MoveBlockInterval = 0.3f;
    public float InputSyncInterval = 0.1f;



    private float inputSyncTime;
    private Vector2 moveVectorClient;
    private bool jumpPressedClient;
    private int frameCount;


    private Rigidbody2D rigidBody;
    private AgarPlayer2 player;


    private Vector2 moveVectorServer;
    private bool jumpPressedServer;

    private Vector2 velocity = Vector2.zero;
    private float moveBlockTime;


    public struct InputInfo
    {
        public Vector2 MoveVector;
        public bool JumpPressed;
    }


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

        moveVectorClient += target;
        jumpPressedClient = jumpPressedClient || Input.GetButtonDown("Jump");
        frameCount++;

        if (inputSyncTime < Time.time)
        {
            var inputInfo = new InputInfo()
            {
                MoveVector = (moveVectorClient / frameCount).normalized,
                JumpPressed = jumpPressedClient
            };

            CmdSendInputInfo(inputInfo);

            moveVectorClient = Vector2.zero;
            jumpPressedClient = false;
            inputSyncTime = Time.time + InputSyncInterval;
        }
    }


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = isServer;
        //player = GetComponent<AgarPlayer2>();
    }


    [ServerCallback]
    private void FixedUpdate()
    {
        Move();

        if (jumpPressedServer)
            player.Split(moveVectorServer);
    }

    [Server]
    public void GiveStartVelocity(Vector2 startVelocityDir)
    {
        moveBlockTime = Time.fixedDeltaTime + MoveBlockInterval;

        Vector2 targetVelocity = startVelocityDir * JumpSpeed * Time.fixedDeltaTime * 50f / (float)Math.Sqrt(transform.localScale.x);
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, MovementSmoothing);
    }

    [Command]
    private void CmdSendInputInfo(InputInfo input)
    {
        moveVectorServer = input.MoveVector.normalized;
        jumpPressedServer = input.JumpPressed;
    }

    [Server]
    private void Move()
    {
        if (moveBlockTime > Time.fixedTime)
            return;

        Vector2 targetVelocity = moveVectorServer * Speed * Time.fixedDeltaTime * 50f / (float)Math.Sqrt(transform.localScale.x);
        rigidBody.velocity = Vector2.SmoothDamp(rigidBody.velocity, targetVelocity, ref velocity, MovementSmoothing);
    }

}
