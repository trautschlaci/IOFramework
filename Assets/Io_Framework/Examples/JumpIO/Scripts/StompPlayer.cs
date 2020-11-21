using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StompPlayer : RewardBase
{
    public Transform HeadLeft;
    public Transform HeadRight;
    public float CheckDistance = 0.05f;
    public LayerMask PlayerLayer;

    private Player player;

    [ServerCallback]
    void Start()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    void FixedUpdate()
    {
        CheckAboveHead();
    }

    [Server]
    void CheckAboveHead()
    {
        RaycastHit2D leftCheck = Physics2D.Raycast(HeadLeft.position, Vector2.up, CheckDistance, PlayerLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast(HeadRight.position, Vector2.up, CheckDistance, PlayerLayer);

        RaycastHit2D contact = leftCheck;
        if (!contact)
            contact = rightCheck;

        if (contact && CanBeGivenToOther(contact.collider.gameObject))
        {
            ClaimReward(contact.collider.gameObject);
        }
    }

    [Server]
    public override void ClaimReward(GameObject player)
    {
        player.GetComponent<PlayerController>().Jump();
        base.ClaimReward(player);
    }

    [Server]
    public override void Destroy()
    {
        player.Destroy();
    }
}
