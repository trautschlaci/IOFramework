using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : PickUpBase
{
    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration *= 8;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration /= 8;
    }
}
