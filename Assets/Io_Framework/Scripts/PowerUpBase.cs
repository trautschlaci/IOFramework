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
        public void PickUp(GameObject player)
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

        public virtual void Update()
        {
            if (!_isHidden)
                return;


            if (isClient)
                HideClient();
            else
                HideServer();
        }

        [TargetRpc]
        public virtual void TargetDisplayCollect(NetworkConnection conn)
        {
            Instantiate(CollectedEffect, transform.position, transform.rotation);
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

        public abstract void HideClient();

        public abstract void HideServer();

        public abstract void ApplyEffect(GameObject player);

        public abstract void RevertEffect(GameObject player);
    }
}
