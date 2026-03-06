using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    [Header("Camera Transition Settings")]
    [Tooltip("The camera that will move.")]
    public Camera mainCamera;
    
    [Tooltip("The Transform (Empty GameObject) representing where the camera should move to.")]
    public Transform targetCameraPosition;
    
    [Tooltip("How many seconds the camera movement should take.")]
    public float transitionDuration = 2.0f;
    
    [Tooltip("Animation curve for smooth easing in and out.")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("UI Fade Settings")]
    [Tooltip("The CanvasGroup component attached to your Title Screen UI Panel.")]
    public CanvasGroup titleUI_CanvasGroup;
    
    [Tooltip("How fast the UI fades out before the camera starts moving (or during).")]
    public float uiFadeDuration = 0.5f;

    private bool isTransitioning = false;

    void Start()
    {
        // Fallback to the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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
            while (fadeTime < uiFadeDuration)
            {
                fadeTime += Time.deltaTime;
                // Lerp alpha from 1 (fully visible) to 0 (invisible)
                titleUI_CanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTime / uiFadeDuration);
                yield return null;
            }
            titleUI_CanvasGroup.alpha = 0f;
        }

        // 2. MOVE THE CAMERA
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        Vector3 endPos = targetCameraPosition.position;
        Quaternion endRot = targetCameraPosition.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float percent = elapsedTime / transitionDuration;
            float curvePercent = transitionCurve.Evaluate(percent);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, curvePercent);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, curvePercent);

            yield return null;
        }

        // Snap to exactly the end position
        mainCamera.transform.position = endPos;
        mainCamera.transform.rotation = endRot;
        
        // TODO: Call your next function here (e.g., fading in the Ability Screen UI)
    }
}
