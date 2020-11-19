using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : PickUpBase
{
    public float JumpHoldDurationModifier = 1f;

    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration += JumpHoldDurationModifier;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration -= JumpHoldDurationModifier;
    }
}
