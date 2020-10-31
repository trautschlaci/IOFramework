using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ObjectSpawner : NetworkBehaviour
{
    public List<SpawnEntry> SpawnEntries;

    public override void OnStartServer()
    {
        StartSpawning();
    }

    [Server]
    public void StartSpawning()
    {
        foreach (var spawnEntry in SpawnEntries)
        {
            if(spawnEntry.AutoStartSpawning)
                StartCoroutine(spawnEntry.StartSpawning());
        }
    }
}

[System.Serializable]
public class SpawnEntry
{
    public GameObject SpawnedObject;
    public float SpawnDelay = 1.0f;
    public bool AutoStartSpawning = true;
    [Range(0.0f, 1.0f)]
    public float ChanceOfSpawning = 1.0f;
    public SpawnPointSelector SpawnPointSelector;

    private bool _stopSpawning = false;


    [Server]
    public IEnumerator StartSpawning()
    {
        while (!_stopSpawning)
        {
            Spawn();
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    [Server]
    private void Spawn()
    {
        float randomFloat = Random.Range(0.0f, 1.0f);
        if (randomFloat > ChanceOfSpawning)
            return;

        var target = SpawnPointSelector.SelectSpawnPoint();

        GameObject spawnGameObject = Object.Instantiate(SpawnedObject, target, Quaternion.identity);
        NetworkServer.Spawn(spawnGameObject);
    }
}