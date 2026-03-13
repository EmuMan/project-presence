using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{
    [Header("Camera Transition Settings")]
    [Tooltip("The camera that will move.")]
    public Camera mainCamera;
    [Tooltip("The Transform (Empty GameObject) representing where the camera should move to.")]
    public Transform targetCameraPosition;
    
    [Tooltip("How many seconds the camera movement should take.")]
    public float transitionDuration = 2.0f;

    [Tooltip("Animation curve for smooth easing. Adjust in Inspector for custom smoothing.")]
    public AnimationCurve transitionCurve;

    [Tooltip("Set target FOV if you want to change it during the transition. Leave as 0 to keep current FOV.")]
    public float targetFOV = 0f;

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
        // Fallback to the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Fade in the title UI when the game starts
        currentUICanvasGroup = titleUICanvasGroup;
        StartCoroutine(FadeInUI());
    }

    private IEnumerator FadeInUI()
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
    /// Call this method from your UI Button's OnClick event!
    /// </summary>
    public void StartCameraTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        // 1. FADE OUT THE UI
        if (titleUICanvasGroup != null)
        {
            // Disable interactions immediately so the user can't click things twice
            titleUICanvasGroup.interactable = false;
            titleUICanvasGroup.blocksRaycasts = false;

            float fadeTime = 0f;
            while (fadeTime < titleUIFadeDuration)
            {
                fadeTime += Time.deltaTime;
                // Lerp alpha from 1 (fully visible) to 0 (invisible)
                titleUICanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTime / titleUIFadeDuration);
                yield return null;
            }
            titleUICanvasGroup.alpha = 0f;
        }

        // 2. MOVE THE CAMERA
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float startFOV = mainCamera.fieldOfView;

        Vector3 endPos = targetCameraPosition.position;
        Quaternion endRot = targetCameraPosition.rotation;
        float endFOV = targetFOV > 0 ? targetFOV : startFOV;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            float percent = Mathf.Clamp01(elapsedTime / transitionDuration);
            float curvePercent = transitionCurve.Evaluate(percent);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, curvePercent);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, curvePercent);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, curvePercent);

            yield return null;
        }

        // Snap to exactly the end position
        mainCamera.transform.position = endPos;
        mainCamera.transform.rotation = endRot;
        mainCamera.fieldOfView = endFOV;

        // Fading in the Ability Screen UI
        if (abilityUICanvasGroup != null)
        {
            // Fade in the next UI
            float fadeInTime = 0f;
            while (fadeInTime < nextUIFadeDuration)
            {
                fadeInTime += Time.deltaTime;
                abilityUICanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeInTime / nextUIFadeDuration);
                yield return null;
            }
            abilityUICanvasGroup.alpha = 1f;
            abilityUICanvasGroup.interactable = true;
            abilityUICanvasGroup.blocksRaycasts = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quit the game
    public void QuitGame()
    {
            // This will stop the game from running in the actual Unity Editor (stops play mode)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // This will quit the built, standalone application
            Application.Quit();
        #endif
    }
}
