using System.Timers;
using UnityEngine;

/// <summary>
/// A class to manage actions that can be buffered, allowing for the input to be pressed a
/// short window of time before the action is actually allowed to be performed. This is useful
/// for actions that have specific timing requirements, such as a jump that can be buffered
/// just before landing or an important ability that can be buffered just before the cooldown
/// ends.
///
/// The class also supports holdable actions, where the action can continue to be
/// considered active for a short duration after the initial activation, as long as the input
/// is still being held. For example, a jump that allows the player to hold the jump button
/// for a short time after leaving the ground to perform a higher jump. Even though the jump
/// becomes invalid as soon as the player leaves the ground, they can still hold the button
/// to continue the jump for a short time.
///
/// To use this class, create an instance of BufferedAction with the desired buffer and hold
/// durations. Then, in your update loop (likely FixedUpdate in this project), call the
/// IsActing method with the current input state and whether the action is allowed. The
/// IsActing method will return true if the action should be performed during that update,
/// based on the timing and input conditions.
///
/// Note that IsActing should be called every update to properly manage the timing and state
/// of the action, even if the input is not currently active, as it needs to track the time
/// since the last repeat and whether the action is in progress. It will handle everything
/// interally, so just give it those inputs and use the result to figure out if the action
/// should be performed during that update.
/// </summary>
public class BufferedAction
{
    private float bufferTime;
    private float lastInputTime;
    private bool inputPressed;

    // How long the input can be held, even after it becomes invalid
    // A value of 0.0 will act as a single use action
    private float holdableFor;
    // The last time an action was successfully started
    private float lastActionStart;
    // Whether the action is currently being activated
    // Used so that you can't release and retrigger the action within this window
    private bool isBeingActivated;

    /// <summary>
    /// Initializes a new instance of the BufferedAction class with the specified repeat interval
    /// and hold duration.
    /// </summary>
    /// <param name="bufferDuration">The time in seconds that an input can be pressed and held
    /// before the action is allowed and still activate the action.</param>
    /// <param name="holdDuration">The time in seconds that the action can continue to
    /// be considered active after the action first activates, as long as the input is still
    /// held. A value of 0.0 means the action will be active instantaneously.</param>
    public BufferedAction(float bufferDuration, float holdDuration = 0.0f)
    {
        bufferTime = bufferDuration;
        lastInputTime = -bufferDuration; // Initialize to allow immediate input
        inputPressed = false;

        holdableFor = holdDuration;
        lastActionStart = -holdDuration;
    }

    private void ProcessInput(bool input)
    {
        if (!inputPressed && input)
        {
            lastInputTime = Time.time;
        }
        inputPressed = input;
    }

    /// <summary>
    /// Determines whether the action should be performed based on the current input state, whether
    /// the action is allowed, and the timing since the last activation. This method should be called
    /// every update to properly manage the internal state of the action.
    /// </summary>
    /// <param name="input">Whether the input for this action is currently active (e.g. button held).</param>
    /// <param name="allowed">Whether the action is currently allowed to be performed (e.g. not on cooldown).</param>
    /// <param name="deltaTime">The time in seconds since the last update, used to manage timing for repeats and holds.</param>
    /// <returns>True if the action should be performed during this update, false otherwise.</returns>
    public bool IsActing(bool input, bool allowed)
    {
        ProcessInput(input);

        // The initial input is valid if:
        // 1. The input is still being pressed
        // 2. The action activation is not currently in progress
        // 3. The input was just pressed within the buffer time
        bool initialInputValid = inputPressed && !isBeingActivated && (Time.time - lastInputTime) <= bufferTime;
        // The held input is valid if:
        // 1. The input is currently being pressed
        // 2. The action activation is ongoing, i.e. it was not interrupted
        // 3. The action has not been held for longer than the holdable duration
        bool heldInputValid = inputPressed && isBeingActivated && (Time.time - lastActionStart) <= holdableFor;

        if (initialInputValid && allowed)
        {
            lastActionStart = Time.time;
            isBeingActivated = true;
            return true;
        }
        else if (heldInputValid)
        {
            return true;
        }
        else
        {
            isBeingActivated = false;
            return false;
        }
    }

    public void ForceReset()
    {
        inputPressed = false;
        isBeingActivated = false;
    }
}
