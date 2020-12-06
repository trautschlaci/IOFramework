using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class StompedPlayer : RewardBase
    {

        #region Server

        public Transform HeadLeft;
        public Transform HeadRight;
        public float CheckDistance = 0.05f;
        public LayerMask PlayerLayer;


        private Player _player;


        [Server]
        public override bool CanBeGivenToOther(GameObject other)
        {
            return base.CanBeGivenToOther(other) && other != gameObject;
        }

        [Server]
        public override void Destroy()
        {
            _player.Destroy();
        }


        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            CheckAboveHead();
        }


        [Server]
        protected override void ClaimReward(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().Jump();
            base.ClaimReward(player);
        }

        [Server]
        private void CheckAboveHead()
        {
            var leftCheck = Physics2D.Raycast(HeadLeft.position, Vector2.up, CheckDistance, PlayerLayer);
            var rightCheck = Physics2D.Raycast(HeadRight.position, Vector2.up, CheckDistance, PlayerLayer);

            var contact = leftCheck;
            if (!contact)
                contact = rightCheck;

            if (contact && CanBeGivenToOther(contact.collider.gameObject))
            {
                ClaimReward(contact.collider.gameObject);
            }
        }

        #endregion

    }
}
