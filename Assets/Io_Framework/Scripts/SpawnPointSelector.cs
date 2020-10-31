using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPointSelector: MonoBehaviour
{
    public abstract Vector3 SelectSpawnPoint();
}
