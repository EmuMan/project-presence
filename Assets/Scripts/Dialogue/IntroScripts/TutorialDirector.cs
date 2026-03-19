using System.Collections;
using UnityEngine;

public class TutorialDirector : MonoBehaviour
{
    public GameObject fightUI;
    public GameObject enemies;
    public GameObject player;

    private IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("SimulationTutorial", 0) == 0)
        {
            fightUI.SetActive(false);
            enemies.SetActive(false); 
            player.GetComponent<PlayerMovement>().enabled = false;
            // allow for dialogue

            GameObject.Find("Script1").GetComponent<DialogueTrigger>().Begin();
            yield return null;
            yield return new WaitUntil(() => DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive);
            yield return new WaitUntil(() => DialogueManager.Instance != null && !DialogueManager.Instance.isDialogueActive);

            Debug.Log("Simulation Tutorial completed!");

            PlayerPrefs.SetInt("SimulationTutorial", 1);
            fightUI.SetActive(true);
            enemies.SetActive(true);
            player.GetComponent<PlayerMovement>().enabled = true;
        }
    }
}
