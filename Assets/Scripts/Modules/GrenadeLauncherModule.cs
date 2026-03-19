using UnityEngine;

public class GrenadeLauncherModule : Module
{
    [Header("Grenade Launcher Settings")]
    public float shootForce = 20f;
    public GameObject grenadePrefab;
    public Transform shootPoint;

    [Header("Visuals")]
    public GameObject muzzleFlashPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject grenade = Instantiate(grenadePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb?.AddForce(direction.normalized * shootForce, ForceMode.VelocityChange);
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
