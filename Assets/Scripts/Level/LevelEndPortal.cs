using UnityEngine;

public class LevelEndPortal : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "TopScene";

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player entered level end portal trigger: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            // TODO: set whatever variables need to be set

            // Transition to the next level or show the end screen
            TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
            transitionScreen.LoadSceneWithBlackout(nextSceneName);
        }
    }
}
