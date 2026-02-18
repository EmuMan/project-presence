using UnityEngine;

public class BlinkModule : Module
{
    [SerializeField] private float maxBlinkDistance = 7.0f; // Maximum distance the player can blink

    protected override void PerformAction(Vector3 direction)
    {
        Transform playerTransform = playerObject.transform;

        // Teleport the player in the direction they are looking, up to a maximum distance
        Vector3 blinkDestination = playerTransform.position + direction.normalized * maxBlinkDistance;

        // Check for obstacles using a spherecast with a small radius to prevent teleporting inside walls or other objects
        float sphereRadius = 0.5f;
        if (Physics.SphereCast(playerTransform.position, sphereRadius, direction.normalized, out RaycastHit hitInfo, maxBlinkDistance))
        {
            blinkDestination = playerTransform.position + direction.normalized * hitInfo.distance;
        }

        playerTransform.position = blinkDestination;
    }
}
