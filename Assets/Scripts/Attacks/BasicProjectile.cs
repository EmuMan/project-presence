using UnityEngine;
using System.Collections.Generic;

public class BasicProjectile : MonoBehaviour
{
    public float speed = 20.0f;
    public float lifetime = 5.0f;
    public float damage = 5.0f;

    private Rigidbody projRigidbody;

    private bool hasHit = false;
    private HashSet<GameObject> insideBarriers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (projRigidbody == null)
        {
            Initialize(Vector3.forward);
        }
    }

    void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 direction)
    {
        projRigidbody = GetComponent<Rigidbody>();
        transform.forward = direction.normalized;
        projRigidbody.linearVelocity = direction.normalized * speed;

        insideBarriers = new HashSet<GameObject>();
        foreach (GameObject barrier in GameObject.FindGameObjectsWithTag("Barrier"))
        {
            Collider barrierCollider = barrier.GetComponent<Collider>();
            if (barrierCollider == null)
            {
                Debug.LogWarning("Barrier " + barrier.name + " does not have a Collider component.");
                continue;
            }
            Vector3 closestPoint = barrierCollider.ClosestPoint(transform.position);
            if (closestPoint == transform.position)
            {
                insideBarriers.Add(barrier);
            }
        }
    }

    private void OnHit(Collider other, bool isTrigger)
    {
        // Ignore triggers that are not barriers
        if (isTrigger && !other.CompareTag("Barrier"))
        {
            return;
        }
        // If we already hit something, ignore further collisions to prevent multiple damage applications
        if (hasHit)
        {
            return;
        }
        // Ignore collisions with barriers we started inside of
        if (insideBarriers.Contains(other.gameObject))
        {
            return;
        }
        hasHit = true;
        if (other.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage); // Example damage value
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(collision.collider, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        insideBarriers.Remove(collision.gameObject);
        OnHit(collision.collider, false);
    }

    private void OnTriggerExit(Collider other)
    {
        insideBarriers.Remove(other.gameObject);
        OnHit(other, true);
    }
}
