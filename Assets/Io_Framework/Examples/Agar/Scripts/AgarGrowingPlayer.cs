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
            return CalculateCameraScale(transform.localScale.x);
        }

        protected override float CalculateCameraScale(float size)
        {
            return 3 + size;
        }

        protected override void Start()
        {
            base.Start();


            if (!isClient)
                return;

            _sprite = GetComponent<SpriteRenderer>();
        }



        [ClientCallback]
        private void Update()
        {
            _sprite.sortingOrder = Score.Score;
            NameText.sortingOrder = Score.Score;


            if (Camera.main == null || Camera.main.transform.parent != transform)
                return;

            Camera.main.orthographicSize = CalculateCameraScale(transform.localScale.x);
        }
    }
}
