using UnityEngine;

namespace Io_Framework
{
    // Base class for components that can select a random position.
    public abstract class RandomPositionSelectorBase: MonoBehaviour
    {
        public abstract Vector3 RandomPosition();
    }
}
