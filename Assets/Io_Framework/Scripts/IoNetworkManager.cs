using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class IoNetworkManager : NetworkManager
{
    public GameObject IndexUI;
    public GameObject RestartUI;
    public GameObject LeaderBoardUI;
    public string PlayerName { get; set; }
    public SpawnPositionSelector SpawnPointSelector;
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

    public override void OnValidate()
    {
        base.OnValidate();
        if (playerPrefab.GetComponent<Player>() == null)
        {
            Debug.LogError("IoNetworkManager: playerPrefab must have Player script");
        }
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
        if (!clientLoadedScene)
        {
            if (!ClientScene.ready) ClientScene.Ready(conn);
            if (autoCreatePlayer)
            {
                CreateNewPlayer();
            }
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        if (IndexUI != null)
            IndexUI.SetActive(true);
        
        if(LeaderBoardUI != null)
            LeaderBoardUI.SetActive(false);

        if (RestartUI != null)
            RestartUI.SetActive(false);
    }

    [Client]
    public void CreateNewPlayer()
    {
        NetworkClient.connection.Send(new CreatePlayerMessage { Name = PlayerName });
    }

    [Client]
    public virtual void RestartPlayerClient()
    {
        if(RestartUI != null)
            RestartUI.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayerMessage);
        if (IndexUI != null)
            IndexUI.SetActive(false);
    }

    void OnCreatePlayerMessage(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
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
