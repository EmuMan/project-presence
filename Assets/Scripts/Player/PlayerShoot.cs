using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private InputAction shootAction;
    private InputAction lookAction;

    private Vector3 lastShootPosition = new Vector3(1, 1, 0);
    private InputTracker shootInputTracker = new InputTracker();
    private RepeatingAction shootRepeatingAction = new RepeatingAction(0.5f);

    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
        shootAction.Enable();

        lookAction = InputSystem.actions.FindAction("Look");
        lookAction.Enable();
    }

    public void Update()
    {
        RequestShootIfPressed();
        /* as fast as the computer is, get vector for shoot direction */
    }

    public void FixedUpdate()
    {
        MaybeShoot();
        /* if pressing the button, shoot at given vector */
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 mousePos = GetMouseWorldPosition(Camera.main, transform.position.y);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mousePos, 0.5f);
            Gizmos.DrawLine(transform.position, mousePos);
        }
        /* visual debugging to show where the player is aiming */
    }

    public void RequestShootIfPressed()
    {
        if (shootInputTracker.SetPressed(shootAction.IsPressed()))
        {
            // lastShootPosition = GetMouseWorldPosition(Camera.main, transform.position.y);
            Vector2 dummy = lookAction.ReadValue<Vector2>();
            Vector2 test = new Vector2(0,0);
           
            Debug.Log(dummy);
            Debug.Log(test);
            
            if (dummy != test)
            {
                lastShootPosition = new Vector3(dummy.x + shootPoint.position.x, shootPoint.position.y, dummy.y + shootPoint.position.z);
                Debug.Log("bruh");
            }
        }
        /* get direction  */
    }

    public Vector3 GetMouseWorldPosition(Camera camera, float planeY)
    {
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Create a plane at the player's Y level, facing up
        Plane plane = new Plane(Vector3.up, new Vector3(0, planeY, 0));

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero; // Fallback if ray doesn't hit plane
    }

    public void ShootAtPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - shootPoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<BasicProjectile>().Initialize(direction);
    }

    public void MaybeShoot()
    {
        if (shootRepeatingAction.IsActing(shootInputTracker.GetPressed(), true, Time.fixedDeltaTime))
        {
            ShootAtPosition(lastShootPosition);
        }
    }
}
