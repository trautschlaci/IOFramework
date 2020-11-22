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
        RpcNotifyClients();
        var _leaderBoard = FindObjectOfType<LeaderBoard>();
        _leaderBoard.RemovePlayer(connectionToClient.connectionId);
        base.Destroy();
    }

    [ClientRpc]
    public void RpcNotifyClients()
    {
        Instantiate(DeathEffect, transform.position, transform.rotation);

        if (!hasAuthority)
            return;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        IoNetworkManager networkManager = (IoNetworkManager) NetworkManager.singleton;
        networkManager.RestartPlayerClient();
    }
}
