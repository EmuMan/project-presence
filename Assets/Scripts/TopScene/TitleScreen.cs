using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The main camera.")]
    public Camera mainCamera;

    [Header("Transition Settings")]

    [Tooltip("The CanvasGroup for the game over UI elements.")]
    public CanvasGroup gameOverUICanvasGroup;

    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("The CanvasGroup for the title UI elements.")]
    public CanvasGroup titleUICanvasGroup;

    [Tooltip("The coords for ability UI.")]
    public Transform abilityCameraPosition;

    [Tooltip("The coords for game over UI.")]
    public Transform gameOverCameraPosition;

    [Tooltip("The coords for title UI.")]
    public Transform titleCameraPosition;

    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, 
                abilityCameraPosition, 
                titleUICanvasGroup, 
                abilityUICanvasGroup, 
                60f
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
