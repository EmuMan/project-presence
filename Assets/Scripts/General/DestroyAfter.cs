using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour
{
    public float lifetime = 5.0f;
    public GameObject destroyEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyTimer(lifetime));
    }

    private IEnumerator DestroyTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
