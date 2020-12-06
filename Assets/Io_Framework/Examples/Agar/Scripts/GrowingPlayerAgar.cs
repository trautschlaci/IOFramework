using Mirror;
using TMPro;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class GrowingPlayerAgar : GrowingPlayerBase
    {

        #region Public fields Client

        public TextMeshPro NameText;

        #endregion



        #region Client

        private SpriteRenderer _sprite;


        [ClientCallback]
        private void Update()
        {
            _sprite.sortingOrder = Score.Score;
            NameText.sortingOrder = Score.Score;


            if (Camera.main == null || Camera.main.transform.parent != OwnTransform)
                return;

            Camera.main.orthographicSize = CalculateCameraSize(OwnTransform.localScale.x);
        }

        #endregion



        #region Client and Server

        public float GetCameraSize()
        {
            return CalculateCameraSize(OwnTransform.localScale.x);
        }


        protected override void Awake()
        {
            base.Awake();
            _sprite = GetComponent<SpriteRenderer>();
        }


        protected override float CalculateCameraSize(float ownSize)
        {
            return 3 + ownSize;
        }

        #endregion



        #region Server

        [Server]
        protected override float CalculateSizeFromScore(int score)
        {
            return Mathf.Sqrt(1.0f + score / (2.0f * Mathf.PI));
        }

        #endregion

    }
}
