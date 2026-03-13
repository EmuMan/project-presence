using UnityEngine;

public class BlinkModule : Module
{
    [Header("Blink Settings")]
    [SerializeField] private float maxBlinkDistance = 7.0f; // Maximum distance the player can blink

    public ModuleData data;

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

        playerTransform.GetComponent<CharacterController>().Move(blinkDisplacement);
    }
}
