using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiwi : PickUpBase
{
    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpForce *= 2;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().JumpForce /= 2;
    }
}
