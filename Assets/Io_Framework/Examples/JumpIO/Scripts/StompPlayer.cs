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

    public Player PlayerObject;

    void FixedUpdate()
    {
        CheckAboveHead();
    }

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

    public override void ClaimReward(GameObject player)
    {
        player.GetComponent<PlayerController>().Jump();
        base.ClaimReward(player);
    }

    public override void Destroy()
    {
        PlayerObject.Destroy();
    }
}
