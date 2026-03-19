using System.Collections;
using UnityEngine;

public class IntroDirector : MonoBehaviour
{
    public GameObject planet;

    private IEnumerator Start()
    {

        if (PlayerPrefs.GetInt("IntroTutorial", 0) == 0)
        {
            planet.transform.position = new Vector3(2870, 1328, 1748);

            GameObject.Find("Script1").GetComponent<DialogueTrigger>().Begin();
            yield return null;
            yield return new WaitUntil(() => DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive);
            yield return new WaitUntil(() => DialogueManager.Instance != null && !DialogueManager.Instance.isDialogueActive);

            yield return StartCoroutine(MoveTo(new Vector3(1201, 513, 480), 1f));

            GameObject.Find("Script2").GetComponent<DialogueTrigger>().Begin();
            yield return null;
            yield return new WaitUntil(() => DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive);
            yield return new WaitUntil(() => DialogueManager.Instance != null && !DialogueManager.Instance.isDialogueActive);

            yield return StartCoroutine(MoveTo(new Vector3(2870, 1328, 1748), 1f));

            Debug.Log("Intro sequence finished!");

            PlayerPrefs.SetInt("IntroTutorial", 1);

            GameObject.Find("SceneManager").GetComponent<SceneLoader>().LoadSpecificScene("TopScene");
        }
    }

    public IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        yield return StartCoroutine(MoveRoutine(targetPosition, duration));
    }

    private IEnumerator MoveRoutine(Vector3 target, float duration)
    {
        Vector3 start = planet.transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = 1 - Mathf.Pow(1 - t, 2);

            planet.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        planet.transform.position = target;
    }
}