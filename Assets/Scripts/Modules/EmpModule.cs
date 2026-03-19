using UnityEngine;

public class EmpModule : Module
{
    [Header("EMP Settings")]
    public GameObject empPrefab;
    public GameObject empChargeObject;

    protected override void PerformAction(Vector3 direction)
    {
        Instantiate(empPrefab, transform.position, Quaternion.LookRotation(direction));
        if (empChargeObject != null)
        {
            empChargeObject.SetActive(false);
        }
    }

    protected override void OnCooldownComplete()
    {
        if (empChargeObject != null)
        {
            empChargeObject.SetActive(true);
        }
    }
}
