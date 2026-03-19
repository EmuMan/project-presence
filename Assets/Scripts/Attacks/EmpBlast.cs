using UnityEngine;

public class EmpBlast : MonoBehaviour
{
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
        SetAlpha((1f - (blastTimer / blastDuration)) / 2f);
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
