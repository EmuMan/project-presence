using UnityEngine;
using UnityEngine.SceneManagement;

public class Billboard : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        // Starting from our location, look in the same direction as the camera
        transform.LookAt(transform.position + cam.transform.forward);

        // Alternative method:  This does not look good for this example
        //transform.LookAt(cam.transform);  // looks at camera but forward direction is on bock
        //transform.Rotate(0, 180, 0);  // so need to flip back around
    }
}
