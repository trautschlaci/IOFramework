using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class PlayerJumpIO : Player
    {

        #region Public fields Client

        public GameObject DeathEffect;

        #endregion


        #region Client and Server

        [ClientRpc]
        protected override void RpcDisplayDestroy()
        {
            base.RpcDisplayDestroy();

            Instantiate(DeathEffect, transform.position, transform.rotation);


            if (!hasAuthority)
                return;

            var networkManager = (IONetworkManager)NetworkManager.singleton;
            networkManager.ShowRestartUI();
        }

        #endregion
    }
}
