using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBase : MonoBehaviour
{
    public GameObject CollectedEffect;
    public float Duration = 30.0f;

    private bool used;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            StartCoroutine(PickUp(other));
        }
    }

    IEnumerator PickUp(Collider2D player)
    {
        AddCollectedEffect();
        DisablePickUp();

        ApplyEffect(player);

        yield return new WaitForSeconds(Duration);

        RevertEffect(player);

        Destroy(gameObject);
    }

    private void AddCollectedEffect()
    {
        Instantiate(CollectedEffect, transform.position, transform.rotation);
    }

    private void DisablePickUp()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public virtual void ApplyEffect(Collider2D player)
    {
        // Check if player exists
    }

    public virtual void RevertEffect(Collider2D player)
    {
        // Check if player still exits
    }
}
