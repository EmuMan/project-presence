using UnityEngine;
using System.Collections;

public class CloakModule : Module
{
    [Header("Cloak Settings")]
    public float cloakDuration;
    public ParticleSystem trickleEffect;
    public ParticleSystem cloakEffect;

    private float cloakTimer;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isCloaked)
        {
            cloakTimer -= Time.fixedDeltaTime;
            if (cloakTimer <= 0f)
            {
                StopCloak();
            }
        }
    }

    protected override void PerformAction(Vector3 direction)
    {
        StartCloak();
    }

    protected override void OnCooldownComplete()
    {
        StartTrickle();
    }

    private void StartCloak()
    {
        isCloaked = true;
        cloakTimer = cloakDuration;
        if (cloakEffect != null)
        {
            var emission = cloakEffect.emission;
            emission.enabled = true;
        }
        if (trickleEffect != null)
        {
            var emission = trickleEffect.emission;
            emission.enabled = false;
        }
    }

    private void StopCloak()
    {
        isCloaked = false;
        cloakTimer = 0f;
        if (cloakEffect != null)
        {
            var emission = cloakEffect.emission;
            emission.enabled = false;
        }
    }

    private void StartTrickle()
    {
        if (trickleEffect != null)
        {
            var emission = trickleEffect.emission;
            emission.enabled = true;
        }
    }
}
