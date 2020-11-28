using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class PlayerCameraAgar : NetworkBehaviour
    {
        public Transform LocalCameraTransform;

        public override void OnStartLocalPlayer()
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = LocalCameraTransform.localPosition;
            Camera.main.transform.localRotation = LocalCameraTransform.localRotation;
            Camera.main.orthographicSize = GetComponent<AgarGrowingPlayer>().GetCameraScale();
        }

        [ClientCallback]
        private void OnDisable()
        {
            if (Camera.main == null || Camera.main.transform.parent != transform) return;

            Camera.main.transform.SetParent(null);
        }

    }
}
