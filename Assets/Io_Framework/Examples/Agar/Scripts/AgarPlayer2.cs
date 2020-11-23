﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AgarPlayer2 : CloneablePlayerObject
{
    public int MinScoreToSplit = 10;

    private PlayerScore playerScore;

    [Server]
    private void Start()
    {
        playerScore = GetComponent<PlayerScore>();
    }

    [Server]
    public void Split(Vector2 startVelocityDir)
    {
        if (!CanCreateClone())
            return;

        playerScore.Score = (int)(playerScore.Score / 2.0f);

        Vector3 target = transform.position + (Vector3)(startVelocityDir * GetComponent<Collider2D>().bounds.extents.x);
        GameObject half = InstantiateClone(target, Quaternion.identity);
        half.GetComponent<PlayerScore>().Score = playerScore.Score;
        NetworkServer.Spawn(half, connectionToClient);
        half.GetComponent<PlayerControllerAgar>().GiveStartVelocity(startVelocityDir);
    }

    public override bool CanCreateClone()
    {
        return playerScore.Score >= MinScoreToSplit && base.CanCreateClone();
    }
}
