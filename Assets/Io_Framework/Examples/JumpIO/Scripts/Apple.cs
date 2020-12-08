using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Apple : PowerUpJumpIOBase
    {

        #region Server

        public int ExtraJumpModifier = 1;


        [Server]
        protected override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().ExtraJumps += ExtraJumpModifier;
        }

        [Server]
        protected override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().ExtraJumps -= ExtraJumpModifier;
        }

        #endregion

    }
}
