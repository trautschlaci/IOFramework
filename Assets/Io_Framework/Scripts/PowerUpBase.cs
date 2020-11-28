using System.Collections;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public abstract class PowerUpBase : NetworkBehaviour
    {
        public bool IsAvailable = true;
        public float Duration = 30.0f;
        public GameObject CollectedEffect;

        [SyncVar]
        private bool _isHidden;

        [Server]
        protected void PickUp(GameObject player)
        {
            if(CanBeGivenToPlayerServer(player))
                StartCoroutine(PickUpCoroutine(player));
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


        protected virtual void Update()
        {
            if (!_isHidden)
                return;


            if (isClient)
                HideClient();
            else
                HideServer();
        }



        [TargetRpc]
        protected virtual void TargetDisplayCollect(NetworkConnection conn)
        {
            Instantiate(CollectedEffect, transform.position, transform.rotation);
        }



        protected abstract void HideClient();

        protected abstract void HideServer();

        protected abstract void ApplyEffect(GameObject player);

        protected abstract void RevertEffect(GameObject player);
    }
}
