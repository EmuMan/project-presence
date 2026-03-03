using UnityEngine;

public class MachineGunModule : Module
{
    [Header("Hitscan Settings")]
    public Transform shootPoint;
    public float range = 100.0f;
    public float damagePerShot = 2.0f;

    [Header("Visuals")]
    public GameObject hitscanTrailPrefab;

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

            // Spawn a hitscan trail from the shoot point to the hit point
            SpawnTrail(shootPoint.position, hit.point);
        }
        else
        {
            // If we didn't hit anything, spawn a trail to the maximum range point
            SpawnTrail(shootPoint.position, shootPoint.position + direction.normalized * range);
        }
    }

    private void SpawnTrail(Vector3 start, Vector3 end)
    {
        if (hitscanTrailPrefab != null)
        {
            GameObject trail = Instantiate(hitscanTrailPrefab, start, Quaternion.identity);
            HitscanTrail hitscanTrail = trail.GetComponent<HitscanTrail>();
            if (hitscanTrail != null)
            {
                hitscanTrail.SetTrail(start, end);
            }
        }
    }
}
