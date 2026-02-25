using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerLoadout", menuName = "Data/Player Loadout")]
public class PlayerLoadout : ScriptableObject
{
    [System.Serializable]
    public class EquippedSlot
    {
        public ModuleData.ModuleSlot slotType;
        public ModuleData equippedModule;
    }

    [Tooltip("The modules currently equipped by the player.")]
    public List<EquippedSlot> equippedSlots = new List<EquippedSlot>();

    // Helper method to get a module for a specific slot
    public ModuleData GetEquippedModule(ModuleData.ModuleSlot slotType)
    {
        var slot = equippedSlots.Find(s => s.slotType == slotType);
        return slot != null ? slot.equippedModule : null;
    }

    // Helper method to save a module to a specific slot
    public void SetEquippedModule(ModuleData.ModuleSlot slotType, ModuleData module)
    {
        var slot = equippedSlots.Find(s => s.slotType == slotType);
        if (slot != null)
        {
            slot.equippedModule = module;
        }
        else
        {
            equippedSlots.Add(new EquippedSlot { slotType = slotType, equippedModule = module });
        }
    }
}