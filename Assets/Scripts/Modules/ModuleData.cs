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

    public string moduleName;
    public Sprite moduleIcon;
    public GameObject instancePrefab;
    
    [Tooltip("Which body part this module can be attached to.")]
    public ModuleSlot slotType;

    public bool isRepeating = false;
    public float bufferDuration = 0.2f;
    public float holdDuration = 0.0f;
    public float cooldownDuration = 10.0f;
}
