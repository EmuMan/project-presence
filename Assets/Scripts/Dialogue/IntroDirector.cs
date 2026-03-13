using System.Collections;
using UnityEngine;

public class IntroDirector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        GameObject.Find("Script1").GetComponent<DialogueTrigger>().Begin();
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
        // waits until the dialogue is completed via the isDialogueActive boolean in DialogueManager

        GameObject.Find("Script2").GetComponent<DialogueTrigger>().Begin();
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);

        Debug.Log("Intro sequence finished!");
    }
    
}
