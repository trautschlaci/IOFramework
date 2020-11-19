using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class PlayerCameraJumpIO : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    public override void OnStartLocalPlayer()
    {
        CinemachineBrain cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (cameraBrain == null)
        {
            Debug.LogError("CinemachineBrain missing from main camera");
            return;
        }
        cinemachineCamera = cameraBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        cinemachineCamera.Follow = transform;
    }

    [ClientCallback]
    void OnDisable()
    {
        if (cinemachineCamera == null || cinemachineCamera.Follow != transform) return;

        cinemachineCamera.Follow = null;
    }

}
