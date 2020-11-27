using UnityEngine;

namespace Io_Framework
{
    public class SpawnPositionSelector : MonoBehaviour
    {
        public RandomPositionSelector PositionSelector;
        public float SafeRadius = 1.0f;
        public LayerMask CheckLayers;

        public Vector3 SelectSpawnPosition(out bool doesCollide)
        {
            doesCollide = false;
            var selectedPosition = PositionSelector.RandomPosition();

            if (Physics2D.OverlapCircle(selectedPosition, SafeRadius, CheckLayers) != null)
            {
                doesCollide = true;
            }

            return selectedPosition;
        }
    }
}
