using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class RandomColor : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetColor))]
        public Color32 BodyColor = Color.black;


        private Material _cachedMaterial;


        public override void OnStartServer()
        {
            BodyColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

        [Client]
        private void SetColor(Color32 _, Color32 newColor)
        {
            if (_cachedMaterial == null) _cachedMaterial = GetComponentInChildren<Renderer>().material;
            _cachedMaterial.color = newColor;
        }

        private void OnDestroy()
        {
            Destroy(_cachedMaterial);
        }
    }
}
