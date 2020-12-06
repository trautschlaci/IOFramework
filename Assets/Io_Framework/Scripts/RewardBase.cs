using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Base class for objects that can give score to player-objects.
    public abstract class RewardBase : NetworkBehaviour
    {

        #region Server

        [Tooltip("Can players still claim it?")]
        public bool IsAvailable = true;

        [Tooltip("The base score this reward gives.")]
        public int ConstantScore = 1;


        [Server]
        public virtual bool CanBeGivenToOther(GameObject other)
        {
            return IsAvailable && other.GetComponent<PlayerScore>() != null;
        }

        [Server]
        public virtual int EarnedScore()
        {
            return ConstantScore;
        }

        public abstract void Destroy();


        [Server]
        protected virtual void ClaimReward(GameObject other)
        {
            IsAvailable = false;
            other.GetComponent<PlayerScore>().Score += EarnedScore();
            Destroy();
        }

        #endregion

    }
}
