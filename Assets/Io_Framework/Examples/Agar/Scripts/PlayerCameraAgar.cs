using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class PlayerCameraAgar : PlayerCameraBase
    {

        #region Client

        public Transform LocalCameraTransform;


        [Client]
        protected override void FollowWithCamera()
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = LocalCameraTransform.localPosition;
            Camera.main.transform.localRotation = LocalCameraTransform.localRotation;
            Camera.main.orthographicSize = GetComponent<GrowingPlayerAgar>().GetCameraSize();
        }

        [Client]
        protected override bool IsCameraFollowing()
        {
            return Camera.main != null && Camera.main.transform.parent == transform;
        }

        [Client]
        protected override void StopFollowing()
        {
            Camera.main.transform.SetParent(null);
        }

        #endregion

    }
}
