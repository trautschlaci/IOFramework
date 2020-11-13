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

    public int PlayerId { get; private set; }

    public event Action OnPlayerDestroyedServer;

    // Server use only
    public float ViewRange = 10.0f;

    public override void OnStartServer()
    {
        PlayerId = connectionToClient.connectionId;
    }

    public virtual void Destroy()
    {
        OnPlayerDestroyedServer?.Invoke();
        NetworkServer.Destroy(gameObject);
    }
}
