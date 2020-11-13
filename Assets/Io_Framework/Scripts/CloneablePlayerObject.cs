using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CloneablePlayerObject : Player
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerObjectManager.singleton.AddPlayerObject(PlayerId, this);
    }

    [Server]
    public override void Destroy()
    {
        PlayerObjectManager.singleton.DeleteGameObject(PlayerId, this);
        base.Destroy();
    }

    [Server]
    public virtual GameObject InstantiateClone(Vector3 targetPos, Quaternion rotation)
    {
        GameObject clone = PlayerObjectManager.singleton.InstantiatePlayerObject(targetPos, rotation);
        clone.GetComponent<Player>().PlayerName = PlayerName;
        return clone;
    }

    public virtual int CompareTo(Player other)
    {
        return 0;
    }
}
