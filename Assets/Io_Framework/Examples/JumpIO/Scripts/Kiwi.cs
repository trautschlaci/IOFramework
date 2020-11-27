using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Kiwi : JumpIOPowerUpBase
    {
        public float JumpForceModifier = 4f;

        [Server]
        public override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpForce += JumpForceModifier;
        }

        [Server]
        public override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpForce -= JumpForceModifier;
        }
    }
}
