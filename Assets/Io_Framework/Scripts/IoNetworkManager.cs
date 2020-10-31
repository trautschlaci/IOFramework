using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class IoNetworkManager : NetworkManager
{
    public GameObject IndexUI;
    public string PlayerName { get; set; }

    public class CreatePlayerMessage : MessageBase
    {
        public string name;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
        IndexUI.SetActive(false);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // tell the server to create a player with this name
        conn.Send(new CreatePlayerMessage { name = PlayerName });
        IndexUI.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        IndexUI.SetActive(true);
    }

    void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
    {
        // create a gameobject using the name supplied by client
        GameObject playergo = Instantiate(playerPrefab);
        playergo.GetComponent<Player>().PlayerName = createPlayerMessage.name;

        // set it as the player
        NetworkServer.AddPlayerForConnection(connection, playergo);
    }
}
