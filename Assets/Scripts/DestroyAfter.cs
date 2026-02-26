using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float lifetime = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
