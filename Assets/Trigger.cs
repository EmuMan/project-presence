using UnityEngine;

public class EnemyTriggerAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    private Transform player;
    private bool playerInTrigger = false;
    private float lastAttackTime;

    void Update()
    {
        if (playerInTrigger && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Face the player
            transform.LookAt(player);

            if (distance <= attackRange)
            {
                TryAttack();
            }
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy attacks player!");

        // health script, call it here
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            player = other.transform;
            Debug.Log("Player entered enemy range!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            player = null;
            Debug.Log("Player left enemy range.");
        }
    }
}