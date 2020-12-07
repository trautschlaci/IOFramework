using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // If CloneablePlayerObjects are used, an object is needed with this class to control the multiple player-objects.
    // The PlayerObjectManager is a singleton, so you can only use one, but accessing it is simple.
    public class PlayerObjectManager: NetworkBehaviour
    {

        #region Server

        [Tooltip("Prefab of the player-object. It must have component derived from CloneablePlayerObjectBase.")]
        public GameObject PlayerPrefab;


        public static PlayerObjectManager Singleton { get; private set; }


        private readonly Dictionary<int, List<CloneablePlayerObjectBase>> _objectsOfPlayers = new Dictionary<int, List<CloneablePlayerObjectBase>>();


        [Server]
        public void AddPlayerObjectToList(int playerId, CloneablePlayerObjectBase playerObject)
        {
            if(!_objectsOfPlayers.ContainsKey(playerId))
                _objectsOfPlayers.Add(playerId, new List<CloneablePlayerObjectBase>());
            _objectsOfPlayers[playerId].Add(playerObject);
        }

        [ServerCallback]
        public void RemovePlayerObjectFromList(int playerId, CloneablePlayerObjectBase playerObject)
        {
            if (!_objectsOfPlayers.ContainsKey(playerId))
                return;

            _objectsOfPlayers[playerId].Remove(playerObject);
            if (_objectsOfPlayers[playerId].Count < 1)
            {
                _objectsOfPlayers.Remove(playerId);
            }
        }

        [Server]
        public GameObject InstantiatePlayerObject(Vector3 targetPos, Quaternion rotation)
        {
            var spawnedPlayerObject = Instantiate(PlayerPrefab, targetPos, rotation);
            return spawnedPlayerObject;
        }


        [Server]
        public void DeletePlayerObject(int playerId, CloneablePlayerObjectBase playerObject, out bool wasLastPlayerObject)
        {
            wasLastPlayerObject = false;
            if (IsMainPlayerObject(playerObject))
                wasLastPlayerObject = !ChangeMainPlayerObject(playerId);

            RemovePlayerObjectFromList(playerId, playerObject);
        }

        // Returns true if it could change the main player-object.
        [Server]
        public bool ChangeMainPlayerObject(int playerId)
        {
            CloneablePlayerObjectBase nextMainPlayer = GetNextMainPlayerObject(playerId);

            if (nextMainPlayer != null)
            {
                NetworkServer.ReplacePlayerForConnection(nextMainPlayer.connectionToClient, nextMainPlayer.gameObject, true);
                return true;
            }

            return false;
        }

        [Server]
        public List<CloneablePlayerObjectBase> GetPlayerObjects(int playerId)
        {
            if (!_objectsOfPlayers.ContainsKey(playerId)) return null;

            return _objectsOfPlayers[playerId];

        }

        [Server]
        public int GetNumberOfPlayerObjects(int playerId)
        {
            return GetPlayerObjects(playerId).Count;
        }


        private void OnValidate()
        {
            if (PlayerPrefab.GetComponent<CloneablePlayerObjectBase>() == null)
            {
                Debug.LogError("PlayerObjectManager: PlayerPrefab must have CloneablePlayerObjectBase script");
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
        private bool IsMainPlayerObject(CloneablePlayerObjectBase playerObject)
        {
            return playerObject.connectionToClient.identity == playerObject.netIdentity;
        }

        [Server]
        private CloneablePlayerObjectBase GetNextMainPlayerObject(int playerId)
        {
            CloneablePlayerObjectBase nextMainPlayerObject = null;
            foreach (var playerObject in _objectsOfPlayers[playerId])
            {
                if (!IsMainPlayerObject(playerObject) && (nextMainPlayerObject == null || playerObject.CompareTo(nextMainPlayerObject) > 0))
                {
                    nextMainPlayerObject = playerObject;
                }
            }

            return nextMainPlayerObject;
        }

        #endregion

    }
}
