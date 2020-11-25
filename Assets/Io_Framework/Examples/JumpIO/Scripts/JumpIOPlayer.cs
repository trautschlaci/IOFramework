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
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        RpcDisplayDestroy();
        var leaderBoard = FindObjectOfType<LeaderBoard>();
        leaderBoard.RemovePlayer(connectionToClient.connectionId);
        base.Destroy();
    }

    [ClientRpc]
    private void RpcDisplayDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        Instantiate(DeathEffect, transform.position, transform.rotation);


        if (!hasAuthority)
            return;

        IoNetworkManager networkManager = (IoNetworkManager) NetworkManager.singleton;
        networkManager.RestartPlayerClient();
    }
}
