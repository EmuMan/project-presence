using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Module")]
public class ModuleData : ScriptableObject
{
    public enum ModuleSlot
    {
        Head,
        Core,
        LeftArm,
        RightArm,
        Movement
    }

    public string moduleName;
    public Sprite moduleIcon;
    public GameObject instancePrefab;

    public bool isRepeating = false;
    public float bufferDuration = 0.2f;
    public float holdDuration = 0.0f;
    public float cooldownDuration = 10.0f;
}
