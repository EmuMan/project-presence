using UnityEngine;

public class NailGunModule : Module
{
    public GameObject nailPrefab;
    public Transform shootPoint;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject projectile = Instantiate(nailPrefab, shootPoint.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<BasicProjectile>().Initialize(direction);
    }
}
