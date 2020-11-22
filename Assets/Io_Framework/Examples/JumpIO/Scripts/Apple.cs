using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using UnityEngine;

public class Apple : JumpIOPickUpBase
{
    public int ExtraJumpModifier = 1;

    public override void ApplyEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().ExtraJumps += ExtraJumpModifier;
    }

    public override void RevertEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().ExtraJumps -= ExtraJumpModifier;
    }
}
