using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float lifetime = 3.0f;
    public float damage = 5.0f;
    public GameObject explosionPrefab;

    private Rigidbody rb;

    void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0.0f)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
