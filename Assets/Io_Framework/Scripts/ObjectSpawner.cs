﻿using System;
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

        public List<WeightedGameObject> SelectableObjectsToSpawn;
        public SpawnPositionSelector SpawnPointSelector;
        public bool AutoStartSpawning = true;
        public float MinSpawnDelay = 0.1f;
        public float MaxSpawnDelay = 0.1f;
        public int BulkSpawn = 1;
        public bool IsStopped;


        [Server]
        public void StartSpawning()
        {
            IsStopped = false;
            StartCoroutine(ExecuteSpawning());
        }

        [Server]
        public virtual void Spawn()
        {
            var doesNotCollide = SpawnPointSelector.SelectSpawnPosition(out var targetPosition);


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
            IsStopped = true;
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
        private IEnumerator ExecuteSpawning()
        {
            while (!IsStopped)
            {
                for(var i = 0; i < BulkSpawn; i++)
                    Spawn();
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
