using UnityEngine;

public class MachineGunModule : Module
{
    [Header("Hitscan Settings")]
    public Transform shootPoint;
    public float range = 100.0f;
    public float damagePerShot = 2.0f;

    private LayerMask hitMask;

    private Vector3 lastShootDirection;
    private Vector3 lastShootOrigin;

    void Start()
    {
        hitMask = LayerMask.GetMask("Enemy", "Terrain");
    }

    void OnDrawGizmos()
    {
        if (shootPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastShootOrigin, lastShootOrigin + lastShootDirection.normalized * range);
        }
    }

    protected override void PerformAction(Vector3 direction)
    {
        lastShootDirection = direction;
        lastShootOrigin = shootPoint.position;

        RaycastHit[] allHits = Physics.RaycastAll(shootPoint.position, direction, range);
        foreach (var h in allHits)
        {
            Debug.Log($"Hit: {h.collider.gameObject.name} | Layer: {h.collider.gameObject.layer}");
        }

        // Perform a raycast to simulate hitscan shooting
        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, direction, out hit, range, hitMask))
        {
            // Check if the hit object has a health component and apply damage
            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damagePerShot);
            }
        }
    }
}
