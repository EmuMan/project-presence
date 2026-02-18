using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float speed;
    public float lifetime = 5.0f;

    private Rigidbody projRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (projRigidbody == null)
        {
            Initialize(Vector3.forward);
        }
    }

    void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 direction)
    {
        projRigidbody = GetComponent<Rigidbody>();
        transform.forward = direction.normalized;
        projRigidbody.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision _collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // disable game object if tag is "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
