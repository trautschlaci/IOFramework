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
        private LeaderBoard _leaderBoard;

        [Client]
        private void SetScoreClient(int oldScore, int newScore)
        {
            OnScoreChangedClient?.Invoke(oldScore, newScore);
        }

        public override void OnStartServer()
        {
            _playerObject = GetComponent<Player>();
            _leaderBoard = FindObjectOfType<LeaderBoard>();
            OnScoreChangedServer += ScoreChangedServer;
            OnScoreChangedServer?.Invoke(0, _score);
            _playerObject.OnPlayerDestroyedServer += PlayerDestroyed;
        }

        [Server]
        private void ScoreChangedServer(int oldScore, int newScore)
        {
            _leaderBoard.ChangeScore(connectionToClient.connectionId, _playerObject.PlayerName, newScore-oldScore);
        }

        [Server]
        private void PlayerDestroyed()
        {
            _leaderBoard.RemoveScore(connectionToClient.connectionId, _score);
        }

    }
}
