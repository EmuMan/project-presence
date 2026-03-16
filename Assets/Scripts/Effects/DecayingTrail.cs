using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DecayingTrail : MonoBehaviour
{
    public float duration = 0.5f;
    public float startWidth = 0.1f;
    public float endWidth = 0.1f;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.widthCurve = GenerateWidthCurve(1f);
        lr.positionCount = 2;
    }

    public void SetTrail(Vector3 start, Vector3 end)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float elapsed = 0f;
        Color startColor = lr.material.color;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t);
            lr.material.color = new Color(startColor.r, startColor.g, startColor.b, a);
            lr.widthCurve = GenerateWidthCurve(t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private AnimationCurve GenerateWidthCurve(float t)
    {
        t = 1f - t;
        return AnimationCurve.Linear(0, startWidth * t, 1, endWidth * t);
    }
}
