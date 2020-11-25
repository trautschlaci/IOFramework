﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControllerAgar : NetworkBehaviour
{
    public float Speed = 3.0f;
    public float JumpSpeed = 10.0f;
    public float MoveBlockInterval = 0.3f;
    public float InputSyncInterval = 0.1f;


    private float inputSyncTime;
    private Vector2 mousePosClient;
    private bool jumpPressedClient;


    private Rigidbody2D rigidBody;
    private AgarPlayer player;
    private RectTransform boundary;


    private Vector3[] boundaryCorners;
    private Vector2 moveVectorServer;
    private bool jumpPressedServer;

    private float moveBlockTime;


    public struct InputInfo
    {
        public Vector2 MousePos;
        public bool JumpPressed;
    }


    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        mousePosClient = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        jumpPressedClient = jumpPressedClient || Input.GetButtonDown("Jump");

        if (inputSyncTime < Time.time)
        {
            var inputInfo = new InputInfo()
            {
                MousePos = mousePosClient,
                JumpPressed = jumpPressedClient
            };

            CmdSendInputInfo(inputInfo);

            jumpPressedClient = false;
            inputSyncTime = Time.time + InputSyncInterval;
        }
    }


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = isServer;
        player = GetComponent<AgarPlayer>();

        boundary = GameObject.Find("Map").GetComponent<RectTransform>();
        boundaryCorners = new Vector3[4];
        boundary.GetWorldCorners(boundaryCorners);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = isServer;
        player = GetComponent<AgarPlayer>();
    }


    [ServerCallback]
    private void FixedUpdate()
    {
        Move();

        if (jumpPressedServer)
        {
            jumpPressedServer = false;
            player.Split(moveVectorServer.normalized);
        }

        var newX = Mathf.Clamp(transform.position.x, boundaryCorners[0].x - 0.01f, boundaryCorners[2].x + 0.01f);
        var newY = Mathf.Clamp(transform.position.y, boundaryCorners[0].y - 0.01f, boundaryCorners[2].y + 0.01f);
        transform.position = new Vector3(newX, newY, 0);
    }

    [Server]
    public void GiveStartVelocity(Vector2 startVelocityDir)
    {
        moveBlockTime = Time.fixedTime + MoveBlockInterval;

        rigidBody.velocity = startVelocityDir * JumpSpeed * Time.fixedDeltaTime * 50f * Mathf.Sqrt(transform.localScale.x);
    }

    [Command]
    private void CmdSendInputInfo(InputInfo input)
    {
        var clones = PlayerObjectManager.singleton.GetPlayerObjects(connectionToClient.connectionId);
        if (clones == null) return;

        foreach (var playerClone in clones)
        {
            playerClone.GetComponent<PlayerControllerAgar>().UpdateMovement(input);
        }
    }

    public void UpdateMovement(InputInfo input)
    {
        var mousePos = input.MousePos;
        moveVectorServer = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        
        if (moveVectorServer.magnitude > 1)
            moveVectorServer = moveVectorServer.normalized;

        if (input.JumpPressed)
            jumpPressedServer = true;
    }

    [Server]
    private void Move()
    {
        if (moveBlockTime > Time.fixedTime)
            return;

        rigidBody.velocity = moveVectorServer * Speed * Time.fixedDeltaTime * 50f / Mathf.Sqrt(transform.localScale.x);
    }

}
