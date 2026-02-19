using UnityEngine;

public class BlinkModule : Module
{
    [SerializeField] private float maxBlinkDistance = 7.0f; // Maximum distance the player can blink

    public ModuleData data;

    public Material onColor;
    public Material offColor;

    private float coolTime;
    private bool acted = false;

    protected override void PerformAction(Vector3 direction)
    {
        Transform playerTransform = playerObject.transform;

        // Teleport the player in the direction they are looking, up to a maximum distance
        Vector3 blinkDisplacement = direction.normalized * maxBlinkDistance;

        // Check for obstacles using a spherecast with a small radius to prevent teleporting inside walls or other objects
        float sphereRadius = 0.5f;
        if (Physics.SphereCast(playerTransform.position, sphereRadius, direction.normalized, out RaycastHit hitInfo, maxBlinkDistance))
        {
            blinkDisplacement = direction.normalized * hitInfo.distance;
        }

        playerTransform.GetComponent<CharacterController>().Move(blinkDisplacement);
        acted = true;
    }

    void FixedUpdate()
    {
        if (coolTime > 0.0f)
        {
            coolTime -= Time.deltaTime;
        }
        else
        {
            if (acted == true)
            {
                GetComponent<Renderer>().material = offColor;
                coolTime = data.cooldownDuration;
                acted = false;
            }
            else
            {
                GetComponent<Renderer>().material = onColor;
            }
        }
    }
}
