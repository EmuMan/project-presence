using UnityEngine;

public class ModuleItem : MonoBehaviour
{
    [Header("Module Item Settings")]
    [SerializeField] private ModuleData moduleData;
    [SerializeField] private Transform instantiationPoint;

    [Header("Effects")]
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private ParticleSystem trickleEffect;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobbingAmplitude = 0.25f;
    [SerializeField] private float bobbingFrequency = 0.5f;
    [SerializeField] private float postPickupYOffset = -2.0f;
    [SerializeField] private float postPickupMoveSpeed = -0.5f;

    private float timeSinceStart = 0f;
    private float moduleInitialY;
    private float initialY;
    private bool isPickedUp = false;
    private float postPickupYDisplacement = 0f;

    void Start()
    {
        Instantiate(moduleData.instancePrefab, instantiationPoint);

        moduleInitialY = instantiationPoint.position.y;
    }

    void Update()
    {
        if (!isPickedUp)
        {
            MoveModule();
        }
        else
        {
            MovePostPickup();
        }
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
            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            isPickedUp = true;
            initialY = transform.position.y;
            DisableTrickleEffect();

            Destroy(instantiationPoint.gameObject);
        }
    }

    private void MoveModule()
    {
        // Rotate the module item
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob up and down
        timeSinceStart += Time.deltaTime;
        float yDisp = Mathf.Sin(timeSinceStart * bobbingFrequency * 2f * Mathf.PI) * bobbingAmplitude;
        instantiationPoint.position = new Vector3(
            instantiationPoint.position.x,
            moduleInitialY + yDisp,
            instantiationPoint.position.z
        );
    }

    private void MovePostPickup()
    {
        postPickupYDisplacement += postPickupMoveSpeed * Time.deltaTime;
        transform.position = new Vector3(
            transform.position.x,
            initialY + postPickupYDisplacement,
            transform.position.z
        );

        if (postPickupYDisplacement <= postPickupYOffset)
        {
            Destroy(gameObject);
        }
    }

    private void DisableTrickleEffect()
    {
        if (trickleEffect != null)
        {
            trickleEffect.Stop();
        }
    }
}
