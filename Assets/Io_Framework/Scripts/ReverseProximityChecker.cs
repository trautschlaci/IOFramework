using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkIdentity))]
public class ReverseProximityChecker : NetworkVisibility
{

    // How often (in seconds) that this object should update the list of observers that can see it.
    public float visUpdateInterval = 1;

    // Flag to force this object to be hidden for players.
    public bool forceHidden;


    public override void OnStartServer()
    {
        InvokeRepeating(nameof(RebuildObservers), 0, visUpdateInterval);
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
        if (forceHidden)
            return false;

        var playerIdentity = conn.identity;

        if (playerIdentity.GetComponent<Player>() == null)
            return false;

        return Vector3.Distance(playerIdentity.transform.position, transform.position) < playerIdentity.GetComponent<Player>().VisRange;
    }

    public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        if (forceHidden)
            return;

        // 'transform.' calls GetComponent, only do it once
        Vector3 position = transform.position;

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn != null && conn.identity != null && conn.identity.GetComponent<Player>() != null)
            {
                var playerIdentity = conn.identity;

                // check distance
                if (Vector3.Distance(playerIdentity.transform.position, position) < playerIdentity.GetComponent<Player>().VisRange)
                {
                    observers.Add(conn);
                }
            }
        }
    }
}
