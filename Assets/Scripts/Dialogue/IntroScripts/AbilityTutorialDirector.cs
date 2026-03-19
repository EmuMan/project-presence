using System.Collections;
using UnityEngine;

public class AbilityTutorialDirector : MonoBehaviour
{

    [Tooltip("Place the main camera here!")]
    public Camera mainCamera;

    [Tooltip("Place the coords for ability UI here.")]
    public Transform abilityCameraPosition;

    [Tooltip("Place the CanvasGroup for the ability UI elements here.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("Place the CanvasGroup for the title UI elements here.")]
    public CanvasGroup titleUICanvasGroup;


    private IEnumerator Start()
    {
        if ((PlayerPrefs.GetInt("IntroCompleted", 0) != 0) && (PlayerPrefs.GetInt("AbilityTutorialCompleted", 0) == 0))
        // important that if getint does not exist to set to 0 and wait till it is set to something different
        {
            GameObject.Find("mainCamera").GetComponent<Transform>().position = new Vector3(0, 0, 0);

            CamTransitionToAbility();

            GameObject.Find("AbilityTutorialScript1").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // waits until the dialogue is completed via the isDialogueActive boolean in DialogueManager
            // this dialogue is the player waking up and getting updated on where they are


            Debug.Log("Ability tutorial finished!");

            PlayerPrefs.SetInt("AbilityTutorialCompleted", 1);
            PlayerPrefs.Save();
            // sets a PlayerPref to indicate the ability tutorial has been completed, so it won't play again

        }
        
    }

    public void CamTransitionToAbility()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera, // The camera to move
                abilityCameraPosition, // The target position for the camera to move to
                titleUICanvasGroup, // The current UI to fade out 
                abilityUICanvasGroup, // The next UI to fade in
                60f // The FOV to set during the transition (optional, don't set to keep current FOV)
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
