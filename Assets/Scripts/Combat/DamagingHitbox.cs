using UnityEngine;
using System.Collections.Generic;

public class DamagingHitbox : MonoBehaviour
{
    public float damage = 10.0f;

    public bool singleHit = true; // If true, each collider can only be hit once
    public float damageInterval = 0.5f; // Time in seconds between damage applications when staying in the hitbox

    // Tracks colliders and the last time they were hit
    private Dictionary<Collider, float> hitColliders = new Dictionary<Collider, float>();

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

    private void TryHit(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            float currentTime = Time.time;
            if (!hitColliders.ContainsKey(other) || currentTime - hitColliders[other] >= damageInterval)
            {
                health.TakeDamage(damage);
                hitColliders[other] = currentTime; // Update the last hit time
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
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
