using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Melon : PowerUpJumpIOBase
    {

        #region Server

        public float SpeedModifier = 1.0f;
        public float MaxSpeed = 8.0f;


        [Server]
        public override bool CanAffectPlayerServer(GameObject player)
        {
            return player.GetComponent<PlayerControllerJumpIO>().RunSpeed + SpeedModifier <= MaxSpeed;
        }

        [Server]
        protected override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().RunSpeed += SpeedModifier;
        }

        [Server]
        protected override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().RunSpeed -= SpeedModifier;
        }

        #endregion

    }
}
