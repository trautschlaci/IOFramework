using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Pineapple : JumpIOPowerUpBase
    {
        [Server]
        public override void ApplyEffect(GameObject player)
        {
            player.GetComponent<StompPlayer>().IsAvailable = false;
        }

        [Server]
        public override void RevertEffect(GameObject player)
        {
            player.GetComponent<StompPlayer>().IsAvailable = true;
        }
    }
}
