using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using UnityEngine;

public class Kiwi : JumpIOPickUpBase
{
    public float JumpForceModifier = 4f;

    public override void ApplyEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpForce += JumpForceModifier;
    }

    public override void RevertEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpForce -= JumpForceModifier;
    }
}
