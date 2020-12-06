using System;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Score of a player-object.
    [RequireComponent(typeof(Player))]
    public class PlayerScore : NetworkBehaviour
    {

        #region Client

        public event Action<int, int> OnScoreChangedClient;


        [Client]
        private void SetScoreClient(int oldScore, int newScore)
        {
            OnScoreChangedClient?.Invoke(oldScore, newScore);
        }

        #endregion



        #region Client and Server

        [SerializeField]
        [SyncVar(hook = nameof(SetScoreClient))]
        private int _score;

        // Should only be changed from server.
        public int Score
        {
            get => _score;
            set
            {
                OnScoreChangedServer?.Invoke(_score, value);
                _score = value;
            }
        }


        private void Awake()
        {
            _playerObject = GetComponent<Player>();
        }

        #endregion



        #region Server

        public event Action<int, int> OnScoreChangedServer;


        private Player _playerObject;


        public override void OnStartServer()
        {
            OnScoreChangedServer += ScoreChangedServer;
            OnScoreChangedServer?.Invoke(0, _score);
            _playerObject.OnPlayerDestroyedServer += PlayerDestroyed;
        }

        [Server]
        private void ScoreChangedServer(int oldScore, int newScore)
        {
            Leaderboard.ServerSingleton.ChangeScore(connectionToClient.connectionId, _playerObject.PlayerName, newScore-oldScore);
        }

        [Server]
        private void PlayerDestroyed()
        {
            Leaderboard.ServerSingleton.RemoveScore(connectionToClient.connectionId, _score);
        }

        #endregion

    }
}
