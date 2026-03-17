using UnityEngine;

public class ModuleItem : MonoBehaviour
{
    [SerializeField] private ModuleData moduleData;
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private Transform instantiationPoint;

    void Start()
    {
        Instantiate(moduleData.instancePrefab, instantiationPoint);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ModuleManager.Instance != null)
            {
                // Unlock the module for the player
                ModuleManager.Instance.UnlockModule(moduleData);
            }

            // Show pickup effect
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Destroy the module item after pickup
            Destroy(gameObject);
        }
    }
}
