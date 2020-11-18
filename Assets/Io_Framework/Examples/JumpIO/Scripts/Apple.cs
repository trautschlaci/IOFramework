using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : PickUpBase
{
    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().ExtraJumps += 1;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().ExtraJumps -= 1;
    }
}
