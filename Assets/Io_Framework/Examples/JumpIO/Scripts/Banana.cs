﻿using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class Banana : JumpIOPowerUpBase
    {
        public float JumpHoldDurationModifier = 1f;

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
    }
}
