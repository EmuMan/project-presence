using UnityEngine;

/// <summary>
/// A class to manage actions that can be repeated while an input is held, with a specified
/// interval between repeats and an optional hold duration which allows the action to be
/// extended if the input is continuously held.
///
/// For example, if you wanted to create a gun that fires once every 0.5 seconds while the
/// fire button is held, you could use this class with a repeat duration of 0.5 seconds and
/// a hold duration of 0.0 seconds. If you wanted to create a beam weapon that fires for
/// 0.3 seconds continuously every 1 second while the fire button is held, you could use
/// this class with a repeat duration of 1.0 seconds and a hold duration of 0.3 seconds.
///
/// To use this class, create an instance of RepeatingAction with the desired repeat and hold
/// durations. Then, in your update loop (likely FixedUpdate in this project), call the
/// IsActing method with the current input state, whether the action is allowed, and the delta
/// time since the last update. The IsActing method will return true if the action should be
/// performed during that update, based on the timing and input conditions.
///
/// Note that IsActing should be called every update to properly manage the timing and state
/// of the action, even if the input is not currently active, as it needs to track the time
/// since the last repeat and whether the action is in progress. It will handle everything
/// interally, so just give it those inputs and use the result to figure out if the action
/// should be performed during that update.
/// </summary>
public class RepeatingAction
{
    private float repeatInterval;
    private float holdableFor;

    private float timeSinceLastRepeat;

    // This boolean prevents multiple triggers within the holdable duration.
    // Held presses will only count if they have been continually in progress.
    private bool inProgress = false;

    /// <summary>
    /// Initializes a new instance of the RepeatingAction class with the specified repeat interval
    /// and hold duration.
    /// </summary>
    /// <param name="repeatDuration">The time in seconds between each repeat of the action while the input is held.</param>
    /// <param name="holdDuration">The time in seconds that the action can continue to
    /// be considered active after the repeat interval has passed, as long as the input is still
    /// held. A value of 0.0 means the action will only be active during the repeat interval.</param>
    public RepeatingAction(float repeatDuration, float holdDuration = 0.0f)
    {
        repeatInterval = repeatDuration;
        holdableFor = holdDuration;

        timeSinceLastRepeat = repeatInterval; // Initialize to allow immediate action
    }

    /// <summary>
    /// Determines whether the action should be performed based on the current input state, whether
    /// the action is allowed, and the timing since the last repeat. This method should be called
    /// every update to properly manage the internal state of the action.
    /// </summary>
    /// <param name="input">Whether the input for this action is currently active (e.g. button held).</param>
    /// <param name="allowed">Whether the action is currently allowed to be performed (e.g. not on reload cooldown).</param>
    /// <param name="deltaTime">The time in seconds since the last update, used to manage timing for repeats and holds.</param>
    /// <returns>True if the action should be performed during this update, false otherwise.</returns>
    public bool IsActing(bool input, bool allowed, float deltaTime)
    {
        timeSinceLastRepeat += deltaTime;

        if (input && allowed)
        {
            // The action is allowed and the input is being held, and we need to trigger a new repeat.
            if (timeSinceLastRepeat >= repeatInterval)
            {
                timeSinceLastRepeat = 0.0f;
                inProgress = true;
                return true;
            }
            // The action is still in progress and being held within the holdable duration.
            else if (inProgress && timeSinceLastRepeat <= holdableFor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            inProgress = false;
            return false;
        }
    }
}
