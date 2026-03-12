using UnityEngine;

public class EmpModule : Module
{
    [Header("EMP Settings")]
    public GameObject empPrefab;

    protected override void PerformAction(Vector3 direction)
    {
        Instantiate(empPrefab, transform.position, Quaternion.LookRotation(direction));
    }
}
