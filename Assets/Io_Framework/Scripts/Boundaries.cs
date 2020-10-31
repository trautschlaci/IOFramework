using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    public GameObject Map;

    void LateUpdate()
    {
        Vector3 position = transform.position;
        float xBound = Map.GetComponent<SpriteRenderer>().bounds.extents.x - GetComponent<Collider2D>().bounds.extents.x;
        position.x = Mathf.Clamp(position.x, -1.0f * xBound, xBound);
        float yBound = Map.GetComponent<SpriteRenderer>().bounds.extents.y - GetComponent<Collider2D>().bounds.extents.y;
        position.y = Mathf.Clamp(position.y, -1.0f * yBound, yBound);
        transform.position = position;
    }
}
