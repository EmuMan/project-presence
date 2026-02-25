using UnityEngine;

public class TankTreadsModule : Module
{
    [Header("Speed Boost Settings")]
    public float speedIncrease = 2.0f; // Base speed modifier for the tank treads
    public float speedBoostResourceMax = 5.0f; // Maximum resource for speed boost
    public float speedBoostResourceCurrent = 5.0f; // Current resource for speed boost
    public float speedBoostConsumptionRate = 1.0f; // Resource consumed per second while speed boost is active
    public float speedBoostRegenRate = 0.5f; // Resource regenerated per second when not using speed boost

    private float originalSpeed;
    private bool isSpeedBoostActive;

    void FixedUpdate()
    {
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

    private void TryStartSpeedBoost()
    {
        if (!isSpeedBoostActive && speedBoostResourceCurrent > 0.0f)
        {
            originalSpeed = speedModifier;
            speedModifier *= speedIncrease; // Increase speed by 50%
            isSpeedBoostActive = true;
        }
    }

    private void TryStopSpeedBoost()
    {
        if (isSpeedBoostActive)
        {
            speedModifier = originalSpeed; // Reset to original speed
            isSpeedBoostActive = false;
        }
    }

    protected override void StartPerformingAction(Vector3 direction)
    {
        Debug.Log($"Starting speed boost with ${speedBoostResourceCurrent} resource");
        TryStartSpeedBoost();
    }

    protected override void StopPerformingAction(Vector3 direction)
    {
        TryStopSpeedBoost();
    }
}
