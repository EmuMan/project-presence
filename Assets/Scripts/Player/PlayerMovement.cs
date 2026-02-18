using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum JumpState
    {
        Grounded,
        Jumping,
        Rising,
        Falling
    }

    public float speed = 5.0f;
    public float jumpStrength = 5.0f;
    public float jumpBufferTime = 0.2f;
    public float jumpCoyoteTime = 0.2f;
    public float extraGroundCheckDistance = 0.05f;
    public float fallMultiplier = 2.0f;

    public JumpState jumpState = JumpState.Grounded;

    private CharacterController controller;

    public Vector3 velocity;

    private InputAction moveAction;
    private Vector3 movementInput;

    private InputAction jumpAction;
    private InputTracker jumpInputTracker = new InputTracker();
    private BufferedAction jumpBufferedAction;
    private float lastGroundedTime = 0.0f;
    private bool canJump = true;

    private bool isUsingController = false;
    private InputAction lookAction;
    public Vector3 lookDirection;
    private Vector2 lastMousePosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");

        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpBufferedAction = new BufferedAction(jumpBufferTime);
        /* this is a custom function where you give it a buffer time of how early
        you can press jump, and then give it a duration time for how long you can hold it */

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
        JumpCharacter();
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

    void GetMovementInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movementInput = new Vector3(input.x, 0.0f, input.y);

        jumpInputTracker.SetPressed(jumpAction.IsPressed());
    }

    void GetLookInput()
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

    void RotatePlayerToTarget()
    {
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = targetRotation;
        }
    }

    void ApplyGravity()
    {
        if (jumpState != JumpState.Grounded)
        {
            float fallDeltaSpeed = Physics.gravity.y * Time.fixedDeltaTime;
            if (jumpState == JumpState.Falling || jumpState == JumpState.Rising)
            {
                fallDeltaSpeed *= fallMultiplier;
            }
            velocity.y += fallDeltaSpeed;
        }
        else
        {
            velocity.y = 0.0f;
        }
    }

    void ApplyMovementInput()
    {
        Vector3 horizontalVelocity = movementInput * speed;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    void MoveCharacter()
    {
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    void JumpCharacter()
    {
        if (Time.time - lastGroundedTime > jumpCoyoteTime)
        {
            // Coyote time expired
            canJump = false;
        }

        if (jumpBufferedAction.IsActing(jumpInputTracker.IsPressed(), canJump))
        {
            // These lines of code will run during the full time the player is jumping.
            velocity.y = jumpStrength;
            jumpState = JumpState.Jumping;
            // Invalidate coyote time after jumping
            lastGroundedTime = -Mathf.Infinity;
        }
        else
        {
            if (IsGrounded())
            {
                jumpState = JumpState.Grounded;
                lastGroundedTime = Time.time;
                canJump = true;
            }
            else if (velocity.y > 0)
            {
                jumpState = JumpState.Rising;
            }
            else
            {
                jumpState = JumpState.Falling;
            }
        }
    }

    bool IsGrounded()
    {
        if (controller.isGrounded)
        {
            return true;
        }

        // Get the bottom center of the character capsule
        float radius = controller.radius;
        float height = controller.height;
        Vector3 center = controller.transform.position + controller.center;

        // Cast a sphere slightly below the character
        float castDistance = height / 2f - radius + controller.skinWidth;

        return Physics.SphereCast(
            center,
            radius * 0.99f,  // Slightly smaller radius to avoid edge cases
            Vector3.down,
            out RaycastHit hit,
            castDistance + extraGroundCheckDistance,
            LayerMask.GetMask("Terrain"),
            QueryTriggerInteraction.Ignore
        );
    }
}
