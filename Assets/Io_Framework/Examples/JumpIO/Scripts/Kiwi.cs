using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiwi : PickUpBase
{
    public float JumpForceModifier = 4f;

    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpForce += JumpForceModifier;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpForce -= JumpForceModifier;
    }
}
