using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public abstract class GrowingPlayer : NetworkBehaviour
    {
        public float ViewRangeScaler = 3.0f;
        public bool Is2D = true;
        

        protected PlayerScore Score;
        protected Player OwnPlayer;


        private ReverseProximityChecker _reverseProximityChecker;


        [Server]
        protected virtual void ChangeSizeServer(int oldScore, int newScore)
        {
            var lastSize = transform.localScale.x;
            var newSize = CalculateSizeFromScore(newScore);

            if (Is2D)
                transform.localScale = new Vector3(newSize, newSize, 1.0f);
            else
                transform.localScale = new Vector3(newSize, newSize, newSize);
            

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
