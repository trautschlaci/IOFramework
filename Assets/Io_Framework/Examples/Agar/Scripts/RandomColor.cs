using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RandomColor : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetColor))]
    public Color32 PlayerColor = Color.black;

    private Material _cachedMaterial;

    [Server]
    public override void OnStartServer()
    {
        PlayerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    [Client]
    private void SetColor(Color32 _, Color32 newColor)
    {
        if (_cachedMaterial == null) _cachedMaterial = GetComponentInChildren<Renderer>().material;
        _cachedMaterial.color = newColor;
    }

    [ClientCallback]
    void OnDestroy()
    {
        Destroy(_cachedMaterial);
    }
}
