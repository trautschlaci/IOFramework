using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Pineapple : PowerUpBaseJumpIO
    {
        [Server]
        protected override void ApplyEffect(GameObject player)
        {
            player.GetComponent<StompedPlayer>().IsAvailable = false;
        }

        [Server]
        protected override void RevertEffect(GameObject player)
        {
            player.GetComponent<StompedPlayer>().IsAvailable = true;
        }
    }
}
