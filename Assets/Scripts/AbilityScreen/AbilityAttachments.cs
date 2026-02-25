using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityAttachments : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("The persistent loadout data that carries over to the game scene.")]
    public PlayerLoadout playerLoadout;

    [Header("Character Setup")]
    [Tooltip("The main player object that modules will be initialized with.")]
    public GameObject playerObject;
    
    [Header("Explicit Attachment Points")]
    [Tooltip("Drag the Left Arm bone/transform here.")]
    public Transform leftArmAttachmentPoint;
    
    [Tooltip("Drag the Right Arm bone/transform here.")]
    public Transform rightArmAttachmentPoint;
    
    [Tooltip("Drag the Core/Chest bone/transform here.")]
    public Transform coreAttachmentPoint;

    // Keep track of the spawned modules so we can destroy them when swapping
    private Module instantiatedLeftArmModule;
    private Module instantiatedRightArmModule;
    private Module instantiatedCoreModule;

    [Header("UI Setup (World Space)")]
    [Tooltip("The World Space UI Canvas or Panel that shows available modules to choose from.")]
    public GameObject moduleSelectionPanel; 
    
    [Tooltip("The parent transform (e.g., a Vertical Layout Group) where module buttons will be spawned.")]
    public Transform moduleButtonContainer;
    
    [Tooltip("The UI Button prefab used for each module option.")]
    public GameObject moduleButtonPrefab;

    [Header("Available Modules")]
    [Tooltip("A master list of all modules the player currently owns or can equip.")]
    public List<ModuleData> allAvailableModules = new List<ModuleData>();

    // Keeps track of which slot is currently being modified by the UI
    private ModuleData.ModuleSlot currentlySelectedSlot;

    void Start()
    {
        // Initialize UI state
        if (moduleSelectionPanel != null)
        {
            moduleSelectionPanel.SetActive(false);
        }

        // Optional: If you want to load the player's previously saved loadout right away
        if (playerLoadout != null)
        {
            EquipModule(ModuleData.ModuleSlot.LeftArm, playerLoadout.GetEquippedModule(ModuleData.ModuleSlot.LeftArm));
            EquipModule(ModuleData.ModuleSlot.RightArm, playerLoadout.GetEquippedModule(ModuleData.ModuleSlot.RightArm));
            EquipModule(ModuleData.ModuleSlot.Core, playerLoadout.GetEquippedModule(ModuleData.ModuleSlot.Core));
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
            UpdateUIForSlot(currentlySelectedSlot);
        }
    }

    /// <summary>
    /// Populates the selection panel with buttons for modules that match the selected slot.
    /// </summary>
    private void UpdateUIForSlot(ModuleData.ModuleSlot slot)
    {
        // 1. Clear existing buttons in the container
        foreach (Transform child in moduleButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Find and instantiate buttons for matching modules
        foreach (ModuleData module in allAvailableModules)
        {
            if (module.slotType == slot)
            {
                GameObject buttonObj = Instantiate(moduleButtonPrefab, moduleButtonContainer);
                
                // 3. Setup button visuals
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = module.moduleName;
                }

                // 4. Setup button click event dynamically
                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    ModuleData capturedModule = module; 
                    btn.onClick.AddListener(() => OnModuleSelectedFromUI(capturedModule));
                }
            }
        }
    }

    /// <summary>
    /// Called by a World Space UI Button representing a specific module in the selection panel.
    /// Equips the module to the currently selected body part.
    /// </summary>
    public void OnModuleSelectedFromUI(ModuleData newModuleData)
    {
        if (newModuleData.slotType != currentlySelectedSlot) return;

        // 1. Save the choice to the persistent loadout
        if (playerLoadout != null)
        {
            playerLoadout.SetEquippedModule(currentlySelectedSlot, newModuleData);
            Debug.Log($"Saved {newModuleData.moduleName} to {currentlySelectedSlot} in Loadout.");
        }

        // 2. Equip it visually on the dummy character in this scene
        EquipModule(currentlySelectedSlot, newModuleData);

        if (moduleSelectionPanel != null)
        {
            moduleSelectionPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Handles the logic of removing the old module and instantiating the new one based on explicit slots.
    /// </summary>
    public void EquipModule(ModuleData.ModuleSlot slotType, ModuleData newModuleData)
    {
        if (newModuleData == null) return;

        Transform targetAttachmentPoint = null;
        Module currentInstantiatedModule = null;

        // 1. Determine which explicit slot we are dealing with
        switch (slotType)
        {
            case ModuleData.ModuleSlot.LeftArm:
                targetAttachmentPoint = leftArmAttachmentPoint;
                currentInstantiatedModule = instantiatedLeftArmModule;
                break;
            case ModuleData.ModuleSlot.RightArm:
                targetAttachmentPoint = rightArmAttachmentPoint;
                currentInstantiatedModule = instantiatedRightArmModule;
                break;
            case ModuleData.ModuleSlot.Core:
                targetAttachmentPoint = coreAttachmentPoint;
                currentInstantiatedModule = instantiatedCoreModule;
                break;
            default:
                Debug.LogWarning($"Slot {slotType} is not explicitly set up in AbilityAttachments!");
                return;
        }

        if (targetAttachmentPoint == null)
        {
            Debug.LogWarning($"Attachment point for {slotType} is not assigned in the Inspector!");
            return;
        }

        // 2. Remove old module if it exists
        if (currentInstantiatedModule != null)
        {
            Destroy(currentInstantiatedModule.gameObject);
        }

        // 3. Instantiate the new module's prefab at the attachment point
        if (newModuleData.instancePrefab != null)
        {
            GameObject moduleObj = Instantiate(newModuleData.instancePrefab, targetAttachmentPoint);
            
            // 4. Initialize the module using your existing Module.cs logic
            Module moduleComponent = moduleObj.GetComponent<Module>();
            if (moduleComponent != null)
            {
                moduleComponent.moduleData = newModuleData;
                moduleComponent.Initialize(playerObject);

                // 5. Save the reference so we can destroy it later if we swap again
                switch (slotType)
                {
                    case ModuleData.ModuleSlot.LeftArm:
                        instantiatedLeftArmModule = moduleComponent;
                        break;
                    case ModuleData.ModuleSlot.RightArm:
                        instantiatedRightArmModule = moduleComponent;
                        break;
                    case ModuleData.ModuleSlot.Core:
                        instantiatedCoreModule = moduleComponent;
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"Prefab for {newModuleData.moduleName} is missing a Module component!");
            }
        }

        Debug.Log($"Equipped {newModuleData.moduleName} to {slotType}");
    }
}
