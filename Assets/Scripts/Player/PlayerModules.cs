using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerModules : MonoBehaviour
{
    // Needed for look direction
    private PlayerMovement playerMovement;

    [Header("Module Locations")]
    [SerializeField] public Transform headTransform;
    [SerializeField] public Transform coreTransform;
    [SerializeField] public Transform leftArmTransform;
    [SerializeField] public Transform rightArmTransform;
    [SerializeField] public Transform movementTransform;

    [Header("Module Data")]
    [SerializeField] public ModuleData headModuleData;
    [SerializeField] public ModuleData coreModuleData;
    [SerializeField] public ModuleData leftArmModuleData;
    [SerializeField] public ModuleData rightArmModuleData;
    [SerializeField] public ModuleData movementModuleData;

    [Header("Module Status UI")]
    [SerializeField] public ModuleStatus headModuleStatus;
    [SerializeField] public ModuleStatus coreModuleStatus;
    [SerializeField] public ModuleStatus leftArmModuleStatus;
    [SerializeField] public ModuleStatus rightArmModuleStatus;
    [SerializeField] public ModuleStatus movementModuleStatus;

    private Module headModule;
    private Module coreModule;
    private Module leftArmModule;
    private Module rightArmModule;
    private Module movementModule;

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

        // Instantiate all the modules based on the assigned ModuleData and parent them to the appropriate transforms
        SpawnAllModules();

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

    private void SpawnAllModules()
    {
        headModule = SpawnModule(headModuleData, headTransform, headModuleStatus);
        coreModule = SpawnModule(coreModuleData, coreTransform, coreModuleStatus);
        leftArmModule = SpawnModule(leftArmModuleData, leftArmTransform, leftArmModuleStatus);
        rightArmModule = SpawnModule(rightArmModuleData, rightArmTransform, rightArmModuleStatus);
        movementModule = SpawnModule(movementModuleData, movementTransform, movementModuleStatus);
    }

    private Module SpawnModule(ModuleData moduleData, Transform parentTransform, ModuleStatus moduleStatusUI)
    {
        if (moduleData == null)
        {
            // This just means they don't have anything equipped.
            return null;
        }

        if (moduleData.instancePrefab == null)
        {
            // This means that the module data is not set up correctly, since it should always have a prefab if it's assigned.
            Debug.LogError($"ModuleData {moduleData.moduleName} does not have an instance prefab assigned.");
            return null;
        }

        GameObject moduleInstance = Instantiate(moduleData.instancePrefab, parentTransform);
        Module moduleComponent = moduleInstance.GetComponent<Module>();
        if (moduleComponent != null)
        {
            // Initialize the module with its data and parent transform
            moduleComponent.Initialize(gameObject, moduleStatusUI);
            return moduleComponent;
        }
        else
        {
            Debug.LogError("The instantiated module prefab does not have a Module component.");
            Destroy(moduleInstance);
        }

        return null;
    }

    public float GetTotalSpeedModifier()
    {
        float totalModifier = 1.0f;
        if (headModule != null) totalModifier *= headModule.speedModifier;
        if (coreModule != null) totalModifier *= coreModule.speedModifier;
        if (leftArmModule != null) totalModifier *= leftArmModule.speedModifier;
        if (rightArmModule != null) totalModifier *= rightArmModule.speedModifier;
        if (movementModule != null) totalModifier *= movementModule.speedModifier;
        return totalModifier;
    }
}
