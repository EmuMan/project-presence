using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentHealth;

    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;

    public GameObject deathEffect;

    public UnityEvent onHealthBoost;

    public bool Alive => currentHealth > 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!Alive) return;
        currentHealth -= damage;
        onDamageTaken?.Invoke();
        if (!Alive)
        {
            Die();
        }
    }

    public void AddHealth(float healthBoost)
    {
        //cap at max health
        if (currentHealth + healthBoost > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healthBoost;
        }
        onHealthBoost?.Invoke();

    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        onDeath?.Invoke();
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
