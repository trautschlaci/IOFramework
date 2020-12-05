using Mirror;

namespace Io_Framework
{
    // Base class of player-object following camera control.
    public abstract class PlayerCameraBase : NetworkBehaviour
    {

        #region Client

        public override void OnStartLocalPlayer()
        {
            FollowWithCamera();
        }

        private void OnDisable()
        {
            if (!IsCameraFollowing()) return;

            StopFollowing();
        }


        protected abstract void FollowWithCamera();

        protected abstract bool IsCameraFollowing();

        protected abstract void StopFollowing();


        #endregion

    }
}
