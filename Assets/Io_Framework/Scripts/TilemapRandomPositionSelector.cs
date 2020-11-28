using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Io_Framework
{
    public class TilemapRandomPositionSelector : RandomPositionSelector
    {
        public Tilemap TilemapToUse;

        private List<Vector3> _emptyPositions;
        private BoundsInt _tilemapBounds;

        private void Start()
        {
            _emptyPositions = new List<Vector3>();
            _tilemapBounds = TilemapToUse.cellBounds;
            var tileArray = TilemapToUse.GetTilesBlock(_tilemapBounds);
            for (var index = 0; index < tileArray.Length; index++)
            {
                if (tileArray[index] != null) 
                    continue;

                var coords = TilemapToUse.GetCellCenterWorld(ArrayIndexToCellPosition(index));
                _emptyPositions.Add(coords);
            }
        }

        public override Vector3 RandomPosition()
        {
            return _emptyPositions[Random.Range(0, _emptyPositions.Count)];
        }


        private Vector3Int ArrayIndexToCellPosition(int index)
        {
            var x = index % _tilemapBounds.size.x + _tilemapBounds.x;
            var y = index / _tilemapBounds.size.x + _tilemapBounds.y;

            return new Vector3Int(x, y, 0);
        }
    }
}
