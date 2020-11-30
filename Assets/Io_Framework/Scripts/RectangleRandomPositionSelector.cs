using UnityEngine;

namespace Io_Framework
{
    public class RectangleRandomPositionSelector : RandomPositionSelectorBase
    {
        public RectTransform AreaToChooseFrom;


        private Vector3[] _corners;


        public override Vector3 RandomPosition()
        {
            var x = Random.Range(_corners[0].x, _corners[2].x);
            var y = Random.Range(_corners[0].y, _corners[2].y);

            var target = new Vector3(x, y, 0);

            return target;
        }

        private void Start()
        {
            _corners = new Vector3[4];
            AreaToChooseFrom.GetWorldCorners(_corners);
        }
    }
}
