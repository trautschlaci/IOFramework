using System;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public class PlayerScore : NetworkBehaviour
    {
        [SerializeField]
        [SyncVar(hook = nameof(SetScoreClient))]
        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                OnScoreChangedServer?.Invoke(_score, value);
                _score = value;
            }
        }

        public event Action<int, int> OnScoreChangedClient;
        public event Action<int, int> OnScoreChangedServer;

        // Server
        private Player _playerObject;

        [Client]
        private void SetScoreClient(int oldScore, int newScore)
        {
            OnScoreChangedClient?.Invoke(oldScore, newScore);
        }

        private void Awake()
        {
            _playerObject = GetComponent<Player>();
        }

        public override void OnStartServer()
        {
            OnScoreChangedServer += ScoreChangedServer;
            OnScoreChangedServer?.Invoke(0, _score);
            _playerObject.OnPlayerDestroyedServer += PlayerDestroyed;
        }

        [Server]
        private void ScoreChangedServer(int oldScore, int newScore)
        {
            LeaderBoard.ServerSingleton.ChangeScore(connectionToClient.connectionId, _playerObject.PlayerName, newScore-oldScore);
        }

        [Server]
        private void PlayerDestroyed()
        {
            LeaderBoard.ServerSingleton.RemoveScore(connectionToClient.connectionId, _score);
        }

    }
}
