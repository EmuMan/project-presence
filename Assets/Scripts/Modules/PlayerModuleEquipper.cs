using UnityEngine;
using System.Collections.Generic;

public class PlayerModuleEquipper : MonoBehaviour
{
    [System.Serializable]
    public class AttachmentPoint
    {
        public ModuleData.ModuleSlot slotType;
        public Transform point;
    }

    [Tooltip("The persistent loadout data saved from the Ability Screen.")]
    public PlayerLoadout playerLoadout;

    [Tooltip("The physical transforms on this character where modules spawn.")]
    public List<AttachmentPoint> attachmentPoints = new List<AttachmentPoint>();

    private List<Module> activeModules = new List<Module>();

    void Start()
    {
        EquipLoadout();
    }

    public void EquipLoadout()
    {
        if (playerLoadout == null) return;

        foreach (var loadoutSlot in playerLoadout.equippedSlots)
        {
            if (loadoutSlot.equippedModule == null) continue;

            // Find the matching physical attachment point on the player
            AttachmentPoint attachPoint = attachmentPoints.Find(p => p.slotType == loadoutSlot.slotType);
            
            if (attachPoint != null && attachPoint.point != null)
            {
                // Instantiate the prefab
                GameObject moduleObj = Instantiate(loadoutSlot.equippedModule.instancePrefab, attachPoint.point);
                
                // Initialize it
                Module moduleComponent = moduleObj.GetComponent<Module>();
                if (moduleComponent != null)
                {
                    moduleComponent.moduleData = loadoutSlot.equippedModule;
                    moduleComponent.Initialize(this.gameObject); // Pass the real player object
                    activeModules.Add(moduleComponent);
                }
            }
        }
    }
}