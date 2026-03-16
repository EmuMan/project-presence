using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The normal title screen camera.")]
    public Camera mainCamera;
    
    [Tooltip("The camera positioned at the specific Game Over angle/location.")]
    public Camera gameOverCamera;

    [Header("UI")]
    [Tooltip("The Game Over UI Panel to display.")]
    public GameObject gameOverUIPanel;

    [Header("Data Settings")]
    [Tooltip("The PlayerPrefs key used to check the game over state.")]
    public string gameOverPrefKey = "IsGameOver";

    void Start()
    {
        // Check if the game over flag is set to 1 (true). Default to 0 (false) if it doesn't exist.
        bool isGameOver = PlayerPrefs.GetInt(gameOverPrefKey, 0) == 1;

        if (isGameOver)
        {
            SetupGameOverState();
        }
        else
        {
            SetupNormalState();
        }
    }

    private void SetupGameOverState()
    {
        // Switch cameras
        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (gameOverCamera != null) gameOverCamera.gameObject.SetActive(true);

        // Show Game Over UI
        if (gameOverUIPanel != null) gameOverUIPanel.SetActive(true);
        
        // Optional: Reset the flag immediately so the next time they hit the title screen, it is normal
        PlayerPrefs.SetInt(gameOverPrefKey, 0);
        PlayerPrefs.Save();
    }

    private void SetupNormalState()
    {
        // Switch cameras naturally
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (gameOverCamera != null) gameOverCamera.gameObject.SetActive(false);

        // Hide Game Over UI
        if (gameOverUIPanel != null) gameOverUIPanel.SetActive(false);
    }
}
