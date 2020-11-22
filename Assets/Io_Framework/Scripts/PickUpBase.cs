using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

public abstract class PickUpBase : NetworkBehaviour
{
    public bool IsAvailable = true;
    public float Duration = 30.0f;
    public GameObject CollectedEffect;

    private bool didEffect;

    [Server]
    public void PickUp(GameObject player)
    {
        if(CanBeGivenToPlayerServer(player))
            StartCoroutine(PickUpCoroutine(player));
    }

    [Server]
    private IEnumerator PickUpCoroutine(GameObject player)
    {
        IsAvailable = false;
        HideServer();

        TargetDisplayCollect(player.GetComponent<NetworkBehaviour>().connectionToClient);
        RpcNotifyClients();


        if (!CanAffectPlayerServer(player))
            yield break;

        ApplyEffect(player);

        yield return new WaitForSeconds(Duration);

        if(player != null)
            RevertEffect(player);

        NetworkServer.Destroy(gameObject);
    }


    [TargetRpc]
    public virtual void TargetDisplayCollect(NetworkConnection conn)
    {
        Instantiate(CollectedEffect, transform.position, transform.rotation);
    }

    [ClientRpc]
    public virtual void RpcNotifyClients()
    {
        HideClient();
    }

    [Server]
    public virtual bool CanBeGivenToPlayerServer(GameObject player)
    {
        return IsAvailable;
    }

    [Server]
    public virtual bool CanAffectPlayerServer(GameObject player)
    {
        return true;
    }

    public abstract void HideClient();

    public abstract void HideServer();

    public abstract void ApplyEffect(GameObject player);

    public abstract void RevertEffect(GameObject player);
}
