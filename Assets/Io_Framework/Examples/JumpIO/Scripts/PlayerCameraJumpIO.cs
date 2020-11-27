using Cinemachine;
using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.JumpIO
{
    public class PlayerCameraJumpIO : NetworkBehaviour
    {
        private CinemachineVirtualCamera _cinemachineCamera;

        public override void OnStartLocalPlayer()
        {
            CinemachineBrain cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (cameraBrain == null)
            {
                Debug.LogError("CinemachineBrain missing from main camera");
                return;
            }
            _cinemachineCamera = cameraBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            _cinemachineCamera.Follow = transform;
        }

        [ClientCallback]
        void OnDisable()
        {
            if (_cinemachineCamera == null || _cinemachineCamera.Follow != transform) return;

            _cinemachineCamera.Follow = null;
        }

    }
}
