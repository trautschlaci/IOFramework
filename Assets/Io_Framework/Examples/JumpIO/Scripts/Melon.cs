using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Melon : JumpIOPickUpBase
{
    public float SpeedModifier = 2.0f;
    public float MaxSpeed = 10.0f;

    [Server]
    public override bool CanAffectPlayerServer(GameObject player)
    {
        return player.GetComponent<PlayerController>().RunSpeed + SpeedModifier <= MaxSpeed;
    }

    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().RunSpeed += SpeedModifier;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().RunSpeed -= SpeedModifier;
    }
}
