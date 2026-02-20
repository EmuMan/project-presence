using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    [SerializeField] private Health health;

    void Start()
    {
        health.onDamageTaken.AddListener(UpdateHealth);
    }

    public void UpdateHealth()
    {
        SetHealth(health.currentHealth, health.maxHealth);
    }


    public void SetHealth(float health, float maxHealth)
    {
        float percentFilled = health / maxHealth;

        // Alternative method:  Instead of using a coroutine to lerp change, could do this
        fill.fillAmount = percentFilled;
        fill.color = gradient.Evaluate(percentFilled);
    }
}
