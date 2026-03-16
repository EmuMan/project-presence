using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AbilityScreen : MonoBehaviour
{
    [Header("Character Setup")]

    [Header("Explicit Attachment Points")]
    [Tooltip("Drag the Left Arm bone/transform here.")]
    public Transform leftArmAttachmentPoint;

    [Tooltip("Drag the Right Arm bone/transform here.")]
    public Transform rightArmAttachmentPoint;

    [Tooltip("Drag the Core bone/transform here.")]
    public Transform coreAttachmentPoint;

    [Tooltip("Drag the Head bone/transform here.")]
    public Transform headAttachmentPoint;

    [Tooltip("Drag the Movement bone/transform here.")]
    public Transform movementAttachmentPoint;

    // Keep track of the spawned modules so we can destroy them when swapping
    private Module instantiatedLeftArmModule;
    private Module instantiatedRightArmModule;
    private Module instantiatedCoreModule;
    private Module instantiatedHeadModule;
    private Module instantiatedMovementModule;

    [Header("UI Setup (World Space)")]
    [Tooltip("The World Space UI Canvas or Panel that shows available modules to choose from.")]
    public GameObject moduleSelectionPanel;

    [Tooltip("The parent transform (e.g., a Vertical Layout Group) where module buttons will be spawned.")]
    public Transform moduleButtonContainer;

    [Tooltip("The UI Button prefab used for each module option.")]
    public GameObject moduleButtonPrefab;

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
        if (PlayerLoadout.Instance != null)
        {
            EquipModule(ModuleData.ModuleSlot.LeftArm, PlayerLoadout.Instance.GetEquippedModule(ModuleData.ModuleSlot.LeftArm));
            EquipModule(ModuleData.ModuleSlot.RightArm, PlayerLoadout.Instance.GetEquippedModule(ModuleData.ModuleSlot.RightArm));
            EquipModule(ModuleData.ModuleSlot.Core, PlayerLoadout.Instance.GetEquippedModule(ModuleData.ModuleSlot.Core));
            EquipModule(ModuleData.ModuleSlot.Head, PlayerLoadout.Instance.GetEquippedModule(ModuleData.ModuleSlot.Head));
            EquipModule(ModuleData.ModuleSlot.Movement, PlayerLoadout.Instance.GetEquippedModule(ModuleData.ModuleSlot.Movement));
        }
    }

    // Close the module selection panel when clicking on a close button
    public void CloseMSP()
    {
        moduleSelectionPanel.SetActive(false);
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

        var unlockedModules = ModuleManager.Instance.GetUnlockedModules();

        Debug.Log($"UpdateUIForSlot called for: {slot}");
        Debug.Log($"Total modules unlocked: {unlockedModules.Count}");

        int matchingModules = 0;

        // 2. Find and instantiate buttons for matching modules
        foreach (ModuleData module in unlockedModules)
        {
            if (module == null)
            {
                Debug.LogWarning("Found null module in unlockedModules!");
                continue;
            }

            string slotsDebug = module.compatibleSlots != null ? string.Join(", ", module.compatibleSlots) : "none";
            Debug.Log($"Checking module: {module.moduleName} with compatible slots: {slotsDebug}");

            if (module.compatibleSlots != null && System.Array.IndexOf(module.compatibleSlots, slot) >= 0)
            {
                matchingModules++;
                GameObject buttonObj = Instantiate(moduleButtonPrefab, moduleButtonContainer);

                // 3. Setup button visuals - Try both Text and TextMeshProUGUI
                Debug.Log($"Attempting to set button text for module: '{module.moduleName}'");

                bool textSet = false;

                // Try legacy Text component first
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = module.moduleName;
                    Debug.Log($"Set Text component to: '{module.moduleName}'");
                    textSet = true;
                }

                // Try TextMeshPro if Text wasn't found
                if (!textSet)
                {
                    TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = module.moduleName;
                        Debug.Log($"Set TextMeshPro component to: '{module.moduleName}'");
                        textSet = true;
                    }
                }

                if (!textSet)
                {
                    Debug.LogWarning($"Button prefab is missing both Text and TextMeshProUGUI components! Module: {module.moduleName}");
                    Debug.LogWarning($"Button hierarchy: {GetHierarchyPath(buttonObj.transform)}");
                }

                // 4. Setup button click event dynamically
                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    ModuleData capturedModule = module;
                    btn.onClick.AddListener(() => OnModuleSelectedFromUI(capturedModule));
                }
                else
                {
                    Debug.LogWarning("Button prefab is missing Button component!");
                }
            }
        }

        Debug.Log($"Created {matchingModules} buttons for slot: {slot}");

        // Force rebuild the layout after spawning buttons
        Canvas.ForceUpdateCanvases();
        if (moduleButtonContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(moduleButtonContainer as RectTransform);
        }
    }

    private string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    /// <summary>
    /// Called by a World Space UI Button representing a specific module in the selection panel.
    /// Equips the module to the currently selected body part.
    /// </summary>
    public void OnModuleSelectedFromUI(ModuleData newModuleData)
    {
        // Sanity check to ensure the module is compatible with the currently selected slot, and checks if the selected slot is in the compatibleSlots array
        if (newModuleData.compatibleSlots == null || System.Array.IndexOf(newModuleData.compatibleSlots, currentlySelectedSlot) < 0) return;

        // 1. Save the choice to the persistent loadout
        if (PlayerLoadout.Instance != null)
        {
            PlayerLoadout.Instance.SetEquippedModule(currentlySelectedSlot, newModuleData);
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
    /// Instantiation in this context refers to the menu, not the actual gameplay scene.
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
            case ModuleData.ModuleSlot.Head:
                targetAttachmentPoint = headAttachmentPoint;
                currentInstantiatedModule = instantiatedHeadModule;
                break;
            case ModuleData.ModuleSlot.Movement:
                targetAttachmentPoint = movementAttachmentPoint;
                currentInstantiatedModule = instantiatedMovementModule;
                break;
            default:
                Debug.LogWarning($"Slot {slotType} is not set up in AbilityScreen script!");
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
                //moduleComponent.Initialize(playerObject, null);

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
                    case ModuleData.ModuleSlot.Head:
                        instantiatedHeadModule = moduleComponent;
                        break;
                    case ModuleData.ModuleSlot.Movement:
                        instantiatedMovementModule = moduleComponent;
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
