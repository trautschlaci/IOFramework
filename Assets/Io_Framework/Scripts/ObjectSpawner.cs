using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mirror;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public List<Spawnable> SpawnPrefabs = new List<Spawnable>();

    public override void OnStartServer()
    {
        foreach (var spawnObject in SpawnPrefabs)
        {
            StartCoroutine(spawnObject.SpawnRepeat());
        }
    }
}
