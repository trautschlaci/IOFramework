using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mirror;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public bool AutoStartSpawning = true;
    public List<Spawnable> SpawnPrefabs = new List<Spawnable>();

    public override void OnStartServer()
    {
        if (!AutoStartSpawning) return;

        StartSpawning();
    }

    public void StartSpawning()
    {
        foreach (var spawnObject in SpawnPrefabs)
        {
            StartCoroutine(spawnObject.SpawnRepeat());
        }
    }
}
