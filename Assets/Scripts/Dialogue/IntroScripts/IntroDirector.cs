using System.Collections;
using UnityEngine;

public class IntroDirector : MonoBehaviour
{

    public GameObject planet;

    public bool moveIn = true;

    private IEnumerator Start()
    {
        PlayerPrefs.SetInt("IntroTutorial", 0);
        // MAKE SURE TO DELETE THE ABOVE STATEMENT, ONLY FOR TESTING

        if (PlayerPrefs.GetInt("IntroTutorial", 0) == 0)
        {
            planet.transform.position = new Vector3(2870, 1328, 1748);

            GameObject.Find("Script1").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // waits until the dialogue is completed via the isDialogueActive boolean in DialogueManager
            // this dialogue is the player waking up and getting updated on where they are

            MoveTo(new Vector3(1201, 513, 480), 1);
            yield return new WaitUntil(() => !moveIn);
            // all to move the planet into view

            GameObject.Find("Script2").GetComponent<DialogueTrigger>().Begin();
            yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
            // this script is Trick being shown the world they are set to free

            MoveTo(new Vector3(2870, 1328, 1748), 1);
            yield return new WaitUntil(() => !moveIn);

            Debug.Log("Intro sequence finished!");

            PlayerPrefs.SetInt("IntroTutorial", 1);

            GameObject.Find("SceneManager").GetComponent<SceneLoader>().LoadSpecificScene("TopScene");
        }
    }





    public void MoveTo(Vector3 targetPosition, float duration)
    {
        moveIn = true;
        StartCoroutine(MoveRoutine(targetPosition, duration));
        moveIn = false;
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
