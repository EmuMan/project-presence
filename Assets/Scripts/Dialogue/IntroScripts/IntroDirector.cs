using System.Collections;
using UnityEngine;

public class IntroDirector : MonoBehaviour
{

    public GameObject planet;

    private IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("IntroCompleted", 0) == 0)
        {
            planet.transform.position = new Vector3(2870, 1328, 1748);

            GameObject.Find("Script1").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // waits until the dialogue is completed via the isDialogueActive boolean in DialogueManager
            // this dialogue is the player waking up and getting updated on where they are

            MoveTo(new Vector3(1201, 513, 480), 1);
            // all to move the planet into view

            GameObject.Find("Script2").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // this script is Trick being shown the world they are set to free

            Debug.Log("Intro sequence finished!");

            PlayerPrefs.SetInt("IntroCompleted", 1);
            PlayerPrefs.Save();
            // sets a PlayerPref to indicate the intro has been completed, so it won't play again

            GameObject.Find("SceneManager").GetComponent<SceneLoader>().LoadSpecificScene("TopScene");
        }
    }




    public void MoveTo(Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveRoutine(targetPosition, duration));
    }
    // to move the planet into the camera view

    private IEnumerator MoveRoutine(Vector3 target, float duration)
    {
        Vector3 start = planet.transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Ease Out (quadratic)
            t = 1 - Mathf.Pow(1 - t, 2);

            planet.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        planet.transform.position = target;
    }

}
