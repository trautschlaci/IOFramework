﻿using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Kiwi : JumpIO2PowerUpBase
{
    public float JumpForceModifier = 4f;

    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().JumpForce += JumpForceModifier;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().JumpForce -= JumpForceModifier;
    }
}
