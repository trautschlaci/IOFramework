using System.Collections.Generic;
using UnityEngine;

public class ListSpawnPointSelector : SpawnPointSelector
{
    public List<Transform> SpawnPoints;

    public override Vector3 SelectSpawnPoint()
    {
        return SpawnPoints[Random.Range(0, SpawnPoints.Count)].position;
    }
}
