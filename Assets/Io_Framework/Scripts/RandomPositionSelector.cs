using UnityEngine;

namespace Io_Framework
{
    public abstract class RandomPositionSelector: MonoBehaviour
    {
        public abstract Vector3 RandomPosition();
    }
}
