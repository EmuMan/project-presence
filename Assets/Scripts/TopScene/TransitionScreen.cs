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
    [Tooltip("The CanvasGroup component attached to your Title Screen UI Panel.")]
    public CanvasGroup titleUI_CanvasGroup;

    [Tooltip("The next UI CanvasGroup to fade in after the camera transition (e.g., Ability Screen).")]
    public CanvasGroup nextUI_CanvasGroup;

    [Tooltip("How fast the UI fades out before the camera starts moving (or during).")]
    public float titleUIFadeDuration = 0.5f;

    [Tooltip("How fast the next UI fades in after the camera finishes moving.")]
    public float nextUIFadeDuration = 2f;

    private bool isTransitioning = false;

    void Awake()
    {
        // Make UIs invisible at the start
        if (titleUI_CanvasGroup != null)
        {
            titleUI_CanvasGroup.alpha = 0f;
            titleUI_CanvasGroup.interactable = false;
            titleUI_CanvasGroup.blocksRaycasts = false;
        }

        if (nextUI_CanvasGroup != null)
        {
            nextUI_CanvasGroup.alpha = 0f;
            nextUI_CanvasGroup.interactable = false;
            nextUI_CanvasGroup.blocksRaycasts = false;
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
        StartCoroutine(FadeInTitleUI());
    }

    private IEnumerator FadeInTitleUI()
    {
        if (titleUI_CanvasGroup == null) yield break;

        float fadeTime = 0f;
        while (fadeTime < titleUIFadeDuration)
        {
            fadeTime += Time.deltaTime;
            titleUI_CanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeTime / titleUIFadeDuration);
            yield return null;
        }

        titleUI_CanvasGroup.alpha = 1f;
        titleUI_CanvasGroup.interactable = true;
        titleUI_CanvasGroup.blocksRaycasts = true;
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
        if (titleUI_CanvasGroup != null)
        {
            // Disable interactions immediately so the user can't click things twice
            titleUI_CanvasGroup.interactable = false;
            titleUI_CanvasGroup.blocksRaycasts = false;

            float fadeTime = 0f;
            while (fadeTime < titleUIFadeDuration)
            {
                fadeTime += Time.deltaTime;
                // Lerp alpha from 1 (fully visible) to 0 (invisible)
                titleUI_CanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTime / titleUIFadeDuration);
                yield return null;
            }
            titleUI_CanvasGroup.alpha = 0f;
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
        if (nextUI_CanvasGroup != null)
        {
            // Fade in the next UI
            float fadeInTime = 0f;
            while (fadeInTime < nextUIFadeDuration)
            {
                fadeInTime += Time.deltaTime;
                nextUI_CanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeInTime / nextUIFadeDuration);
                yield return null;
            }
            nextUI_CanvasGroup.alpha = 1f;
            nextUI_CanvasGroup.interactable = true;
            nextUI_CanvasGroup.blocksRaycasts = true;
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
