using UnityEngine;

public class PlanetSpin : MonoBehaviour
{
    [Tooltip("Speed of rotation in degrees per second.")]
    public Vector3 rotationSpeed = new Vector3(0f, 15f, 0f);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
