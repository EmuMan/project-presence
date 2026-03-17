using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The main camera.")]
    public Camera mainCamera;

    [Header("Transition Settings")]
    [Tooltip("The coords for ability UI.")]
    public Transform abilityCameraPosition;

    [Tooltip("The coords for game over UI.")]
    public Transform gameOverCameraPosition;

    [Tooltip("The coords for title UI.")]
    public Transform titleCameraPosition;

    [Tooltip("The coords to transition into the game.")]
    public Transform redeployCameraPosition;

    [Header("UI")]
    [Tooltip("The Game Over UI Panel to display.")]
    public GameObject gameOverUIPanel;

    [Tooltip("The CanvasGroup for the game over UI elements.")]
    public CanvasGroup gameOverUICanvasGroup;

    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("The CanvasGroup for the title UI elements.")]
    public CanvasGroup titleUICanvasGroup;

    [Header("Data Settings")]
    [Tooltip("The PlayerPrefs key used to check the game over state.")]
    public string gameOverPrefKey = "IsGameOver";

    void Start()
    {
        //TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();

        //StartCoroutine(transitionScreen.FadeFromBlack());

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
        // Send main camera to the game over position and rotation immediately without transition
        if (mainCamera != null)
        {
            mainCamera.transform.position = gameOverCameraPosition.position;
            mainCamera.transform.rotation = gameOverCameraPosition.rotation;
        }

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
        //if (gameOverCamera != null) gameOverCamera.gameObject.SetActive(false);

        // Hide Game Over UI
        if (gameOverUIPanel != null) gameOverUIPanel.SetActive(false);
    }


    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                abilityCameraPosition,
                gameOverUICanvasGroup,
                abilityUICanvasGroup,
                60f
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }

    public void CamTransitionToGame(string useLoadScreen)
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                redeployCameraPosition,
                gameOverUICanvasGroup,
                titleUICanvasGroup,
                50f,
                true,
                useLoadScreen
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
