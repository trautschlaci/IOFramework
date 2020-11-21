using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class JumpIOPlayer : Player
{
    public GameObject DeathEffect;

    [Server]
    public override void Destroy()
    {
        RpcPlayDeathEffect();
        base.Destroy();
    }

    [ClientRpc]
    public void RpcPlayDeathEffect()
    {
        Instantiate(DeathEffect, transform.position, transform.rotation);
    }
}
