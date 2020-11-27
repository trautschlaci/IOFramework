using UnityEngine;

namespace Io_Framework
{
    public class SpawnPositionSelector : MonoBehaviour
    {
        public RandomPositionSelector PositionSelector;
        public float SafeRadius = 1.0f;
        public LayerMask CheckLayers;

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
    }
}
