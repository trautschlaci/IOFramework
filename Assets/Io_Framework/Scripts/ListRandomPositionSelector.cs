using System.Collections.Generic;
using UnityEngine;

namespace Io_Framework
{
    public class ListRandomPositionSelector : RandomPositionSelectorBase
    {
        public List<Transform> PointsToChooseFrom;


        public override Vector3 RandomPosition()
        {
            return PointsToChooseFrom[Random.Range(0, PointsToChooseFrom.Count)].position;
        }
    }
}
