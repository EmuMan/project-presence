using UnityEngine;

public class Module : MonoBehaviour
{
    public ModuleData moduleData;

    public GameObject playerObject;

    // Depending on the type of action, only one of these will be non-null
    private BufferedAction bufferedAction;
    private RepeatingAction repeatingAction;

    // This is only used with BufferedAction, as RepeatingAction manages its own cooldowns internally
    private float cooldown;

    public void Initialize(GameObject playerObject)
    {
        this.playerObject = playerObject;

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
        if (moduleData.isRepeating)
        {
            if (repeatingAction.IsActing(input, true, deltaTime))
            {
                PerformAction(direction);
            }
        }
        else
        {
            if (cooldown > 0.0f)
            {
                cooldown -= deltaTime;
            }

            if (bufferedAction.IsActing(input, cooldown <= 0.0f))
            {
                PerformAction(direction);
                cooldown = moduleData.cooldownDuration;
            }
        }
    }

    protected virtual void PerformAction(Vector3 direction)
    {
        Debug.Log($"Performing action for module: {moduleData.moduleName}");
    }
}
