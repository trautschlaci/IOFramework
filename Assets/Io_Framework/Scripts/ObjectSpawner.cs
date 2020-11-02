using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;


public class ObjectSpawner: NetworkBehaviour
{
    public List<GameObject> SelectableObjectsToSpawn;
    public RandomPositionSelector RandomPositionSelector;
    public bool AutoStartSpawning = true;
    public float MinSpawnDelay = 0.1f;
    public float MaxSpawnDelay = 0.1f;
    [Range(0.0f, 1.0f)]
    public float ChanceOfSpawning = 1.0f;
    public int BulkSpawn = 1;

    protected bool IsStopped;


    public override void OnStartServer()
    {
        if (SelectableObjectsToSpawn.Any(spawnObject => !NetworkManager.singleton.spawnPrefabs.Contains(spawnObject)))
        {
            Debug.LogError("Prefabs to spawn should also be added to the list of the NetworkManager");
            return;
        }

        if (AutoStartSpawning)
            StartSpawning();
    }

    [Server]
    public virtual void StartSpawning()
    {
        IsStopped = false;
        StartCoroutine(ExecuteSpawning());
    }

    [Server]
    public virtual void StopSpawning()
    {
        IsStopped = true;
    }


    [Server]
    protected virtual IEnumerator ExecuteSpawning()
    {
        while (!IsStopped)
        {
            for(var i = 0; i < BulkSpawn; i++)
                Spawn();
            yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
        }
    }

    [Server]
    protected virtual GameObject SelectObjectToSpawn()
    {
        return SelectableObjectsToSpawn[Random.Range(0, SelectableObjectsToSpawn.Count)];
    }

    [Server]
    public virtual void Spawn()
    {
        var randomFloat = Random.Range(0.0f, 1.0f);
        if (randomFloat > ChanceOfSpawning)
            return;

        var target = RandomPositionSelector.RandomPosition();

        var spawnGameObject = Instantiate(SelectObjectToSpawn(), target, Quaternion.identity);
        NetworkServer.Spawn(spawnGameObject);
    }
}