using UnityEngine;

namespace Io_Framework
{
    public class RectangleRandomPositionSelector : RandomPositionSelector
    {
        public RectTransform SpawnArea;

        public override Vector3 RandomPosition()
        {
            var corners = new Vector3[4];
            SpawnArea.GetWorldCorners(corners);
            var x = Random.Range(corners[0].x, corners[2].x);
            var y = Random.Range(corners[0].y, corners[2].y);

            var target = new Vector3(x, y, 0);

            return target;
        }
    }
}
