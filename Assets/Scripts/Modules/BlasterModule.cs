using UnityEngine;

public class BlasterModule : Module
{
    [Header("Projectile Settings")]
    public GameObject nailPrefab;
    public Transform shootPoint;

    [Header("Visuals")]
    public GameObject muzzleFlashPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject projectile = Instantiate(nailPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<BasicProjectile>().Initialize(direction);

        SpawnMuzzleFlash(direction);
    }

    private void SpawnMuzzleFlash(Vector3 direction)
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        }
    }
}
