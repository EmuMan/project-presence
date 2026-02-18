using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModules : MonoBehaviour
{
    // Needed for look direction
    private PlayerMovement playerMovement;

    public Module headModule;
    public Module coreModule;
    public Module leftArmModule;
    public Module rightArmModule;
    public Module movementModule;

    public InputTracker headInputTracker = new InputTracker();
    public InputTracker coreInputTracker = new InputTracker();
    public InputTracker leftArmInputTracker = new InputTracker();
    public InputTracker rightArmInputTracker = new InputTracker();
    public InputTracker movementInputTracker = new InputTracker();

    public InputAction headInputAction;
    public InputAction coreInputAction;
    public InputAction leftArmInputAction;
    public InputAction rightArmInputAction;
    public InputAction movementInputAction;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Initialize modules with the player object
        headModule?.Initialize(gameObject);
        coreModule?.Initialize(gameObject);
        leftArmModule?.Initialize(gameObject);
        rightArmModule?.Initialize(gameObject);
        movementModule?.Initialize(gameObject);

        // Find input actions
        headInputAction = InputSystem.actions.FindAction("HeadAction");
        coreInputAction = InputSystem.actions.FindAction("CoreAction");
        leftArmInputAction = InputSystem.actions.FindAction("LeftArmAction");
        rightArmInputAction = InputSystem.actions.FindAction("RightArmAction");
        movementInputAction = InputSystem.actions.FindAction("MovementAction");
    }

    void Update()
    {
        // Update input trackers based on current input states
        headInputTracker.SetPressed(headInputAction?.IsPressed() ?? false);
        coreInputTracker.SetPressed(coreInputAction?.IsPressed() ?? false);
        leftArmInputTracker.SetPressed(leftArmInputAction?.IsPressed() ?? false);
        rightArmInputTracker.SetPressed(rightArmInputAction?.IsPressed() ?? false);
        movementInputTracker.SetPressed(movementInputAction?.IsPressed() ?? false);
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        Vector3 lookDirection = playerMovement.lookDirection;

        // Perform module actions if their inputs are active
        headModule?.PerformActionIfAvailable(headInputTracker.IsPressed(), deltaTime, lookDirection);
        coreModule?.PerformActionIfAvailable(coreInputTracker.IsPressed(), deltaTime, lookDirection);
        leftArmModule?.PerformActionIfAvailable(leftArmInputTracker.IsPressed(), deltaTime, lookDirection);
        rightArmModule?.PerformActionIfAvailable(rightArmInputTracker.IsPressed(), deltaTime, lookDirection);
        movementModule?.PerformActionIfAvailable(movementInputTracker.IsPressed(), deltaTime, lookDirection);
    }
}
