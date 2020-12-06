using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class PlayerAgar : CloneablePlayerObjectBase
    {

        #region Public fields Server

        public int MinScoreToSplit = 10;

        #endregion



        #region Client and Server

        private PlayerScore _playerScore;
        private Collider2D _collider;
        private RandomColor _color;


        private void Awake()
        {
            _playerScore = GetComponent<PlayerScore>();
            _collider = GetComponent<Collider2D>();
            _color = GetComponent<RandomColor>();
        }


        [TargetRpc]
        private void TargetLastObjectDestroyed()
        {
            var networkManager = (IONetworkManager)NetworkManager.singleton;
            networkManager.ShowRestartUI();
        }

        #endregion



        #region Server

        [Server]
        public void Split(Vector2 startVelocityDir)
        {
            if (!CanCreateClone())
                return;

            _playerScore.Score = (int)(_playerScore.Score / 2.0f);

            var target = transform.position + (Vector3)(startVelocityDir * _collider.bounds.extents.x * 1.1f);
            var half = SpawnClone(target, Quaternion.identity);
            half.GetComponent<PlayerScore>().Score = _playerScore.Score;
            half.GetComponent<RandomColor>().BodyColor = _color.BodyColor;
            half.GetComponent<PlayerControllerAgar>().GiveStartVelocity(startVelocityDir);
        }
    
        [Server]
        public override bool CanCreateClone()
        {
            return _playerScore.Score >= MinScoreToSplit && base.CanCreateClone();
        }

        [Server]
        public override int CompareTo(CloneablePlayerObjectBase other)
        {
            var otherScore = other.GetComponent<PlayerScore>();
            if (otherScore == null)
                return base.CompareTo(other);

            return _playerScore.Score.CompareTo(otherScore.Score);
        }


        [Server]
        protected override void OnLastPlayerObjectDestroyed()
        {
            Leaderboard.ServerSingleton.RemovePlayer(connectionToClient.connectionId);
            TargetLastObjectDestroyed();
        }

        #endregion

    }
}
