using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRandomPositionSelector : RandomPositionSelector
{
    public Tilemap TilemapToUse;

    private List<Vector3> emptyPositions;
    private BoundsInt tilemapBounds;

    void Start()
    {
        emptyPositions = new List<Vector3>();
        tilemapBounds = TilemapToUse.cellBounds;
        TileBase[] tileArray = TilemapToUse.GetTilesBlock(tilemapBounds);
        for (var index = 0; index < tileArray.Length; index++)
        {
            if (tileArray[index] == null)
            {
                var coords = TilemapToUse.GetCellCenterWorld(ArrayIndexToCellPosition(index));
                emptyPositions.Add(coords);
            }
        }
    }

    public override Vector3 RandomPosition()
    {
        return emptyPositions[Random.Range(0, emptyPositions.Count)];
    }


    private Vector3Int ArrayIndexToCellPosition(int index)
    {
        int x = index % tilemapBounds.size.x + tilemapBounds.x;
        int y = index / tilemapBounds.size.x + tilemapBounds.y;

        return new Vector3Int(x, y, 0);
    }
}
