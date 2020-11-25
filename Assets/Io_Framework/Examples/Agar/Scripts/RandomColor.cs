using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    Material cachedMaterial;

    private void Start()
    {
        cachedMaterial = GetComponentInChildren<Renderer>().material;
        cachedMaterial.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void OnDestroy()
    {
        Destroy(cachedMaterial);
    }
}
