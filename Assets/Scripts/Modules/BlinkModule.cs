using UnityEngine;

public class BlinkModule : Module
{
    [Header("Blink Settings")]
    [SerializeField] private float maxBlinkDistance = 7.0f; // Maximum distance the player can blink

    [Header("Visuals")]
    [SerializeField] private GameObject blinkTrailPrefab;
    [SerializeField] private GameObject disappearEffectPrefab;
    [SerializeField] private ParticleSystem trickleEffect;

    protected override void PerformAction(Vector3 direction)
    {
        Transform playerTransform = playerObject.transform;

        // Teleport the player in the direction they are looking, up to a maximum distance
        Vector3 blinkDisplacement = direction.normalized * maxBlinkDistance;

        // Check for obstacles using a spherecast with a small radius to prevent teleporting inside walls or other objects
        float sphereRadius = 0.5f;
        int layerMask = LayerMask.GetMask("Terrain");
        if (Physics.SphereCast(playerTransform.position, sphereRadius, direction.normalized, out RaycastHit hitInfo, maxBlinkDistance, layerMask))
        {
            blinkDisplacement = direction.normalized * hitInfo.distance;
        }

        // Spawn a trail effect from the player's current position to the target position
        SpawnBlinkTrail(playerTransform.position, playerTransform.position + blinkDisplacement);

        // Spawn a disappear effect at the player's current position
        SpawnDisappearEffect(playerTransform.position);

        // Disable the trickle effect until the cooldown comes back online
        DisableTrickleEffect();

        playerTransform.GetComponent<CharacterController>().Move(blinkDisplacement);
    }

    protected override void OnCooldownComplete()
    {
        EnableTrickleEffect();
    }

    private void SpawnBlinkTrail(Vector3 start, Vector3 end)
    {
        if (blinkTrailPrefab != null)
        {
            GameObject trail = Instantiate(blinkTrailPrefab, start, Quaternion.identity);
            trail.GetComponent<DecayingTrail>()?.SetTrail(start, end);
        }
    }

    private void SpawnDisappearEffect(Vector3 position)
    {
        if (disappearEffectPrefab != null)
        {
            Instantiate(disappearEffectPrefab, position, Quaternion.identity);
        }
    }

    private void EnableTrickleEffect()
    {
        if (trickleEffect != null)
        {
            var emission = trickleEffect.emission;
            emission.enabled = true;
        }
    }

    private void DisableTrickleEffect()
    {
        if (trickleEffect != null)
        {
            var emission = trickleEffect.emission;
            emission.enabled = false;
        }
    }
}
