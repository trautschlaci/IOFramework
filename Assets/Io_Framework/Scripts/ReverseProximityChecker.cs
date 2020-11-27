﻿using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkIdentity))]
public class ReverseProximityChecker : NetworkVisibility
{

    public float OwnExtent = 1;

    // How often (in seconds) that this object should update the list of observers that can see it.
    public float VisUpdateInterval = 0.1f;

    // Flag to force this object to be hidden for players.
    public bool ForceHidden;


    public override void OnStartServer()
    {
        InvokeRepeating(nameof(RebuildObservers), 0, VisUpdateInterval);
    }

    public override void OnStopServer()
    {
        CancelInvoke(nameof(RebuildObservers));
    }

    void RebuildObservers()
    {
        netIdentity.RebuildObservers(false);
    }

    public override bool OnCheckObserver(NetworkConnection conn)
    {
        if (ForceHidden)
            return false;

        var playerIdentity = conn.identity;

        if (playerIdentity.GetComponent<Player>() == null)
            return false;

        return Vector3.Distance(playerIdentity.transform.position, transform.position) < playerIdentity.GetComponent<Player>().ViewRange + OwnExtent;
    }

    public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        if (ForceHidden)
            return;

        // 'transform.' calls GetComponent, only do it once
        Vector3 position = transform.position;

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn != null && conn.identity != null && conn.identity.GetComponent<Player>() != null)
            {
                var playerIdentity = conn.identity;

                // check distance
                if (Vector3.Distance(playerIdentity.transform.position, position) < playerIdentity.GetComponent<Player>().ViewRange + OwnExtent)
                {
                    observers.Add(conn);
                }
            }
        }
    }
}
