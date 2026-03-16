using UnityEngine;
using System.Collections;

public class BouncyModule : Module
{
    [Header("Jump Settings")]
    public float jumpStrength = 10.0f;
    public float jumpCoyoteTime = 0.2f;

    [Header("Visuals")]
    [SerializeField] private Transform scaleOrigin;
    [SerializeField] private float scaleMultiplier = 1.5f;
    [SerializeField] private float scaleEffectDuration = 0.5f;

    private float timeSinceLastGrounded = 0.0f;
    private bool canJump = true;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = playerObject.GetComponent<PlayerMovement>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerMovement.IsGrounded())
        {
            timeSinceLastGrounded = 0.0f;
            canJump = true;
        }
        else
        {
            timeSinceLastGrounded += Time.fixedDeltaTime;
        }

        if (timeSinceLastGrounded > jumpCoyoteTime)
        {
            canJump = false;
        }
    }

    protected override void PerformAction(Vector3 direction)
    {
        playerMovement.velocity.y = jumpStrength;
        StartCoroutine(JumpScaleEffect());
    }

    public override bool CanPerformAction()
    {
        return canJump;
    }

    private IEnumerator JumpScaleEffect()
    {
        Vector3 originalScale = scaleOrigin.localScale;
        Vector3 expandedScale = originalScale;
        expandedScale.y *= scaleMultiplier;
        Vector3 contractedScale = originalScale;
        contractedScale.y /= scaleMultiplier;

        float firstSegmentTime = scaleEffectDuration / 5f;
        float otherSegmentTime = scaleEffectDuration / 2.5f;
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < firstSegmentTime)
        {
            scaleOrigin.localScale = Vector3.Lerp(originalScale, expandedScale, elapsedTime / firstSegmentTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Overshoot
        elapsedTime = 0f;
        while (elapsedTime < otherSegmentTime)
        {
            scaleOrigin.localScale = Vector3.Lerp(expandedScale, contractedScale, elapsedTime / otherSegmentTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale back to original
        elapsedTime = 0f;
        while (elapsedTime < otherSegmentTime)
        {
            scaleOrigin.localScale = Vector3.Lerp(contractedScale, originalScale, elapsedTime / otherSegmentTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scaleOrigin.localScale = originalScale;
        Debug.Log("Returned to original scale: " + scaleOrigin.localScale);
    }
}
