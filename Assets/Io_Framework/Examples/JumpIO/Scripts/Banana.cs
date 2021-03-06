﻿using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Banana : PowerUpJumpIOBase
    {

        #region Server

        public float JumpHoldDurationModifier = 0.5f;


        [Server]
        protected override void ApplyEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration += JumpHoldDurationModifier;
        }

        [Server]
        protected override void RevertEffect(GameObject player)
        {
            player.GetComponent<PlayerControllerJumpIO>().JumpHoldDuration -= JumpHoldDurationModifier;
        }

        #endregion

    }
}
