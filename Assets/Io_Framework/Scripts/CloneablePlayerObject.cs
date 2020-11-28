using Mirror;
using UnityEngine;

namespace Io_Framework
{
    public abstract class CloneablePlayerObject : Player
    {
        public int MaxNumberOfClones = 20;


        public override void OnStartServer()
        {
            base.OnStartServer();
            PlayerObjectManager.Singleton.AddPlayerObject(connectionToClient.connectionId, this);
        }

        [Server]
        public override void Destroy()
        {
            var wasLast = PlayerObjectManager.Singleton.DeleteGameObject(connectionToClient.connectionId, this);
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
            return PlayerObjectManager.Singleton.GetNumberOfPlayerObjects(connectionToClient.connectionId) < MaxNumberOfClones;
        }

        [Server]
        protected virtual void OnLastPlayerObjectDestroyed()
        {
            LeaderBoard.ServerSingleton.RemovePlayer(connectionToClient.connectionId);
        }

        [Server]
        public virtual int CompareTo(CloneablePlayerObject other)
        {
            return 0;
        }

    }
}
