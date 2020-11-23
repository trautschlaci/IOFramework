using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Pineapple : JumpIO2PowerUpBase
{
    [Server]
    public override void ApplyEffect(GameObject player)
    {
        player.GetComponent<StompPlayer>().IsAvailable = false;
    }

    [Server]
    public override void RevertEffect(GameObject player)
    {
        player.GetComponent<StompPlayer>().IsAvailable = true;
    }
}
