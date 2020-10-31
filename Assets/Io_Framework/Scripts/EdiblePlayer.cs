using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EdiblePlayer : Edible
{
    public Player playerObject;
    public PlayerScore playerScore;

    [Range(0.0f, 1.0f)]
    public float ScoreDecay;

    public override bool CanBeEatenBy(GameObject other)
    {
        return Vector3.Distance(transform.position, other.gameObject.transform.position) < other.GetComponent<Collider2D>().bounds.extents.x
               && GetComponent<Collider2D>().bounds.extents.x < 0.9f * other.GetComponent<Collider2D>().bounds.extents.x
               && base.CanBeEatenBy(other);
    }

    public override int EarnedScore()
    {
        return base.EarnedScore() + (int)((1.0f-ScoreDecay) * playerScore.Score);
    }

    public override void Destroy()
    {
        playerObject.Destroy();
    }
}
