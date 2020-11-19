using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : PickUpBase
{
    public int ExtraJumpModifier = 1;

    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().ExtraJumps += ExtraJumpModifier;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().ExtraJumps -= ExtraJumpModifier;
    }
}
