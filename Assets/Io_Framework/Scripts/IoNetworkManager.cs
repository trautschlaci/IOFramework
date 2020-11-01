using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class IoNetworkManager : NetworkManager
{
    public GameObject IndexUI;
    public string PlayerName { get; set; }
    public PlayerSpawnPositionSelector SpawnPointSelector;
    public float SpawnRetryDelay = 0.01f;
    public float SpawnTimeout = 2.0f;

    private float _spawnRetryTime;

    public class CreatePlayerMessage : MessageBase
    {
        public string Name;
    }

    public class CouldNotSpawnMessage : MessageBase
    {
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<CouldNotSpawnMessage>(OnCouldNotSpawnPlayer);
    }

    public virtual void OnCouldNotSpawnPlayer(NetworkConnection conn, CouldNotSpawnMessage message)
    {
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // tell the server to create a player with this name
        conn.Send(new CreatePlayerMessage { Name = PlayerName });
        IndexUI.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        IndexUI.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        IndexUI.SetActive(false);
    }

    void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
    {
        StartCoroutine(SpawnPlayer(connection.connectionId, createPlayerMessage.Name));
    }

    IEnumerator SpawnPlayer(int connectionId, string playerName)
    {
        Vector3 spawnPosition;
        var couldSelectSpawnPosition = SpawnPointSelector.SelectSpawnPosition(out spawnPosition);
        while (!couldSelectSpawnPosition && _spawnRetryTime < SpawnTimeout)
        {
            couldSelectSpawnPosition = SpawnPointSelector.SelectSpawnPosition(out spawnPosition);
            _spawnRetryTime += SpawnRetryDelay;
            yield return new WaitForSeconds(SpawnRetryDelay);
        }

        _spawnRetryTime = 0.0f;

        if (!couldSelectSpawnPosition)
        {
            NetworkServer.connections[connectionId].Send(new CouldNotSpawnMessage());
            NetworkServer.connections[connectionId].Disconnect();
            yield break;
        }

        GameObject playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerGameObject.GetComponent<Player>().PlayerName = playerName;

        NetworkServer.AddPlayerForConnection(NetworkServer.connections[connectionId], playerGameObject);

    }
}
