using UnityEngine;

namespace Io_Framework
{
    public class RectangleRandomPositionSelector : RandomPositionSelector
    {
        public RectTransform SpawnArea;


        private Vector3[] _corners;


        private void Start()
        {
            _corners = new Vector3[4];
            SpawnArea.GetWorldCorners(_corners);
        }

        public override Vector3 RandomPosition()
        {
            var x = Random.Range(_corners[0].x, _corners[2].x);
            var y = Random.Range(_corners[0].y, _corners[2].y);

            var target = new Vector3(x, y, 0);

            return target;
        }
    }
}
