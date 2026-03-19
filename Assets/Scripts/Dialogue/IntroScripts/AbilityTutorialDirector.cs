using System.Collections;
using UnityEngine;

public class AbilityTutorialDirector : MonoBehaviour
{

    public Camera mainCamera;
    
    public Transform abilityCameraPosition;

    public CanvasGroup abilityUICanvasGroup;

    public CanvasGroup titleUICanvasGroup;

    private IEnumerator Start()
    {
         PlayerPrefs.SetInt("AbilityTutorial", 0);
        // MAKE SURE TO DELETE THE ABOVE STATEMENT, ONLY FOR TESTING

        if ((PlayerPrefs.GetInt("IntroTutorial", 0) == 1) && (PlayerPrefs.GetInt("AbilityTutorial", 0) == 0))
        {
            Debug.Log("Ability tutorial sequence Started!");

            mainCamera.transform.position = new Vector3(0, 0, 0);

            CamTransitionToAbility();

            GameObject.Find("AbilityTutorialScript1").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // waits until the dialogue is completed via the isDialogueActive boolean in DialogueManager
            // this dialogue is the player waking up and getting updated on where they are


            Debug.Log("Ability tutorial sequence finished!");

            PlayerPrefs.SetInt("AbilityTutorial", 1);  
            GameObject.Find("SceneManager").GetComponent<SceneLoader>().LoadSpecificScene("TopScene");
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
