using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Selects a random position for spawning and checks if the spawned object would collide with other objects.
    public class SpawnPositionSelector : NetworkBehaviour
    {
        #region Server

        [Tooltip("The random position selector.")]
        public RandomPositionSelectorBase PositionSelector;

        [Tooltip("How far from the selected random position should it check for other objects.")]
        public float SafeRadius = 1.0f;

        [Tooltip("What layers should be checked for collision detection with other objects.")]
        public LayerMask CheckLayers;


        // Returns true if the spawn position could be used and it would not cause collision.
        public bool SelectSpawnPosition(out Vector3 selectedPosition)
        {
            var doesNotCollide = true;
            selectedPosition = PositionSelector.RandomPosition();

            if (Physics2D.OverlapCircle(selectedPosition, SafeRadius, CheckLayers) != null)
            {
                doesNotCollide = false;
            }

            return doesNotCollide;
        }

        #endregion

    }
}
