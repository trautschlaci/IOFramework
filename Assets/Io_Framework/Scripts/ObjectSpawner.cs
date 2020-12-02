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

        #region Server

        [Tooltip("List of objects to randomly choose from for spawning. Set the weight on each object to change it's probability of spawning")]
        public List<WeightedGameObject> SelectableObjectsToSpawn;

        [Tooltip("The object that selects positions randomly.")]
        public SpawnPositionSelector SpawnPointSelector;

        [Tooltip("Should the spawning start immediately?")]
        public bool AutoStartSpawning = true;

        [Tooltip("The minimum delay between two spawning.")]
        public float MinSpawnDelay = 0.1f;

        [Tooltip("The maximum delay between two spawning.")]
        public float MaxSpawnDelay = 0.1f;

        [Tooltip("How many objects should be spawned at the same time.")]
        public int BulkSpawn = 1;


        private bool _isStopped;


        [Server]
        public void StartSpawning()
        {
            _isStopped = false;
            StartCoroutine(SpawnCoroutine());
        }

        [Server]
        public void StopSpawning()
        {
            _isStopped = true;
        }

        // Selects a position to spawn to and an object to spawn, then executes the spawning. 
        [Server]
        public virtual void SpawnOne()
        {
            var doesNotCollide = SpawnPointSelector.SelectSpawnPosition(out var targetPosition);


            // If the SpawnPointSelector selects a position that already has an object on, don't execute the spawning. 
            if (!doesNotCollide)
                return;


            var selectedGo = SelectObjectToSpawn();

            if (selectedGo != null)
            {
                var spawnGameObject = Instantiate(selectedGo, targetPosition, Quaternion.identity);
                NetworkServer.Spawn(spawnGameObject);
            }
        }

        public override void OnStartServer()
        {
            if (SelectableObjectsToSpawn.Any(spawnObject => spawnObject.GameObject != null 
                                                            && !NetworkManager.singleton.spawnPrefabs.Contains(spawnObject.GameObject)))
            {
                Debug.LogError("Prefabs to spawn should also be added to the list of the NetworkManager");
                return;
            }


            if (AutoStartSpawning)
                StartSpawning();
        }

        public override void OnStopServer()
        {
            _isStopped = true;
        }

        [Server]
        protected virtual GameObject SelectObjectToSpawn()
        {
            var weightSum = SelectableObjectsToSpawn.Sum(weightedGo => weightedGo.Weight);

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
        private IEnumerator SpawnCoroutine()
        {
            while (!_isStopped)
            {
                for(var i = 0; i < BulkSpawn; i++)
                    SpawnOne();
                yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
            }
        }

        #endregion

    }


    [Serializable]
    public struct WeightedGameObject
    {
        public GameObject GameObject;
        [DefaultValue(1)]
        public int Weight;
    }

}
