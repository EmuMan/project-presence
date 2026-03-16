using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The normal title screen camera.")]
    public Camera mainCamera;

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

    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, 
                targetCameraPosition, 
                titleUICanvasGroup, 
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
