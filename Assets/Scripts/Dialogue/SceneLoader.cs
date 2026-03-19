using UnityEngine;
using UnityEngine.SceneManagement; // Required namespace

public class SceneLoader : MonoBehaviour
{
    // Public function to change the scene
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Loads the scene by its name
    }

    // Alternative: Public function to load the next scene in the build order
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}