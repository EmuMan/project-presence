using UnityEngine;

public class NailGunModule : Module
{
    public GameObject nailPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject projectile = Instantiate(nailPrefab, transform.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<BasicProjectile>().Initialize(direction);
    }
}
