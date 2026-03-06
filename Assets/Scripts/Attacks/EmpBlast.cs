using UnityEngine;

public class EmpBlast : MonoBehaviour
{
    [SerializeField] private float endRadius = 20f;
    [SerializeField] private float blastDuration = 1f;
    [SerializeField] private float disableDuration = 5f;

    private Renderer blastRenderer;

    private float blastTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blastRenderer = GetComponent<Renderer>();
        blastTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        blastTimer += Time.deltaTime;
        float currentRadius = Mathf.Lerp(0f, endRadius, blastTimer / blastDuration);
        transform.localScale = new Vector3(currentRadius, currentRadius, currentRadius);
        SetAlpha((1f - (blastTimer / blastDuration)) / 2f);

        if (blastTimer >= blastDuration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.DisableForSeconds(disableDuration);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (blastRenderer != null)
        {
            Color color = blastRenderer.material.color;
            color.a = alpha;
            blastRenderer.material.color = color;
        }
    }
}
