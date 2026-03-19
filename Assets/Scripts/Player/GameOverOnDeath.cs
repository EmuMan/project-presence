using UnityEngine;

public class GameOverOnDeath : MonoBehaviour
{
    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.onDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            Debug.LogError("Health component not found on " + gameObject.name);
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player has died. Triggering game over sequence.");

        // Sets the game over flag in PlayerPrefs to indicate the player has died
        // This is helpful for the GameOverScreen to know when to show the game over state
        PlayerPrefs.SetInt("IsGameOver", 1);
        PlayerPrefs.Save(); // Ensures it saves immediately

        // Scene transition to the Game Over screen
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        transitionScreen.LoadSceneWithBlackout("TopScene");
    }
}
