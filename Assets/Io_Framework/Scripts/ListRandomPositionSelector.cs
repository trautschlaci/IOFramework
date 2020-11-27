using System.Collections.Generic;
using UnityEngine;

namespace Io_Framework
{
    public class ListRandomPositionSelector : RandomPositionSelector
    {
        public List<Transform> SpawnPoints;

        public override Vector3 RandomPosition()
        {
            return SpawnPoints[Random.Range(0, SpawnPoints.Count)].position;
        }
    }
}
