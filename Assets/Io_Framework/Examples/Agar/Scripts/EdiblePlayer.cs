﻿using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class EdiblePlayer : Food
    {

        #region Server

        [Range(0.0f, 1.0f)]
        public float ScoreDecay;


        private Collider2D _collider;
        private PlayerScore _score;


        [Server]
        public override bool CanBeGivenToOther(GameObject other)
        {
            return base.CanBeGivenToOther(other) 
                   && _collider.bounds.extents.x < 0.9f * other.GetComponent<Collider2D>().bounds.extents.x;
        }

        [Server]
        public override int EarnedScore()
        {
            return base.EarnedScore() + (int)((1.0f-ScoreDecay) * _score.Score);
        }

        [Server]
        public override void Destroy()
        {
            GetComponent<Player>().Destroy();
        }


        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _score = GetComponent<PlayerScore>();
        }

        #endregion

    }
}
