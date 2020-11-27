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
        public override void HideClient()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }

        [Server]
        public override void HideServer()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
