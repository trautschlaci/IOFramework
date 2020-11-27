using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Io_Framework
{
    public class ObjectSpawner: NetworkBehaviour
    {
        public List<WeightedGameObject> SelectableObjectsToSpawn;
        public RandomPositionSelector RandomPositionSelector;
        public bool AutoStartSpawning = true;
        public float MinSpawnDelay = 0.1f;
        public float MaxSpawnDelay = 0.1f;
        public int BulkSpawn = 1;
        public bool IsStopped;


        [Server]
        public override void OnStartServer()
        {
            if (SelectableObjectsToSpawn.Any(spawnObject => spawnObject.GameObject != null && !NetworkManager.singleton.spawnPrefabs.Contains(spawnObject.GameObject)))
            {
                Debug.LogError("Prefabs to spawn should also be added to the list of the NetworkManager");
                return;
            }

            if (AutoStartSpawning)
                StartSpawning();
        }

        [Server]
        public override void OnStopServer()
        {
            IsStopped = true;
        }

        [Server]
        public void StartSpawning()
        {
            IsStopped = false;
            StartCoroutine(ExecuteSpawning());
        }

        [Server]
        private IEnumerator ExecuteSpawning()
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
            var weightSum = 0;
            foreach (var weightedGo in SelectableObjectsToSpawn) weightSum += weightedGo.Weight;


            var randomWeight = Random.Range(0, weightSum);
            foreach (var weightedGo in SelectableObjectsToSpawn)
            {
                randomWeight -= weightedGo.Weight;
                if (randomWeight < 0)
                {
                    return weightedGo.GameObject;
                }
            }

            return null;
        }

        [Server]
        public virtual void Spawn()
        {
            var target = RandomPositionSelector.RandomPosition();
            var selectedGo = SelectObjectToSpawn();

            if (selectedGo != null)
            {
                var spawnGameObject = Instantiate(selectedGo, target, Quaternion.identity);
                NetworkServer.Spawn(spawnGameObject);
            }
        }
    }


    [Serializable]
    public struct WeightedGameObject
    {
        public GameObject GameObject;
        [DefaultValue(1)]
        public int Weight;
    }
}
