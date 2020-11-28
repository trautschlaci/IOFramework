using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Kiwi : JumpIOPowerUpBase
    {
        public float JumpForceModifier = 4f;

        [Server]
        protected override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpForce += JumpForceModifier;
        }

        [Server]
        protected override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpForce -= JumpForceModifier;
        }
    }
}
