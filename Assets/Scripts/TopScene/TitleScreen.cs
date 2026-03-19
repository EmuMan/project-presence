using UnityEngine;
using UnityEngine.EventSystems; // Add this namespace

public class TitleScreen : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The main camera.")]
    public Camera mainCamera;

    [Header("UI Navigation")]

    [Header("UI Navigation Targets")]
    [Tooltip("The first button that should be highlighted when the Ability screen opens (e.g., the Core or Head button).")]
    public GameObject firstAbilityButton;

    [Tooltip("The first button that should be highlighted when the Options screen opens.")]
    public GameObject firstOptionsButton;

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

    [Tooltip("The coords to get into game.")]
    public Transform deployCameraPosition;


    [Tooltip("The coords for title UI.")]
    public Transform titleCameraPosition;
    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            // Pass the target to the transition (add false and null to skip the blackout/scene load arguments)
            if (PlayerPrefs.GetInt("IntroTutorial", 0) == 0){
                deployCameraPosition.position = new Vector3(1000, 1000, 1000);
                CamTransitionToGame("Intro!!!");
            }
            else{
                 transitionScreen.StartCameraTransition(
                    mainCamera, 
                    abilityCameraPosition, 
                    titleUICanvasGroup, 
                    abilityUICanvasGroup, 
                    60f,
                    false,
                    null,
                    firstAbilityButton // <- WE PASS IT HERE!
                );
            }
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
                50f,
                false,
                null
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }


    public void CamTransitionToGame(string useLoadScene)
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                deployCameraPosition,
                abilityUICanvasGroup,
                titleUICanvasGroup,
                60f,
                true,
                useLoadScene
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
