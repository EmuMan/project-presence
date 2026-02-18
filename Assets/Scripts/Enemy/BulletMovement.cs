using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

}
