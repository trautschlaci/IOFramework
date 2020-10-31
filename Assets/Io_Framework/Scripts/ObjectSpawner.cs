using System.Collections;
using Mirror;
using UnityEngine;


public class ObjectSpawner: NetworkBehaviour
{
    public GameObject SpawnedObject;
    public SpawnPointSelector SpawnPointSelector;
    public bool AutoStartSpawning = true;
    public float MinSpawnDelay = 0.1f;
    public float MaxSpawnDelay = 0.1f;
    [Range(0.0f, 1.0f)]
    public float ChanceOfSpawning = 1.0f;

    protected bool IsStopped;


    public override void OnStartServer()
    {
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
            Spawn();
            yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
        }
    }

    [Server]
    public virtual void Spawn()
    {
        var randomFloat = Random.Range(0.0f, 1.0f);
        if (randomFloat > ChanceOfSpawning)
            return;

        var target = SpawnPointSelector.SelectSpawnPoint();

        var spawnGameObject = Instantiate(SpawnedObject, target, Quaternion.identity);
        NetworkServer.Spawn(spawnGameObject);
    }
}