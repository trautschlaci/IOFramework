using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

public abstract class PickUpBase : NetworkBehaviour
{
    public GameObject CollectedEffect;
    public float Duration = 30.0f;
    public bool IsAvailable = true;

    public void PickUp(GameObject player)
    {
        if(IsAvailable)
            StartCoroutine(PickUpCoroutine(player));
    }

    [Server]
    private IEnumerator PickUpCoroutine(GameObject player)
    {
        IsAvailable = false;
        HideServer();
        TargetDisplayCollect(player.GetComponent<NetworkBehaviour>().connectionToClient);
        RpcNotifyClients();

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

    public virtual void RpcApplyEffect(GameObject player){}

    public virtual void RpcRevertEffect(GameObject player){}

    public virtual void TargetApplyEffect(NetworkConnection conn, GameObject player){}

    public virtual void TargetRevertEffect(NetworkConnection conn, GameObject player){}

    public virtual void ApplyEffectServer(GameObject player){}

    public virtual void RevertEffectServer(GameObject player){}



    public abstract void HideClient();

    public abstract void HideServer();

    public abstract void ApplyEffect(GameObject player);

    public abstract void RevertEffect(GameObject player);
}
