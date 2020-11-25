using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class CloneablePlayerObject : Player
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
        bool wasLast = PlayerObjectManager.singleton.DeleteGameObject(connectionToClient.connectionId, this);
        if(wasLast)
            OnLastPlayerObjectDestroyed();

        base.Destroy();
    }

    [Server]
    public virtual GameObject SpawnClone(Vector3 targetPos, Quaternion rotation)
    {
        GameObject clone = PlayerObjectManager.singleton.InstantiatePlayerObject(targetPos, rotation);
        clone.GetComponent<Player>().PlayerName = PlayerName;
        NetworkServer.Spawn(clone, connectionToClient);
        return clone;
    }

    [Server]
    public virtual bool CanCreateClone()
    {
        return PlayerObjectManager.singleton.GetNumberOfPlayerObjects(connectionToClient.connectionId) < MaxNumberOfClones;
    }

    [Server]
    public virtual void OnLastPlayerObjectDestroyed()
    {
        var leaderBoard = FindObjectOfType<LeaderBoard>();
        leaderBoard.RemovePlayer(connectionToClient.connectionId);
    }

    [Server]
    public virtual int CompareTo(CloneablePlayerObject other)
    {
        return 0;
    }

}
