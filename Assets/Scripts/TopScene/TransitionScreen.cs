using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Tooltip("How fast the UI fades out before the camera starts moving (or during).")]
    public float titleUIFadeDuration = 0.5f;

    [Tooltip("How fast the next UI fades in after the camera finishes moving.")]
    public float nextUIFadeDuration = 2f;

    [Header("Data Settings")]
    [Tooltip("The PlayerPrefs key used to check the game over state.")]
    public string gameOverPrefKey = "IsGameOver";

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
        bool isGameOver = PlayerPrefs.GetInt(gameOverPrefKey, 0) == 1;

        // Fade in the title UI when the game starts if it's a normal start,
        // or fade in the game over UI if they are coming from a game over state
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
        while (fadeTime < titleUIFadeDuration)
        {
            fadeTime += Time.deltaTime;
            currentUICanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeTime / titleUIFadeDuration);
            yield return null;
        }

        currentUICanvasGroup.alpha = 1f;
        currentUICanvasGroup.interactable = true;
        currentUICanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Highly modular method to transition any camera to any target, while fading specific UIs.
    /// </summary>
    /// <param name="camToMove">The specific camera you want to move.</param>
    /// <param name="target">The transform to move the camera to.</param>
    /// <param name="fadeOutUI">Optional: The UI CanvasGroup to fade out before moving.</param>
    /// <param name="fadeInUI">Optional: The UI CanvasGroup to fade in after moving.</param>
    /// <param name="overrideFOV">Optional: The FOV to transition to. Leave as 0 to keep current FOV.</param>
    public void StartCameraTransition(Camera camToMove, Transform target, CanvasGroup fadeOutUI = null, CanvasGroup fadeInUI = null, float overrideFOV = 0f)
    {
        if (!isTransitioning && camToMove != null && target != null)
        {
            StartCoroutine(TransitionRoutine(camToMove, target, fadeOutUI, fadeInUI, overrideFOV));
        }
        else if (camToMove == null || target == null)
        {
            Debug.LogWarning("StartCameraTransition failed: Camera or Target is null!");
        }
    }

    private IEnumerator TransitionRoutine(Camera camToMove, Transform target, CanvasGroup fadeOutUI, CanvasGroup fadeInUI, float expectedFOV)
    {
        isTransitioning = true;

        // 1. FADE OUT THE SPECIFIED UI
        if (fadeOutUI != null)
        {
            // Disable interactions immediately
            fadeOutUI.interactable = false;
            fadeOutUI.blocksRaycasts = false;

            float fadeOutTime = 0f;
            while (fadeOutTime < titleUIFadeDuration)
            {
                fadeOutTime += Time.deltaTime;
                fadeOutUI.alpha = Mathf.Lerp(1f, 0f, fadeOutTime / titleUIFadeDuration);
                yield return null;
            }
            fadeOutUI.alpha = 0f;
        }

        // 2. MOVE THE CAMERA
        Vector3 startPos = camToMove.transform.position;
        Quaternion startRot = camToMove.transform.rotation;
        float startCamFOV = camToMove.fieldOfView;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;
        float endCamFOV = expectedFOV > 0 ? expectedFOV : startCamFOV;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float percent = Mathf.Clamp01(elapsedTime / transitionDuration);
            float curvePercent = transitionCurve.Evaluate(percent);

            camToMove.transform.position = Vector3.Lerp(startPos, endPos, curvePercent);
            camToMove.transform.rotation = Quaternion.Slerp(startRot, endRot, curvePercent);
            camToMove.fieldOfView = Mathf.Lerp(startCamFOV, endCamFOV, curvePercent);

            yield return null;
        }

        // Snap to exactly the end position
        camToMove.transform.position = endPos;
        camToMove.transform.rotation = endRot;
        camToMove.fieldOfView = endCamFOV;

        // 3. FADE IN THE SPECIFIED UI
        if (fadeInUI != null)
        {
            float fadeInTime = 0f;
            while (fadeInTime < nextUIFadeDuration)
            {
                fadeInTime += Time.deltaTime;
                fadeInUI.alpha = Mathf.Lerp(0f, 1f, fadeInTime / nextUIFadeDuration);
                yield return null;
            }
            fadeInUI.alpha = 1f;
            fadeInUI.interactable = true;
            fadeInUI.blocksRaycasts = true;
            
            // Keep track of the newly activated UI
            currentUICanvasGroup = fadeInUI;
        }

        isTransitioning = false; // Allow future transitions
    }

    // Load Specific Scene
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
}
