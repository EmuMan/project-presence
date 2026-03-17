using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Module")]
public class ModuleData : ScriptableObject
{
    public enum ModuleSlot
    {
        Head,     // 0
        Core,     // 1
        LeftArm,  // 2
        RightArm, // 3
        Movement  // 4
    }

    [Header("Basic Info")]
    public string moduleName;
    public string moduleKey;
    public Sprite moduleIcon;
    public GameObject instancePrefab;

    [Tooltip("Which body part(s) this module can be attached to. Can select multiple slots.")]
    public ModuleSlot[] compatibleSlots;

    [Tooltip("Whether this module is equipped by default when the game starts.")]
    public bool isDefault = false;

    [Header("Action Settings")]
    public bool isRepeating = false;
    public float bufferDuration = 0.2f;
    public float holdDuration = 0.0f;
    public float cooldownDuration = 10.0f;
}
