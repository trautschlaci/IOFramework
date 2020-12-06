using Mirror;
using UnityEngine;

namespace Io_Framework.Examples.Agar
{
    public class RandomColor : NetworkBehaviour
    {

        #region Public fields Client and Server

        [SyncVar(hook = nameof(SetColor))]
        public Color32 BodyColor = Color.black;

        #endregion



        #region Client

        [Client]
        private void SetColor(Color32 _, Color32 newColor)
        {
            if (_cachedMaterial == null) _cachedMaterial = GetComponentInChildren<Renderer>().material;
            _cachedMaterial.color = newColor;
        }

        #endregion



        #region Client and Server

        private Material _cachedMaterial;


        private void OnDestroy()
        {
            Destroy(_cachedMaterial);
        }

        #endregion



        #region Server

        public override void OnStartServer()
        {
            BodyColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

        #endregion

    }
}
