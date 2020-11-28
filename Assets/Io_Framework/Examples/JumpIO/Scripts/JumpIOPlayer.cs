using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class JumpIOPlayer : Player
    {
        public GameObject DeathEffect;

        [Server]
        public override void Destroy()
        {
            LeaderBoard.ServerSingleton.RemovePlayer(connectionToClient.connectionId);
            base.Destroy();
        }

        [ClientRpc]
        protected override void RpcDisplayDestroy()
        {
            base.RpcDisplayDestroy();

            Instantiate(DeathEffect, transform.position, transform.rotation);


            if (!hasAuthority)
                return;

            var networkManager = (IoNetworkManager) NetworkManager.singleton;
            networkManager.RestartPlayerClient();
        }
    }
}
