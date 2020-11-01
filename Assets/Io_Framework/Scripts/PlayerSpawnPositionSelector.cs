using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPositionSelector : MonoBehaviour
{
    public RandomPositionSelector PositionSelector;
    public float SafeRadius = 1.0f;

    public bool SelectSpawnPosition(out Vector3 spawnPosition)
    {
        var position = PositionSelector.RandomPosition();

        LayerMask mask = LayerMask.GetMask("Player");
        if (Physics2D.OverlapCircle(position, SafeRadius, mask) != null)
        {
            spawnPosition = position;
            return false;
        }

        spawnPosition = position;
        return true;
    }
}
