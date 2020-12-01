using Mirror;
using TMPro;
using UnityEngine;

namespace Io_Framework
{
    public class PlayerNamePresenter : NetworkBehaviour
    {
        public Player PlayerToPresent;
        public TextMeshPro NameText;
        public Color32 HighlightColor;

        private int _direction = 1;


        [ClientCallback]
        private void Start()
        {
            if(PlayerToPresent == null)
                PlayerToPresent = GetComponent<Player>();
        }

        [ClientCallback]
        private void Update()
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

            if (hasAuthority)
                NameText.color = HighlightColor;

        }
    }
}
