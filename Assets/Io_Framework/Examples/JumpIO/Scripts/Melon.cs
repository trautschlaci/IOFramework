using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Melon : JumpIOPowerUpBase
    {
        public float SpeedModifier = 2.0f;
        public float MaxSpeed = 10.0f;

        [Server]
        public override bool CanAffectPlayerServer(GameObject player)
        {
            return player.GetComponent<PlayerControllerJumpIO>().RunSpeed + SpeedModifier <= MaxSpeed;
        }

        [Server]
        public override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().RunSpeed += SpeedModifier;
        }

        [Server]
        public override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().RunSpeed -= SpeedModifier;
        }
    }
}
