using UnityEngine;

public class InstantExpand : MonoBehaviour
{
    [Header("Expansion Settings")]
    public float expandedScale = 2.0f;
    public float expansionDuration = 0.5f;
    public AnimationCurve expansionCurve;

    private Vector3 originalScale;
    private float expansionTimer = 0.0f;
    private bool isExpanding = false;

    void Start()
    {
        originalScale = transform.localScale;
        isExpanding = true;
        expansionTimer = 0.0f;
    }

    void FixedUpdate()
    {
        if (isExpanding)
        {
            expansionTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(expansionTimer / expansionDuration);
            float curveValue = expansionCurve.Evaluate(t);
            float currentScaleFactor = Mathf.Lerp(1.0f, expandedScale, curveValue);
            transform.localScale = originalScale * currentScaleFactor;

            if (t >= 1.0f)
            {
                isExpanding = false;
            }
        }
    }
}
