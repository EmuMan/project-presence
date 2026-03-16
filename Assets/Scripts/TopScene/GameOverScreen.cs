using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The normal title screen camera.")]
    public Camera mainCamera;
    
    [Tooltip("The camera positioned at the specific Game Over angle/location.")]
    public Camera gameOverCamera;

    [Header("Transition Settings")]
    [Tooltip("The target position/rotation for the camera to transition to when going from title to ability screen.")]
    public Transform targetCameraPosition;
    [Tooltip("The CanvasGroup for the game over UI elements.")]
    public CanvasGroup gameoverUICanvasGroup;
    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup abilityUICanvasGroup;
    [Tooltip("The CanvasGroup for the title UI elements.")]
    public CanvasGroup titleUICanvasGroup;
    [Tooltip("The target field of view for the camera transition.")]
    public float targetFOV = 60f;

    [Header("UI")]
    [Tooltip("The Game Over UI Panel to display.")]
    public GameObject gameOverUIPanel;

    [Tooltip("The Game Over UI CanvasGroup goes here.")]
    public CanvasGroup gameOverUICanvasGroup;

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

        // Reset the flag immediately so the next time they hit the title screen, it is normal
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


    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                gameOverCamera,
                targetCameraPosition,
                gameOverUICanvasGroup,
                abilityUICanvasGroup,
                targetFOV
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
