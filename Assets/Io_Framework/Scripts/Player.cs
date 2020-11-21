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

    // Server use only
    public float ViewRange = 10.0f;

    public float DestroyDelay = 0.1f;

    [Server]
    public virtual void Destroy()
    {
        OnPlayerDestroyedServer?.Invoke();
        Invoke(nameof(ExecuteDestroy), DestroyDelay);
    }

    public virtual void ExecuteDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
