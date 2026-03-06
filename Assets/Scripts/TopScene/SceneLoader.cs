using UnityEngine;
using UnityEngine.SceneManagement;


public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quit the game
    public void QuitGame()
    {
        // This will stop the game from running in the actual Unity Editor (stops play mode)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // This will quit the built, standalone application
            Application.Quit();
        #endif
    }
}
