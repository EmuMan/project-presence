using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar2 : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    //[SerializeField] float healthBarChangeDelay = 0.5f;

    [SerializeField] int maxHealth = 80;
    private int currentHealth;
    
    public static HealthBar2 Instance;

    void Start()
    {
        Instance = this;
        currentHealth = maxHealth;
    }

    public void hit(int damage)
    // a singleton to show the damage that a player has taken
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Time.timeScale = 0f;
        }
        // checks the health and if it took too much damage, it stops the game

        SetHealth(currentHealth, maxHealth);
    }


    public void SetHealth(int health, int maxHealth)
    {
        float percentFilled = (float) health / (float) maxHealth;
        // StartCoroutine(GradualSetHealth(percentFilled));
        
        // Alternative method:  Instead of using a coroutine to lerp change, could do this
        fill.fillAmount = percentFilled;
        fill.color = gradient.Evaluate(percentFilled);
    }


}
