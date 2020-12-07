using Cinemachine;
using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class PlayerCameraJumpIO : PlayerCameraBase
    {

        #region Client

        private CinemachineVirtualCamera _cinemachineCamera;


        [Client]
        protected override void FollowWithCamera()
        {
            var cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (cameraBrain == null)
            {
                Debug.LogError("CinemachineBrain missing from main camera");
                return;
            }
            _cinemachineCamera = cameraBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            _cinemachineCamera.Follow = transform;
        }

        protected override bool IsCameraFollowing()
        {
            return _cinemachineCamera != null && _cinemachineCamera.Follow == transform;
        }

        protected override void StopFollowing()
        {
            _cinemachineCamera.Follow = null;
        }

        #endregion

    }
}
