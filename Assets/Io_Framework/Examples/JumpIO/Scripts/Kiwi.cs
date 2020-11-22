using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Kiwi : JumpIOPickUpBase
{
    public float JumpForceModifier = 4f;

    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpForce += JumpForceModifier;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpForce -= JumpForceModifier;
    }
}
