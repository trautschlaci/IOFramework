using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Banana : JumpIOPowerUpBase
    {
        public float JumpHoldDurationModifier = 1f;

        [Server]
        public override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration += JumpHoldDurationModifier;
        }

        [Server]
        public override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration -= JumpHoldDurationModifier;
        }
    }
}
