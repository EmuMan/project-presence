using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{
    [Header("Camera Transition Settings")]
    [Tooltip("How many seconds the camera movement should take.")]
    public float transitionDuration = 2.0f;

    [Tooltip("Animation curve for smooth easing. Adjust in Inspector for custom smoothing.")]
    public AnimationCurve transitionCurve;

    [Header("UI Fade Settings")]
    [Tooltip("The Title Screen UI CanvasGroup goes here.")]
    public CanvasGroup titleUICanvasGroup;

    [Tooltip("The Ability UI CanvasGroup goes here.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("The Game Over UI CanvasGroup goes here.")]
    public CanvasGroup gameOverUICanvasGroup;

    [Tooltip("The Options UI CanvasGroup goes here.")]
    public CanvasGroup optionsUICanvasGroup;

    [Tooltip("The Level UI CanvasGroup goes here.")]
    public CanvasGroup levelUICanvasGroup;

    [Tooltip("How fast the UI fades in/out before the camera starts moving (or during).")]
    public float currentUIFadeDuration = 0.5f;

    [Tooltip("How fast the next UI fades in after the camera finishes moving.")]
    public float nextUIFadeDuration = 2f;

    [Header("Data Settings")]
    [Tooltip("The PlayerPrefs key used to check the game over state.")]
    public string gameOverPrefKey = "IsGameOver";

    [Header("Blackout Settings")]
    [Tooltip("A CanvasGroup attached to a full-screen black panel.")]
    public CanvasGroup blackoutCanvasGroup;
    public float blackoutFadeDuration = 1f;

    private bool isTransitioning = false;
    private CanvasGroup currentUICanvasGroup;

    void Awake()
    {
        // Make UIs invisible at the start
        if (titleUICanvasGroup != null)
        {
            titleUICanvasGroup.alpha = 0f;
            titleUICanvasGroup.interactable = false;
            titleUICanvasGroup.blocksRaycasts = false;
        }

        if (abilityUICanvasGroup != null)
        {
            abilityUICanvasGroup.alpha = 0f;
            abilityUICanvasGroup.interactable = false;
            abilityUICanvasGroup.blocksRaycasts = false;
        }

        if (gameOverUICanvasGroup != null)
        {
            gameOverUICanvasGroup.alpha = 0f;
            gameOverUICanvasGroup.interactable = false;
            gameOverUICanvasGroup.blocksRaycasts = false;
        }

        if (optionsUICanvasGroup != null)
        {
            optionsUICanvasGroup.alpha = 0f;
            optionsUICanvasGroup.interactable = false;
            optionsUICanvasGroup.blocksRaycasts = false;
        }

        if (levelUICanvasGroup != null)
        {
            levelUICanvasGroup.alpha = 0f;
            levelUICanvasGroup.interactable = false;
            levelUICanvasGroup.blocksRaycasts = false;
        }

        // Initialize curve with proper smooth ease-out
        if (transitionCurve == null || transitionCurve.keys.Length == 0)
        {
            transitionCurve = new AnimationCurve();

            // Add keyframes
            Keyframe startKey = new Keyframe(0f, 0f);
            startKey.outTangent = 0f;
            startKey.weightedMode = WeightedMode.Both;
            startKey.outWeight = 0.3f;

            Keyframe endKey = new Keyframe(1f, 1f);
            endKey.inTangent = 0f;
            endKey.weightedMode = WeightedMode.Both;
            endKey.inWeight = 0.7f;

            transitionCurve.AddKey(startKey);
            transitionCurve.AddKey(endKey);
        }
    }


    void Start()
    {
        StartCoroutine(FadeFromBlack()); // Fade in from black 

        // Fade in the title UI when the game starts if it's a normal start,
        // or fade in the game over UI if they are coming from a game over state
        bool isGameOver = PlayerPrefs.GetInt(gameOverPrefKey, 0) == 1;
        if (!isGameOver)
        {
            StartCoroutine(FadeInUI(titleUICanvasGroup));
        }
        else
        {
            StartCoroutine(FadeInUI(gameOverUICanvasGroup));
        }
    }

    public IEnumerator FadeInUI(CanvasGroup currentUICanvasGroup)
    {
        if (currentUICanvasGroup == null) yield break;

        float fadeTime = 0f;
        while (fadeTime < currentUIFadeDuration)
        {
            fadeTime += Time.deltaTime;
            currentUICanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeTime / currentUIFadeDuration);
            yield return null;
        }

        currentUICanvasGroup.alpha = 1f;
        currentUICanvasGroup.interactable = true;
        currentUICanvasGroup.blocksRaycasts = true;

        // Focus the first button on the screen when the game starts
        FocusFirstButton(currentUICanvasGroup);
    }

    /// <summary>
    /// Highly modular method to transition any camera to any target, while fading specific UIs.
    /// </summary>
    /// <param name="camToMove">The specific camera you want to move.</param>
    /// <param name="target">The transform to move the camera to.</param>
    /// <param name="fadeOutUI">Optional: The UI CanvasGroup to fade out before moving.</param>
    /// <param name="fadeInUI">Optional: The UI CanvasGroup to fade in after moving.</param>
    /// <param name="overrideFOV">Optional: The FOV to transition to. Leave as 0 to keep current FOV.</param>
    /// <param name="useBlackout">If true, the screen fades to black, the camera snaps instantly, and fades back in.</param>
    public void StartCameraTransition(
        Camera camToMove, 
        Transform target, 
        CanvasGroup fadeOutUI = null, 
        CanvasGroup fadeInUI = null, 
        float overrideFOV = 0f, 
        bool useBlackout = false,
        string useLoadScene = null,
        GameObject specificFocusTarget = null // ADDED THIS PARAMETER
        )
    {
        if (!isTransitioning && camToMove != null && target != null)
        {
            StartCoroutine(TransitionRoutine(camToMove, target, fadeOutUI, fadeInUI, overrideFOV, useBlackout, useLoadScene, specificFocusTarget));
        }
        else if (camToMove == null || target == null)
        {
            Debug.LogWarning("StartCameraTransition failed: Camera or Target is null!");
        }
    }

    private IEnumerator TransitionRoutine(
        Camera camToMove, 
        Transform target, 
        CanvasGroup fadeOutUI, 
        CanvasGroup fadeInUI, 
        float expectedFOV, 
        bool useBlackout,
        string useLoadScene = null,
        GameObject specificFocusTarget = null // ADDED THIS PARAMETER
        )
    {
        isTransitioning = true;

        // 1. FADE OUT THE SPECIFIED UI
        if (fadeOutUI != null)
        {
            fadeOutUI.interactable = false;
            fadeOutUI.blocksRaycasts = false;
        }

        if (useBlackout && blackoutCanvasGroup != null)
        {
            blackoutCanvasGroup.blocksRaycasts = true; // Prevent clicking while we are going to black
        }

        float fadeOutTime = 0f;
        while (fadeOutTime < currentUIFadeDuration)
        {
            fadeOutTime += Time.deltaTime;
            float lerpVal = fadeOutTime / currentUIFadeDuration;

            // Only fade out the UI right now, leave the blackout canvas alone (alpha 0)
            if (fadeOutUI != null) fadeOutUI.alpha = Mathf.Lerp(1f, 0f, lerpVal);
            yield return null;
        }
        
        if (fadeOutUI != null) fadeOutUI.alpha = 0f;

        // 2. MOVE THE CAMERA (And Fade to Black at the end)
        float startCamFOV = camToMove.fieldOfView;
        float endCamFOV = expectedFOV > 0 ? expectedFOV : startCamFOV;

        Vector3 startPos = camToMove.transform.position;
        Quaternion startRot = camToMove.transform.rotation;

        float elapsedTime = 0f;
        
        // Calculate when we should start making the screen go black
        // E.g., if total transition is 3s, and blackout duration is 1s, we start at 2s.
        // We use Mathf.Max to ensure it doesn't go below 0 if blackoutFadeDuration is longer than the camera move.
        float startBlackoutTime = Mathf.Max(0f, transitionDuration - blackoutFadeDuration);

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / transitionDuration);
            float curvePercent = transitionCurve.Evaluate(percent);

            // Move Camera
            camToMove.transform.position = Vector3.Lerp(startPos, target.position, curvePercent);
            camToMove.transform.rotation = Quaternion.Slerp(startRot, target.rotation, curvePercent);
            camToMove.fieldOfView = Mathf.Lerp(startCamFOV, endCamFOV, curvePercent);

            // Handle Blackout Logic based on time left
            if (useBlackout && blackoutCanvasGroup != null)
            {
                if (elapsedTime >= startBlackoutTime)
                {
                    // Calculate how far along the blackout segment we are (0 to 1)
                    float blackoutPercent = (elapsedTime - startBlackoutTime) / blackoutFadeDuration;
                    blackoutCanvasGroup.alpha = Mathf.Lerp(0f, 1f, blackoutPercent);
                }
            }

            yield return null;
        }

        // Snap to exactly the target end position
        camToMove.transform.position = target.position;
        camToMove.transform.rotation = target.rotation;
        camToMove.fieldOfView = endCamFOV;

        // Ensure it is fully black if enabled
        if (useBlackout && blackoutCanvasGroup != null) 
        {
            blackoutCanvasGroup.alpha = 1f;
        }

        // 3. FADE IN THE SPECIFIED UI (and fade back from black if we didn't load a scene)
        float fadeInTime = 0f;
        while (fadeInTime < nextUIFadeDuration)
        {
            fadeInTime += Time.deltaTime;
            float lerpVal = fadeInTime / nextUIFadeDuration;

            if (fadeInUI != null) fadeInUI.alpha = Mathf.Lerp(0f, 1f, lerpVal);
            
            // Only fade back from black if we aren't about to load a new scene
            // (If we load a scene, we want it to stay black until the new scene starts)
            if (useBlackout && blackoutCanvasGroup != null && string.IsNullOrEmpty(useLoadScene)) 
            {
                blackoutCanvasGroup.alpha = Mathf.Lerp(1f, 0f, lerpVal);
            }

            yield return null;
        }

        if (fadeInUI != null)
        {
            fadeInUI.alpha = 1f;
            fadeInUI.interactable = true;
            fadeInUI.blocksRaycasts = true;
            currentUICanvasGroup = fadeInUI;

            // NEW FOCUS LOGIC:
            if (specificFocusTarget != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(specificFocusTarget);
            }
            else
            {
                // Fallback to the old method if we didn't specify one
                FocusFirstButton(fadeInUI);
            }
        }
        
        // Clean up the blackout canvas state
        if (useBlackout && blackoutCanvasGroup != null && string.IsNullOrEmpty(useLoadScene))
        {
            blackoutCanvasGroup.alpha = 0f;
            blackoutCanvasGroup.blocksRaycasts = false;
        }

        if (!string.IsNullOrEmpty(useLoadScene)) 
        {
            // Load the next scene after the transition (while still black)
            SceneManager.LoadScene(useLoadScene);
        }

        isTransitioning = false;
    }

    // Basic Load Scene function
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// Hook this up to a Button's OnClick event to "Restart".
    /// </summary>
    public void ReloadCurrentScene()
    {
        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Load it again by its Build Index
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Quit the game
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Fades the screen to black, switches the scene, then the new scene takes over.
    /// </summary>
    public void LoadSceneWithBlackout(string sceneName)
    {
        StartCoroutine(BlackoutAndLoadRoutine(sceneName));
    }

    private IEnumerator BlackoutAndLoadRoutine(string sceneName)
    {
        // 1. Fade to black
        if (blackoutCanvasGroup != null)
        {
            blackoutCanvasGroup.blocksRaycasts = true; // Prevent clicking during fade
            float time = 0f;
            while (time < blackoutFadeDuration)
            {
                time += Time.deltaTime;
                blackoutCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / blackoutFadeDuration);
                yield return null;
            }
            blackoutCanvasGroup.alpha = 1f;
        }

        // 2. Load the scene
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Fades the black screen away (useful to call in Start() of a new scene)
    /// </summary>
    public IEnumerator FadeFromBlack()
    {
        if (blackoutCanvasGroup != null)
        {
            blackoutCanvasGroup.alpha = 1f;
            float time = 0f;
            while (time < blackoutFadeDuration)
            {
                time += Time.deltaTime;
                blackoutCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / blackoutFadeDuration);
                yield return null;
            }
            blackoutCanvasGroup.alpha = 0f;
            blackoutCanvasGroup.blocksRaycasts = false; // Allow clicking again
        }
    }

    private void FocusFirstButton(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        // Find the first Selectable (Button, Slider, etc.) that is active in the CanvasGroup
        Selectable firstSelectable = canvasGroup.GetComponentInChildren<Selectable>();
        if (firstSelectable != null)
        {
            // Clear current selection
            EventSystem.current.SetSelectedGameObject(null);
            // Select the new UI element
            firstSelectable.Select();
        }
    }
}
