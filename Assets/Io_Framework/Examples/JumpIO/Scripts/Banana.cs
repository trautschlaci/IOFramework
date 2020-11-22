using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using UnityEngine;

public class Banana : JumpIOPickUpBase
{
    public float JumpHoldDurationModifier = 1f;

    public override void ApplyEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration += JumpHoldDurationModifier;
    }

    public override void RevertEffectServer(GameObject player)
    {
        player.GetComponent<PlayerController>().JumpHoldDuration -= JumpHoldDurationModifier;
    }
}
