using UnityEngine;
using System.Collections.Generic;

public class AbilityAttachments : MonoBehaviour
{
    [System.Serializable]
    public class BodyPartSlot
    {
        [Tooltip("The type of slot (e.g., LeftArm, RightArm, Core).")]
        public ModuleData.ModuleSlot slotType;
        
        [Tooltip("The physical transform where the module's prefab will be instantiated.")]
        public Transform attachmentPoint; 
        
        [Tooltip("The currently equipped module data for this slot.")]
        public ModuleData currentModuleData;
        
        [HideInInspector]
        public Module instantiatedModule; // Reference to the active module component in the scene
    }

    [Header("Character Setup")]
    [Tooltip("The main player object that modules will be initialized with.")]
    public GameObject playerObject;
    
    [Tooltip("Define the attachment points for each body part here.")]
    public List<BodyPartSlot> bodyPartSlots = new List<BodyPartSlot>();

    [Header("UI Setup (World Space)")]
    [Tooltip("The World Space UI Canvas or Panel that shows available modules to choose from.")]
    public GameObject moduleSelectionPanel; 

    // Keeps track of which slot is currently being modified by the UI
    private ModuleData.ModuleSlot currentlySelectedSlot;

    void Start()
    {
        // Initialize UI state
        if (moduleSelectionPanel != null)
        {
            moduleSelectionPanel.SetActive(false);
        }

        // Optional: Initialize starting modules if any are pre-assigned in the inspector
        foreach (var slot in bodyPartSlots)
        {
            if (slot.currentModuleData != null)
            {
                EquipModule(slot.slotType, slot.currentModuleData);
            }
        }
    }

    /// <summary>
    /// Called by a World Space UI Button representing a specific body part on the character.
    /// Opens the selection menu for that part.
    /// </summary>
    /// <param name="slotIndex">Cast to int from ModuleData.ModuleSlot enum in the Unity Inspector</param>
    public void OnBodyPartSelected(int slotIndex)
    {
        currentlySelectedSlot = (ModuleData.ModuleSlot)slotIndex;
        Debug.Log($"Selected body part: {currentlySelectedSlot} for modification.");
        
        // Show the UI panel to select a module
        if (moduleSelectionPanel != null)
        {
            moduleSelectionPanel.SetActive(true);
            
            // TODO: Populate the panel with available ModuleData buttons that match the currentlySelectedSlot
            // Example: UpdateUIForSlot(currentlySelectedSlot);
        }
    }

    /// <summary>
    /// Called by a World Space UI Button representing a specific module in the selection panel.
    /// Equips the module to the currently selected body part.
    /// </summary>
    public void OnModuleSelectedFromUI(ModuleData newModuleData)
    {
        // Optional: Ensure the module matches the slot type before equipping
        // if (newModuleData.slotType != currentlySelectedSlot) return;

        EquipModule(currentlySelectedSlot, newModuleData);

        // Hide selection panel after equipping
        if (moduleSelectionPanel != null)
        {
            moduleSelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the logic of removing the old module and instantiating the new one.
    /// </summary>
    public void EquipModule(ModuleData.ModuleSlot slotType, ModuleData newModuleData)
    {
        BodyPartSlot slot = bodyPartSlots.Find(s => s.slotType == slotType);
        
        if (slot != null)
        {
            // 1. Remove old module if it exists
            if (slot.instantiatedModule != null)
            {
                Destroy(slot.instantiatedModule.gameObject);
            }

            // 2. Assign new module data
            slot.currentModuleData = newModuleData;

            // 3. Instantiate the new module's prefab at the attachment point
            if (newModuleData != null && newModuleData.instancePrefab != null)
            {
                GameObject moduleObj = Instantiate(newModuleData.instancePrefab, slot.attachmentPoint);
                
                // 4. Initialize the module using your existing Module.cs logic
                Module moduleComponent = moduleObj.GetComponent<Module>();
                if (moduleComponent != null)
                {
                    moduleComponent.moduleData = newModuleData;
                    moduleComponent.Initialize(playerObject);
                    slot.instantiatedModule = moduleComponent;
                }
                else
                {
                    Debug.LogWarning($"Prefab for {newModuleData.moduleName} is missing a Module component!");
                }
            }

            Debug.Log($"Equipped {newModuleData.moduleName} to {slotType}");
        }
        else
        {
            Debug.LogWarning($"Body part slot {slotType} not found in the bodyPartSlots list!");
        }
    }
}
