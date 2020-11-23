using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CloneablePlayerObject : Player
{
    public int MaxNumberOfClones = 20;

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerObjectManager.singleton.AddPlayerObject(connectionToClient.connectionId, this);
    }

    [Server]
    public override void Destroy()
    {
        PlayerObjectManager.singleton.DeleteGameObject(connectionToClient.connectionId, this);
        base.Destroy();
    }

    [Server]
    public virtual GameObject InstantiateClone(Vector3 targetPos, Quaternion rotation)
    {
        GameObject clone = PlayerObjectManager.singleton.InstantiatePlayerObject(targetPos, rotation);
        clone.GetComponent<Player>().PlayerName = PlayerName;
        return clone;
    }

    [Server]
    public virtual bool CanCreateClone()
    {
        return PlayerObjectManager.singleton.GetNumberOfPlayerObjects(connectionToClient.connectionId) <= MaxNumberOfClones;
    }

    public virtual int CompareTo(Player other)
    {
        return 0;
    }
}
