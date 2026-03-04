using UnityEngine;

public class BarrierModule : Module
{
    [Header("Barrier Settings")]
    public GameObject barrierPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject projectile = Instantiate(barrierPrefab, transform.position, Quaternion.LookRotation(direction));
    }
}
