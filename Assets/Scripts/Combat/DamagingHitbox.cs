using UnityEngine;
using System.Collections.Generic;

public class DamagingHitbox : MonoBehaviour
{
    public float damage = 10.0f;

    public bool singleHit = true; // If true, each collider can only be hit once

    private HashSet<Collider> hitColliders = new HashSet<Collider>();

    private Collider hitboxCollider;

    private void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            Debug.LogError("DamagingHitbox requires a Collider component.");
        }
        else
        {
            hitboxCollider.isTrigger = true; // Ensure the collider is set to trigger
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health) && !hitColliders.Contains(other))
        {
            health.TakeDamage(damage);
            hitColliders.Add(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Health health) && !hitColliders.Contains(other))
        {
            health.TakeDamage(damage);
            hitColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hitColliders.Contains(other) && !singleHit)
        {
            hitColliders.Remove(other);
        }
    }

    public void ResetHitbox()
    {
        hitColliders.Clear();
    }

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }
        ResetHitbox();
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }
}
