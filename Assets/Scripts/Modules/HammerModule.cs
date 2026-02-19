using UnityEngine;
using System.Collections;

public class HammerModule : Module
{
    public float hammerDownAngle = 90.0f;
    public float hammerUpAngle = 0.0f;
    public float hammerWindUpAngle = -45.0f;

    public float windUpTime = 0.2f;
    public float swingTime = 0.05f;
    public float recoveryTime = 0.2f;

    public DamagingHitbox hitbox;

    protected override void PerformAction(Vector3 direction)
    {
        StartCoroutine(SwingHammer(direction));
    }

    private IEnumerator SwingHammer(Vector3 direction)
    {
        // Wind up the hammer
        float elapsed = 0.0f;
        while (elapsed < windUpTime)
        {
            float angle = Mathf.Lerp(hammerUpAngle, hammerWindUpAngle, elapsed / windUpTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Swing down
        hitbox.EnableHitbox();
        elapsed = 0.0f;
        while (elapsed < swingTime)
        {
            float angle = Mathf.Lerp(hammerWindUpAngle, hammerDownAngle, elapsed / swingTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hitbox.DisableHitbox();

        // Recovery
        elapsed = 0.0f;
        while (elapsed < recoveryTime)
        {
            float angle = Mathf.Lerp(hammerDownAngle, hammerUpAngle, elapsed / recoveryTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
