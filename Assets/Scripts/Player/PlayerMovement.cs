using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;

    private CharacterController controller;
    private PlayerModules playerModules;

    public Vector3 velocity;

    private InputAction moveAction;
    private Vector3 movementInput;

    private bool isUsingController = false;
    private InputAction lookAction;
    public Vector3 lookDirection;
    private Vector2 lastMousePosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerModules = GetComponent<PlayerModules>();

        moveAction = InputSystem.actions.FindAction("Move");

        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        GetMovementInput();
        GetLookInput();
        /* get the general update as fast as you */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotatePlayerToTarget();
        ApplyGravity();
        ApplyMovementInput();
        MoveCharacter();
        /* on fixed update, use the stored input and act */
    }

    void OnDrawGizmos()
    {
        // This function is just for debug visualization
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + lookDirection * 5);
        }
    }

    private void GetMovementInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movementInput = new Vector3(input.x, 0.0f, input.y);
    }

    private void GetLookInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 controllerInput = lookAction.ReadValue<Vector2>();

        if (controllerInput != Vector2.zero)
        {
            isUsingController = true;
        }
        else if (mousePosition != lastMousePosition)
        {
            isUsingController = false;
        }
        lastMousePosition = mousePosition;

        // Keep track of this separately; if it's zero, we don't want to update the
        // actual look direction.
        Vector3 nextLookDirection = Vector3.zero;

        if (isUsingController)
        {
            // If there is nonzero input from the controller, use that.
            nextLookDirection = new Vector3(controllerInput.x, 0, controllerInput.y).normalized;
        }
        else
        {
            // Otherwise, use the mouse position to determine the target direction.
            // This assumes a top-down perspective where the player is on the XZ plane and the camera is looking down from above.
            // Create a ray from the camera through the mouse position and find where it intersects with the player's y-plane.
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                nextLookDirection = (hitPoint - transform.position).normalized;
            }
        }

        if (nextLookDirection != Vector3.zero)
        {
            lookDirection = nextLookDirection;
        }
    }

    private void RotatePlayerToTarget()
    {
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = targetRotation;
        }
    }

    private void ApplyGravity()
    {
        float fallDeltaSpeed = Physics.gravity.y * Time.fixedDeltaTime;
        velocity.y += fallDeltaSpeed;
    }

    private void ApplyMovementInput()
    {
        Vector3 horizontalVelocity = movementInput * speed * playerModules.GetTotalSpeedModifier();
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    private void MoveCharacter()
    {
        CollisionFlags moveResult = controller.Move(velocity * Time.fixedDeltaTime);
        if ((moveResult & CollisionFlags.Below) != 0)
        {
            velocity.y = 0; // Reset vertical velocity when grounded
        }
    }

    public bool IsGrounded()
    {
        return controller.isGrounded;
    }
}
