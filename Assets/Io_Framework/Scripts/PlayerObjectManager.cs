﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Websocket;
using UnityEngine;

public class PlayerObjectManager: MonoBehaviour
{
    public GameObject PlayerPrefab;
    public static PlayerObjectManager singleton { get; private set; }


    private readonly Dictionary<int, List<CloneablePlayerObject>> _objectsOfPlayers = new Dictionary<int, List<CloneablePlayerObject>>();

    [ServerCallback]
    void Awake()
    {
        InitializeSingleton();
    }

    [ServerCallback]
    void Start()
    {
        InitializeSingleton();
    }

    [Server]
    private void InitializeSingleton()
    {
        if (singleton != null)
        {
            if (singleton == this) return;

            Debug.Log("Multiple PlayerObjectManagers detected in the scene. The duplicate will be destroyed.");
            Destroy(gameObject);

            return;
        }

        singleton = this;
    }

    [Server]
    public void AddPlayerObject(int playerId, CloneablePlayerObject playerObject)
    {
        if(!_objectsOfPlayers.ContainsKey(playerId))
            _objectsOfPlayers.Add(playerId, new List<CloneablePlayerObject>());
        _objectsOfPlayers[playerId].Add(playerObject);
    }

    [Server]
    public CloneablePlayerObject GetNextMainPlayerObject(int playerId)
    {
        CloneablePlayerObject nextMainPlayerObject = null;
        foreach (var playerObject in _objectsOfPlayers[playerId])
        {
            if (!IsMainPlayerObject(playerObject) && (nextMainPlayerObject == null || playerObject.CompareTo(nextMainPlayerObject) > 0))
            {
                nextMainPlayerObject = playerObject;
            }
        }

        return nextMainPlayerObject;
    }

    [Server]
    public void RemovePlayerObject(int playerId, CloneablePlayerObject playerObject)
    {
        _objectsOfPlayers[playerId].Remove(playerObject);
        if (_objectsOfPlayers[playerId].Count < 1)
        {
            _objectsOfPlayers.Remove(playerId);
        }
    }

    [Server]
    public GameObject InstantiatePlayerObject(Vector3 targetPos, Quaternion rotation)
    {
        GameObject spawnedPlayerObject = Instantiate(PlayerPrefab, targetPos, rotation);
        return spawnedPlayerObject;
    }

    [Server]
    public void DeleteGameObject(int playerId, CloneablePlayerObject playerObject)
    {
        bool isLastPlayerObject = false;
        if (IsMainPlayerObject(playerObject))
            isLastPlayerObject = !ChangeMainPlayerObject(playerId);

        RemovePlayerObject(playerId, playerObject);
        if (isLastPlayerObject)
        {
            NetworkServer.connections[playerId].Disconnect();
        }
    }

    [Server]
    public bool IsMainPlayerObject(CloneablePlayerObject playerObject)
    {
        return playerObject.connectionToClient.identity == playerObject.netIdentity;
    }

    [Server]
    public bool ChangeMainPlayerObject(int playerId)
    {
        CloneablePlayerObject nextMainPlayer = GetNextMainPlayerObject(playerId);

        if (nextMainPlayer != null)
        {
            NetworkServer.ReplacePlayerForConnection(nextMainPlayer.connectionToClient, nextMainPlayer.gameObject);
            return true;
        }

        return false;
    }
}
