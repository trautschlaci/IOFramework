using System;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public class Player : NetworkBehaviour
    {
        [SyncVar] 
        public string PlayerName;

        public event Action OnPlayerDestroyedServer;

        // Server
        public float ViewRange = 10.0f;
        public float DestroyDelay = 0.1f;

        [Server]
        public virtual void Destroy()
        {
            HideObject();
            RpcDisplayDestroy();
            OnPlayerDestroyedServer?.Invoke();
            Invoke(nameof(ExecuteDestroy), DestroyDelay);
        }

        [Server]
        private void ExecuteDestroy()
        {
            NetworkServer.Destroy(gameObject);
        }



        protected virtual void HideObject()
        {
            foreach (var c in GetComponentsInChildren<Collider2D>()) { c.enabled = false; }
            foreach (var c in GetComponentsInChildren<Collider>()) { c.enabled = false; }
            foreach (var r in GetComponentsInChildren<Renderer>()) { r.enabled = false; }
        }



        [ClientRpc]
        protected virtual void RpcDisplayDestroy()
        {
            HideObject();
        }
    }
}
