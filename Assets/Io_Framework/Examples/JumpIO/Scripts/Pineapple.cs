using System.Collections;
using System.Collections.Generic;
using Assets.Io_Framework.Examples.JumpIO.Scripts;
using Mirror;
using UnityEngine;

public class Pineapple : JumpIOPickUpBase
{
    [Server]
    public override bool CanBeGivenToPlayerServer(GameObject player)
    {
        return player.GetComponent<StompPlayer>().IsAvailable;
    }

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
