using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SyncVar] 
    public string PlayerName;

    public event Action OnPlayerDestroyedServer;

    // Server
    public float ViewRange = 10.0f;
    public float DestroyDelay = 0.1f;

    [Server]
    public virtual void Destroy()
    {
        HideObject();
        RpcDisplayDestroy();
        OnPlayerDestroyedServer?.Invoke();
        Invoke(nameof(ExecuteDestroy), DestroyDelay);
    }

    [Server]
    private void ExecuteDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }


    public virtual void HideObject()
    {
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) { c.enabled = false; }
        foreach (Collider c in GetComponentsInChildren<Collider>()) { c.enabled = false; }
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) { r.enabled = false; }
    }


    [ClientRpc]
    public virtual void RpcDisplayDestroy()
    {
        HideObject();
    }
}
