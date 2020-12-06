using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class Food : RewardBase
    {

        #region Server

        [Server]
        public override bool CanBeGivenToOther(GameObject other)
        {
            return base.CanBeGivenToOther(other) 
                   && other.CompareTag("Player") 
                   && Vector3.Distance(transform.position, other.transform.position) < other.GetComponent<Collider2D>().bounds.extents.x;
        }

        [Server]
        public override void Destroy()
        {
            NetworkServer.Destroy(gameObject);
        }


        [ServerCallback]
        private void OnTriggerStay2D(Collider2D other)
        {
            if (CanBeGivenToOther(other.gameObject))
            {
                ClaimReward(other.gameObject);
            }
        }

        #endregion

    }
}
