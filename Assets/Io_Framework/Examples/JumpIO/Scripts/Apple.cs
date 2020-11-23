using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Apple : JumpIOPowerUpBase
{
    public int ExtraJumpModifier = 1;

    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().ExtraJumps += ExtraJumpModifier;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<PlayerControllerJumpIO>().ExtraJumps -= ExtraJumpModifier;
    }
}
