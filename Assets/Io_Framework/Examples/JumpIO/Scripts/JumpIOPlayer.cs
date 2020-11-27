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
            var leaderBoard = FindObjectOfType<LeaderBoard>();
            leaderBoard.RemovePlayer(connectionToClient.connectionId);
            base.Destroy();
        }

        [ClientRpc]
        public override void RpcDisplayDestroy()
        {
            base.RpcDisplayDestroy();

            Instantiate(DeathEffect, transform.position, transform.rotation);


            if (!hasAuthority)
                return;

            IoNetworkManager networkManager = (IoNetworkManager) NetworkManager.singleton;
            networkManager.RestartPlayerClient();
        }
    }
}
