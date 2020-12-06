using Mirror;
using UnityEngine;

namespace Io_Framework
{
    // Abstract base class to use on player-objects if one player can have multiple objects.
    public abstract class CloneablePlayerObjectBase : Player
    {

        #region Server

        [Tooltip("Maximum how many player-object can a player have.")]
        public int MaxNumberOfClones = 20;


        [Server]
        public override void Destroy()
        {
            PlayerObjectManager.Singleton.DeletePlayerObject(connectionToClient.connectionId, this, out var wasLast);
            if(wasLast)
                OnLastPlayerObjectDestroyed();

            base.Destroy();
        }

        [Server]
        public virtual GameObject SpawnClone(Vector3 targetPos, Quaternion rotation)
        {
            var clone = PlayerObjectManager.Singleton.InstantiatePlayerObject(targetPos, rotation);
            clone.GetComponent<Player>().PlayerName = PlayerName;
            NetworkServer.Spawn(clone, connectionToClient);
            return clone;
        }

        [Server]
        public virtual bool CanCreateClone()
        {
            // The player can only create more clones if it has less player-objects than the specified max number.
            return PlayerObjectManager.Singleton.GetNumberOfPlayerObjects(connectionToClient.connectionId) < MaxNumberOfClones;
        }

        [Server]
        public virtual int CompareTo(CloneablePlayerObjectBase other)
        {
            return 0;
        }


        public override void OnStartServer()
        {
            PlayerObjectManager.Singleton.AddPlayerObjectToList(connectionToClient.connectionId, this);
        }


        protected abstract void OnLastPlayerObjectDestroyed();

        #endregion

    }
}
