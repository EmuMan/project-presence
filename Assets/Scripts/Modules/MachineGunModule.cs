using UnityEngine;

public class MachineGunModule : Module
{
    [Header("Hitscan Settings")]
    public Transform shootPoint;
    public float range = 100.0f;
    public float damagePerShot = 2.0f;

    [Header("Visuals")]
    public GameObject hitscanTrailPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;

    private LayerMask hitMask;

    private Vector3 lastShootDirection;
    private Vector3 lastShootOrigin;

    void Start()
    {
        hitMask = LayerMask.GetMask("Default", "Enemy", "Terrain");
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
            // Spawn a hit effect at the point of impact
            SpawnHitEffect(hit.point, hit.normal);
        }
        else
        {
            // If we didn't hit anything, spawn a trail to the maximum range point
            SpawnTrail(shootPoint.position, shootPoint.position + direction.normalized * range);
        }

        // Spawn a muzzle flash at the shoot point
        SpawnMuzzleFlash(direction);
    }

    private void SpawnTrail(Vector3 start, Vector3 end)
    {
        if (hitscanTrailPrefab != null)
        {
            GameObject trail = Instantiate(hitscanTrailPrefab, start, Quaternion.identity);
            DecayingTrail hitscanTrail = trail.GetComponent<DecayingTrail>();
            if (hitscanTrail != null)
            {
                hitscanTrail.SetTrail(start, end);
            }
        }
    }

    private void SpawnMuzzleFlash(Vector3 direction)
    {
        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        }
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
        }
    }
}
