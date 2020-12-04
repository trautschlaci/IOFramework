using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Abstract base class for player objects that can change size based on their score.
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerScore))]
    public abstract class GrowingPlayerBase : NetworkBehaviour
    {

        #region Public fields Server

        [Tooltip("The calculated size of the camera gets multiplied by this value to determine the ViewRange of the Player.")]
        public float ViewRangeScaler = 3.0f;

        [Tooltip("Set this true if the object is in 2D otherwise false.")]
        public bool Is2D = true;

        #endregion



        #region Client and Server

        protected PlayerScore Score;
        protected Player OwnPlayer;
        protected Transform OwnTransform;


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


        // Calculate the size of the camera from the own size of the player-object.
        protected abstract float CalculateCameraSize(float ownSize);

        #endregion



        #region Server

        private ReverseProximityChecker _reverseProximityChecker;


        // Callback method used when the score of the player gets changed.
        [Server]
        protected virtual void ChangeSizeServer(int oldScore, int newScore)
        {
            var lastSize = OwnTransform.localScale.x;
            var newSize = CalculateSizeFromScore(newScore);

            // Change the size of the player-object based on score.
            if (Is2D)
                OwnTransform.localScale = new Vector3(newSize, newSize, 1.0f);
            else
                OwnTransform.localScale = new Vector3(newSize, newSize, newSize);


            // Change the ViewRange and OwnExtent based on the new size of the player-object.
            OwnPlayer.ViewRange = ViewRangeScaler * CalculateCameraSize(newSize);
            if (_reverseProximityChecker != null)
            {
                _reverseProximityChecker.OwnExtent *= (newSize / lastSize);
            }
        }

        // Calculate the new size of the player-object from the score.
        protected abstract float CalculateSizeFromScore(int score);

        #endregion

    }
}
