using Mirror;
using TMPro;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class AgarGrowingPlayer : GrowingPlayer
    {
        public TextMeshPro NameText;


        private SpriteRenderer _sprite;


        [Server]
        protected override float CalculateSizeFromScore(int score)
        {
            return Mathf.Sqrt(1.0f + score / (2.0f * Mathf.PI));
        }



        public float GetCameraScale()
        {
            return CalculateCameraScale(OwnTransform.localScale.x);
        }

        protected override float CalculateCameraScale(float size)
        {
            return 3 + size;
        }

        protected override void Awake()
        {
            base.Awake();
            _sprite = GetComponent<SpriteRenderer>();
        }

        [ClientCallback]
        private void Update()
        {
            _sprite.sortingOrder = Score.Score;
            NameText.sortingOrder = Score.Score;


            if (Camera.main == null || Camera.main.transform.parent != OwnTransform)
                return;

            Camera.main.orthographicSize = CalculateCameraScale(OwnTransform.localScale.x);
        }
    }
}
