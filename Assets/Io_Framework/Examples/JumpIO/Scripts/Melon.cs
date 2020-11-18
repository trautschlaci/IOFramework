using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melon : PickUpBase
{
    public override void ApplyEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().RunSpeed *= 2;
    }

    public override void RevertEffect(Collider2D player)
    {
        player.GetComponent<PlayerController>().RunSpeed /= 2;
    }
}
