﻿using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public abstract class PowerUpJumpIOBase: PowerUpBase
    {

        #region Server

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsAvailable) return;

            if (other.CompareTag("Player"))
            {
                PickUp(other.gameObject);
            }
        }

        #endregion

    }
}
