using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class StompPlayer : RewardBase
    {
        public Transform HeadLeft;
        public Transform HeadRight;
        public float CheckDistance = 0.05f;
        public LayerMask PlayerLayer;

        private Player _player;

        [ServerCallback]
        void Start()
        {
            _player = GetComponent<Player>();
        }

        [ServerCallback]
        void FixedUpdate()
        {
            CheckAboveHead();
        }

        [Server]
        void CheckAboveHead()
        {
            RaycastHit2D leftCheck = Physics2D.Raycast(HeadLeft.position, Vector2.up, CheckDistance, PlayerLayer);
            RaycastHit2D rightCheck = Physics2D.Raycast(HeadRight.position, Vector2.up, CheckDistance, PlayerLayer);

            RaycastHit2D contact = leftCheck;
            if (!contact)
                contact = rightCheck;

            if (contact && CanBeGivenToOther(contact.collider.gameObject))
            {
                ClaimReward(contact.collider.gameObject);
            }
        }

        public override bool CanBeGivenToOther(GameObject other)
        {
            return base.CanBeGivenToOther(other) && other != gameObject;
        }

        [Server]
        public override void ClaimReward(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().Jump();
            base.ClaimReward(player);
        }

        [Server]
        public override void Destroy()
        {
            _player.Destroy();
        }
    }
}
