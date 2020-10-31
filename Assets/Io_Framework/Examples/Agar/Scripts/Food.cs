using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Food : Edible
{
    public override bool CanBeEatenBy(GameObject other)
    {
        return Vector3.Distance(transform.position, other.gameObject.transform.position) < other.GetComponent<Collider2D>().bounds.extents.x
        && base.CanBeEatenBy(other);
    }

    public override void Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
