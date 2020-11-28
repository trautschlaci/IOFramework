using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class AgarPlayer : CloneablePlayerObject
    {
        public int MinScoreToSplit = 10;

        private PlayerScore _playerScore;


        [ServerCallback]
        private void Start()
        {
            _playerScore = GetComponent<PlayerScore>();
        }

        [ServerCallback]
        private void Awake()
        {
            _playerScore = GetComponent<PlayerScore>();
        }

        [Server]
        public void Split(Vector2 startVelocityDir)
        {
            if (!CanCreateClone())
                return;

            _playerScore.Score = (int)(_playerScore.Score / 2.0f);

            var target = transform.position + (Vector3)(startVelocityDir * GetComponent<Collider2D>().bounds.extents.x * 1.1f);
            var half = SpawnClone(target, Quaternion.identity);
            half.GetComponent<PlayerScore>().Score = _playerScore.Score;
            half.GetComponent<RandomColor>().BodyColor = GetComponent<RandomColor>().BodyColor;
            half.GetComponent<PlayerControllerAgar>().GiveStartVelocity(startVelocityDir);
        }
    
        [Server]
        public override bool CanCreateClone()
        {
            return _playerScore.Score >= MinScoreToSplit && base.CanCreateClone();
        }

        [Server]
        protected override void OnLastPlayerObjectDestroyed()
        {
            base.OnLastPlayerObjectDestroyed();
            TargetLastObjectDestroyed();
        }


        [TargetRpc]
        private void TargetLastObjectDestroyed()
        {
            var networkManager = (IoNetworkManager)NetworkManager.singleton;
            networkManager.RestartPlayerClient();
        }


        public override int CompareTo(CloneablePlayerObject other)
        {
            var otherScore = other.GetComponent<PlayerScore>();
            if (otherScore == null)
                return base.CompareTo(other);

            return _playerScore.Score.CompareTo(otherScore.Score);
        }
    }
}
