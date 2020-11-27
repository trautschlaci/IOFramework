using TMPro;
using UnityEngine;

namespace Io_Framework
{
    public class PlayerNamePresenter : MonoBehaviour
    {
        public Player PlayerToPresent;
        public TextMeshPro NameText;

        private int _direction = 1;

        void Start()
        {
            if(PlayerToPresent == null)
                PlayerToPresent = GetComponent<Player>();
        }

        void Update()
        {
            NameText.text = PlayerToPresent.PlayerName;
            NameText.transform.rotation = Camera.main.transform.rotation;
            var scale = NameText.transform.localScale;
            if (PlayerToPresent.transform.localScale.x * _direction < 0)
            {
                _direction *= -1;
                scale.x *= -1;
            }
            NameText.transform.localScale = scale;
        }
    }
}
