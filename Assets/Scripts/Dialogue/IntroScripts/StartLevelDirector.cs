using System.Collections;
using UnityEngine;

public class StartLevelDirector : MonoBehaviour
{
    public Camera mainCamera;
    public Transform abilityCameraPosition;
    public CanvasGroup abilityUICanvasGroup;
    public GameObject abilityUI;
    public CanvasGroup titleUICanvasGroup;

    [SerializeField] private float transitionWaitTime = 2f;

    private IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("SimulationTutorial", 0) == 1 && PlayerPrefs.GetInt("FinalTutorial", 0) == 0)
        {
            Debug.Log("Last tutorial sequence Started!");

            mainCamera.transform.position = new Vector3(1000, 1000, 1000);
            abilityUI.SetActive(false);

            yield return StartCoroutine(CamTransitionToAbility());

            // wait long enough for the visual transition to actually finish
            yield return new WaitForSeconds(transitionWaitTime);

            Debug.Log("Camera transition to ability tutorial complete!");

            yield return new WaitUntil(() => DialogueManager.Instance != null);

            DialogueTrigger trigger = GameObject.Find("StartLevelScript1")?.GetComponent<DialogueTrigger>();
            if (trigger == null)
            {
                Debug.LogError("AbilityTutorialScript1 / DialogueTrigger not found.");
                yield break;
            }

            trigger.Begin();

            yield return null;
            yield return new WaitUntil(() => DialogueManager.Instance.isDialogueActive);
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);

            Debug.Log("Tutorial sequence finished!");

            PlayerPrefs.SetInt("FinalTutorial", 1);
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
