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

    private Vector3 velocity;

    private InputAction moveAction;
    private Vector3 movementInput;

    private InputAction jumpAction;
    private InputTracker jumpInputTracker = new InputTracker();
    private BufferedAction jumpBufferedAction;
    private float lastGroundedTime = 0.0f;
    private bool canJump = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");

        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpBufferedAction = new BufferedAction(jumpBufferTime);
        /* this is a custom function where you give it a buffer time of how early
        you can press jump, and then give it a duration time for how long you can hold it */
    }

    void Update()
    {
        GetInput();
        /* get the general update as fast as you */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyGravity();
        ApplyMovementInput();
        MoveCharacter();
        JumpCharacter();
        /* on fixed update, use the stored input and act */
    }

    void GetInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movementInput = new Vector3(input.x, 0.0f, input.y);
        jumpInputTracker.SetPressed(jumpAction.IsPressed());
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
        Vector3 rotatedMove = transform.TransformDirection(movementInput);
        Vector3 horizontalVelocity = rotatedMove * speed;
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

        if (jumpBufferedAction.IsActing(jumpInputTracker.GetPressed(), canJump))
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
