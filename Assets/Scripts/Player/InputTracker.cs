using UnityEngine;

public class InputTracker
{
    private bool pressedState = false;
    private bool valueRead = true;

    public bool SetPressed(bool isPressed)
    {
        // The value has been collected already.
        // We can update it to the ground truth now.
        if (valueRead)
        {
            pressedState = isPressed;
            valueRead = false;
        }
        // The value has not been collected, so don't overwrite a true value with false.
        else if (isPressed)
        {
            pressedState = true;
        }
        return pressedState;
    }

    public bool IsPressed()
    {
        valueRead = true;
        return pressedState;
    }
}
