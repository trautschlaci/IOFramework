using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CloneablePlayerObject : Player
{
    private PlayerObjectManager _playerManager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _playerManager = FindObjectOfType<PlayerObjectManager>();
        _playerManager.AddPlayerObject(PlayerId, this);
    }

    [Server]
    public GameObject SpawnClone(Vector3 targetPos)
    {
        GameObject clone = _playerManager.SpawnPlayerObject(targetPos);
        clone.GetComponent<Player>().PlayerName = PlayerName;
        return clone;
    }

    [Server]
    public override void Destroy()
    {
        _playerManager.DeleteGameObject(PlayerId, this);
        base.Destroy();
    }
}
