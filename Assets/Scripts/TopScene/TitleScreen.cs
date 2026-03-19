using UnityEngine;
using UnityEngine.EventSystems; // Add this namespace

public class TitleScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The main camera.")]
    public Camera mainCamera;

    [Header("UI Navigation")]

    [Header("Transition Settings")]
    [Tooltip("The CanvasGroup for the options UI elements.")]
    public CanvasGroup optionsUICanvasGroup;

    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("The CanvasGroup for the title UI elements.")]
    public CanvasGroup titleUICanvasGroup;

    [Tooltip("The coords for ability UI.")]
    public Transform abilityCameraPosition;

    [Tooltip("The coords for options UI.")]
    public Transform optionsCameraPosition;

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

    public void CamTransitionToOptions()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, 
                optionsCameraPosition, 
                titleUICanvasGroup, 
                optionsUICanvasGroup, 
                50f
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
