using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCamera2D : NetworkBehaviour
{
    public Transform LocalCameraTransform;

    private static Transform _cameraStartTransform;
    private static float _startOrthographicSize;

    public override void OnStartLocalPlayer()
    {
        if (_cameraStartTransform == null)
        {
            _cameraStartTransform = Camera.main.transform;
            _startOrthographicSize = Camera.main.orthographicSize;
        }

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = LocalCameraTransform.localPosition;
        Camera.main.transform.localRotation = LocalCameraTransform.localRotation;
    }

    [ClientCallback]
    void OnDisable()
    {
        if (Camera.main == null || Camera.main.transform.parent != transform) return;

        Camera.main.transform.SetParent(null);
        Camera.main.transform.localPosition = _cameraStartTransform.localPosition;
        Camera.main.transform.localRotation = _cameraStartTransform.localRotation;
        Camera.main.orthographicSize = _startOrthographicSize;
    }

}
