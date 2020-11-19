using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionSelector : MonoBehaviour
{
    public RandomPositionSelector PositionSelector;
    public float SafeRadius = 1.0f;
    public LayerMask CheckLayers;

    public bool SelectSpawnPosition(out Vector3 spawnPosition)
    {
        var position = PositionSelector.RandomPosition();

        if (Physics2D.OverlapCircle(position, SafeRadius, CheckLayers) != null)
        {
            spawnPosition = position;
            return false;
        }

        spawnPosition = position;
        return true;
    }
}
