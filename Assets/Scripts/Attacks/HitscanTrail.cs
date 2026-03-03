using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HitscanTrail : MonoBehaviour
{
    public float duration = 0.08f;
    public AnimationCurve widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.1f);

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.widthCurve = widthCurve;
        lr.positionCount = 2;
        SetTrail(Vector3.zero, Vector3.up * 10f);
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
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / duration);
            lr.material.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        Destroy(gameObject);
    }
}
