using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public class PlayerObjectManager: NetworkBehaviour
    {
        public GameObject PlayerPrefab;
        public static PlayerObjectManager Singleton { get; private set; }


        private readonly Dictionary<int, List<CloneablePlayerObject>> _objectsOfPlayers = new Dictionary<int, List<CloneablePlayerObject>>();


        private void OnValidate()
        {
            if (PlayerPrefab.GetComponent<CloneablePlayerObject>() == null)
            {
                Debug.LogError("PlayerObjectManager: PlayerPrefab must have CloneablePlayerObject script");
            }
        }


        public override void OnStartServer()
        {
            InitializeSingleton();
        }

        [Server]
        private void InitializeSingleton()
        {
            if (Singleton != null)
            {
                if (Singleton == this) return;

                Debug.Log("Multiple PlayerObjectManagers detected in the scene. The duplicate will be destroyed.");
                Destroy(gameObject);

                return;
            }

            Singleton = this;
        }

        [Server]
        public void AddPlayerObject(int playerId, CloneablePlayerObject playerObject)
        {
            if(!_objectsOfPlayers.ContainsKey(playerId))
                _objectsOfPlayers.Add(playerId, new List<CloneablePlayerObject>());
            _objectsOfPlayers[playerId].Add(playerObject);
        }

        [Server]
        public CloneablePlayerObject GetNextMainPlayerObject(int playerId)
        {
            CloneablePlayerObject nextMainPlayerObject = null;
            foreach (var playerObject in _objectsOfPlayers[playerId])
            {
                if (!IsMainPlayerObject(playerObject) && (nextMainPlayerObject == null || playerObject.CompareTo(nextMainPlayerObject) > 0))
                {
                    nextMainPlayerObject = playerObject;
                }
            }

            return nextMainPlayerObject;
        }

        [Server]
        public void RemovePlayerObject(int playerId, CloneablePlayerObject playerObject)
        {
            _objectsOfPlayers[playerId].Remove(playerObject);
            if (_objectsOfPlayers[playerId].Count < 1)
            {
                _objectsOfPlayers.Remove(playerId);
            }
        }

        [Server]
        public GameObject InstantiatePlayerObject(Vector3 targetPos, Quaternion rotation)
        {
            GameObject spawnedPlayerObject = Instantiate(PlayerPrefab, targetPos, rotation);
            return spawnedPlayerObject;
        }

        // Returns true if it was the last object of the player else false.
        [Server]
        public bool DeleteGameObject(int playerId, CloneablePlayerObject playerObject)
        {
            bool isLastPlayerObject = false;
            if (IsMainPlayerObject(playerObject))
                isLastPlayerObject = !ChangeMainPlayerObject(playerId);

            RemovePlayerObject(playerId, playerObject);

            return isLastPlayerObject;
        }

        [Server]
        public bool IsMainPlayerObject(CloneablePlayerObject playerObject)
        {
            return playerObject.connectionToClient.identity == playerObject.netIdentity;
        }

        [Server]
        public bool ChangeMainPlayerObject(int playerId)
        {
            CloneablePlayerObject nextMainPlayer = GetNextMainPlayerObject(playerId);

            if (nextMainPlayer != null)
            {
                NetworkServer.ReplacePlayerForConnection(nextMainPlayer.connectionToClient, nextMainPlayer.gameObject, true);
                return true;
            }

            return false;
        }

        [Server]
        public int GetNumberOfPlayerObjects(int playerId)
        {
            return _objectsOfPlayers[playerId].Count;
        }

        [Server]
        public List<CloneablePlayerObject> GetPlayerObjects(int playerId)
        {
            if (!_objectsOfPlayers.ContainsKey(playerId)) return null;

            return _objectsOfPlayers[playerId];

        }
    }
}
