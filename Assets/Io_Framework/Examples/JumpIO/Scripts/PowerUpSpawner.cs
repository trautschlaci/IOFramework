using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class PowerUpSpawner : ObjectSpawner
    {
        public float CheckRadius = 0.1f;
        public LayerMask CheckLayers;

        public override void Spawn()
        {
            var target = RandomPositionSelector.RandomPosition();

            if (Physics2D.OverlapCircle(target, CheckRadius, CheckLayers) != null)
            {
                return;
            }

            var selectedGo = SelectObjectToSpawn();

            if (selectedGo != null)
            {
                var spawnGameObject = Instantiate(selectedGo, target, Quaternion.identity);
                NetworkServer.Spawn(spawnGameObject);
            }
        }
    }
}
