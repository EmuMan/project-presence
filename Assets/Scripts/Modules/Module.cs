using UnityEngine;

public class Module : MonoBehaviour
{
    [Header("Module Data")]
    public ModuleData moduleData;

    [Header("Modifiers")]
    public float speedModifier = 1.0f;
    public bool isCloaked = false;

    protected GameObject playerObject;
    protected ModuleStatus moduleStatusUI;

    // Depending on the type of action, only one of these will be non-null
    private BufferedAction bufferedAction;
    private RepeatingAction repeatingAction;

    // This is only used with BufferedAction, as RepeatingAction manages its own cooldowns internally
    private float cooldown;

    private bool wasPerformingAction;

    private void Update()
    {
        if (moduleStatusUI != null)
        {
            float cooldownFraction = moduleData.isRepeating ? repeatingAction.GetFractionUntilNextRepeat() : (cooldown / moduleData.cooldownDuration);
            moduleStatusUI.UpdateStatus(
                GetResourceRemaining(),
                cooldownFraction,
                CanPerformAction()
            );
        }
    }

    public void Initialize(GameObject playerObject, ModuleStatus moduleStatusUI)
    {
        this.playerObject = playerObject;
        this.moduleStatusUI = moduleStatusUI;

        if (moduleData.isRepeating)
        {
            repeatingAction = new RepeatingAction(moduleData.cooldownDuration, moduleData.holdDuration);
        }
        else
        {
            bufferedAction = new BufferedAction(moduleData.bufferDuration, moduleData.holdDuration);
            cooldown = 0.0f;
        }
    }

    /// <summary>
    /// Checks the input and timing conditions for this module's action and performs the action if all
    /// conditions are met. This method should be called every update with the current input state and
    /// delta time to properly manage cooldowns and action timing.
    /// </summary>
    /// <param name="input">Whether the input for this action is currently active (e.g. button held).</param>
    /// <param name="deltaTime">The time in seconds since the last update, used to manage timing for
    /// cooldowns and action repeats.</param>
    public void PerformActionIfAvailable(bool input, float deltaTime, Vector3 direction)
    {
        bool shouldPerformAction = false;

        if (moduleData.isRepeating)
        {
            if (repeatingAction.IsActing(input, CanPerformAction(), deltaTime))
            {
                shouldPerformAction = true;
            }
        }
        else
        {
            if (cooldown > 0.0f)
            {
                cooldown -= deltaTime;
            }

            bool allowed = CanPerformAction();
            if (bufferedAction.IsActing(input, cooldown <= 0.0f && allowed, allowed, deltaTime))
            {
                shouldPerformAction = true;
                cooldown = moduleData.cooldownDuration;
            }
        }

        if (shouldPerformAction)
        {
            if (!wasPerformingAction)
            {
                StartPerformingAction(direction);
            }
            PerformAction(direction);
        }
        else
        {
            if (wasPerformingAction)
            {
                StopPerformingAction(direction);
            }
        }

        wasPerformingAction = shouldPerformAction;
    }

    protected virtual void StartPerformingAction(Vector3 direction) { }

    protected virtual void PerformAction(Vector3 direction) { }

    protected virtual void StopPerformingAction(Vector3 direction) { }

    protected virtual bool CanPerformAction()
    {
        // This can be overridden by specific modules to add additional conditions for whether the action can be performed
        return true;
    }

    protected virtual float GetResourceRemaining()
    {
        // This can be overridden by specific modules that have a resource to track, like ammo or energy
        return 1.0f; // Default to full resource
    }
}
