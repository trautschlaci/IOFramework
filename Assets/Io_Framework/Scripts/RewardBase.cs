﻿using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public abstract class RewardBase : NetworkBehaviour
    {
        public bool IsAvailable = true;
        public int ConstantScore = 1;


        public virtual bool CanBeGivenToOther(GameObject other)
        {
            return IsAvailable;
        }

        public virtual int EarnedScore()
        {
            return ConstantScore;
        }

        protected virtual void ClaimReward(GameObject other)
        {
            IsAvailable = false;
            other.GetComponent<PlayerScore>().Score += EarnedScore();
            Destroy();
        }

        public abstract void Destroy();

    }
}
