using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public abstract class JumpIOPowerUpBase: PowerUpBase
    {
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsAvailable) return;

            if (other.CompareTag("Player"))
            {
                PickUp(other.gameObject);
            }
        }

        [Client]
        protected override void HideClient()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }

        [Server]
        protected override void HideServer()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
