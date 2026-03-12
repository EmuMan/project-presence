using UnityEngine;
using System.Collections;

public class CloakModule : Module
{
    [Header("Cloak Settings")]
    public float cloakDuration;

    private float cloakTimer;

    void FixedUpdate()
    {
        if (isCloaked)
        {
            cloakTimer -= Time.fixedDeltaTime;
            if (cloakTimer <= 0f)
            {
                cloakTimer = 0f;
                isCloaked = false;
            }
        }
    }

    protected override void PerformAction(Vector3 direction)
    {
        isCloaked = true;
        cloakTimer = cloakDuration;
    }
}
