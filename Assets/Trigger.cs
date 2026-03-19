using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Header("Enemies to activate")]
    public Enemy[] enemies;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
                enemy.Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (Enemy enemy in enemies)
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
            if (enemy != null)
                enemy.Deactivate();
        }
    }
}