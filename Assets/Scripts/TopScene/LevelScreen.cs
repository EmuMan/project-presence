using UnityEngine;

public class LevelScreen : MonoBehaviour
{
    // This script is a template for setting up a scene with camera transitions and UI elements!
    // You can use this as a starting point for creating new scenes with similar functionality, and
    // customize the camera positions, UI elements, and transition settings as needed.
    [Header("Cameras")]
    [Tooltip("Place the main camera here!")]
    public Camera mainCamera;

    [Header("Transition Settings")]

    [Tooltip("Place the CanvasGroup for the ability UI elements here.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("Place the CanvasGroup for the level UI elements here.")]
    public CanvasGroup levelUICanvasGroup;

    [Tooltip("Place the coords for ability UI here.")]
    public Transform abilityCameraPosition;

    [Tooltip("Place the coords for level UI here.")]
    public Transform levelCameraPosition;

    [Tooltip("The coords to get into game.")]
    public Transform deployCameraPosition;

    // Example method to trigger the camera transition to the ability UI
    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, // The camera to move
                abilityCameraPosition, // The target position for the camera to move to
                levelUICanvasGroup, // The current UI to fade out 
                abilityUICanvasGroup, // The next UI to fade in
                60f // The FOV to set during the transition (optional, don't set to keep current FOV)
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }

    // Example method to trigger the camera transition to another part of the scene,
    // with an optional blackout and load scene parameters for transitioning to a new scene after the camera transition is complete.
    public void CamTransitionToGame(string useLoadScene)
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                deployCameraPosition,
                levelUICanvasGroup,
                abilityUICanvasGroup,
                60f,
                true, // Use blackout during the transition (default is false)
                useLoadScene // The name of the scene to load after the transition is complete (optional, don't set to not load a new scene)
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
