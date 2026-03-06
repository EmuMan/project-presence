using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton that tracks the player's currently equipped modules across scenes.
/// Persists between scene loads via DontDestroyOnLoad.
/// <para>
/// Access via <c>PlayerLoadout.Instance</c>. Use <see cref="GetEquippedModule"/> and
/// <see cref="SetEquippedModule"/> to read and write module loadout slots.
/// </para>
/// </summary>
public class PlayerLoadout : MonoBehaviour
{
    public static PlayerLoadout Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Dictionary<ModuleData.ModuleSlot, ModuleData> equippedSlots = new Dictionary<ModuleData.ModuleSlot, ModuleData>();

    // Helper method to get a module for a specific slot
    public ModuleData GetEquippedModule(ModuleData.ModuleSlot slotType)
    {
        equippedSlots.TryGetValue(slotType, out var module);
        return module;
    }

    // Helper method to save a module to a specific slot
    public void SetEquippedModule(ModuleData.ModuleSlot slotType, ModuleData module)
    {
        equippedSlots[slotType] = module;
    }
}
