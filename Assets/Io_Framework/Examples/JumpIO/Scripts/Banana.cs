using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Banana : JumpIO2PowerUpBase
{
    public float JumpHoldDurationModifier = 1f;

    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration += JumpHoldDurationModifier;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration -= JumpHoldDurationModifier;
    }
}
