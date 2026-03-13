using UnityEngine;

public class BarrierModule : Module
{
    [Header("Barrier Settings")]
    public GameObject barrierCharge;
    public GameObject barrierPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        GameObject projectile = Instantiate(barrierPrefab, transform.position, Quaternion.LookRotation(direction));
        barrierCharge.SetActive(false);
    }

    protected override void OnCooldownComplete()
    {
        barrierCharge.SetActive(true);
    }
}
