using UnityEngine;

public class GrenadeLauncherModule : Module
{
    [Header("Grenade Launcher Settings")]
    public float shootForce = 20f;
    public GameObject grenadePrefab;
    public Transform shootPoint;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject grenade = Instantiate(grenadePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb?.AddForce(direction.normalized * shootForce, ForceMode.VelocityChange);
    }
}
