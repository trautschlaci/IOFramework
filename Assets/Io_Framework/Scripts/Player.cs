using System;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public class Player : NetworkBehaviour
    {

        #region Public fields Client and Server

        [SyncVar]
        public string PlayerName;

        #endregion



        #region Public fields Server

        [Tooltip("Objects with ReverseProximityChecker will only appear for the player if their distance from the player is less than this value.")]
        public float ViewRange = 10.0f;

        [Tooltip("How long should be waited before actually destroying the object. This is to ensure that all methods get executed in the client.")]
        public float DestroyDelay = 0.1f;

        #endregion



        #region Client and Server

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

        #endregion



        #region Server

        public event Action OnPlayerDestroyedServer;


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

        #endregion

    }
}
