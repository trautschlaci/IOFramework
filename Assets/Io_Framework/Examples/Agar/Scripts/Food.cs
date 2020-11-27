﻿using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class Food : RewardBase
    {
        public override bool CanBeGivenToOther(GameObject other)
        {
            return base.CanBeGivenToOther(other) 
                   && other.tag == "Player" 
                   && Vector3.Distance(transform.position, other.transform.position) < other.GetComponent<Collider2D>().bounds.extents.x;
        }

        public override void Destroy()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        void OnTriggerStay2D(Collider2D other)
        {
            if (CanBeGivenToOther(other.gameObject))
            {
                ClaimReward(other.gameObject);
            }
        }
    }
}
