using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    public int damage = 5;
    // maybe want to change this to something else

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            HealthBar2.Instance.hit(damage);
        }
        // first checks if the bullet hits the player. If so, add the damage
        
        if (_collision.gameObject.layer != LayerMask.NameToLayer("PlayerAttacks") || _collision.gameObject.layer != LayerMask.NameToLayer("EnemyAttacks"))
        {
            Destroy(gameObject);
        }
        // if hits anything thats not considered a bullet, then destroy. Probably can make this better
    }
    
}
