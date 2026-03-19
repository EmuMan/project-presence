using System.Collections;
using UnityEngine;

public class AbilityTutorialDirector : MonoBehaviour
{
    public Camera mainCamera;
    public Transform abilityCameraPosition;
    public CanvasGroup abilityUICanvasGroup;
    public GameObject abilityUI;
    public CanvasGroup titleUICanvasGroup;

    [SerializeField] private float transitionWaitTime = 2f;

    private IEnumerator Start()
    {

        PlayerPrefs.SetInt("AbilityTutorial", 0);
        // MAKE SURE TO DELETE THE ABOVE STATEMENT, ONLY FOR TESTING

        if ((PlayerPrefs.GetInt("IntroTutorial", 0) == 1) && (PlayerPrefs.GetInt("AbilityTutorial", 0) == 0))
        {
            Debug.Log("Ability tutorial sequence Started!");

            mainCamera.transform.position = new Vector3(1000, 1000, 1000);
            abilityUI.SetActive(false);

            yield return StartCoroutine(CamTransitionToAbility());

            // wait long enough for the visual transition to actually finish
            yield return new WaitForSeconds(transitionWaitTime);

            Debug.Log("Camera transition to ability tutorial complete!");

            yield return new WaitUntil(() => DialogueManager.Instance != null);

            DialogueTrigger trigger = GameObject.Find("AbilityTutorialScript1")?.GetComponent<DialogueTrigger>();
            if (trigger == null)
            {
                Debug.LogError("AbilityTutorialScript1 / DialogueTrigger not found.");
                yield break;
            }

            trigger.Begin();

            yield return null;
            yield return new WaitUntil(() => DialogueManager.Instance.isDialogueActive);
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);

            Debug.Log("Ability tutorial sequence finished!");

            PlayerPrefs.SetInt("AbilityTutorial", 1);
            abilityUI.SetActive(true);
        }
    }

    public IEnumerator CamTransitionToAbility()
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

        yield return null;
    }
}