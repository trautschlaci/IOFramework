using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSpawnPointSelector : SpawnPointSelector
{
    public RectTransform SpawnArea;

    public override Vector3 SelectSpawnPoint()
    {
        float x = Random.Range(SpawnArea.rect.xMin * SpawnArea.localScale.x, SpawnArea.rect.xMax * SpawnArea.localScale.x);
        float y = Random.Range(SpawnArea.rect.yMin * SpawnArea.localScale.y, SpawnArea.rect.yMax * SpawnArea.localScale.y);

        Vector3 target = new Vector3(x, y, 0);

        return target;
    }
}
