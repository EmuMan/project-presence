using UnityEngine;

public class TimescaleModifier : MonoBehaviour
{
    public void FreezeTime()
    {
        Time.timeScale = 0f;
    }
}
