using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Header("Enemies to activate")]
    public Enemy[] enemies;

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
        {
            if (enemy != null)
                enemy.Deactivate();
        }
    }
}