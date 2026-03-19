using UnityEngine;

public class OptionsScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("Place the main camera here!")]
    public Camera mainCamera;

    [Header("Transition Settings")]

    [Tooltip("Place the CanvasGroup for the title UI elements here.")]
    public CanvasGroup titleUICanvasGroup;

    [Tooltip("Place the CanvasGroup for the options UI elements here.")]
    public CanvasGroup optionsUICanvasGroup;

    [Tooltip("Place the coords for options UI here.")]
    public Transform optionsCameraPosition;

    [Tooltip("Place the coords for title UI here.")]
    public Transform titleCameraPosition;

    // Transitions the camera back to the start menu position
    public void CamTransitionToStart()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, // The camera to move
                titleCameraPosition, // The target position for the camera to move to
                optionsUICanvasGroup, // The current UI to fade out 
                titleUICanvasGroup, // The next UI to fade in
                50f // The FOV to set during the transition
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }

    public void HuangsButton()     {
        // Add code here to enable God Mode in game
    }

    public void ControllerConfig()
    {
        // Add code here to change the controller configuration

    }
}
