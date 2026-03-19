using UnityEngine;

public class TankTreadsModule : Module
{
    [Header("Speed Boost Settings")]
    public float speedIncrease = 2.0f; // Base speed modifier for the tank treads
    public float speedBoostResourceMax = 5.0f; // Maximum resource for speed boost
    public float speedBoostResourceCurrent = 5.0f; // Current resource for speed boost
    public float speedBoostConsumptionRate = 1.0f; // Resource consumed per second while speed boost is active
    public float speedBoostRegenRate = 0.5f; // Resource regenerated per second when not using speed boost

    [Header("Visuals")]
    public ParticleSystem speedBoostEffect;

    private PlayerMovement playerMovement;

    private float originalSpeed;
    private bool isSpeedBoostActive;

    public override void Initialize(GameObject playerObject, ModuleStatus moduleStatusUI)
    {
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        base.Initialize(playerObject, moduleStatusUI);
    }

    protected override void Update()
    {
        base.Update();
        TurnTowardsPlayerVelocity();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isSpeedBoostActive)
        {
            // Consume resource while speed boost is active
            speedBoostResourceCurrent -= speedBoostConsumptionRate * Time.fixedDeltaTime;
            if (speedBoostResourceCurrent <= 0.0f)
            {
                speedBoostResourceCurrent = 0.0f;
                TryStopSpeedBoost();
            }
        }
        else
        {
            // Regenerate resource when not using speed boost
            if (speedBoostResourceCurrent < speedBoostResourceMax)
            {
                speedBoostResourceCurrent += speedBoostRegenRate * Time.fixedDeltaTime;
                if (speedBoostResourceCurrent > speedBoostResourceMax)
                {
                    speedBoostResourceCurrent = speedBoostResourceMax;
                }
            }
        }
    }

    private void TurnTowardsPlayerVelocity()
    {
        Vector3 velocity = playerMovement.velocity;
        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
        }
    }

    private void TryStartSpeedBoost()
    {
        if (!isSpeedBoostActive && speedBoostResourceCurrent > 0.0f)
        {
            originalSpeed = speedModifier;
            speedModifier *= speedIncrease; // Increase speed by 50%
            isSpeedBoostActive = true;
            EnableSpeedBoostEffect();
        }
    }

    private void TryStopSpeedBoost()
    {
        if (isSpeedBoostActive)
        {
            speedModifier = originalSpeed; // Reset to original speed
            isSpeedBoostActive = false;
            DisableSpeedBoostEffect();
        }
    }

    private void EnableSpeedBoostEffect()
    {
        if (speedBoostEffect != null)
        {
            var emission = speedBoostEffect.emission;
            emission.enabled = true;
        }
    }

    private void DisableSpeedBoostEffect()
    {
        if (speedBoostEffect != null)
        {
            var emission = speedBoostEffect.emission;
            emission.enabled = false;
        }
    }

    public override bool CanPerformAction()
    {
        // Can perform action if we have resource to boost
        return speedBoostResourceCurrent > 0.0f;
    }

    protected override void StartPerformingAction(Vector3 direction)
    {
        TryStartSpeedBoost();
    }

    protected override void StopPerformingAction(Vector3 direction)
    {
        TryStopSpeedBoost();
    }

    public override float GetResourceRemaining()
    {
        return speedBoostResourceCurrent / speedBoostResourceMax;
    }
}
