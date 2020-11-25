using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AgarPlayer : CloneablePlayerObject
{
    public int MinScoreToSplit = 10;

    private PlayerScore playerScore;

    [ServerCallback]
    private void Start()
    {
        playerScore = GetComponent<PlayerScore>();
    }

    [ServerCallback]
    private void Awake()
    {
        playerScore = GetComponent<PlayerScore>();
    }

    [Server]
    public void Split(Vector2 startVelocityDir)
    {
        if (!CanCreateClone())
            return;

        playerScore.Score = (int)(playerScore.Score / 2.0f);

        Vector3 target = transform.position + (Vector3)(startVelocityDir * GetComponent<Collider2D>().bounds.extents.x * 1.1f);
        GameObject half = SpawnClone(target, Quaternion.identity);
        half.GetComponent<PlayerScore>().Score = playerScore.Score;
        half.GetComponent<PlayerControllerAgar>().GiveStartVelocity(startVelocityDir);
    }
    
    [Server]
    public override bool CanCreateClone()
    {
        return playerScore.Score >= MinScoreToSplit && base.CanCreateClone();
    }

    [Server]
    public override void Destroy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        RpcDisplayDestroy();
        base.Destroy();
    }

    [Server]
    public override void OnLastPlayerObjectDestroyed()
    {
        base.OnLastPlayerObjectDestroyed();
        TargetLastObjectDestroyed();
    }


    [ClientRpc]
    private void RpcDisplayDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    [TargetRpc]
    private void TargetLastObjectDestroyed()
    {
        IoNetworkManager networkManager = (IoNetworkManager)NetworkManager.singleton;
        networkManager.RestartPlayerClient();
    }


    public override int CompareTo(CloneablePlayerObject other)
    {
        var otherScore = other.GetComponent<PlayerScore>();
        if (otherScore == null)
            return base.CompareTo(other);

        return playerScore.Score.CompareTo(otherScore.Score);
    }
}
