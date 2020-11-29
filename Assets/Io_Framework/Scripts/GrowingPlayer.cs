using Mirror;
using UnityEngine;

namespace Io_Framework
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerScore))]
    public abstract class GrowingPlayer : NetworkBehaviour
    {
        public float ViewRangeScaler = 3.0f;
        public bool Is2D = true;
        

        protected PlayerScore Score;
        protected Player OwnPlayer;
        protected Transform OwnTransform;


        private ReverseProximityChecker _reverseProximityChecker;


        [Server]
        protected virtual void ChangeSizeServer(int oldScore, int newScore)
        {
            var lastSize = OwnTransform.localScale.x;
            var newSize = CalculateSizeFromScore(newScore);

            if (Is2D)
                OwnTransform.localScale = new Vector3(newSize, newSize, 1.0f);
            else
                OwnTransform.localScale = new Vector3(newSize, newSize, newSize);
            

            OwnPlayer.ViewRange = ViewRangeScaler * CalculateCameraScale(newSize);
            if (_reverseProximityChecker != null)
            {
                _reverseProximityChecker.OwnExtent *= (newSize/lastSize);
            }
        }

        protected abstract float CalculateSizeFromScore(int score);



        protected abstract float CalculateCameraScale(float size);

        protected virtual void Awake()
        {
            Score = GetComponent<PlayerScore>();
            OwnPlayer = GetComponent<Player>();
            OwnTransform = transform;
            _reverseProximityChecker = GetComponent<ReverseProximityChecker>();
        }

        protected virtual void Start()
        {
            if (!isServer)
                return;

            Score.OnScoreChangedServer += ChangeSizeServer;
            ChangeSizeServer(0, Score.Score);
        }
    }
}
