using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private InputAction shootAction;

    private Vector3 lastShootPosition;
    private InputTracker shootInputTracker = new InputTracker();
    private RepeatingAction shootRepeatingAction = new RepeatingAction(0.5f);

    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Attack");
        shootAction.Enable();
    }

    public void Update()
    {
        RequestShootIfPressed();
    }

    public void FixedUpdate()
    {
        MaybeShoot();
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
    }

    public void RequestShootIfPressed()
    {
        if (shootInputTracker.SetPressed(shootAction.IsPressed()))
        {
            lastShootPosition = GetMouseWorldPosition(Camera.main, transform.position.y);
        }
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
        if (shootRepeatingAction.IsActing(shootInputTracker.IsPressed(), true, Time.fixedDeltaTime))
        {
            ShootAtPosition(lastShootPosition);
        }
    }
}
