using UnityEngine;

public class HealthBoost : MonoBehaviour
{

    public float healthBoost = 5.0f;
    private Rigidbody boostRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boostRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.TryGetComponent(out Health health);
            health.AddHealth(healthBoost);

            //after boost is added, remove the health boost game object
            Destroy(gameObject);  
        }

        else{return;}
 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.gameObject.TryGetComponent(out Health health);
            health.AddHealth(healthBoost);

            //after boost is added, remove the health boost game object
            Destroy(gameObject);  
        }

        else{return;}
 
    }
}
