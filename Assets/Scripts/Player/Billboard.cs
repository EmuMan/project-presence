using UnityEngine;
using UnityEngine.SceneManagement;

public class Billboard : MonoBehaviour
{
    Camera cam;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This is needed because this script is attached to the Player with used DontDestroyOnLoad().
    // Since the gameobject this script is attached to is a child of the Player, it will also persist
    // between scenes.  This means that Awake() will only be called once for the first scence.  This
    // method is needed to reattach to the main camera of the new scence that was just loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        cam = Camera.main;
    }

    //void Awake()
    //{
    //    cam = Camera.main;
    //}

    void LateUpdate()
    {
        // Starting from our location, look in the same direction as the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Alternative method:  This does not look good for this example
        //transform.LookAt(cam.transform);  // looks at camera but forward direction is on bock
        //transform.Rotate(0, 180, 0);  // so need to flip back around
    }
}
