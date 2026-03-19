using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    [Header("UI")]
    [Tooltip("The scene to transition to when the player clicks the Deploy button.")]
    public string gameplaySceneName = "GameplayScene";

    [Tooltip("The World Space UI Canvas or Panel that shows available modules to choose from.")]
    public GameObject moduleSelectionPanel;

    [Tooltip("The parent transform (e.g., a Vertical Layout Group) where module buttons will be spawned.")]
    public Transform moduleButtonContainer;

    [Tooltip("The UI Button prefab used for each module option.")]
    public GameObject moduleButtonPrefab;

    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup abilityUICanvasGroup;

    [Tooltip("The CanvasGroup for the title UI elements.")]
    public CanvasGroup titleUICanvasGroup;

    [Tooltip("The CanvasGroup for the ability UI elements.")]
    public CanvasGroup levelUICanvasGroup;

    [Header("Cameras")]
    [Tooltip("The normal main camera.")]
    public Camera mainCamera;

    [Header("Transition Settings")]
    [Tooltip("The coords for ability UI.")]
    public Transform abilityCameraPosition;

    [Tooltip("The coords for game over UI.")]
    public Transform gameOverCameraPosition;

    [Tooltip("The coords for title UI.")]
    public Transform titleCameraPosition;

    [Tooltip("The coords for title UI.")]
    public Transform levelCameraPosition;

    [Tooltip("The coords to get into game.")]
    public Transform deployCameraPosition;

    // Keeps track of which slot is currently being modified by the UI
    private ModuleData.ModuleSlot currentlySelectedSlot;

    [Header("MSP Navigation")]
    [Tooltip("Drag the Close/Exit button of the MSP here.")]
    public Button mspCloseButton;

    // Remembers the body part button you clicked to open the menu
    private GameObject lastSelectedBeforeMSP;

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

        // 2. Return focus to the body part we originally clicked
        if (lastSelectedBeforeMSP != null && UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(lastSelectedBeforeMSP);
        }
    }

    /// <summary>
    /// Called by a World Space UI Button representing a specific body part on the character.
    /// Opens the selection menu for that part.
    /// </summary>
    /// <param name="slotIndex">Cast to int from ModuleData.ModuleSlot enum in the Unity Inspector</param>
    public void OnBodyPartSelected(int slotIndex)
    {
        // 1. Remember what button we clicked so we can return to it when we close the panel
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            lastSelectedBeforeMSP = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        }

        currentlySelectedSlot = (ModuleData.ModuleSlot)slotIndex;
        Debug.Log($"Selected body part: {currentlySelectedSlot} for modification.");

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
        foreach (Transform child in moduleButtonContainer)
        {
            Destroy(child.gameObject);
        }

        var unlockedModules = ModuleManager.Instance.GetUnlockedModules();

        // Add a list to track the buttons we spawn!
        List<Button> spawnedButtons = new List<Button>();

        foreach (ModuleData module in unlockedModules)
        {
            if (module == null) continue;

            if (module.compatibleSlots != null && System.Array.IndexOf(module.compatibleSlots, slot) >= 0)
            {
                GameObject buttonObj = Instantiate(moduleButtonPrefab, moduleButtonContainer);

                bool textSet = false;

                // 3. Setup button visuals - Try both Text and TextMeshProUGUI
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = module.moduleName;
                    textSet = true;
                }

                if (!textSet)
                {
                    TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = module.moduleName;
                        textSet = true;
                    }
                }

                if (!textSet)
                {
                    Debug.LogWarning($"Button prefab is missing both Text and TextMeshProUGUI components! Module: {module.moduleName}");
                }

                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    ModuleData capturedModule = module;
                    btn.onClick.AddListener(() => OnModuleSelectedFromUI(capturedModule));

                    // Add to our tracker list
                    spawnedButtons.Add(btn);
                }
            }
        }

        // ==========================================
        // EXPLICIT NAVIGATION SETUP FOR THE CONTROLLER
        // ==========================================
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;

            // Up action: If it's the first UI item, go to Close Button. Otherwise, go to the previous item.
            customNav.selectOnUp = (i == 0) ? mspCloseButton : spawnedButtons[i - 1];

            // Down action: If it's the last item, stay. Otherwise, go to the next item.
            customNav.selectOnDown = (i == spawnedButtons.Count - 1) ? null : spawnedButtons[i + 1];

            // Assign the navigation rules
            spawnedButtons[i].navigation = customNav;
        }

        // Connect the Close Button downward into the list
        if (mspCloseButton != null)
        {
            Navigation closeNav = mspCloseButton.navigation;
            closeNav.mode = Navigation.Mode.Explicit;

            if (spawnedButtons.Count > 0)
            {
                closeNav.selectOnDown = spawnedButtons[0]; // Press down from close -> goes to first module
                mspCloseButton.navigation = closeNav;

                // Step 3: Automatically focus the first module button or the close button
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(spawnedButtons[0].gameObject);
            }
            else
            {
                closeNav.selectOnDown = null;
                mspCloseButton.navigation = closeNav;

                // If there are no modules to select, focus the close button so the player isn't stuck
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(mspCloseButton.gameObject);
            }
        }

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

        // When a module is selected, the panel closes. Call CloseMSP to handle the panel state and refocus the UI properly.
        CloseMSP();
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
    public void CamTransitionToStart()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                titleCameraPosition,
                abilityUICanvasGroup,
                titleUICanvasGroup,
                50f
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }

    public void CamTransitionToLevel()
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            Debug.Log($"AbilityTutorial: {PlayerPrefs.GetInt("AbilityTutorial", 0)}, SimulationTutorial: {PlayerPrefs.GetInt("SimulationTutorial", 0)}, FinalTutorial: {PlayerPrefs.GetInt("FinalTutorial", 0)}");
            if ((PlayerPrefs.GetInt("AbilityTutorial", 0) == 1) && (PlayerPrefs.GetInt("SimulationTutorial", 0) == 0))
            {
                Debug.Log("Transitioning to simulation");
                CamTransitionToGame("Simulation");
                // some checks to change the ability screen deploy
            }
            else if ((PlayerPrefs.GetInt("FinalTutorial", 0) == 1) && (PlayerPrefs.GetInt("Next", 0) == 0))
            {
                CamTransitionToGame("Main Level");
                PlayerPrefs.SetInt("Next", 1);
            }
            else
            {
                transitionScreen.StartCameraTransition(
                    mainCamera,
                    levelCameraPosition,
                    abilityUICanvasGroup,
                    levelUICanvasGroup,
                    60f
                );
            }
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }

    public void CamTransitionToGame(string useLoadScene)
    {
        TransitionScreen transitionScreen = Object.FindFirstObjectByType<TransitionScreen>();
        if (transitionScreen != null)
        {
            transitionScreen.StartCameraTransition(
                mainCamera,
                deployCameraPosition,
                abilityUICanvasGroup,
                titleUICanvasGroup,
                60f,
                true,
                useLoadScene
            );
        }
        else
        {
            Debug.LogError("TransitionScreen component not found in the scene.");
        }
    }
}
