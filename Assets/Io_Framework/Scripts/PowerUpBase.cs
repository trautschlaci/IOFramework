using System.Collections;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public abstract class PowerUpBase : NetworkBehaviour
    {

        #region Public fields Server

        public bool IsAvailable = true;
        public float Duration = 30.0f;

        #endregion



        #region Public fields Client

        public GameObject CollectedEffect;

        #endregion



        #region Client and Server

        [SyncVar]
        private bool _isHidden;


        protected virtual void Update()
        {
            if (!_isHidden)
                return;


            if (isClient)
                HideObject();
            else
                HideObject();
        }

        protected virtual void HideObject()
        {
            foreach (var c in GetComponentsInChildren<Collider2D>()) { c.enabled = false; }
            foreach (var c in GetComponentsInChildren<Collider>()) { c.enabled = false; }
            foreach (var r in GetComponentsInChildren<Renderer>()) { r.enabled = false; }
        }


        [TargetRpc]
        protected virtual void TargetDisplayCollect(NetworkConnection conn)
        {
            Instantiate(CollectedEffect, transform.position, transform.rotation);
        }


        protected abstract void ApplyEffect(GameObject player);

        protected abstract void RevertEffect(GameObject player);

        #endregion



        #region Server

        [Server]
        public void PickUp(GameObject player)
        {
            if (CanBeGivenToPlayerServer(player))
                StartCoroutine(PickUpCoroutine(player));
        }


        [Server]
        public virtual bool CanBeGivenToPlayerServer(GameObject player)
        {
            return IsAvailable;
        }

        [Server]
        public virtual bool CanAffectPlayerServer(GameObject player)
        {
            return true;
        }


        [Server]
        private IEnumerator PickUpCoroutine(GameObject player)
        {
            IsAvailable = false;
            _isHidden = true;

            TargetDisplayCollect(player.GetComponent<NetworkBehaviour>().connectionToClient);

            if (CanAffectPlayerServer(player))
            {
                ApplyEffect(player);

                yield return new WaitForSeconds(Duration);

                if (player != null)
                    RevertEffect(player);
            }

            NetworkServer.Destroy(gameObject);
        }

        #endregion

    }
}
