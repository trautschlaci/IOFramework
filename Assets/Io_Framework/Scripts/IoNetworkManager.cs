using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Io_Framework
{
    public class IONetworkManager : NetworkManager
    {

        #region Public fields Client

        [Tooltip("The main GameObject of the index UI. It gets enabled when the player disconnects from the server.")]
        public GameObject IndexUI;

        [Tooltip("The main GameObject of the restart UI. Restart UI is the UI that gets shown after a player dies.")]
        public GameObject RestartUI;

        [Tooltip("The InputField in the RestartUI where the player can input it's name.")]
        public InputField RestartUINameInputField;


        public string PlayerName { get; set; }

        #endregion



        #region Public fields Server

        [Tooltip("An object that selects positions to spawn to for the player.")]
        public SpawnPositionSelector SpawnPointSelector;

        [Tooltip("The minimum delay between two spawn attempts, in seconds.")]
        public float SpawnRetryDelay = 0.01f;

        [Tooltip("The maximum amount of time should be spent trying to spawn the player, in seconds.")]
        public float SpawnTimeout = 2.0f;

        #endregion



        #region Client

        // Sends the server a message to create an object for the player with it's name.
        [Client]
        public void CreateNewPlayer()
        {
            NetworkClient.connection.Send(new CreatePlayerMessage { Name = PlayerName });
        }

        [Client]
        public virtual void ShowRestartUI()
        {
            if (RestartUINameInputField != null)
                RestartUINameInputField.text = PlayerName;

            if (RestartUI != null)
                RestartUI.SetActive(true);
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterHandler<CouldNotSpawnMessage>(OnCouldNotSpawnPlayer);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            if (clientLoadedScene)
                return;


            if (!ClientScene.ready)
                ClientScene.Ready(conn);

            if (autoCreatePlayer)
                CreateNewPlayer();

        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            if (IndexUI != null)
                IndexUI.SetActive(true);

            if (RestartUI != null)
                RestartUI.SetActive(false);
        }


        // Override this method to specify what should happen in the client of the player whose object couldn't have been spawned.
        [Client]
        protected virtual void OnCouldNotSpawnPlayer(NetworkConnection conn, CouldNotSpawnMessage message)
        {
        }

        #endregion



        #region Client and Server

        private float _spawnRetryTime;


        protected class CreatePlayerMessage : MessageBase
        {
            public string Name;
        }

        protected class CouldNotSpawnMessage : MessageBase
        {
        }


        public override void OnValidate()
        {
            base.OnValidate();
            if (playerPrefab.GetComponent<Player>() == null)
            {
                Debug.LogError("IONetworkManager: playerPrefab must have Player script");
            }
        }

        #endregion



        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayerMessage);
        }


        [Server]
        private void OnCreatePlayerMessage(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
        {
            StartCoroutine(SpawnPlayer(connection.connectionId, createPlayerMessage.Name));
        }

        [Server]
        private IEnumerator SpawnPlayer(int connectionId, string playerName)
        {
            var couldSelectSpawnPosition = SpawnPointSelector.SelectSpawnPosition(out var spawnPosition);


            // If the position is occupied try to select other.
            while (!couldSelectSpawnPosition && _spawnRetryTime < SpawnTimeout)
            {
                couldSelectSpawnPosition = SpawnPointSelector.SelectSpawnPosition(out spawnPosition);
                _spawnRetryTime += SpawnRetryDelay;
                yield return new WaitForSeconds(SpawnRetryDelay);
            }

            _spawnRetryTime = 0.0f;

            // If spawn position couldn't have been selected, send a message to the player and disconnect it.
            if (!couldSelectSpawnPosition)
            {
                NetworkServer.connections[connectionId].Send(new CouldNotSpawnMessage());
                NetworkServer.connections[connectionId].Disconnect();
                yield break;
            }


            // Create the object for the player.
            var playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerGameObject.GetComponent<Player>().PlayerName = playerName;

            NetworkServer.AddPlayerForConnection(NetworkServer.connections[connectionId], playerGameObject);

        }

        #endregion

    }
}
